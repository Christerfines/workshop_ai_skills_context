namespace NordicBike.Portal;

public static class DemoClock
{
    public static readonly DateOnly Today = new(2026, 8, 19);
    public static DateTimeOffset Now => new(Today.ToDateTime(new TimeOnly(10, 0), DateTimeKind.Utc));
}

public sealed record Product(string Id, string Name, string Category, decimal Price, string Description, string Mark, string Specs, string[] CompatibleBikes, string[] Options);
public sealed record Customer(string Id, string Name, string Email, string City);
public sealed record CartItem(string Id, string ProductId, int Quantity, string? Configuration);
public sealed record OrderLine(string ProductId, string Name, int Quantity, decimal UnitPrice, string? Configuration);
public sealed record TimelineEvent(DateTimeOffset At, string Title, string Detail);
public sealed record CaseMessage(string Author, string Role, string Visibility, string Body, DateTimeOffset At);
public sealed record AuditEvent(string Id, string CorrelationId, string Actor, string Action, string EntityId, string Detail, DateTimeOffset At);
public sealed record SupportIssue(string Id, string Title, string Severity, string Area, string Change, string Evidence, string Investigation, string Acceptance, string Status);

public sealed class Order
{
    public required string Id { get; init; }
    public required string CustomerId { get; init; }
    public required List<OrderLine> Lines { get; init; }
    public required string Status { get; set; }
    public required string City { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; set; }
    public string? Tracking { get; set; }
    public List<TimelineEvent> Timeline { get; } = [];
    public decimal Total => Lines.Sum(line => line.Quantity * line.UnitPrice);
}

public sealed class Bike
{
    public required string Id { get; init; }
    public required string CustomerId { get; init; }
    public required string ProductId { get; init; }
    public required string ProductName { get; init; }
    public required string Serial { get; init; }
    public required DateOnly Purchased { get; init; }
    public required string Registration { get; set; }
    public required string ServiceStatus { get; set; }
}

public sealed class SupportCase
{
    public required string Id { get; init; }
    public required string CustomerId { get; init; }
    public required string Subject { get; init; }
    public required string Description { get; init; }
    public required string Topic { get; init; }
    public required string Priority { get; set; }
    public required string Status { get; set; }
    public string? Assignee { get; set; }
    public bool Escalated { get; set; }
    public string? OrderId { get; init; }
    public string? BikeId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; set; }
    public List<CaseMessage> Messages { get; } = [];
    public List<TimelineEvent> Timeline { get; } = [];
}

public sealed class ServiceRequest
{
    public required string Id { get; init; }
    public required string CaseId { get; init; }
    public required string Center { get; init; }
    public required string State { get; set; }
}

