namespace NordicBike.Portal;

public static class DeliveryEstimateEndpoint
{
    public static IResult GetEstimate()
    {
        var generatedAt = DateTime.UtcNow;
        return Results.Ok(new { generatedAt });
    }
}