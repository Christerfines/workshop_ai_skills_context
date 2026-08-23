namespace NordicBike.Portal;

public static class StartupReporter
{
    public static async Task PrintAsync(WebApplication app)
    {
        var bannerPath = Path.Combine(app.Environment.ContentRootPath, "startup-banner.txt");
        var banner = await File.ReadAllTextAsync(bannerPath);
        Console.Write(banner);
        if (!banner.EndsWith('\n')) Console.WriteLine();

        var url = app.Urls.OrderBy(value => value, StringComparer.Ordinal).FirstOrDefault() ?? "http://localhost:5000";
        Console.WriteLine($"Open: {url.TrimEnd('/')}");
    }
}
