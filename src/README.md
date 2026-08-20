# NordicBike Portal

Self-contained .NET 10 demo portal for NordicBike customer, support, and business workflows. It uses deterministic in-memory data, which resets when the process stops.

```powershell
dotnet run
```

Open the printed localhost URL and select a persona from the header menu:

- Anna Karlsson: customer shop, orders, bikes, and support.
- Support agent: support queue and operational case actions.
- Support lead: escalation control and dashboards.
- Business operations: fulfilment and business metrics.

JSON APIs are under `/api`; `/api/health` is a no-data health check. Console logs and the internal Diagnostics page use `X-Correlation-ID` values.