public sealed class PortalStore
{
    private int _order = 8;
    private int _case = 206;
    private int _audit = 1;
    private int _service = 1;
    public List<Product> Products { get; } =
    [
        new("aurora-x3", "Aurora X3", "City e-bike", 34900, "A durable daily commuter for Nordic weather.", "A", "250W mid-drive · 720Wh integrated battery · up to 80 km", ["aurora-x3", "vinter-pro"], ["S", "M", "L", "Nordic Black", "Fjord Blue"]),
        new("fjord-cargo", "Fjord Cargo", "Cargo e-bike", 44900, "A reinforced family cargo bike.", "F", "250W mid-drive · 900Wh battery · 100kg cargo", ["fjord-cargo"], ["Slate Grey"]),
        new("vinter-pro", "Vinter Pro", "Winter e-bike", 37900, "Studded confidence for the coldest commute.", "V", "250W rear hub · 720Wh · studded tires", ["aurora-x3", "vinter-pro"], ["S", "M", "L", "Arctic White"]),
        new("powerpack-720", "PowerPack 720", "Spare battery", 6900, "Replacement battery for Aurora X3 and Vinter Pro.", "720", "720Wh · 4.5h charge", ["aurora-x3", "vinter-pro"], []),
        new("powerpack-900", "PowerPack 900", "Spare battery", 8900, "Replacement battery for Fjord Cargo.", "900", "900Wh · 5.5h charge", ["fjord-cargo"], []),
        new("pannier-bag", "Pannier bag", "Accessory", 1200, "Waterproof 25L storage.", "25L", "Waterproof · 25L", [], []),
        new("led-light-set", "LED light set", "Accessory", 450, "USB rechargeable visibility.", "LED", "Front and rear · USB", [], []),
        new("frame-lock", "Frame lock", "Accessory", 950, "ART-approved frame security.", "LOCK", "ART-approved", [], []),
        new("phone-mount", "Phone mount", "Accessory", 350, "Weatherproof route guidance.", "MAP", "Weatherproof", [], [])
    ];
    public List<Customer> Customers { get; } = [new("NB-CUST-10041", "Anna Karlsson", "anna.karlsson@example.se", "Malmo"), new("NB-CUST-10042", "Erik Svensson", "erik.svensson@example.se", "Stockholm"), new("NB-CUST-10043", "Lena Bjork", "lena.bjork@example.se", "Gothenburg"), new("NB-CUST-10044", "Johan Lindqvist", "johan.lindqvist@example.se", "Umea")];
    public List<Order> Orders { get; } = [];
    public List<Bike> Bikes { get; } = [];
    public List<SupportCase> Cases { get; } = [];
    public List<ServiceRequest> ServiceRequests { get; } = [];
    public List<AuditEvent> Audits { get; } = [];
    public List<SupportIssue> SupportIssues { get; } = [];
    public Dictionary<string, List<CartItem>> Carts { get; } = [];
    public PortalStore() => Seed();
    public string NewOrderId() => $"NB-ORD-{DemoClock.Today:yyyyMMdd}-{++_order:D3}";
    public string NewCaseId() => $"NB-CASE-{++_case:D5}";
    public string NewServiceId() => $"SR-{++_service:D4}";
    public List<CartItem> Cart(string customerId) => Carts.TryGetValue(customerId, out var cart) ? cart : Carts[customerId] = [];
    public void Audit(string correlation, string actor, string action, string entityId, string detail) => Audits.Insert(0, new AuditEvent($"AUD-{_audit++:D5}", correlation, actor, action, entityId, detail, DemoClock.Now));
    private Order SeedOrder(string id, string customer, string status, string city, string product, int daysAgo, string? tracking = null)
    {
        var item = Products.Single(productItem => productItem.Id == product);
        var at = DemoClock.Now.AddDays(daysAgo);
        var order = new Order { Id = id, CustomerId = customer, Lines = [new OrderLine(item.Id, item.Name, 1, item.Price, null)], Status = status, City = city, CreatedAt = at, UpdatedAt = at.AddHours(3), Tracking = tracking };
        order.Timeline.Add(new TimelineEvent(at, "Order confirmed", "Your simulated order was accepted.")); order.Timeline.Add(new TimelineEvent(at.AddHours(3), status, $"Order is now {status.ToLowerInvariant()}.")); return order;
    }
    private void Seed()
    {
        Orders.AddRange([SeedOrder("NB-ORD-20250310-001", "NB-CUST-10041", "Delivered", "Malmo", "aurora-x3", -120, "NBX-77124"), SeedOrder("NB-ORD-20260722-002", "NB-CUST-10041", "Shipped", "Malmo", "led-light-set", -4, "NBX-88102"), SeedOrder("NB-ORD-20260801-003", "NB-CUST-10042", "Processing", "Stockholm", "fjord-cargo", -2), SeedOrder("NB-ORD-20260816-004", "NB-CUST-10043", "Confirmed", "Gothenburg", "vinter-pro", -1), SeedOrder("NB-ORD-20260701-005", "NB-CUST-10043", "Delivered", "Gothenburg", "pannier-bag", -49, "NBX-77293"), SeedOrder("NB-ORD-20260502-006", "NB-CUST-10044", "Returned", "Umea", "phone-mount", -100), SeedOrder("NB-ORD-20260810-007", "NB-CUST-10042", "Cancelled", "Stockholm", "frame-lock", -9), SeedOrder("NB-ORD-20260620-008", "NB-CUST-10044", "Return requested", "Umea", "powerpack-720", -60)]);
        Bikes.AddRange([new Bike { Id = "BIKE-001", CustomerId = "NB-CUST-10041", ProductId = "aurora-x3", ProductName = "Aurora X3", Serial = "AX3-25A-00417", Purchased = new(2025, 3, 10), Registration = "Verified", ServiceStatus = "Ready to ride" }, new Bike { Id = "BIKE-002", CustomerId = "NB-CUST-10042", ProductId = "fjord-cargo", ProductName = "Fjord Cargo", Serial = "FJC-25B-00113", Purchased = new(2025, 12, 4), Registration = "Verified", ServiceStatus = "Ready to ride" }, new Bike { Id = "BIKE-003", CustomerId = "NB-CUST-10043", ProductId = "vinter-pro", ProductName = "Vinter Pro", Serial = "VTP-24A-00201", Purchased = new(2024, 7, 15), Registration = "Verified", ServiceStatus = "Inspection booked" }]);
        SupportIssues.AddRange([
            new("ISSUE-001", "Checkout redirect points at a nonexistent order", "Easy", "Checkout", "Change only the client redirect formatting after a successful checkout.", "customer report, browser URL, API response, and two correlated request logs", "compare the created canonical ID with the UI navigation value", "the wrong redirect reproduces reliably and the fix restores /orders/{id}", "Planned"),
            new("ISSUE-002", "Cart total does not refresh after a quantity update", "Easy", "Cart", "Preserve the API mutation but render the summary from a stale client snapshot.", "support screenshot, cart audit event, and API response with the correct quantity", "isolate UI state from API mutation behavior", "quantity updates persist and the summary refreshes from current data", "Planned"),
            new("ISSUE-003", "Shipped order remains Processing for a customer", "Moderate", "Orders", "Omit Shipped from the customer-status projection but retain it for operations.", "customer report, business screenshot, status-change audit, and partial API payload", "trace one state transition through entity, DTO, and view mapping", "customer view matches internal Shipped state after the fix", "Planned"),
            new("ISSUE-004", "Support agent cannot create a Malmo service request", "Moderate", "Support", "Add whitespace or normalization mismatch to the selected service-center value.", "agent report, 400 problem-details response, submitted payload, unrelated old warnings", "find the controlled-vocabulary validation seam; do not confuse it with warranty eligibility", "valid Malmo requests succeed after the fix", "Planned"),
            new("ISSUE-005", "Vinter Pro owner is recommended an incompatible battery", "Moderate", "Catalog", "Reverse one UI compatibility lookup while catalog API data stays correct.", "product screenshot, catalog API response, customer conversation, and a distracting warranty note", "diagnose UI/API inconsistency rather than changing policy data", "UI and API recommendations match after the fix", "Planned"),
            new("ISSUE-006", "Support agent clears escalation with no audit trail", "Hard", "Support workspace", "Permit one PATCH branch to clear escalation without checking the role or recording an audit event.", "case timeline, diagnostics extract, partial console logs, and compensation discussion noise", "compare all authorization paths and use missing observability evidence carefully", "only support leads can clear escalation and each clearance is audited", "Planned"),
            new("ISSUE-007", "Customer sees internal note after switching roles", "Hard", "Support workspace", "Cache an internal case projection and reuse it after the role cookie changes.", "exact reproduction sequence, navigation timing, sanitized network capture, unrelated correlation IDs", "reason about state lifetime, identity boundaries, and projection filtering", "customer view never leaks internal notes after role changes", "Planned"),
            new("ISSUE-008", "Slow connection creates duplicate checkout orders", "Hard", "Checkout", "Remove client submit protection and process repeated POSTs without idempotency handling.", "similar orders, two correlation IDs, timing data, and unrelated payment terminology", "distinguish retry from refresh, prove source, then propose idempotency without blocking later valid purchases", "duplicate checkout orders no longer occur after the fix", "Planned")
        ]);
        AddCase("NB-CASE-00201", "NB-CUST-10041", "Intermittent charging on Aurora X3", "Bike or battery fault", "High", "Waiting for service center", "Sofia Nilsson", false, "NB-ORD-20250310-001", "BIKE-001", -5);
        AddCase("NB-CASE-00202", "NB-CUST-10042", "Tracking reference has not updated", "Order and delivery", "Normal", "In progress", "Oskar Bergman", false, "NB-ORD-20260801-003", null, -2);
        AddCase("NB-CASE-00203", "NB-CUST-10043", "Requesting a compensation payment", "Warranty and repair", "High", "In progress", "Sofia Nilsson", true, "NB-ORD-20260701-005", "BIKE-003", -6);
        AddCase("NB-CASE-00204", "NB-CUST-10044", "Return request for PowerPack 720", "Return request", "Normal", "New", null, false, "NB-ORD-20260620-008", null, -1);
        AddCase("NB-CASE-00205", "NB-CUST-10041", "Can I add a phone mount?", "Product question", "Low", "Resolved", "Oskar Bergman", false, null, "BIKE-001", -12);
        AddCase("NB-CASE-00206", "NB-CUST-10043", "Winter range question", "Product question", "Normal", "Waiting for customer", "Sofia Nilsson", false, "NB-ORD-20260816-004", null, -3);
        foreach (var order in Orders) Audit("seed", "System", "Seeded order", order.Id, order.Status);
    }
    private void AddCase(string id, string customer, string subject, string topic, string priority, string status, string? assignee, bool escalated, string? orderId, string? bikeId, int daysAgo)
    {
        var at = DemoClock.Now.AddDays(daysAgo); var supportCase = new SupportCase { Id = id, CustomerId = customer, Subject = subject, Description = "Seeded demo case for the NordicBike portal.", Topic = topic, Priority = priority, Status = status, Assignee = assignee, Escalated = escalated, OrderId = orderId, BikeId = bikeId, CreatedAt = at, UpdatedAt = at.AddHours(2) };
        supportCase.Messages.Add(new CaseMessage(Customers.Single(item => item.Id == customer).Name, "Customer", "Customer", "I need help with this item.", at)); if (escalated) supportCase.Messages.Add(new CaseMessage("Sofia Nilsson", "Support lead", "Internal", "Escalated because the cash compensation request exceeds support authority.", at.AddHours(1))); supportCase.Timeline.Add(new TimelineEvent(at, "Case created", topic)); supportCase.Timeline.Add(new TimelineEvent(at.AddHours(2), status, "Seeded workflow state.")); Cases.Add(supportCase);
    }
}