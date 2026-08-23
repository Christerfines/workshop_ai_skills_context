namespace NordicBike.Portal;

public static class PortalAudit
{
    public static void Record(HttpContext context, PortalStore store, string action, string entityId, string detail)
    {
        var correlation = context.Items["correlation"]?.ToString() ?? "unknown";
        store.Audit(correlation, PortalIdentity.Actor(context), action, entityId, detail);
    }
}
