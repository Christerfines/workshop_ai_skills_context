---
name: systematic-debugging
description: "Use when investigating a bug, failing test, build failure, integration problem, performance regression, or other unexpected behaviour before deciding what to change."
---

# Systematic Debugging

## Purpose

Find the cause of an unexpected result before proposing or applying a fix. Keep the investigation reproducible, local where possible, and narrow enough that each observation changes the next decision.

## Boundaries

This skill may guide a repair when the user asks for one, but the investigation comes first. It does not guess from a symptom, bundle unrelated fixes, alter production data, or claim that a problem is fixed without fresh verification. Do not use a remote repository, service connection, or external issue tracker as a prerequisite when the checked-out files and local tools can answer the question.

## Inputs

Start with the smallest available set of:

- the observed symptom and expected result;
- the command, route, test, or user path that fails;
- the current working-tree state and relevant local configuration;
- the smallest safe fixture or reproduction;
- recent changes, when timing makes them relevant.

Treat user guesses, stale logs, and second-hand descriptions as hypotheses, not facts.

## Workflow

1. **State the failure.** Record `expected`, `actual`, target, input, environment, and whether the observation is first-hand. Do not put a suspected cause in the expected or actual statement.
2. **Reproduce safely.** Run the smallest local reproduction. Repeat it when safe. Record the exact command and result. If it cannot be reproduced, report that limitation and gather one observation that would distinguish intermittent behaviour from a setup error.
3. **Collect focused evidence.** Read the error output completely, then inspect only the relevant boundary: working-tree diff, recent local history, configuration, caller/callee, API payload, or test fixture. Prefer a working comparison when one exists. Do not open broad context without a question it answers.
4. **Form one hypothesis.** Write `I think <specific cause> because <evidence>`. Name at least one plausible alternative and the observation that would rule it out.
5. **Run one discriminating check.** Choose the smallest read-only observation, instrumentation change, or failing test that separates the hypothesis from its strongest alternative. Change one variable at a time.
6. **Decide from the result.** Confirm, revise, or reject the hypothesis. If it is rejected, return to step 3 with the new evidence. If it is supported and a repair was requested, make the smallest change that addresses the cause; do not combine it with cleanup.
7. **Verify the result.** Re-run the original reproduction, the new regression test, and the narrow relevant checks. Preserve the command, exit code, and meaningful output. A passing lint check alone does not prove a build or behaviour claim.

## Stop Conditions

Stop and report `blocked` when the required reproduction, identifier, fixture, or environment is unavailable. Stop and ask before changing architecture when three focused fixes have failed or each fix reveals a different coupled failure. Stop investigating when the evidence supports one cause, the strongest alternative is ruled out or explicitly unresolved, and the requested verification has run.

## Output

Return a structured investigation record:

```json
{
  "status": "investigating | blocked | ready_for_change | verified",
  "symptom": {
    "expected": "",
    "actual": "",
    "target": "",
    "reproduction": "reproduced | intermittent | not_reproduced | not_run"
  },
  "evidence": [
    {
      "question": "",
      "source": "local path, command, or output",
      "observation": "",
      "supports": "hypothesis or alternative"
    }
  ],
  "hypothesis": "",
  "alternatives": [
    {
      "explanation": "",
      "status": "ruled_out | unresolved",
      "ruling_observation": ""
    }
  ],
  "next_step": "",
  "verification": {
    "commands": [],
    "exit_codes": [],
    "result": "",
    "remaining_gaps": []
  }
}
```

Use `blocked` or `ready_for_change` instead of inventing a root cause. Use `verified` only when the original symptom and relevant regression checks were run after the final change.

## Examples

- In scope: "The checkout test started returning 302. Reproduce it locally, compare the last working commit, and identify the first boundary where the response changes."
- In scope: "The .NET build fails after a configuration change. Record the compiler output, form one hypothesis, and run one check before editing code."
- Out of scope: "Rewrite the module for readability" without an observed failure. Use a refactoring or design workflow instead.
