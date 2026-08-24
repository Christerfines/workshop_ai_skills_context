# Backlog Brief: Reliable Warranty Service Routing

## Business Outcome

NordicBike customers with eligible registered bikes should have warranty repair cases routed promptly to the nearest capable service centre. Support teams need a clear, auditable workflow for handling those requests.

## Users

- Customer: owns a registered Aurora X3, Fjord Cargo, or Vinter Pro bike and needs warranty help.
- Support agent: creates and manages the case and service request.
- Support lead: handles escalations and reviews changes.

## Scope

- Link a warranty request to a customer, registered bike, and optional order.
- Route an eligible case to Stockholm, Gothenburg, or Malmo.
- Support a clear request lifecycle from created to scheduled, in service, and completed.
- Prevent customers from viewing internal-only case notes.
- Record who changed service routing or escalation state, with a correlation identifier where available.

## Constraints

- A customer can access only their own bikes, orders, and cases.
- Support operations use the current user identity; they do not trust an actor supplied in a request body.
- Invalid centre or state transitions must return a useful validation response and leave the case unchanged.
- The first release uses the in-memory portal store and must include integration coverage for the main happy path and one rejected request.

## Out of Scope

- Live bookings with third-party repair providers.
- Parts inventory, payment, and compensation decisions.
- Migrating existing minimal API endpoints to MVC.