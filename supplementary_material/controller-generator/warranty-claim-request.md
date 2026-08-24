# Controller Request: Warranty Claims

Create an MVC API controller that allows a signed-in NordicBike customer to submit a warranty claim for one of their registered bikes.

## Request Behaviour

- Route: `POST /api/warranty-claims`.
- Request data: registered bike identifier, issue summary, and optional related order identifier.
- A customer may submit a claim only for a bike they own.
- If an order identifier is supplied, it must belong to the same customer.
- The response must make it possible for the customer to track the newly created support case.
- Invalid input, a missing record, or an inaccessible record must not create a claim.
- The mutation must create an audit event without exposing private details in the audit summary.

## Non-Goals

- Do not implement repair scheduling, compensation, payment, or third-party integrations.
- Do not replace the existing minimal API endpoint architecture.