# NordicBike MVC Controller Standards

## Shape

- Use an API controller with attribute routing and a typed request contract.
- Return typed success results and appropriate HTTP error results.
- Keep endpoint orchestration in the controller; reuse existing domain models and `PortalStore` rather than duplicating store logic.

## Validation and Access

- Validate required fields before mutating state.
- Resolve the acting customer and role through `PortalIdentity`; never trust actor or customer identifiers supplied in a request body.
- Return `404 Not Found` when a requested entity is absent or not accessible to the caller, matching the portal's ownership convention.
- Return a useful `400 Bad Request` for malformed or invalid input. Do not mutate state for rejected requests.

## Mutation Safeguards

- Record successful mutations through `PortalAudit.Record` using a concise, non-sensitive detail.
- Add a timeline entry when the new support case becomes visible to the customer.
- Use the portal's deterministic `DemoClock` for newly created dates and timestamps.

## Tests

- Add an integration test for a valid customer claim.
- Add an integration test that rejects an inaccessible bike or order.
- Verify the rejected scenario leaves no new support case or audit event.