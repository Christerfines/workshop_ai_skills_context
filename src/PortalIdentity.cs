namespace NordicBike.Portal;

public static class PortalIdentity
{
    public static string Key(HttpContext context) => context.Request.Cookies["nb-role"] is { } role && PortalConfig.Roles.ContainsKey(role) ? role : "anna";
    public static string Role(HttpContext context) => Key(context) switch
    {
        "support" => "Support agent",
        "lead" => "Support lead",
        "business" => "Business operations",
        _ => "Customer"
    };
    public static string Actor(HttpContext context) => PortalConfig.Roles[Key(context)].Name;
    public static string Customer(HttpContext context) => PortalConfig.Roles[Key(context)].Customer.Length > 0 ? PortalConfig.Roles[Key(context)].Customer : "NB-CUST-10041";
    public static bool IsInternal(HttpContext context) => Key(context) is "support" or "lead" or "business";
    public static string Landing(string key) => key is "support" or "lead" ? "/support" : key == "business" ? "/business" : "/shop";
}
