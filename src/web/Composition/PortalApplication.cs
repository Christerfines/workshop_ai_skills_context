using NordicBike.Portal.Middleware;

namespace NordicBike.Portal;

public static class PortalApplication
{
    public static WebApplication Create(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureBuilder(builder);

        var app = builder.Build();
        ConfigurePipeline(app);
        return app;
    }

    public static async Task RunAsync(WebApplication app, CancellationToken cancellationToken = default)
    {
        await app.StartAsync(cancellationToken);
        await StartupReporter.PrintAsync(app);
        await app.WaitForShutdownAsync(cancellationToken);
    }

    private static void ConfigureBuilder(WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.WebHost.UseSetting("SuppressStatusMessages", "true");
        builder.Services.AddSingleton<PortalStore>(services => new PortalStore(services.GetRequiredService<IHostEnvironment>().ContentRootPath));
    }

    private static void ConfigurePipeline(WebApplication app)
    {
        app.UseStaticFiles();
        app.UseMiddleware<CorrelationMiddleware>();
        app.MapPortalApi();
        app.MapPortalPages();
    }
}