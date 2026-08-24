# NordicBike Portal

Self-contained .NET 10 demo portal for NordicBike customer, support, business, and catalog workflows. It uses deterministic in-memory data, which resets when the process stops.

## Run quietly

From the repository root:

```powershell
dotnet run --project .\src\web\NordicBike.Portal.csproj --no-launch-profile --urls http://localhost:57146
```

When the terminal is already in `src`, use:

```powershell
dotnet run --project .\web\NordicBike.Portal.csproj --no-launch-profile --urls http://localhost:57146
```

The application prints only the contents of `startup-banner.txt` followed by one `Open:` URL. `--no-launch-profile` keeps the .NET CLI from adding launch-profile chatter; the first run may show normal build output. Use `--no-build` for a quiet restart after a successful build. The development launch profile is configured with one URL and no automatic browser launch.

Select a persona from the header menu:

- Anna Karlsson: customer shop, orders, bikes, and support.
- Support agent: support queue and operational case actions.
- Support lead: escalation control and dashboards.
- Business operations: fulfilment and business metrics.

## Layout

- `Program.cs`: host composition, middleware order, and feature registration.
- `PortalApiEndpoints.cs`: JSON API routes.
- `PortalPageEndpoints.cs`: HTML route registration.
- `PortalPages.cs`: server-rendered catalog, commerce, support, and operations pages.
- `Domain/PortalModels.cs`: catalog and workflow models.
- `Data/PortalSeedData.cs`: deterministic demo records.
- `catalog/products.json`: 72 active catalog items and local image metadata.
- `wwwroot/images/products`: repository-local JPEG product images used by every catalog item.

JSON APIs are under `/api`; `/api/health` is a no-data health check. Correlation IDs and safe audit events remain available in the internal Diagnostics page without request logs being written to the terminal.

## Local catalog assets

Catalog image values must be repository-relative paths below `/images/products/`. Do not add remote image URLs, runtime image fetches, CDN references, or remote fallbacks. Each bike and catalog item needs its own local JPEG primary image, useful alt text, and intrinsic dimensions in `catalog/products.json`.

The module is intentionally temporary. Keep catalog and rendering changes direct and easy to remove; no automated regression-test project or production persistence layer is included.