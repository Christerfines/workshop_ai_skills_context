using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace NordicBike.Portal.Tests.Api;

public sealed class HealthEndpointTests
{
    private PortalWebApplicationFactory factory = null!;
    private HttpClient client = null!;

    [SetUp]
    public void SetUp()
    {
        factory = new PortalWebApplicationFactory();
        client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }

    [TearDown]
    public void TearDown()
    {
        client.Dispose();
        factory.Dispose();
    }

    [Test]
    public async Task Health_endpoint_reports_the_seeded_demo_as_healthy()
    {
        var response = await client.GetAsync("/api/health");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var health = await response.Content.ReadFromJsonAsync<HealthResponse>();

        Assert.That(health, Is.Not.Null);
        Assert.That(health!.Status, Is.EqualTo("healthy"));
        Assert.That(health.Store, Is.EqualTo("in-memory"));
        Assert.That(health.Orders, Is.GreaterThan(0));
    }

    private sealed record HealthResponse(string Status, DateOnly DemoDate, string Version, string Store, int Orders);
}