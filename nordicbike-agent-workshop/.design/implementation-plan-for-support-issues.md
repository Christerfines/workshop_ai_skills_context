# Implementation Plan for Potential Support Issues

Use the same overall approach for every issue:

1. Add one narrowly scoped defect to the portal or data mapping.
2. Keep the rest of the baseline behavior correct.
3. Add a realistic customer-facing report plus the smallest useful supporting evidence.
4. Capture the root cause in one short internal note or audit trail.
5. Verify the fix by reproducing the issue before and after the change.

## Easy: Checkout redirect points at a nonexistent order

- **Change:** In the checkout success path, point the client to the wrong order identifier format for one branch only.
- **Files:** `src/NordicBike.Portal/wwwroot/portal.js`, possibly the `/api/orders` success handling in `Program.cs` if the bug is server-side.
- **Evidence to include:** customer report, checkout response, browser URL, and two request/audit logs.
- **Acceptance:** the wrong redirect reproduces reliably, the created order still exists, and the fix restores redirecting to `/orders/{id}`.

## Easy: Cart total does not refresh after a quantity update

- **Change:** Keep the cart API mutation correct, but make the UI reuse a stale cart summary snapshot after patching quantity.
- **Files:** `src/NordicBike.Portal/wwwroot/portal.js` and the cart page markup in `Program.cs` if the stale value is rendered server-side.
- **Evidence to include:** customer screenshot, cart API response showing the updated quantity, and an audit event for the mutation.
- **Acceptance:** quantity updates persist, but the visible total is stale until refresh; the repair re-renders the cart summary from current data.

## Moderate: Shipped order remains Processing for a customer

- **Change:** Omit `Shipped` from the customer-facing order projection while leaving internal operations intact.
- **Files:** `src/NordicBike.Portal/Program.cs`, especially the `ToOrder`/`OrdersPage` rendering path.
- **Evidence to include:** customer report, business dashboard or audit event showing the status transition, and the customer API payload.
- **Acceptance:** internal tracking shows `Shipped`, customer view incorrectly stays `Processing`, and the fix restores the customer projection.

## Moderate: Support agent cannot create a Malmo service request

- **Change:** Introduce a normalization mismatch for the service-center value so one valid spelling fails validation.
- **Files:** `src/NordicBike.Portal/Program.cs`, service-request handler and validation list.
- **Evidence to include:** agent report, `400` problem-details response, submitted payload, and one unrelated warning log to show noise.
- **Acceptance:** only the malformed/normalized value fails, the valid `Malmo` path works after the fix.

## Moderate: Vinter Pro owner is recommended an incompatible battery

- **Change:** Break the UI compatibility mapping for one lookup while keeping the catalog API data correct.
- **Files:** `src/NordicBike.Portal/wwwroot/portal.js` or product-page rendering helpers in `Program.cs`.
- **Evidence to include:** product screenshot, catalog API response, customer conversation, and a distracting warranty note.
- **Acceptance:** the API remains correct, the UI recommendation is wrong before the fix, and the correct battery appears after the fix.

## Hard: Support agent clears escalation with no audit trail

- **Change:** Allow one PATCH branch to clear escalation without role checking or audit logging.
- **Files:** `src/NordicBike.Portal/Program.cs`, case-update handler.
- **Evidence to include:** case timeline, diagnostics extract, partial console logs, and compensation-discussion noise.
- **Acceptance:** only support leads can clear escalation and every clearance leaves an audit trail after the fix.

## Hard: Customer sees internal note after switching roles

- **Change:** Cache an internal case projection and reuse it after the role cookie changes.
- **Files:** `src/NordicBike.Portal/Program.cs`, case-page projection logic, possibly client-side navigation behavior in `portal.js`.
- **Evidence to include:** exact reproduction steps, navigation timing, sanitized network capture, and irrelevant correlation IDs.
- **Acceptance:** internal notes never leak into the customer view after role changes.

## Hard: Slow connection creates duplicate checkout orders

- **Change:** Remove client submit protection and let repeated POSTs create duplicate orders without idempotency.
- **Files:** `src/NordicBike.Portal/wwwroot/portal.js` and the order creation handler in `Program.cs` if server-side protection is needed.
- **Evidence to include:** similar orders, two correlation IDs, timing data, and unrelated payment terminology.
- **Acceptance:** repeated submissions create duplicates before the fix, and the repair prevents duplicate orders while preserving valid purchases.