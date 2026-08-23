namespace NordicBike.Portal;

public sealed class PortalStore
{
    private int _order = 8;
    private int _case = 206;
    private int _audit = 1;
    private int _service = 1;

    public PortalStore(string contentRoot)
    {
        Products = CatalogSource.Load(contentRoot);
        this.Seed();
    }

    public List<Product> Products { get; }
    public List<Customer> Customers { get; } =
    [
        new("NB-CUST-10041", "Anna Karlsson", "anna.karlsson@example.se", "Malmo"),
        new("NB-CUST-10042", "Erik Svensson", "erik.svensson@example.se", "Stockholm"),
        new("NB-CUST-10043", "Lena Bjork", "lena.bjork@example.se", "Gothenburg"),
        new("NB-CUST-10044", "Johan Lindqvist", "johan.lindqvist@example.se", "Umea")
    ];
    public List<Order> Orders { get; } = [];
    public List<Bike> Bikes { get; } = [];
    public List<SupportCase> Cases { get; } = [];
    public List<ServiceRequest> ServiceRequests { get; } = [];
    public List<AuditEvent> Audits { get; } = [];
    public List<SupportIssue> SupportIssues { get; } = [];
    public Dictionary<string, List<CartItem>> Carts { get; } = [];
    public Dictionary<string, CheckoutAttempt> CheckoutAttempts { get; } = [];
    public object SyncRoot { get; } = new();

    public string NewOrderId() => $"NB-ORD-{DemoClock.Today:yyyyMMdd}-{++_order:D3}";
    public string NewCaseId() => $"NB-CASE-{++_case:D5}";
    public string NewServiceId() => $"SR-{++_service:D4}";
    public List<CartItem> Cart(string customerId) => Carts.TryGetValue(customerId, out var cart) ? cart : Carts[customerId] = [];
    public void Audit(string correlation, string actor, string action, string entityId, string detail) => Audits.Insert(0, new AuditEvent($"AUD-{_audit++:D5}", correlation, actor, action, entityId, detail, DemoClock.Now));
}
