# NordicBike Fictional Coding Standards

Apply these workshop standards only to code in scope for the review. A standard is a review criterion, not a substitute for evidence that behavior is wrong.

## Baseline C# Rules

| ID | Standard | Review Signal |
| --- | --- | --- |
| NB001 | Application code must use `DemoClock.Now` or `DemoClock.Today`; direct `DateTime.Now`, `DateTime.UtcNow`, and `DateTimeOffset.Now` are prohibited. | Deterministic scanner candidate. |
| NB002 | Production code must not block asynchronous work with `.Result`, `.Wait()`, or `GetAwaiter().GetResult()`. | Deterministic scanner candidate. |
| NB003 | The web project must not write diagnostics with `Console.WriteLine`; use the application's structured logging or audit mechanism. `StartupReporter.cs` is the bounded exception for startup-banner output. | Deterministic scanner candidate. |
| NB004 | Public request and response contracts must keep nullable annotations accurate and must not expose internal-only fields. | Requires contract and endpoint context. |

## API And Identity Rules

| ID | Standard | Review Signal |
| --- | --- | --- |
| NB101 | A resource-by-id route must enforce customer ownership unless the route is explicitly internal and guarded by `IsInternal(context)`. Return `NotFound` when the caller must not learn whether another customer's resource exists. | Inspect query predicate and role branch. |
| NB102 | State-changing API handlers must validate request data before mutating `PortalStore` or domain objects. | Inspect input guard and first mutation. |
| NB103 | A customer or actor identity must come from `PortalIdentity`; do not trust identity fields supplied in JSON, form, query, or route input. | Trace identity source. |
| NB104 | Expected client errors must return an explicit HTTP result rather than allowing a null dereference, key lookup, or parsing exception to control the response. | Exercise or trace invalid input path. |

## State And Audit Rules

| ID | Standard | Review Signal |
| --- | --- | --- |
| NB201 | A successful creation or mutation of an order, case, message, bike registration, or service request must call `PortalAudit.Record` with a non-sensitive summary. | Inspect successful mutation paths. |
| NB202 | Audit detail must not contain message bodies, email addresses, payment-like data, or other user-entered sensitive text. | Inspect `PortalAudit.Record` arguments. |
| NB203 | An order status, support escalation, or service-request state transition must validate its allowed predecessor or configured state. | Inspect transition guard. |

## Test Rules

| ID | Standard | Review Signal |
| --- | --- | --- |
| NB301 | A changed observable route, response contract, validation rule, authorization branch, or state transition needs a focused NUnit test unless an existing updated test proves the new behavior. | Compare source and test changes. |
| NB302 | Tests must use the portal web-application factory and assert both HTTP status and the relevant externally observable result. | Inspect changed test. |
| NB303 | A regression test must demonstrate the old failure mode or boundary condition, rather than only executing the happy path. | Inspect test data and assertions. |

## Exceptions

A documented exception can override a standard only when it identifies the rule, the reason, and the bounded scope. `StartupReporter.cs` is the documented NB003 exception because it renders the portal's startup banner. A missing scanner finding does not grant an exception.