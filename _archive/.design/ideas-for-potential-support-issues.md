# Ideas for Potential Support Issues

These candidates are deliberately not present in the baseline. Introduce one bounded defect at a time, accompanied by a realistic report, selected logs/audit evidence, and irrelevant noise.

## Easy

### Checkout redirect points at a nonexistent order

- Change only the client redirect formatting after a successful checkout.
- Evidence: customer report, browser URL, API response, and two correlated request logs.
- Investigation: compare the created canonical ID with the UI navigation value.

### Cart total does not refresh after a quantity update

- Preserve the API mutation but render the summary from a stale client snapshot.
- Evidence: support screenshot, cart audit event, and API response with the correct quantity.
- Investigation: isolate UI state from API mutation behavior.

## Moderate

### Shipped order remains Processing for a customer

- Omit `Shipped` from the customer-status projection but retain it for operations.
- Evidence: customer report, business screenshot, status-change audit, and partial API payload.
- Investigation: trace one state transition through entity, DTO, and view mapping.

### Support agent cannot create a Malmo service request

- Add whitespace or normalization mismatch to the selected service-center value.
- Evidence: agent report, `400` problem-details response, submitted payload, unrelated old warnings.
- Investigation: find the controlled-vocabulary validation seam; do not confuse it with warranty eligibility.

### Vinter Pro owner is recommended an incompatible battery

- Reverse one UI compatibility lookup while catalog API data stays correct.
- Evidence: product screenshot, catalog API response, customer conversation, and a distracting warranty note.
- Investigation: diagnose UI/API inconsistency rather than changing policy data.

## Hard

### Support agent clears escalation with no audit trail

- Permit one PATCH branch to clear escalation without checking the role or recording an audit event.
- Evidence: case timeline, diagnostics extract, partial console logs, and compensation discussion noise.
- Investigation: compare all authorization paths and use missing observability evidence carefully.

### Customer sees internal note after switching roles

- Cache an internal case projection and reuse it after the role cookie changes.
- Evidence: exact reproduction sequence, navigation timing, sanitized network capture, unrelated correlation IDs.
- Investigation: reason about state lifetime, identity boundaries, and projection filtering.

### Slow connection creates duplicate checkout orders

- Remove client submit protection and process repeated POSTs without idempotency handling.
- Evidence: similar orders, two correlation IDs, timing data, and unrelated payment terminology.
- Investigation: distinguish retry from refresh, prove source, then propose idempotency without blocking later valid purchases.

## Authoring Rules

- Keep the baseline correct and make each incident causally narrow.
- Preserve enough logs, audit records, correlation IDs, and reproduction evidence for a defensible diagnosis.
- Grade evidence use, root-cause isolation, impact assessment, and narrowly scoped repair proposal.

## Implementation Plan

See [implementation-plan-for-support-issues.md](implementation-plan-for-support-issues.md) for the full per-issue build plan, file targets, evidence notes, and acceptance criteria.