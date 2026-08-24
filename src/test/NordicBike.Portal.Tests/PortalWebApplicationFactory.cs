using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using NordicBike.Portal;

namespace NordicBike.Portal.Tests;

public sealed class PortalWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseContentRoot(FindWebContentRoot());
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<PortalStore>();
            services.AddSingleton<PortalStore>(serviceProvider =>
                new PortalStore(serviceProvider.GetRequiredService<IHostEnvironment>().ContentRootPath));
        });
    }

    private static string FindWebContentRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            var webDirectory = Path.Combine(directory.FullName, "web");
            if (File.Exists(Path.Combine(webDirectory, "catalog", "products.json"))) return webDirectory;
        }

        throw new DirectoryNotFoundException("Could not locate the web project content root.");
    }
}