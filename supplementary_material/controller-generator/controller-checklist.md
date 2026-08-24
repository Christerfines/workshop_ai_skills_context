# Controller Delivery Checklist

- MVC services and controller routing are registered without changing existing minimal API routes.
- A typed request contract represents the warranty-claim input.
- The controller validates required input before creating a support case.
- Ownership is resolved from `PortalIdentity` and checked for bike and optional order.
- Success returns the created case identifier or tracking representation.
- Rejected input or inaccessible records do not mutate the store.
- Successful mutations add a customer-visible timeline entry and a redacted audit event.
- Integration coverage proves one successful submission and one rejected access attempt.