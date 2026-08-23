# Local reproduction evidence

Target: local checkout request from the demo portal

Expected: a submitted checkout completes once and returns a successful response.
Actual: the browser returns a redirect to the sign-in page after the checkout button is pressed.

Reproduction command: run the local portal, select the customer persona, add one item, and submit checkout.
Observed: the redirect occurred on two consecutive local attempts.

Relevant evidence already captured:
- The request includes a valid local customer id.
- The response status is 302 and the Location header points to `/login`.
- The last local change touched request handling and session state.

This file is evidence, not a root-cause determination. The next check should distinguish an authentication-state problem from a client request problem.
