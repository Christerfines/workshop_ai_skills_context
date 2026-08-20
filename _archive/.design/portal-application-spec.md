# NordicBike Portal - Demo Application Specification

## 1. Purpose

Build a polished, self-contained web portal for the fictional company NordicBike AB. It is a realistic demonstration application, not a production commerce system. It must let customers browse and buy products, track orders, manage their bikes, and contact support. It must also give internal support and business users distinct operational views.

The portal is a later workshop fixture. Participants will receive incomplete, noisy support incidents about its behavior and use a reusable investigation skill to diagnose them. The initial version must therefore be correct, deterministic, observable, and small enough for another LLM to implement and modify confidently.

The application must not implement the existing LLM warranty-agent workshop exercises. It complements them by providing a concrete product to investigate. NordicBike product facts, support channels, and policy terminology should remain consistent with the existing `company/`, `products/`, and `policies/` documents.

## 2. Scope and Principles

### In Scope

- Product catalog, product details, cart, and simulated checkout.
- Customer order history and order tracking.
- Customer bike registration and service/support requests.
- Customer-support workspace for viewing and managing cases.
- Business dashboard for sales, fulfilment, and support trends.
- Seeded demo identities, products, customers, orders, cases, and activity data.
- HTTP APIs consumed by the web UI.
- Console logging, correlation IDs, structured domain events, and audit history.

### Out of Scope

- Real payment processing, shipping-provider integration, email/SMS delivery, authentication provider, or persistence beyond a running process.
- Live inventory, real addresses, legal decisions, automated warranty eligibility, or AI-powered support responses.
- Production security, multi-tenancy, background workers, and external databases.

### Demo Constraints

- All data lives in memory and resets to the same seed state after every application restart.
- All dates, delivery estimates, amounts, and status transitions are deterministic.
- Actions that would ordinarily call an external service should visibly complete in the UI while recording a simulated event in the audit timeline and console logs.
- No data is read from the workshop customer or case markdown files at runtime. Seed data is application-owned so future incident fixtures can evolve independently.

## 3. Technical Direction

### Platform

- Use .NET 10 with `<TargetFramework>net10.0</TargetFramework>`.
- Deliver one ASP.NET Core project, for example `NordicBike.Portal/`, using `Microsoft.NET.Sdk.Web`.
- The same process hosts both the UI and JSON API. Do not create a separate frontend project, service, database project, Docker dependency, or prerequisite installation beyond the .NET 10 SDK.
- Run locally with `dotnet run` from the project directory. On startup, print the bound local URL and the available demo roles to the console.
- Use Razor Components with interactive server rendering or another ASP.NET Core-native UI approach that keeps API and web code in the one project. Avoid a Node/npm build requirement.
- Use Entity Framework Core's InMemory provider, or equivalent in-memory repositories behind interfaces, for all state. A small data-access abstraction is useful because later workshop variants may deliberately corrupt or change one behavior.

### Project Organization

Use feature-oriented folders so an investigator can locate the owning behavior without chasing a large generic layer:

```text
NordicBike.Portal/
  Features/
    Catalog/
    Cart/
    Checkout/
    Orders/
    Bikes/
    Support/
    Business/
    Diagnostics/
  Domain/
  Infrastructure/
    Data/
    Logging/
  Components/
    Layout/
    Pages/
    Shared/
  wwwroot/
  Program.cs
```

Keep endpoint handlers, domain services, and UI state focused on their feature. Do not introduce CQRS, a message broker, repository factories, or other infrastructure that does not serve the demo.

## 4. Roles and Entry Experience

The application uses a visible demo-role switcher instead of authentication. It must be available from the header and retain the chosen role while the browser session is open.

| Demo role | Primary need | Initial landing page |
| --- | --- | --- |
| Customer | Browse, purchase, track and get help | Storefront |
| Support agent | Triage cases and communicate updates | Support queue |
| Support lead | Monitor escalations and team workload | Support dashboard |
| Business operations | Understand sales and fulfilment health | Business dashboard |

The switcher should include named seeded personas, not anonymous roles. A customer persona must expose only its own orders, bikes, and support cases. Internal personas may view all seeded operational records.

Use a clear top-level navigation appropriate to the active role. Customer navigation should include `Shop`, `My Orders`, `My Bikes`, and `Support`. Internal navigation should include `Support`, `Orders`, `Business`, and `Diagnostics` where the active role is permitted.

## 5. Customer Portal

### 5.1 Storefront and Catalog

The customer storefront is the default page and should feel like a credible NordicBike online shop, not a marketing landing page. Use a compact catalog layout with filters and product cards that lead to useful product detail pages.

Catalog products and starting prices must be:

