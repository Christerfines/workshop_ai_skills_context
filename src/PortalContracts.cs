namespace NordicBike.Portal;

public sealed record CartInput(string ProductId, int Quantity, string? Configuration);
public sealed record CheckoutInput(string Name, string Email, string City, bool IsSimulated);
public sealed record CheckoutAttempt(string CustomerId, string Fingerprint, string OrderId);
public sealed record BikeInput(string Serial, DateOnly Purchased);
public sealed record CaseInput(string Subject, string Description, string Topic, string? OrderId, string? BikeId, string? Attachment);
public sealed record MessageInput(string Body, bool Internal);
public sealed record CaseUpdate(string? Status, string? Priority, string? Assignee, bool? Escalated);
public sealed record ServiceInput(string Center, string? State);
public sealed record SupportIssueUpdate(string Status);
