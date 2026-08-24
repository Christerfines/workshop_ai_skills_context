# NordicBike Portal Context

## Solution Layout

`src/NordicBike.Portal.slnx` contains exactly two projects:

| Project | Purpose |
| --- | --- |
| `src/web/NordicBike.Portal.csproj` | .NET 10 ASP.NET Core minimal-API portal application. |
| `src/test/NordicBike.Portal.Tests/NordicBike.Portal.Tests.csproj` | NUnit integration and API tests using `Microsoft.AspNetCore.Mvc.Testing`. |

The web project uses nullable reference types and implicit global usings. Production source is arranged by responsibility rather than controllers:

- `PortalApiEndpoints.cs` maps `/api/...` handlers and contains request validation and response decisions.
- `PortalPageEndpoints.cs` maps server-rendered page routes.
- `PortalIdentity.cs` supplies `Customer(context)`, `Role(context)`, and `IsInternal(context)`.
- `PortalAudit.cs` records consequential user and operational actions.
- `PortalContracts.cs` defines inputs and response contracts.
- `PortalStore.cs` and `Domain/` hold the in-memory application state and domain models.
- `PortalConfig.cs` holds demo configuration and allowed values.
- `PortalViews.cs` and `PortalPages.cs` render HTML.

## Established Patterns To Preserve

- Map routes through `MapPortalApi` or `MapPortalPages`; keep endpoint helpers private to their endpoint module.
- Return `Results.NotFound()` for resources that are absent or not visible to the caller, avoiding an existence disclosure.
- Read the current actor and customer from `PortalIdentity`, not from a request payload or query string.
- Use `PortalAudit.Record` for successful actions that create or change orders, cases, messages, bikes, service requests, or other operational state.
- Use `DemoClock.Now` and `DemoClock.Today` for application time so demo scenarios and tests are deterministic.
- Validate JSON input before state mutation and return a deliberate HTTP result for expected invalid input.
- Add route and behavior tests in `src/test/NordicBike.Portal.Tests/Api` with the existing web-application factory.

These are local conventions for this workshop repository, not universal ASP.NET Core rules.