| Product | Category | Price |
| --- | --- | --- |
| Aurora X3 | City e-bike | 34,900 SEK |
| Fjord Cargo | Cargo e-bike | 44,900 SEK |
| Vinter Pro | Winter e-bike | 37,900 SEK |
| PowerPack 720 | Spare battery | 6,900 SEK |
| PowerPack 900 | Spare battery | 8,900 SEK |
| Pannier bag | Accessory | 1,200 SEK |
| LED light set | Accessory | 450 SEK |
| Frame lock | Accessory | 950 SEK |
| Phone mount | Accessory | 350 SEK |

Requirements:

- Filter by category and search product names.
- Product pages show a concise description, price, key specifications, compatibility, and add-to-cart action.
- Bike pages support size and color selection where applicable. Aurora X3: S/M/L and Nordic Black/Fjord Blue. Vinter Pro: S/M/L and Arctic White. Fjord Cargo: Slate Grey only.
- Battery compatibility must be clear: PowerPack 720 fits Aurora X3 and Vinter Pro; PowerPack 900 fits Fjord Cargo.
- Prevent an incompatible battery from being added as a recommended replacement for a registered bike, with a useful inline explanation.

### 5.2 Cart and Simulated Checkout

The cart must support quantity changes, removal, subtotal, a fixed `0 SEK` simulated delivery fee, and total in SEK with no decimals. It should use stable layout dimensions as values change.

Checkout collects a display name, email, delivery city, and confirmation that the order is simulated. It must not collect real payment card information. Selecting `Place simulated order` must:

1. Create an order with a generated `NB-ORD-YYYYMMDD-###` identifier.
2. Set its initial status to `Confirmed`.
3. Clear the cart.
4. Add an order timeline event and audit event.
5. Show an order-confirmation view with the order number and tracking link.

### 5.3 My Orders and Tracking

Customers see their own orders in a scan-friendly table and can open an order detail page. Each order must show line items, total, delivery city, order date, last update, and a vertical status timeline.

Supported order statuses are `Confirmed`, `Processing`, `Shipped`, `Delivered`, `Cancelled`, `Return requested`, and `Returned`. The normal seeded fulfilment path is `Confirmed -> Processing -> Shipped -> Delivered`.

For `Shipped` orders, show a deterministic simulated carrier and tracking reference. For delivered orders, show delivery date. Customers can request a return from a delivered order; this creates a support case rather than automatically issuing a refund.

### 5.4 My Bikes

Customers can see bikes attached to their purchases, including product name, serial number, purchase date, current service status, and compatible accessories. They can register a bike purchased outside the portal by entering a serial number and purchase date; registration creates a pending verification record, not an automatic ownership confirmation.

Serial validation should use these formats:

- Aurora X3: `AX3-YYA-#####` or `AX3-YYB-#####`
- Fjord Cargo: `FJC-YYA-#####` or `FJC-YYB-#####`
- Vinter Pro: `VTP-YYA-#####` or `VTP-YYB-#####`

The UI should identify malformed values before submission. A well-formed but unknown serial remains a pending verification record.

### 5.5 Customer Support

The support page must let customers submit and follow cases. A new-case form contains:

- Subject and free-text description.
- Topic: `Order and delivery`, `Product question`, `Bike or battery fault`, `Warranty and repair`, `Return request`, or `Other`.
- Optional related order and registered bike.
- Optional simulated attachment metadata: file name and approximate size only. Do not accept or store binary files.

After submission, create a `NB-CASE-#####` case with status `New`, record a timeline entry, and show the customer’s case detail page. Customers can add messages to open cases but cannot change priority, ownership, internal notes, or escalation flags.

Show the support channels consistently with the company material: `support@nordicbike.se`, `+46 8 555 123 00`, Monday-Friday 09:00-17:00 CET, and live chat during support hours.

## 6. Internal Support Workspace

### 6.1 Queue and Case Detail

Support agents need a practical operational workspace, not a customer-facing re-skin. The queue must support filters for status, priority, topic, assignee, escalation state, and free-text search across case ID, customer name, order ID, and serial number.

Each row displays case ID, customer, subject, topic, priority, status, assignee, age, and escalation marker. Seed cases must cover a mix of normal questions, order tracking, return request, technical fault, and warranty question.

The case detail view contains:

- Customer summary and links to related orders and bikes.
- Customer-visible conversation, with actor, timestamp, and message type.
- Separate internal notes, never visible in the customer view.
- Case fields: status, priority, owner, topic, related order, related bike, and escalation flag.
- Activity timeline containing field changes, messages, status changes, assignment, escalation, and simulated fulfilment events.
- A policy-reference panel linking to the relevant workshop policy markdown paths without automatically deciding eligibility.

Agents can assign themselves or another internal seeded user, change status among `New`, `Waiting for customer`, `In progress`, `Waiting for service center`, `Resolved`, and `Closed`, add a customer reply or internal note, and mark a case as escalated.

