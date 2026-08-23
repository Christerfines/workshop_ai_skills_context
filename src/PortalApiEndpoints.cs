using System.Text.RegularExpressions;
using static NordicBike.Portal.PortalIdentity;
using static NordicBike.Portal.PortalViews;

namespace NordicBike.Portal;

public static class PortalApiEndpoints
{
    private const string Patch = "PATCH";
    private const string ReturnRequested = "Return requested";

    public static void MapPortalApi(this WebApplication app)
    {
        MapRoleEndpoint(app);
        MapCatalogEndpoints(app);
        MapCartEndpoints(app);
        MapOrderEndpoints(app);
        MapBikeEndpoints(app);
        MapCaseEndpoints(app);
        MapServiceRequestEndpoints(app);
        MapDashboardEndpoints(app);
        MapDiagnosticsEndpoints(app);
        MapSupportIssueEndpoints(app);
    }

    private static void MapRoleEndpoint(WebApplication app)
    {
        app.MapGet("/role/{role}", (HttpContext context, string role, string? returnTo) =>
        {
            if (!PortalConfig.Roles.ContainsKey(role)) return Results.NotFound();
            context.Response.Cookies.Append("nb-role", role, new CookieOptions { IsEssential = true, HttpOnly = true, Secure = context.Request.IsHttps, SameSite = SameSiteMode.Lax });
            return Results.Redirect(string.IsNullOrWhiteSpace(returnTo) ? Landing(role) : returnTo);
        });
    }

