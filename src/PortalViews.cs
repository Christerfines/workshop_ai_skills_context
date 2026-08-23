using System.Net;

namespace NordicBike.Portal;

public static class PortalViews
{
    public static IResult Problem(string detail, int status) => Results.Problem(detail: detail, statusCode: status, title: "Request could not be completed");

    public static object Cart(PortalStore store, string customer)
    {
        var lines = store.Cart(customer).Select(item => new
        {
            item.Id,
            item.Quantity,
            item.Configuration,
            product = store.Products.Single(product => product.Id == item.ProductId),
            total = store.Products.Single(product => product.Id == item.ProductId).Price * item.Quantity
        }).ToList();
        return new { items = lines, total = lines.Sum(item => item.total), delivery = 0m };
    }

    public static string OrderStatus(Order order, bool _) => order.Status;
    public static object ToOrderView(Order order, bool internalView) => new { order.Id, Status = OrderStatus(order, internalView), order.City, order.CreatedAt, order.UpdatedAt, order.Tracking, order.Total, order.Lines, order.Timeline };
    public static object ToOrder(Order order) => ToOrderView(order, true);

    public static object ToCase(PortalStore store, SupportCase supportCase, bool internalView) => new
    {
        supportCase.Id,
        customer = store.Customers.Single(customer => customer.Id == supportCase.CustomerId),
        supportCase.Subject,
        supportCase.Description,
        supportCase.Topic,
        supportCase.Priority,
        supportCase.Status,
        supportCase.Assignee,
        supportCase.Escalated,
        supportCase.OrderId,
        supportCase.BikeId,
        supportCase.CreatedAt,
        supportCase.UpdatedAt,
        messages = supportCase.Messages.Where(message => internalView || message.Visibility == "Customer"),
        supportCase.Timeline
    };

    public static SupportCase NewCase(PortalStore store, string customer, string subject, string topic, string description, string? order, string? bike)
    {
        var supportCase = new SupportCase
        {
            Id = store.NewCaseId(),
            CustomerId = customer,
            Subject = subject.Trim(),
            Description = description.Trim(),
            Topic = topic,
            Priority = "Normal",
            Status = "New",
            Assignee = null,
            Escalated = false,
            OrderId = order,
            BikeId = bike,
            CreatedAt = DemoClock.Now,
            UpdatedAt = DemoClock.Now
        };
        supportCase.Messages.Add(new CaseMessage(store.Customers.Single(person => person.Id == customer).Name, "Customer", "Customer", description.Trim(), DemoClock.Now));
        supportCase.Timeline.Add(new TimelineEvent(DemoClock.Now, "Case created", topic));
        store.Cases.Insert(0, supportCase);
        return supportCase;
    }

    public static string Search(PortalStore store, SupportCase supportCase) => $"{supportCase.Id} {supportCase.Subject} {store.Customers.Single(customer => customer.Id == supportCase.CustomerId).Name} {supportCase.OrderId} {store.Bikes.SingleOrDefault(bike => bike.Id == supportCase.BikeId)?.Serial}";
    public static string CheckoutFingerprint(CheckoutInput input) => string.Join("|", input.Name.Trim().ToUpperInvariant(), input.Email.Trim().ToUpperInvariant(), input.City.Trim().ToUpperInvariant(), input.IsSimulated);
    public static string H(string? input) => WebUtility.HtmlEncode(input ?? "");
    public static string Date(DateTimeOffset at) => at.ToString("yyyy-MM-dd HH:mm");
    public static string Money(decimal amount) => $"{amount:N0} SEK";
    public static string Badge(string value) => $"<span class='badge'>{H(value)}</span>";
    public static string Events(IEnumerable<TimelineEvent> events) => string.Join("", events.OrderByDescending(item => item.At).Select(item => $"<div class='event'><time>{Date(item.At)}</time><b>{H(item.Title)}</b><span>{H(item.Detail)}</span></div>"));

    public static string ImageTag(Product product, string className = "product-image", string loading = "lazy", bool priority = false) => $"<img class='{H(className)}' src='{H(product.Image)}' alt='{H(product.ImageAlt)}' width='{product.ImageWidth}' height='{product.ImageHeight}' loading='{loading}' decoding='async'{(priority ? " fetchpriority='high'" : "")}>";

    public static IResult Html(HttpContext context, string title, string body)
    {
        var navigation = PortalIdentity.IsInternal(context)
            ? "<a href='/support'>Support</a><a href='/support-issues'>Issues</a><a href='/orders'>Orders</a><a href='/business'>Business</a><a href='/diagnostics'>Diagnostics</a>"
            : "<a href='/shop'>Shop</a><a href='/orders'>My orders</a><a href='/bikes'>My bikes</a><a href='/support'>Support</a><a href='/cart'>Cart</a>";
        var roles = string.Join("", PortalConfig.Roles.Select(role => $"<a href='/role/{role.Key}?returnTo={Uri.EscapeDataString(context.Request.Path + context.Request.QueryString)}'>{H(role.Value.Name)}</a>"));
        var document = $$"""<!doctype html><html lang="en"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>{{H(title)}} | NordicBike</title><link rel="stylesheet" href="/site.css"><link rel="stylesheet" href="/catalog.css"></head><body><header><a class="brand" href="/">NORDIC<span>BIKE</span><small>AB / PORTAL</small></a><nav>{{navigation}}</nav><details class="roles"><summary>{{H(PortalIdentity.Actor(context))}}</summary>{{roles}}</details></header><main>{{body}}</main><footer>Demo date {{DemoClock.Today:yyyy-MM-dd}} <span>support@nordicbike.se · +46 8 555 123 00</span></footer><script src="/portal.js"></script></body></html>""";
        return Results.Content(document, "text/html");
    }

    public static IResult Missing(HttpContext context) => Html(context, "Not found", "<section class='empty'><p class='eyebrow'>404</p><h1>That route is not available here.</h1><a class='button' href='/shop'>Back to shop</a></section>");
}
