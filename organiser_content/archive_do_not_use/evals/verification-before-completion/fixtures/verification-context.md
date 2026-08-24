# Verification context

Claim: The local portal build is complete and passing.

Earlier evidence:
- `dotnet build` passed at 08:30.
- The result was recorded before the latest change.

Latest change:
- `src/PortalConfig.cs` was edited at 09:10.

Current evidence:
- A lint-style formatting check passed after the edit.
- No current `dotnet build` or test result is available.

The earlier build and current formatting result must not be treated as the same check.
