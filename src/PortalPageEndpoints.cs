using static NordicBike.Portal.PortalIdentity;
using static NordicBike.Portal.PortalViews;

namespace NordicBike.Portal;

public static class PortalPageEndpoints
{
    public static void MapPortalPages(this WebApplication app)
    {
        app.MapGet("/", (HttpContext context) => Html(context, "Welcome", PortalPages.Home()));
        app.MapGet("/shop", (HttpContext context, PortalStore store, string? q, string? category, string? type, string? tag, int? page) => Html(context, "Shop", PortalPages.Catalog(store, q, category, type, tag, page)));
        app.MapGet("/shop/{id}", (HttpContext context, PortalStore store, string id) => store.Products.SingleOrDefault(item => item.Id == id) is { } product ? Html(context, product.Name, PortalPages.Product(store, product)) : Missing(context));
        app.MapGet("/cart", (HttpContext context, PortalStore store) => Html(context, "Cart", PortalPages.Cart(store, Customer(context))));
        app.MapGet("/checkout", (HttpContext context, PortalStore store) => Html(context, "Checkout", PortalPages.Checkout(store, Customer(context))));
        app.MapGet("/orders", (HttpContext context, PortalStore store) => Html(context, "Orders", PortalPages.Orders(context, store)));
        app.MapGet("/orders/{id}", (HttpContext context, PortalStore store, string id) => store.Orders.SingleOrDefault(item => item.Id == id && (IsInternal(context) || item.CustomerId == Customer(context))) is { } order ? Html(context, order.Id, PortalPages.Order(context, order)) : Missing(context));
        app.MapGet("/bikes", (HttpContext context, PortalStore store) => Html(context, "My bikes", PortalPages.Bikes(context, store)));
        app.MapGet("/support", (HttpContext context, PortalStore store, string? q, string? status) => Html(context, "Support", IsInternal(context) ? PortalPages.Queue(store, q, status) : PortalPages.CustomerSupport(context, store)));
        app.MapGet("/support/{id}", (HttpContext context, PortalStore store, string id) => store.Cases.SingleOrDefault(item => item.Id == id && (IsInternal(context) || item.CustomerId == Customer(context))) is { } supportCase ? Html(context, supportCase.Id, PortalPages.Case(context, store, supportCase)) : Missing(context));
        app.MapGet("/support-issues", (HttpContext context, PortalStore store, string? q, string? status, string? area) => IsInternal(context) ? Html(context, "Support issues", PortalPages.SupportIssues(store, q, status, area)) : Missing(context));
        app.MapGet("/support-issues/{id}", (HttpContext context, PortalStore store, string id) => IsInternal(context) && store.SupportIssues.SingleOrDefault(item => item.Id == id) is { } issue ? Html(context, issue.Id, PortalPages.SupportIssue(issue)) : Missing(context));
        app.MapGet("/business", (HttpContext context, PortalStore store) => IsInternal(context) ? Html(context, "Business", PortalPages.Business(store)) : Missing(context));
        app.MapGet("/diagnostics", (HttpContext context, PortalStore store, string? correlationId, string? entityId) => IsInternal(context) ? Html(context, "Diagnostics", PortalPages.Diagnostics(store, correlationId, entityId)) : Missing(context));
    }
}
