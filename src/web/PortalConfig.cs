namespace NordicBike.Portal;

public static class PortalConfig
{
    public static readonly Dictionary<string, (string Name, string Customer)> Roles = new()
    {
        ["anna"] = ("Anna Karlsson", "NB-CUST-10041"),
        ["support"] = ("Support agent", ""),
        ["lead"] = ("Support lead", ""),
        ["business"] = ("Business operations", "")
    };

    public static readonly HashSet<string> RepairStates = ["Awaiting item", "Received", "Diagnosing", "Repairing", "Ready for return", "Completed"];
    public static readonly HashSet<string> ServiceCenters = ["Stockholm", "Gothenburg", "Malmo"];
}
