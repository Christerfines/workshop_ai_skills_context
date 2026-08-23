namespace NordicBike.Portal;

using static PortalIdentity;
using static PortalViews;

public static class PortalPages
{
    private const string HomeMarkup = "<section class='hero'><div><p class='eyebrow'>NORDICBIKE / EVERYDAY ELECTRIC</p><h1>Built for the long way home.</h1><p>Shop, track and get support through every season.</p><a class='button signal' href='/shop'>Explore the bikes</a></div><div class='hero-bike'><img class='product-image' src='/images/products/catalog/aurora-x3.jpg' alt='Aurora X3 city e-bike in side view' width='960' height='640' fetchpriority='high'><small>AURORA X3</small></div></section><section class='feature-row'><article><b>3</b><span>service centers</span></article><article><b>24</b><span>month bike warranty</span></article><article><b>1</b><span>place to manage your ride</span></article></section>";
    private const string Selected = "selected";

    public static string Home() => HomeMarkup;

    public static string Catalog(PortalStore store, string? q, string? category, string? type, string? tag, int? page)
    {
        var filtered = store.Products.Where(item =>
            (string.IsNullOrWhiteSpace(q) || item.Name.Contains(q, StringComparison.OrdinalIgnoreCase) || item.Description.Contains(q, StringComparison.OrdinalIgnoreCase) || item.Tags.Any(value => value.Contains(q, StringComparison.OrdinalIgnoreCase))) &&
            (string.IsNullOrWhiteSpace(category) || item.Category.Equals(category, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrWhiteSpace(type) || item.Type.Equals(type, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrWhiteSpace(tag) || item.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase)))
            .OrderBy(item => item.Category)
            .ThenBy(item => item.Name)
            .ToList();
        var pageSize = 24;
        var pageCount = Math.Max(1, (int)Math.Ceiling(filtered.Count / (double)pageSize));
        var currentPage = Math.Clamp(page ?? 1, 1, pageCount);
        var products = filtered.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        var categories = store.Products.Select(item => item.Category).Distinct().OrderBy(item => item);
        var types = store.Products.Select(item => item.Type).Distinct().OrderBy(item => item);
        var categoryOptions = "<option value=''>All categories</option>" + string.Join("", categories.Select(item => $"<option value='{H(item)}' {(item.Equals(category, StringComparison.OrdinalIgnoreCase) ? Selected : "")}>{H(item)}</option>"));
        var typeOptions = "<option value=''>All types</option>" + string.Join("", types.Select(item => $"<option value='{H(item)}' {(item.Equals(type, StringComparison.OrdinalIgnoreCase) ? Selected : "")}>{H(item)}</option>"));
        var pagination = pageCount == 1 ? "" : $"<nav class='pagination' aria-label='Catalog pages'>{string.Join("", Enumerable.Range(1, pageCount).Select(number => $"<a class='filter {(number == currentPage ? "active" : "")}' href='/shop?{CatalogQuery(q, category, type, tag, number)}'>{number}</a>"))}</nav>";
        return $$"""<section class='page-head'><p class='eyebrow'>STORE / NORDIC CONDITIONS</p><h1>Find your everyday electric.</h1><form class='search' method='get'><input name='q' value='{{H(q)}}' placeholder='Search products'><select name='category'>{{categoryOptions}}</select><select name='type'>{{typeOptions}}</select><button>Filter</button></form><p class='catalog-count'>{{filtered.Count}} products</p></section><section class='catalog'>{{string.Join("", products.Select(ProductCard))}}</section>{{pagination}}""";
    }

    private static string ProductCard(Product product)
    {
        var configuration = product.Options.Length == 0 ? "" : $"<label>Configuration<select name='configuration'>{string.Join("", product.Options.Select(option => $"<option>{H(option)}</option>"))}</select></label>";
        return $"<article class='card'><div class='product-media card-media'>{ImageTag(product, "product-image", "lazy")}</div><p class='eyebrow'>{H(product.Category)}</p><h2>{H(product.Name)}</h2><p>{H(product.Description)}</p><div><strong>{Money(product.Price)}</strong><a class='link' href='/shop/{Uri.EscapeDataString(product.Id)}'>Details</a></div><form class='stack js-form card-action' data-api='/api/cart/items' data-redirect='/cart'><input type='hidden' name='productId' value='{H(product.Id)}'><input type='hidden' name='quantity' value='1'>{configuration}<button class='button'>Add to cart</button></form></article>";
    }

    public static string Product(PortalStore store, Product product)
    {
        var gallery = product.Gallery.Length == 0 ? "" : $"<div class='product-gallery'>{string.Join("", product.Gallery.Select(image => $"<img src='{H(image)}' alt='{H(product.ImageAlt)}' loading='lazy' decoding='async'>"))}</div>";
        var configuration = product.Options.Length == 0 ? "" : $"<label>Configuration<select name='configuration'>{string.Join("", product.Options.Select(option => $"<option>{H(option)}</option>"))}</select></label>";
        return $$"""<section class='product'><div class='product-media product-detail-media'>{{ImageTag(product, "product-image large", "eager", true)}}{{gallery}}</div><div><p class='eyebrow'>{{H(product.Category)}}</p><h1>{{H(product.Name)}}</h1><p class='lead'>{{H(product.Description)}}</p><p class='spec'>{{H(product.Specs)}}</p><p class='notice'>Recommended battery: {{H(RecommendedCatalogBattery(store, product))}}</p><strong class='price'>{{Money(product.Price)}}</strong><form class='stack js-form' data-api='/api/cart/items' data-redirect='/cart'><input type='hidden' name='productId' value='{{H(product.Id)}}'><input type='hidden' name='quantity' value='1'>{{configuration}}<button class='button'>Add to cart</button></form></div></section>""";
    }

    private static string RecommendedCatalogBattery(PortalStore store, Product product) => store.Products.Where(item => item.Type.Equals("Battery", StringComparison.OrdinalIgnoreCase) && item.CompatibleBikes.Contains(product.Id, StringComparer.OrdinalIgnoreCase)).OrderBy(item => item.Price).Select(item => item.Name).FirstOrDefault() ?? "No battery recommendation";

    private static string CatalogQuery(string? q, string? category, string? type, string? tag, int page)
    {
        var values = new List<string>();
        if (!string.IsNullOrWhiteSpace(q)) values.Add($"q={Uri.EscapeDataString(q)}");
        if (!string.IsNullOrWhiteSpace(category)) values.Add($"category={Uri.EscapeDataString(category)}");
        if (!string.IsNullOrWhiteSpace(type)) values.Add($"type={Uri.EscapeDataString(type)}");
        if (!string.IsNullOrWhiteSpace(tag)) values.Add($"tag={Uri.EscapeDataString(tag)}");
        values.Add($"page={page}");
        return string.Join("&", values);
    }

    public static string Cart(PortalStore store, string customer)
    {
        var items = store.Cart(customer);
        if (items.Count == 0) return "<section class='empty'><p class='eyebrow'>YOUR CART</p><h1>Nothing is waiting yet.</h1><a class='button' href='/shop'>Browse products</a></section>";
        var total = items.Sum(item => store.Products.Single(product => product.Id == item.ProductId).Price * item.Quantity);
        return $$"""<section class='page-head compact'><p class='eyebrow'>YOUR CART</p><h1>Ready when you are.</h1></section><section class='two'><article class='panel cart-lines'>{{string.Join("", items.Select(item => { var product = store.Products.Single(product => product.Id == item.ProductId); var lineTotal = product.Price * item.Quantity; return $"<div class='row cart-line' data-line-id='{item.Id}'><span><b>{H(product.Name)}</b><small>{H(item.Configuration)}</small></span><form class='inline-form js-form' data-api='/api/cart/items/{item.Id}' data-method='PATCH' data-cart='true'><input type='hidden' name='productId' value='{product.Id}'><input type='number' name='quantity' min='1' value='{item.Quantity}'><button class='button outline'>Update</button></form><b data-line-total>{Money(lineTotal)}</b><button class='delete' data-api='/api/cart/items/{item.Id}'>×</button></div>"; }))}}</article><aside class='panel'><h2>Order summary</h2><div class='row'><span>Subtotal</span><b>{{Money(total)}}</b></div><div class='row'><span>Simulated delivery</span><b>0 SEK</b></div><div class='row total'><span>Total</span><b data-cart-total>{{Money(total)}}</b></div><a class='button' href='/checkout'>Continue to checkout</a></aside></section>""";
    }

    public static string Checkout(PortalStore store, string customer)
    {
        if (store.Cart(customer).Count == 0) return "<section class='empty'><h1>Your cart is empty.</h1><a class='button' href='/shop'>Browse products</a></section>";
        var person = store.Customers.Single(item => item.Id == customer);
        return $$"""<section class='page-head compact'><p class='eyebrow'>SIMULATED CHECKOUT</p><h1>Finish your order.</h1><p>There is no payment collection in this demo.</p></section><section class='panel form'><form class='stack js-form' data-api='/api/orders' data-order='true'><label>Display name<input name='name' value='{{H(person.Name)}}' required></label><label>Email<input name='email' value='{{H(person.Email)}}' type='email' required></label><label>Delivery city<input name='city' value='{{H(person.City)}}' required></label><label class='check'><input name='isSimulated' type='checkbox' value='true' required>I understand this is a simulated order.</label><button class='button'>Place simulated order</button></form></section>""";
    }

    public static string Orders(HttpContext context, PortalStore store)
    {
        var orders = store.Orders.Where(item => IsInternal(context) || item.CustomerId == Customer(context)).OrderByDescending(item => item.CreatedAt);
        return $$"""<section class='page-head compact'><p class='eyebrow'>{{(IsInternal(context) ? "OPERATIONS / ORDERS" : "MY ORDERS")}}</p><h1>{{(IsInternal(context) ? "Fulfilment at a glance." : "Every order, in one place.")}}</h1></section><section class='table'><table><thead><tr><th>Order</th><th>Customer</th><th>Placed</th><th>Status</th><th>Total</th><th></th></tr></thead><tbody>{{string.Join("", orders.Select(item => $"<tr><td>{H(item.Id)}</td><td>{H(store.Customers.Single(customer => customer.Id == item.CustomerId).Name)}</td><td>{Date(item.CreatedAt)}</td><td>{Badge(OrderStatus(item, IsInternal(context)))}</td><td>{Money(item.Total)}</td><td><a class='link' href='/orders/{Uri.EscapeDataString(item.Id)}'>View</a></td></tr>"))}}</tbody></table></section>""";
    }

    public static string Order(HttpContext context, Order order)
    {
        var action = "";
        if (IsInternal(context) && order.Status is "Confirmed" or "Processing" or "Shipped") action = $"<button class='button advance' data-api='/api/orders/{order.Id}/advance'>Advance fulfilment</button>";
        else if (!IsInternal(context) && order.Status == "Delivered") action = $"<button class='button outline return' data-api='/api/orders/{order.Id}/return-requests'>Request a return</button>";
        return $$"""<section class='detail'><div><p class='eyebrow'>ORDER / {{H(order.Id)}}</p><h1>{{Badge(OrderStatus(order, IsInternal(context)))}}</h1><p>Delivery to {{H(order.City)}} · placed {{Date(order.CreatedAt)}}</p></div>{{action}}</section><section class='two'><article class='panel'><h2>Items</h2>{{string.Join("", order.Lines.Select(line => $"<div class='row'><span>{H(line.Name)} {H(line.Configuration)}</span><span>{line.Quantity} × {Money(line.UnitPrice)}</span></div>"))}}<div class='row total'><span>Total</span><b>{{Money(order.Total)}}</b></div>{{(order.Tracking is null ? "" : $"<p class='notice'>NordicBike Logistics · tracking {H(order.Tracking)}</p>")}}</article><article class='panel'><h2>Order timeline</h2>{{Events(order.Timeline)}}</article></section>""";
    }

    public static string Bikes(HttpContext context, PortalStore store) => $$"""<section class='page-head compact'><p class='eyebrow'>MY BIKES</p><h1>Everything that carries you.</h1></section><section class='cards'>{{string.Join("", store.Bikes.Where(item => IsInternal(context) || item.CustomerId == Customer(context)).Select(item => $"<article class='card'><div class='bike'>{H(item.ProductName[..1])}</div><p class='eyebrow'>{H(item.Registration)}</p><h2>{H(item.ProductName)}</h2><p>{H(item.Serial)}</p><dl><dt>Purchased</dt><dd>{item.Purchased:yyyy-MM-dd}</dd><dt>Service</dt><dd>{H(item.ServiceStatus)}</dd></dl><a class='link' href='/support'>Get support</a></article>"))}}</section><section class='panel form'><p class='eyebrow'>REGISTER A BIKE</p><h2>Purchased elsewhere?</h2><form class='stack js-form' data-api='/api/bikes/registrations' data-redirect='/bikes'><label>Serial number<input name='serial' placeholder='AX3-25A-00417' required></label><label>Purchase date<input name='purchased' type='date' required></label><button class='button'>Request verification</button></form></section>""";

    public static string CustomerSupport(HttpContext context, PortalStore store)
    {
        var orders = store.Orders.Where(item => item.CustomerId == Customer(context));
        var bikes = store.Bikes.Where(item => item.CustomerId == Customer(context));
        return $$"""<section class='page-head compact'><p class='eyebrow'>SUPPORT</p><h1>We will help you keep moving.</h1><p>support@nordicbike.se · +46 8 555 123 00 · Monday-Friday 09:00-17:00 CET</p></section><section class='two'><article class='panel'><h2>Start a case</h2><form class='stack js-form' data-api='/api/cases' data-case='true'><label>Subject<input name='subject' required></label><label>Topic<select name='topic'><option>Order and delivery</option><option>Product question</option><option>Bike or battery fault</option><option>Warranty and repair</option><option>Return request</option><option>Other</option></select></label><label>Related order<select name='orderId'><option value=''>None</option>{{string.Join("", orders.Select(item => $"<option>{item.Id}</option>"))}}</select></label><label>Related bike<select name='bikeId'><option value=''>None</option>{{string.Join("", bikes.Select(item => $"<option value='{item.Id}'>{H(item.ProductName)} · {H(item.Serial)}</option>"))}}</select></label><label>Description<textarea name='description' required></textarea></label><label>Simulated attachment metadata<input name='attachment' placeholder='photo.jpg (1.2 MB)'></label><button class='button'>Submit case</button></form></article><article class='panel'><h2>Your cases</h2>{{string.Join("", store.Cases.Where(item => item.CustomerId == Customer(context)).OrderByDescending(item => item.UpdatedAt).Select(item => $"<a class='case' href='/support/{item.Id}'>{Badge(item.Status)}<b>{H(item.Subject)}</b><small>{item.Id} · {Date(item.UpdatedAt)}</small></a>"))}}</article></section>""";
    }

    public static string Queue(PortalStore store, string? q, string? status)
    {
        var cases = store.Cases.Where(item => (string.IsNullOrWhiteSpace(q) || Search(store, item).Contains(q, StringComparison.OrdinalIgnoreCase)) && (string.IsNullOrWhiteSpace(status) || item.Status == status));
        return $$"""<section class='page-head compact'><p class='eyebrow'>SUPPORT WORKSPACE</p><h1>Keep the next customer moving.</h1><form class='search'><input name='q' value='{{H(q)}}' placeholder='Search case, customer, order or serial'><select name='status'><option value=''>All statuses</option>{{string.Join("", store.Cases.Select(item => item.Status).Distinct().Select(item => $"<option {(item == status ? Selected : "")}>{H(item)}</option>"))}}</select><button>Filter</button></form></section><section class='table'><table><thead><tr><th>Case</th><th>Customer</th><th>Subject</th><th>Priority</th><th>Status</th><th>Owner</th><th></th></tr></thead><tbody>{{string.Join("", cases.Select(item => $"<tr><td>{item.Id} {(item.Escalated ? "<i>Escalated</i>" : "")}</td><td>{H(store.Customers.Single(customer => customer.Id == item.CustomerId).Name)}</td><td>{H(item.Subject)}</td><td>{Badge(item.Priority)}</td><td>{Badge(item.Status)}</td><td>{H(item.Assignee ?? "Unassigned")}</td><td><a class='link' href='/support/{item.Id}'>Open</a></td></tr>"))}}</tbody></table></section>""";
    }

    public static string SupportIssues(PortalStore store, string? q, string? status, string? area)
    {
        var issues = store.SupportIssues.Where(item => (string.IsNullOrWhiteSpace(q) || $"{item.Id} {item.Title} {item.Area} {item.Change} {item.Evidence} {item.Investigation} {item.Acceptance} {string.Join(" ", item.ReproSteps)}".Contains(q, StringComparison.OrdinalIgnoreCase)) && (string.IsNullOrWhiteSpace(status) || item.Status == status) && (string.IsNullOrWhiteSpace(area) || item.Area == area));
        var statuses = store.SupportIssues.Select(item => item.Status).Distinct().OrderBy(item => item);
        var areas = store.SupportIssues.Select(item => item.Area).Distinct().OrderBy(item => item);
        return $$"""<section class='page-head compact'><p class='eyebrow'>SUPPORT ISSUES</p><h1>Track the planned fixes.</h1><form class='search'><input name='q' value='{{H(q)}}' placeholder='Search issue, area, evidence or repro steps'><select name='status'><option value=''>All statuses</option>{{string.Join("", statuses.Select(item => $"<option {(item == status ? Selected : "")}>{H(item)}</option>"))}}</select><select name='area'><option value=''>All areas</option>{{string.Join("", areas.Select(item => $"<option {(item == area ? Selected : "")}>{H(item)}</option>"))}}</select><button>Filter</button></form></section><section class='table'><table><thead><tr><th>Issue</th><th>Area</th><th>Severity</th><th>Status</th><th>Acceptance</th></tr></thead><tbody>{{string.Join("", issues.Select(item => $"<tr><td><a class='link' href='/support-issues/{item.Id}'><b>{H(item.Id)}</b> {H(item.Title)}</a></td><td>{H(item.Area)}</td><td>{Badge(item.Severity)}</td><td>{Badge(item.Status)}</td><td>{H(item.Acceptance)}</td></tr>"))}}</tbody></table></section>""";
    }

    public static string SupportIssue(SupportIssue issue) => $$"""<section class='detail'><div><p class='eyebrow'>SUPPORT ISSUE / {{H(issue.Id)}}</p><h1>{{H(issue.Title)}}</h1><p>{{Badge(issue.Area)}} {{Badge(issue.Severity)}} {{Badge(issue.Status)}}</p><p class='lead'>{{IssueContext(issue)}}</p></div><a class='button outline' href='/support-issues'>Back to issues</a></section><section class='two'><article class='panel'><h2>Summary</h2><p>{{H(issue.Change)}}</p><dl><dt>Reproduction</dt><dd><ol>{{string.Join("", issue.ReproSteps.Select(step => $"<li>{H(step)}</li>"))}}</ol></dd><dt>Evidence</dt><dd>{{H(issue.Evidence)}}</dd><dt>Investigation</dt><dd>{{H(issue.Investigation)}}</dd><dt>Acceptance</dt><dd>{{H(issue.Acceptance)}}</dd></dl></article><article class='panel'><h2>Update status</h2><form class='stack js-form' data-api='/api/support-issues/{{issue.Id}}' data-method='PATCH'><label>Status<select name='status'><option {{(issue.Status == "Planned" ? Selected : "")}}>Planned</option><option {{(issue.Status == "In progress" ? Selected : "")}}>In progress</option><option {{(issue.Status == "Fixed" ? Selected : "")}}>Fixed</option></select></label><button class='button'>Save status</button></form></article></section>""";

    private static string IssueContext(SupportIssue issue) => $"{issue.Severity} issue in {issue.Area.ToLowerInvariant()}. The visible failure is {issue.Title.ToLowerInvariant()}, and the investigation should focus on {issue.Investigation}.";

    public static string Case(HttpContext context, PortalStore store, SupportCase supportCase)
    {
        var internalView = IsInternal(context);
        var person = store.Customers.Single(customer => customer.Id == supportCase.CustomerId);
        var messages = supportCase.Messages.Where(message => internalView || message.Visibility == "Customer");
        var controls = "";
        if (internalView) controls = $$"""<article class='panel'><p class='eyebrow'>CASE CONTROLS</p><form class='stack js-form' data-api='/api/cases/{{supportCase.Id}}' data-method='PATCH'><label>Status<select name='status'>{{string.Join("", new[] { "New", "Waiting for customer", "In progress", "Waiting for service center", "Resolved", "Closed" }.Select(value => $"<option {(value == supportCase.Status ? Selected : "")}>{value}</option>"))}}</select></label><label>Priority<select name='priority'><option>Low</option><option selected>Normal</option><option>High</option></select></label><label>Owner<select name='assignee'><option></option><option>Sofia Nilsson</option><option>Oskar Bergman</option></select></label><label class='check'><input name='escalated' type='checkbox' value='true' {{(supportCase.Escalated ? "checked" : "")}}>Escalated</label><button class='button'>Save case</button></form><form class='stack note js-form' data-api='/api/cases/{{supportCase.Id}}/messages'><input type='hidden' name='internal' value='true'><label>Internal note<textarea name='body' required></textarea></label><button class='button outline'>Add note</button></form><form class='stack note js-form' data-api='/api/cases/{{supportCase.Id}}/service-requests'><label>Service center<select name='center'><option>Stockholm</option><option>Gothenburg</option><option>Malmo</option></select></label><button class='button outline'>Create service request</button></form></article>""";
        return $$"""<section class='detail'><div><p class='eyebrow'>SUPPORT CASE / {{supportCase.Id}}</p><h1>{{H(supportCase.Subject)}}</h1><p>{{H(person.Name)}} · {{H(supportCase.Topic)}} · {{Badge(supportCase.Status)}} {{(supportCase.Escalated ? "<i>Escalated</i>" : "")}}</p></div></section><section class='case-grid'><article class='panel'><h2>Conversation</h2>{{string.Join("", messages.Select(message => $"<div class='message {message.Visibility.ToLowerInvariant()}'><b>{H(message.Author)}</b><small>{H(message.Role)} · {Date(message.At)}</small><p>{H(message.Body)}</p></div>"))}}<form class='stack note js-form' data-api='/api/cases/{{supportCase.Id}}/messages'><label>Reply<textarea name='body' required></textarea></label><button class='button'>Send message</button></form></article><aside><article class='panel'><h2>Case context</h2><p>{{H(supportCase.Description)}}</p><dl><dt>Order</dt><dd>{{H(supportCase.OrderId ?? "None")}}</dd><dt>Bike</dt><dd>{{H(supportCase.BikeId ?? "None")}}</dd><dt>Policies</dt><dd>policies/warranty.md<br>policies/escalation.md</dd></dl></article>{{controls}}<article class='panel'><h2>Activity</h2>{{Events(supportCase.Timeline)}}</article></aside></section>""";
    }

    public static string Business(PortalStore store) => $$"""<section class='page-head compact'><p class='eyebrow'>BUSINESS OPERATIONS</p><h1>The current demo picture.</h1><p>Operational demo metrics, not accounting records.</p></section><section class='metrics'><article><p>Confirmed revenue</p><strong>{{Money(store.Orders.Where(item => item.Status is not "Cancelled" and not "Returned").Sum(item => item.Total))}}</strong></article><article><p>Orders</p><strong>{{store.Orders.Count}}</strong></article><article><p>Return requests</p><strong>{{store.Orders.Count(item => item.Status == "Return requested")}}</strong></article><article><p>Open cases</p><strong>{{store.Cases.Count(item => item.Status is not "Resolved" and not "Closed")}}</strong></article></section><section class='two'><article class='panel'><h2>Fulfilment states</h2>{{string.Join("", store.Orders.GroupBy(item => item.Status).Select(group => $"<div class='row'><span>{H(group.Key)}</span><b>{group.Count()}</b></div>"))}}</article><article class='panel'><h2>Top products</h2>{{string.Join("", store.Orders.SelectMany(item => item.Lines).GroupBy(item => item.Name).OrderByDescending(group => group.Count()).Take(5).Select(group => $"<div class='row'><span>{H(group.Key)}</span><b>{group.Sum(item => item.Quantity)} units</b></div>"))}}</article></section>""";

    public static string Diagnostics(PortalStore store, string? correlation, string? entity)
    {
        var events = store.Audits.Where(item => (string.IsNullOrWhiteSpace(correlation) || item.CorrelationId.Contains(correlation, StringComparison.OrdinalIgnoreCase)) && (string.IsNullOrWhiteSpace(entity) || item.EntityId.Contains(entity, StringComparison.OrdinalIgnoreCase))).Take(50);
        return $$"""<section class='page-head compact'><p class='eyebrow'>INTERNAL DIAGNOSTICS</p><h1>Trace the useful evidence.</h1><p>Audit events connect to safe console logs. Message bodies are intentionally absent.</p><form class='search'><input name='correlation' value='{{H(correlation)}}' placeholder='Correlation ID'><input name='entity' value='{{H(entity)}}' placeholder='Order, case or service ID'><button>Filter</button></form></section><section class='table'><table><thead><tr><th>When</th><th>Action</th><th>Entity</th><th>Actor</th><th>Correlation</th><th>Safe detail</th></tr></thead><tbody>{{string.Join("", events.Select(item => $"<tr><td>{Date(item.At)}</td><td>{H(item.Action)}</td><td>{H(item.EntityId)}</td><td>{H(item.Actor)}</td><td><code>{H(item.CorrelationId)}</code></td><td>{H(item.Detail)}</td></tr>"))}}</tbody></table></section>""";
    }
}
