using NordicBike.Portal;
using NordicBike.Portal.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.WebHost.UseSetting("SuppressStatusMessages", "true");
builder.Services.AddSingleton<PortalStore>(services => new PortalStore(services.GetRequiredService<IHostEnvironment>().ContentRootPath));

var app = builder.Build();
app.UseStaticFiles();
app.UseMiddleware<CorrelationMiddleware>();
app.MapPortalApi();
app.MapPortalPages();

await app.StartAsync();
await StartupReporter.PrintAsync(app);
await app.WaitForShutdownAsync();
