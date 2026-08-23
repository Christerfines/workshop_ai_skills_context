namespace NordicBike.Portal;

public static class DemoClock
{
    public static readonly DateOnly Today = new(2026, 8, 19);
    public static DateTimeOffset Now => new(Today.ToDateTime(new TimeOnly(10, 0), DateTimeKind.Utc));
}

public sealed class Product
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Category { get; init; }
    public required decimal Price { get; init; }
    public required string Description { get; init; }
    public required string Mark { get; init; }
    public required string Specs { get; init; }
    public string[] CompatibleBikes { get; init; } = [];
    public string[] Options { get; init; } = [];
    public string Slug { get; init; } = "";
    public string Type { get; init; } = "Product";
    public string[] Tags { get; init; } = [];
    public bool IsActive { get; init; } = true;
    public string Image { get; init; } = "/images/products/catalog/aurora-x3.jpg";
    public string ImageAlt { get; init; } = "NordicBike product";
    public int ImageWidth { get; init; } = 960;
    public int ImageHeight { get; init; } = 640;
    public string[] Gallery { get; init; } = [];
}

public sealed record Customer(string Id, string Name, string Email, string City);
public sealed record CartItem(string Id, string ProductId, int Quantity, string? Configuration);
public sealed record OrderLine(string ProductId, string Name, int Quantity, decimal UnitPrice, string? Configuration);
public sealed record TimelineEvent(DateTimeOffset At, string Title, string Detail);
public sealed record CaseMessage(string Author, string Role, string Visibility, string Body, DateTimeOffset At);
public sealed record AuditEvent(string Id, string CorrelationId, string Actor, string Action, string EntityId, string Detail, DateTimeOffset At);
public sealed record SupportIssue(string Id, string Title, string Severity, string Area, string Change, string Evidence, string Investigation, string Acceptance, string[] ReproSteps, string Status);

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
