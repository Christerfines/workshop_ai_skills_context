---
name: automated-code-review
description: "Use when reviewing changed C# code in the NordicBike Portal for the team's coding standards, endpoint conventions, regression risk, and required test coverage. Runs a local PowerShell candidate scanner and loads standards only when the relevant rule category is in scope."
---

# Automated Code Review

## Purpose

Review a local NordicBike Portal change against the team's fictional C# and minimal-API standards. Run the included deterministic scanner first, then verify every candidate in its source context before reporting it. The scanner identifies review leads; it does not establish a defect.

## Scope And Boundaries

This skill is advisory and read-only. It reviews a local Git diff, a supplied patch, or explicitly named files. It does not edit code, approve a pull request, post comments, or claim that a change is correct merely because the scanner returns no candidates.

The reviewed solution is `src/NordicBike.Portal.slnx`, which contains:

- `src/web/NordicBike.Portal.csproj`, a .NET 10 minimal-API portal;
- `src/test/NordicBike.Portal.Tests/NordicBike.Portal.Tests.csproj`, its NUnit test project.

Exclude `bin`, `obj`, `.vs`, and generated assets unless the request explicitly includes them.

## Required Inputs

Establish one review target before starting:

- current working-tree or staged diff;
- a local base and head revision;
- a supplied patch; or
- one or more named source files.

Ask for the target when none is supplied. Record the exact files and revision or diff mode in the result.

## Workflow

1. Inspect the requested local diff or files and identify whether it changes an API endpoint, data contract, state mutation, HTML rendering, configuration, or tests.
2. Run the candidate scanner from the repository root:

   ```powershell
   pwsh -File organiser_content/completed_skills/automated-code-review/scripts/Invoke-NordicBikeCodeReview.ps1 -RepositoryRoot .
   ```

   To check compilation and automated tests as part of the same run, add `-RunTests`. The script emits JSON, exits `0` when it finds no candidates, and exits `1` when candidates or a requested verification command fails.
3. Load [references/project-context.md](references/project-context.md) for every review. It describes the solution structure and established implementation patterns.
4. Load only the relevant sections of [references/nordicbike-coding-standards.md](references/nordicbike-coding-standards.md): baseline C# rules for all changes, API rules for endpoint changes, state/audit rules for mutations, and test rules for behavior changes.
5. Verify each scanner candidate by reading the changed line and nearby code. Remove candidates covered by an equivalent abstraction or an intentional, documented exception. Inspect changed endpoint handlers for validation, ownership or role checks, status codes, and `PortalAudit.Record` where the change mutates customer, order, case, bike, or service-request state.
6. Review tests proportionally. A changed externally observable route, validation rule, state transition, authorization decision, or serialization contract normally requires a focused NUnit test under `src/test/NordicBike.Portal.Tests`.
7. Return only evidence-backed findings. Use `note` for a standards issue, `minor` for a likely regression or missing targeted test, and `major` for a reachable incorrect result, authorization failure, data leak, or un-audited consequential mutation.

## Output

Return this JSON object first:

```json
{
  "scope": {
    "source": "working tree | staged | base..head | patch | paths",
    "solution": "src/NordicBike.Portal.slnx",
    "files": [],
    "limitations": []
  },
  "checks_run": [
    {
      "command": "",
      "result": "passed | candidates_found | failed | not_run",
      "details": ""
    }
  ],
  "findings": [
    {
      "id": "NB-001",
      "severity": "major | minor | note",
      "standard": "NB###",
      "file": "",
      "line": 0,
      "title": "",
      "evidence": "",
      "impact": "",
      "recommendation": "",
      "validation": ""
    }
  ],
  "summary": "",
  "human_review_required": true,
  "side_effects": "No files changed and no pull-request decision or comment was posted."
}
```

When no finding survives verification, state that no actionable standards issue was supported in the reviewed scope, and retain the commands and limitations. Do not call the change approved.

## Examples

- "Review my working-tree changes to `PortalApiEndpoints.cs` using the Automated Code Review skill and run its tests."
- "Review `main..HEAD` for NordicBike endpoint, audit, and test-standard violations. Return JSON only."
- "Apply all findings and approve the pull request." Report the review findings; applying changes and approval are outside this skill.