An agent may mark a case as escalated but cannot clear an existing escalation. The support lead role may set or clear escalation after adding an internal note explaining the decision.

### 6.2 Service Requests

For a bike or battery fault, an agent can create a service request from the case. It has a selected service center (Stockholm, Gothenburg, or Malmo), a repair state, and a simulated shipping-label outcome. A service request does not decide warranty eligibility; it is simply operational intake.

Supported repair states are `Awaiting item`, `Received`, `Diagnosing`, `Repairing`, `Ready for return`, and `Completed`. Transitions must append timeline and audit events. The standard displayed repair estimate is `5-10 business days from intake` once the item is received.

## 7. Business and Support-Lead Views

### 7.1 Support Lead Dashboard

Provide operational counts for open cases, cases awaiting customer response, escalated cases, average seeded first-response time, and cases by topic. Include a small list of the oldest open cases and recent escalation activity. All metrics are calculated from the in-memory records at request time.

### 7.2 Business Dashboard

The business role sees summary metrics for confirmed revenue, order count by status, top products by units sold, recent orders, return-request count, and support volume by topic. These are demo metrics, not accounting records. Make the distinction visible in the page subtitle.

Business users may update an order from `Confirmed` to `Processing`, `Processing` to `Shipped`, and `Shipped` to `Delivered`. Every status change must update the customer order timeline and write an audit event.

## 8. API Contract

Expose JSON APIs under `/api`. The UI must use these APIs for mutations and primary reads so a later incident can occur in either the UI or API layer. Return problem-details responses for validation failures and missing records.

| Area | Required routes |
| --- | --- |
| Catalog | `GET /api/products`, `GET /api/products/{id}` |
| Cart | `GET /api/cart`, `POST /api/cart/items`, `PATCH /api/cart/items/{id}`, `DELETE /api/cart/items/{id}` |
| Checkout | `POST /api/orders` |
| Customer orders | `GET /api/orders`, `GET /api/orders/{id}`, `POST /api/orders/{id}/return-requests` |
| Bikes | `GET /api/bikes`, `POST /api/bikes/registrations` |
| Support | `GET /api/cases`, `POST /api/cases`, `GET /api/cases/{id}`, `POST /api/cases/{id}/messages`, `PATCH /api/cases/{id}` |
| Service | `POST /api/cases/{id}/service-requests`, `PATCH /api/service-requests/{id}` |
| Dashboards | `GET /api/dashboard/support`, `GET /api/dashboard/business` |
| Diagnostics | `GET /api/diagnostics/events`, `GET /api/health` |

Use request/response DTOs and validate all incoming mutations. Do not expose data-store entities directly. APIs must honour the selected demo identity through a simple session or request context and reject a customer’s attempt to access another customer’s record with `404 Not Found`.

## 9. Domain Model and Seed Data

Use meaningful, stable IDs. Seed data must contain enough related records that every role has useful work on first load.

Minimum entities:

- `Product`: SKU, name, category, price, specifications, compatible bike SKUs, active flag.
- `Customer`: ID, name, email, city.
- `Order` and `OrderLine`: ID, customer ID, timestamps, status, totals, delivery city, carrier/tracking fields.
- `Bike`: ID, owner/customer ID, product SKU, serial number, purchase date, registration status, service status.
- `SupportCase`: ID, customer ID, subject, description, topic, priority, status, assignee, escalation flag, related order/bike IDs, created/updated timestamps.
- `CaseMessage`: ID, case ID, author role/name, visibility, body, timestamp.
- `ServiceRequest`: ID, case ID, item type, service center, repair state, timestamps.
- `AuditEvent`: ID, correlation ID, actor, action, entity type/ID, short structured details, timestamp.

Seed at least:

- All nine catalog products listed above.
- Four customers in different Swedish cities, including Anna Karlsson in Malmo as a continuity detail with the existing workshop material.
- Eight orders spanning every normal fulfilment status and at least one delivered bike order, one shipped order, and one accessory-only order.
- At least three registered bikes with syntactically valid serials across all bike models.
- Six support cases with varied statuses, topics, priorities, and related records; at least one must be escalated and at least one must include an internal note.
- Several historical audit events and timeline entries per order/case so diagnostic views are useful before anyone interacts with the app.

Use fixed seed dates relative to a fixed demo date declared in one `DemoClock` abstraction. The UI should display the demo date where it affects seeded age or timeline language. Do not silently use the machine date for business calculations.

## 10. Logging, Auditability, and Diagnostics

Console logging is a first-class requirement because future workshop cases will include partial and noisy logs. Configure structured console logs at `Information` level by default, with framework noise reduced to `Warning`.

Every HTTP request must receive or generate an `X-Correlation-ID`. Include it in the response header, request-completion log, domain-operation logs, and newly created `AuditEvent` records. Log structured properties rather than composing all context into one string.

