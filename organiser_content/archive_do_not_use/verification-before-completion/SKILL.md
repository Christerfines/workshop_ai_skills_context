---
name: verification-before-completion
description: "Use when someone is about to claim work is complete, fixed, passing, tested, ready to merge, ready to release, or otherwise successful and the claim needs fresh evidence."
---

# Verification Before Completion

## Purpose

Make completion claims traceable to fresh evidence. Match the strength of the claim to the command or inspection that actually proves it, and make gaps visible instead of filling them with confidence.

## Boundaries

This skill verifies a claim; it does not decide whether a feature is desirable, approve a pull request, publish a release, or replace a code review. It must not treat a diff, an agent report, a previous run, a passing lint command, or an unchanged file as proof of a stronger claim. It must not require a GitHub or Azure DevOps status check. Local commands and checked-out files are the default evidence.

## Inputs

Capture:

- the exact claim to verify;
- the changed scope and current working-tree state;
- the command, test, build, or inspection that would prove the claim;
- the expected success signal and relevant failure signals;
- whether evidence must include a local browser or other optional tool.

If the claim is underspecified, narrow it before running a check. "It works" must become an observable behaviour, build, test, or artifact claim.

## Workflow

1. **Parse the claim.** Identify the subject, scope, and strength. A claim that a bug is fixed needs the original reproduction; a claim that a project builds needs the build command; a claim that a release is ready needs the stated release checks.
2. **Choose fresh evidence.** Prefer the narrowest complete command that proves the claim, followed by relevant regression checks. Use the current checkout, not an old transcript or a different branch. Inspect the command before running it if its scope is unclear.
3. **Run the complete check.** Do not replace the full command with a shortcut. Capture the exit code, duration when useful, warnings, skipped tests, and the meaningful output. Do not stop at the first green line.
4. **Inspect the result.** Confirm that the output covers the claimed scope and that no failure was hidden by a pipe, filter, cache, stale artifact, or partial test selection. If a local browser MCP is available, it may provide richer inspection of the local portal; the equivalent command or DOM inspection must remain documented and usable when MCP is absent.
5. **Check regressions.** Run the narrow regression and the smallest broader check needed for the claim. Separate "verified for this scope" from "verified for the whole project."
6. **Report honestly.** Return `verified`, `partially_verified`, `blocked`, or `failed`. Name the exact evidence and every remaining gap. Do not convert a missing check into an implied pass.

## Freshness Rules

Evidence is fresh only when it ran after the final relevant change, against the current checkout, with the intended inputs. Re-run a command when the file, dependency, environment, or branch changed. A previous pass may be useful context but never closes the gate by itself.

## Output

Return a verification record:

```json
{
  "claim": "",
  "scope": "",
  "status": "verified | partially_verified | failed | blocked",
  "checks": [
    {
      "command_or_inspection": "",
      "purpose": "",
      "ran_after_last_change": true,
      "exit_code": 0,
      "observed_result": "",
      "coverage": ""
    }
  ],
  "freshness": "",
  "remaining_gaps": [],
  "conclusion": ""
}
```

Use `verified` only when every check required by the claim passed and the evidence covers the stated scope. Use `partially_verified` when the local result is useful but narrower than the claim.

## Examples

- In scope: "The checkout fix is complete." Re-run the original checkout reproduction and the focused regression test before confirming it.
- In scope: "The portal is ready to merge." Run the relevant local build and tests, inspect warnings and scope, and report anything not checked.
- Out of scope: "Review this diff for security issues." Use a security-review workflow; verification can later verify its findings or checks.