    private static void MapCatalogEndpoints(WebApplication app)
    {
        app.MapGet("/api/health", (PortalStore store) => Results.Ok(new { status = "healthy", demoDate = DemoClock.Today, version = "1.0.0", store = "in-memory", orders = store.Orders.Count }));
        app.MapGet("/api/products", (PortalStore store, string? q, string? category, string? type, string? tag, int? page, int? pageSize) =>
        {
            var filtered = store.Products.Where(item =>
                (string.IsNullOrWhiteSpace(q) || item.Name.Contains(q, StringComparison.OrdinalIgnoreCase) || item.Description.Contains(q, StringComparison.OrdinalIgnoreCase) || item.Tags.Any(value => value.Contains(q, StringComparison.OrdinalIgnoreCase))) &&
                (string.IsNullOrWhiteSpace(category) || item.Category.Equals(category, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrWhiteSpace(type) || item.Type.Equals(type, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrWhiteSpace(tag) || item.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase)))
                .OrderBy(item => item.Category)
                .ThenBy(item => item.Name)
                .ToList();
            var currentPage = Math.Max(page ?? 1, 1);
            var currentPageSize = Math.Clamp(pageSize ?? 24, 1, 100);
            return Results.Ok(new { items = filtered.Skip((currentPage - 1) * currentPageSize).Take(currentPageSize), total = filtered.Count, page = currentPage, pageSize = currentPageSize });
        });
        app.MapGet("/api/products/{id}", (PortalStore store, string id) => store.Products.SingleOrDefault(item => item.Id == id) is { } product ? Results.Ok(product) : Results.NotFound());
    }

    private static void MapCartEndpoints(WebApplication app)
    {
        app.MapGet("/api/cart", (HttpContext context, PortalStore store) => Results.Ok(Cart(store, Customer(context))));
        app.MapPost("/api/cart/items", async (HttpContext context, PortalStore store) =>
        {
            var input = await context.Request.ReadFromJsonAsync<CartInput>();
            var product = input is null ? null : store.Products.SingleOrDefault(item => item.Id == input.ProductId);
            if (input is null || product is null || input.Quantity < 1) return Problem("A product and positive quantity are required.", 400);
            var cart = store.Cart(Customer(context));
            var existing = cart.SingleOrDefault(item => item.ProductId == input.ProductId && item.Configuration == input.Configuration);
            if (existing is null) cart.Add(new CartItem(Guid.NewGuid().ToString("N"), input.ProductId, input.Quantity, input.Configuration));
            else { cart.Remove(existing); cart.Add(existing with { Quantity = existing.Quantity + input.Quantity }); }
            PortalAudit.Record(context, store, "Cart updated", "cart", product.Name);
            return Results.Created("/api/cart", Cart(store, Customer(context)));
        });
        app.MapMethods("/api/cart/items/{id}", [Patch], async (HttpContext context, PortalStore store, string id) =>
        {
            var input = await context.Request.ReadFromJsonAsync<CartInput>();
            var cart = store.Cart(Customer(context));
            var item = cart.SingleOrDefault(entry => entry.Id == id);
            if (item is null) return Results.NotFound();
            if (input is null || input.Quantity < 1) return Problem("Quantity must be at least one.", 400);
            cart.Remove(item);
            cart.Add(item with { Quantity = input.Quantity });
            PortalAudit.Record(context, store, "Cart quantity updated", "cart", id);
            return Results.Ok(Cart(store, Customer(context)));
        });
        app.MapDelete("/api/cart/items/{id}", (HttpContext context, PortalStore store, string id) =>
        {
            var cart = store.Cart(Customer(context));
            var item = cart.SingleOrDefault(entry => entry.Id == id);
            if (item is null) return Results.NotFound();
            cart.Remove(item);
            PortalAudit.Record(context, store, "Cart item removed", "cart", id);
            return Results.NoContent();
        });
    }

    private static void MapOrderEndpoints(WebApplication app)
    {
        app.MapPost("/api/orders", CreateOrderAsync);
        app.MapGet("/api/orders", (HttpContext context, PortalStore store) => Results.Ok(store.Orders.Where(order => IsInternal(context) || order.CustomerId == Customer(context)).OrderByDescending(order => order.CreatedAt).Select(order => ToOrderView(order, IsInternal(context)))));
        app.MapGet("/api/orders/{id}", (HttpContext context, PortalStore store, string id) => store.Orders.SingleOrDefault(order => order.Id == id && (IsInternal(context) || order.CustomerId == Customer(context))) is { } order ? Results.Ok(ToOrderView(order, IsInternal(context))) : Results.NotFound());
        app.MapPost("/api/orders/{id}/return-requests", RequestReturn);
        app.MapPost("/api/orders/{id}/advance", AdvanceOrder);
    }

    private static async Task<IResult> CreateOrderAsync(HttpContext context, PortalStore store)
    {
        var input = await context.Request.ReadFromJsonAsync<CheckoutInput>();
        var idempotencyKey = context.Request.Headers["Idempotency-Key"].FirstOrDefault()?.Trim();
        if (string.IsNullOrWhiteSpace(idempotencyKey)) return Problem("An Idempotency-Key header is required.", 400);
        if (input is null || string.IsNullOrWhiteSpace(input.Name) || string.IsNullOrWhiteSpace(input.Email) || string.IsNullOrWhiteSpace(input.City) || !input.IsSimulated) return Problem("Name, email, delivery city, confirmation, and cart items are required.", 400);
        var customer = Customer(context);
        var fingerprint = CheckoutFingerprint(input);
        lock (store.SyncRoot)
        {
            if (store.CheckoutAttempts.TryGetValue(idempotencyKey, out var previous))
            {
                if (previous.CustomerId != customer || previous.Fingerprint != fingerprint) return Problem("The idempotency key was already used for different checkout data.", 409);
                var previousOrder = store.Orders.SingleOrDefault(item => item.Id == previous.OrderId);
                return previousOrder is null ? Problem("The original checkout result is unavailable.", 409) : Results.Created($"/api/orders/{previousOrder.Id}", ToOrder(previousOrder));
            }

            var cart = store.Cart(customer);
            if (cart.Count == 0) return Problem("Name, email, delivery city, confirmation, and cart items are required.", 400);
            var lines = cart.Select(item =>
            {
                var product = store.Products.Single(product => product.Id == item.ProductId);
                return new OrderLine(product.Id, product.Name, item.Quantity, product.Price, item.Configuration);
            }).ToList();
            var order = new Order { Id = store.NewOrderId(), CustomerId = customer, Lines = lines, Status = "Confirmed", City = input.City.Trim(), CreatedAt = DemoClock.Now, UpdatedAt = DemoClock.Now };
            order.Timeline.Add(new TimelineEvent(DemoClock.Now, "Order confirmed", "Your simulated order was accepted."));
            store.Orders.Insert(0, order);
            cart.Clear();
            store.CheckoutAttempts[idempotencyKey] = new CheckoutAttempt(customer, fingerprint, order.Id);
            PortalAudit.Record(context, store, "Checkout completed", order.Id, "Confirmed simulated order");
            return Results.Created($"/api/orders/{order.Id}", ToOrder(order));
        }
    }

    private static IResult RequestReturn(HttpContext context, PortalStore store, string id)
    {
        var order = store.Orders.SingleOrDefault(item => item.Id == id && item.CustomerId == Customer(context));
        if (order is null) return Results.NotFound();
        if (order.Status != "Delivered") return Problem("Only delivered orders can be returned.", 400);
        order.Status = ReturnRequested;
        order.UpdatedAt = DemoClock.Now;
        order.Timeline.Add(new TimelineEvent(DemoClock.Now, ReturnRequested, "Support case created."));
        var supportCase = NewCase(store, Customer(context), $"Return request for {id}", ReturnRequested, "Please help me return this delivered order.", id, null);
        PortalAudit.Record(context, store, ReturnRequested, id, supportCase.Id);
        return Results.Ok(new { order = ToOrder(order), supportCaseId = supportCase.Id });
    }

    private static IResult AdvanceOrder(HttpContext context, PortalStore store, string id)
    {
        if (!IsInternal(context)) return Results.NotFound();
        var order = store.Orders.SingleOrDefault(item => item.Id == id);
        if (order is null) return Results.NotFound();
        var next = order.Status switch { "Confirmed" => "Processing", "Processing" => "Shipped", "Shipped" => "Delivered", _ => null };
        if (next is null) return Problem("This order has no normal next fulfilment state.", 400);
        order.Status = next;
        order.UpdatedAt = DemoClock.Now;
        order.Tracking ??= next == "Shipped" ? "NBX-" + order.Id[^3..] : null;
        order.Timeline.Add(new TimelineEvent(DemoClock.Now, next, $"Order advanced to {next}."));
        PortalAudit.Record(context, store, "Order status updated", id, next);
        return Results.Ok(ToOrder(order));
    }

    private static void MapBikeEndpoints(WebApplication app)
    {
        app.MapGet("/api/bikes", (HttpContext context, PortalStore store) => Results.Ok(store.Bikes.Where(bike => IsInternal(context) || bike.CustomerId == Customer(context))));
        app.MapPost("/api/bikes/registrations", RegisterBikeAsync);
    }

    private static async Task<IResult> RegisterBikeAsync(HttpContext context, PortalStore store)
    {
        var input = await context.Request.ReadFromJsonAsync<BikeInput>();
        var serial = input?.Serial ?? "";
        if (input is null || input.Purchased == default || !Regex.IsMatch(serial, "^(AX3|FJC|VTP)-\\d{2}[AB]-\\d{5}$", RegexOptions.IgnoreCase)) return Problem("A valid NordicBike serial number and purchase date are required.", 400);
        var productId = "vinter-pro";
        if (serial.StartsWith("AX3", StringComparison.OrdinalIgnoreCase)) productId = "aurora-x3";
        else if (serial.StartsWith("FJC", StringComparison.OrdinalIgnoreCase)) productId = "fjord-cargo";
        var product = store.Products.Single(item => item.Id == productId);
        var bike = new Bike { Id = "BIKE-" + Guid.NewGuid().ToString("N")[..6].ToUpperInvariant(), CustomerId = Customer(context), ProductId = product.Id, ProductName = product.Name, Serial = serial.ToUpperInvariant(), Purchased = input.Purchased, Registration = "Pending verification", ServiceStatus = "Not assessed" };
        store.Bikes.Add(bike);
        PortalAudit.Record(context, store, "Bike registration submitted", bike.Id, bike.Serial);
        return Results.Created($"/api/bikes/{bike.Id}", bike);
    }

    private static void MapCaseEndpoints(WebApplication app)
    {
        app.MapGet("/api/cases", (HttpContext context, PortalStore store, string? q, string? status) => Results.Ok(store.Cases.Where(item => (IsInternal(context) || item.CustomerId == Customer(context)) && (string.IsNullOrWhiteSpace(q) || Search(store, item).Contains(q, StringComparison.OrdinalIgnoreCase)) && (string.IsNullOrWhiteSpace(status) || item.Status == status)).Select(item => ToCase(store, item, IsInternal(context)))));
        app.MapGet("/api/cases/{id}", (HttpContext context, PortalStore store, string id) => store.Cases.SingleOrDefault(item => item.Id == id && (IsInternal(context) || item.CustomerId == Customer(context))) is { } supportCase ? Results.Ok(ToCase(store, supportCase, IsInternal(context))) : Results.NotFound());
        app.MapPost("/api/cases", CreateCaseAsync);
        app.MapPost("/api/cases/{id}/messages", AddCaseMessageAsync);
        app.MapMethods("/api/cases/{id}", [Patch], UpdateCaseAsync);
    }

    private static async Task<IResult> CreateCaseAsync(HttpContext context, PortalStore store)
    {
        var input = await context.Request.ReadFromJsonAsync<CaseInput>();
        if (input is null || string.IsNullOrWhiteSpace(input.Subject) || string.IsNullOrWhiteSpace(input.Description) || string.IsNullOrWhiteSpace(input.Topic)) return Problem("Subject, description, and topic are required.", 400);
        var supportCase = NewCase(store, Customer(context), input.Subject, input.Topic, input.Description, input.OrderId, input.BikeId);
        PortalAudit.Record(context, store, "Support case created", supportCase.Id, input.Topic);
        return Results.Created($"/api/cases/{supportCase.Id}", ToCase(store, supportCase, false));
    }

    private static async Task<IResult> AddCaseMessageAsync(HttpContext context, PortalStore store, string id)
    {
        var input = await context.Request.ReadFromJsonAsync<MessageInput>();
        var supportCase = store.Cases.SingleOrDefault(item => item.Id == id && (IsInternal(context) || item.CustomerId == Customer(context)));
        if (supportCase is null) return Results.NotFound();
        if (input is null || string.IsNullOrWhiteSpace(input.Body)) return Problem("Message body is required.", 400);
        var visibility = IsInternal(context) && input.Internal ? "Internal" : "Customer";
        supportCase.Messages.Add(new CaseMessage(Actor(context), Role(context), visibility, input.Body.Trim(), DemoClock.Now));
        supportCase.UpdatedAt = DemoClock.Now;
        supportCase.Timeline.Add(new TimelineEvent(DemoClock.Now, visibility == "Internal" ? "Internal note added" : "Message added", Actor(context)));
        PortalAudit.Record(context, store, visibility == "Internal" ? "Internal note added" : "Case message added", id, "Body redacted");
        return Results.Ok(ToCase(store, supportCase, IsInternal(context)));
    }

    private static async Task<IResult> UpdateCaseAsync(HttpContext context, PortalStore store, string id)
    {
        if (!IsInternal(context)) return Results.NotFound();
        var input = await context.Request.ReadFromJsonAsync<CaseUpdate>();
        var supportCase = store.Cases.SingleOrDefault(item => item.Id == id);
        if (supportCase is null) return Results.NotFound();
        if (input is null) return Problem("Update payload is required.", 400);
        if (!string.IsNullOrWhiteSpace(input.Status)) supportCase.Status = input.Status.Trim();
        if (!string.IsNullOrWhiteSpace(input.Priority)) supportCase.Priority = input.Priority.Trim();
        if (!string.IsNullOrWhiteSpace(input.Assignee)) supportCase.Assignee = input.Assignee.Trim();
        var auditAction = "Case updated";
        var auditDetail = "Fields updated";
        if (input.Escalated is not null && input.Escalated.Value != supportCase.Escalated)
        {
            if (!input.Escalated.Value && Role(context) != "Support lead") return Problem("Only a support lead can clear escalation.", 403);
            supportCase.Escalated = input.Escalated.Value;
            auditAction = input.Escalated.Value ? "Case escalated" : "Escalation cleared";
            auditDetail = input.Escalated.Value ? "Case marked escalated" : "Escalation cleared by support lead";
            supportCase.Timeline.Add(new TimelineEvent(DemoClock.Now, input.Escalated.Value ? "Case escalated" : "Escalation cleared", Actor(context)));
        }
        supportCase.UpdatedAt = DemoClock.Now;
        supportCase.Timeline.Add(new TimelineEvent(DemoClock.Now, "Case updated", Actor(context)));
        PortalAudit.Record(context, store, auditAction, id, auditDetail);
        return Results.Ok(ToCase(store, supportCase, true));
    }

    private static void MapServiceRequestEndpoints(WebApplication app)
    {
        app.MapPost("/api/cases/{id}/service-requests", CreateServiceRequestAsync);
        app.MapMethods("/api/service-requests/{id}", [Patch], UpdateServiceRequestAsync);
    }

    private static async Task<IResult> CreateServiceRequestAsync(HttpContext context, PortalStore store, string id)
    {
        if (!IsInternal(context)) return Results.NotFound();
        var input = await context.Request.ReadFromJsonAsync<ServiceInput>();
        var supportCase = store.Cases.SingleOrDefault(item => item.Id == id);
        if (supportCase is null) return Results.NotFound();
        if (input is null || string.IsNullOrWhiteSpace(input.Center)) return Problem("A valid service center is required.", 400);
        var center = input.Center.Trim();
        if (!PortalConfig.ServiceCenters.Contains(center)) return Problem("A valid service center is required.", 400);
        var request = new ServiceRequest { Id = store.NewServiceId(), CaseId = id, Center = center, State = "Awaiting item" };
        store.ServiceRequests.Add(request);
        supportCase.Status = "Waiting for service center";
        supportCase.Timeline.Add(new TimelineEvent(DemoClock.Now, "Service request created", $"{center}; prepaid simulated label prepared."));
        PortalAudit.Record(context, store, "Service request created", request.Id, center);
        return Results.Created($"/api/service-requests/{request.Id}", request);
    }

    private static async Task<IResult> UpdateServiceRequestAsync(HttpContext context, PortalStore store, string id)
    {
        if (!IsInternal(context)) return Results.NotFound();
        var input = await context.Request.ReadFromJsonAsync<ServiceInput>();
        var request = store.ServiceRequests.SingleOrDefault(item => item.Id == id);
        if (request is null) return Results.NotFound();
        var state = input?.State;
        if (input is null || !PortalConfig.RepairStates.Contains(state ?? "")) return Problem("A valid repair state is required.", 400);
        request.State = state ?? "";
        var supportCase = store.Cases.Single(item => item.Id == request.CaseId);
        supportCase.Timeline.Add(new TimelineEvent(DemoClock.Now, "Service request updated", request.State));
        PortalAudit.Record(context, store, "Service request transitioned", id, request.State);
        return Results.Ok(request);
    }

    private static void MapDashboardEndpoints(WebApplication app)
    {
        app.MapGet("/api/dashboard/support", (HttpContext context, PortalStore store) => !IsInternal(context) ? Results.NotFound() : Results.Ok(new { open = store.Cases.Count(item => item.Status is not "Resolved" and not "Closed"), waitingForCustomer = store.Cases.Count(item => item.Status == "Waiting for customer"), escalated = store.Cases.Count(item => item.Escalated), averageFirstResponseHours = 3.4, byTopic = store.Cases.GroupBy(item => item.Topic).Select(group => new { topic = group.Key, count = group.Count() }) }));
        app.MapGet("/api/dashboard/business", (HttpContext context, PortalStore store) => !IsInternal(context) ? Results.NotFound() : Results.Ok(new { confirmedRevenue = store.Orders.Where(item => item.Status is not "Cancelled" and not "Returned").Sum(item => item.Total), statuses = store.Orders.GroupBy(item => item.Status).Select(group => new { status = group.Key, count = group.Count() }), returnRequests = store.Orders.Count(item => item.Status == ReturnRequested) }));
    }

    private static void MapDiagnosticsEndpoints(WebApplication app)
    {
        app.MapGet("/api/diagnostics/events", (HttpContext context, PortalStore store, string? correlationId, string? entityId) => !IsInternal(context) ? Results.NotFound() : Results.Ok(store.Audits.Where(item => (string.IsNullOrWhiteSpace(correlationId) || item.CorrelationId.Contains(correlationId, StringComparison.OrdinalIgnoreCase)) && (string.IsNullOrWhiteSpace(entityId) || item.EntityId.Contains(entityId, StringComparison.OrdinalIgnoreCase))).Take(50)));
    }

    private static void MapSupportIssueEndpoints(WebApplication app)
    {
        app.MapGet("/api/support-issues", (HttpContext context, PortalStore store, string? q, string? status, string? area) => !IsInternal(context) ? Results.NotFound() : Results.Ok(store.SupportIssues.Where(item => (string.IsNullOrWhiteSpace(q) || $"{item.Id} {item.Title} {item.Area} {item.Change} {item.Evidence} {item.Investigation} {item.Acceptance} {string.Join(" ", item.ReproSteps)}".Contains(q, StringComparison.OrdinalIgnoreCase)) && (string.IsNullOrWhiteSpace(status) || item.Status == status) && (string.IsNullOrWhiteSpace(area) || item.Area == area)).Select(item => new { item.Id, item.Title, item.Severity, item.Area, item.Change, item.Evidence, item.Investigation, item.Acceptance, item.ReproSteps, item.Status })));
        app.MapGet("/api/support-issues/{id}", (HttpContext context, PortalStore store, string id) => !IsInternal(context) ? Results.NotFound() : store.SupportIssues.SingleOrDefault(item => item.Id == id) is { } issue ? Results.Ok(issue) : Results.NotFound());
        app.MapMethods("/api/support-issues/{id}", [Patch], UpdateSupportIssueAsync);
    }

    private static async Task<IResult> UpdateSupportIssueAsync(HttpContext context, PortalStore store, string id)
    {
        if (!IsInternal(context)) return Results.NotFound();
        var input = await context.Request.ReadFromJsonAsync<SupportIssueUpdate>();
        var issue = store.SupportIssues.SingleOrDefault(item => item.Id == id);
        if (issue is null) return Results.NotFound();
        if (input is null || string.IsNullOrWhiteSpace(input.Status)) return Problem("A status is required.", 400);
        var index = store.SupportIssues.IndexOf(issue);
        store.SupportIssues[index] = issue with { Status = input.Status.Trim() };
        return Results.Ok(store.SupportIssues[index]);
    }
}
