# Behavior change

Change `formatGreeting(name)` so a name with leading or trailing whitespace is normalized before the greeting is returned.

Expected behavior:
- `formatGreeting("  Anna  ")` returns `"Hello, Anna!"`.
- The existing behavior for an already normalized name remains unchanged.
- Empty input may be handled as the implementation currently handles it; do not add unrelated validation.

The fixture has no test runner. A small Node.js assertion script is an acceptable focused test.