Required log events include:

- Request method, route, status code, elapsed milliseconds, active demo role, and correlation ID.
- Cart mutation, checkout attempt and result, order-status change, and return-request creation.
- Support-case creation, message, assignment, status transition, escalation change, and service-request transition.
- Validation rejection and unexpected exception, including correlation ID and affected entity ID when safe.

Never log a full support-message body, email address, phone number, or any simulated payment-like input. Logs may include IDs, event names, status transitions, role, and safe short metadata. The diagnostics page, available only to internal roles, shows the most recent audit events and lets the user filter by correlation ID, entity ID, and event category. It is an audit viewer, not a live tail of raw console output.

`GET /api/health` returns a small JSON document containing `status`, fixed demo date, application version, and in-memory store state. It must not expose customer data.

## 11. Intentional Incident Readiness

The first build must be correct. Do not pre-inject bugs, fake error banners, or deliberately misleading data. Instead, make these investigation surfaces explicit so later workshop variants can introduce isolated defects and supporting evidence:

| Investigation surface | Correct baseline behavior | Suitable future incident seam |
| --- | --- | --- |
| Checkout | Creates one confirmed order, clears cart, records correlated events | Idempotency, total calculation, stale cart state |
| Order tracking | Shows the same status in customer and internal views | Mapping, cache/state refresh, status-transition validation |
| Case visibility | Customers see only their own visible messages | Authorization/context selection, message visibility filter |
| Escalation | Agent can set but not clear; lead can explain and clear | Role authorization, audit omission, state validation |
| Service request | Transitions write case and service timelines | Cross-entity update, partial failure, event ordering |
| Catalog compatibility | Recommends only compatible batteries | Product compatibility lookup, UI/API validation mismatch |
| Dashboard metrics | Derive from current in-memory records | Aggregation/filtering, stale calculation, status classification |
| Diagnostics | Correlation ID connects request, audit, and console evidence | Correlation propagation, PII redaction, incomplete event recording |

Future incident authors should be able to alter one bounded feature seam and produce a support ticket, snippets of console logs, an audit-event extract, and irrelevant conversation noise. Do not create a catch-all `BugMode` switch: the point is for participants to investigate realistic implementation behavior, not detect a labelled exercise toggle.

## 12. User Interface Requirements

The UI should feel like a refined operational and retail application for a Scandinavian e-bike company: clear, capable, and calm. It should not feel like a generic dashboard or a promotional landing page.

- Build responsive desktop and mobile layouts. The catalog should become a practical single-column browsing flow on narrow screens; data tables should remain readable through responsive columns or detail links.
- Use product photography or high-quality local bitmap placeholders that show the actual bike/product category. Do not rely solely on decorative gradients or SVG illustrations.
- Use a restrained multi-color palette with strong contrast, a readable non-default typeface, compact 8px-or-less card radii, and explicit focus states.
- Use Lucide icons where they clarify controls. Icon-only controls need accessible labels and tooltips.
- Use semantic HTML, keyboard-accessible controls, visible validation messages, empty states, loading states, and error states.
- Support UI density suited to repeated work: compact queue rows, persistent filters where helpful, deliberate status badges, and no nested card layouts.
- Display amounts as `34,900 SEK`; dates should use an unambiguous ISO-like Swedish-friendly format such as `2026-08-19`.

## 13. Quality and Acceptance Criteria

The implementation is complete when all of the following are true:

1. `dotnet run` launches one local application without database, Node, Docker, cloud credentials, or manual seed steps.
2. The catalog, cart, and checkout flow produce a confirmed order that appears in the customer order history.
3. Product configurations and battery compatibility rules behave as specified.
4. Customers can access only their own orders, bikes, and customer-visible cases.
5. A support agent can search, assign, update, message, escalate, and create a service request for a case.
6. A support lead can view operational metrics and clear an escalation with a recorded explanation.
7. A business user can advance normal order status and see derived business metrics update.
8. Customer and internal timelines show the correct visibility and reflect every supported mutation.
9. Validation failures use useful problem-details responses and do not partially mutate state.
10. Each API request and domain mutation produces correlation-aware, structured console output with no sensitive text fields.
11. The internal diagnostics screen can connect audit activity to a correlation ID and affected order, case, or service request.
12. The application has focused automated tests for checkout, access isolation, status transitions, escalation authorization, service-request lifecycle, compatibility validation, and correlation-ID propagation.
13. The browser experience is usable at desktop and mobile viewport widths, with no overlapping text or hidden primary actions.

## 14. Build Handoff

The implementing LLM should create the application in a new `portal/` directory under the workshop repository, preserve the existing workshop material unchanged, and add a short `portal/README.md` with the exact local run command and demo-role instructions. It should use this specification as the implementation source of truth where it differs from assumptions inferred from existing marketing or policy documents.