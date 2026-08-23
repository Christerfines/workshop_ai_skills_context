# Optional Skill Evaluations

This directory contains optional Waza suites for the six completed reference skills. The suites are designed for a developer workstation: fixtures are local, no GitHub repository or API is needed, no Azure DevOps connection is needed, and no GitHub Actions workflow is assumed.

The Waza Copilot executor still uses the developer's GitHub Copilot entitlement to run the agent. That is an AI runtime requirement, not a dependency on a GitHub-hosted source repository. Use the mock executor or a local Waza setup when you only need to inspect the suite shape.

## Baseline First

Run the dependency-free checker before using Waza:

```powershell
pwsh -NoProfile -File .\workshop\validate-reference-skills.ps1
```

The checker deliberately skips the unfinished `support-diagnosis` skill and the three participant prompt scaffolds.

## Suite Layout

Each suite follows the Waza convention:

```text
evals/<skill-name>/
├── eval.yaml
├── fixtures/
└── tasks/
```

The fixtures are compact evidence snapshots. They do not contain credentials, live service data, facilitator answer keys, or instructions to contact a platform.

Available suites:

- `systematic-debugging` — reproduction, hypothesis testing, and evidence before fixes.
- `test-driven-development` — RED-GREEN-REFACTOR and meaningful test failure.
- `verification-before-completion` — fresh evidence and claim-scoped verification.
- `security-review` — read-only findings, confidence, and false-positive handling.
- `release-notes` — local change history, user-facing categories, and missing metadata.
- `structured-pr-review` — local patch review, line evidence, prioritization, and advisory output.

## Run A Suite

Install Waza separately according to its current documentation, then run the suite from the repository root:

```powershell
waza check .\.github\skills\systematic-debugging
waza spec verify .\.github\skills\systematic-debugging .\evals\systematic-debugging\eval.yaml
waza run .\evals\systematic-debugging\eval.yaml --verbose
```

Repeat with another suite by replacing the skill and evaluation directory names. To run every suite locally:

```powershell
Get-ChildItem .\evals -Filter eval.yaml -Recurse | ForEach-Object {
    waza run $_.FullName --verbose
}
```

The task files use the Copilot SDK executor so they can test the actual skill instructions. If authentication or a model is unavailable, run the structural checker and review the tasks manually; do not claim that a behavioral evaluation passed. Waza result files should be written to an ignored output directory when possible.

## What The Suites Check

The suites include positive cases and near-misses. The intended invariants are:

- debugging records expected and actual behavior, reproduces before fixing, and changes one variable at a time;
- TDD observes a meaningful RED failure before GREEN and does not weaken a test to obtain a pass;
- verification reruns fresh evidence against the current scope and does not infer success from an old command or a diff;
- security review remains read-only, redacts sensitive values, distinguishes evidence from suspicion, and hands off remediation;
- release notes use local change evidence, qualify uncertain impact, and produce no release-side effects;
- PR review anchors actionable findings to the local patch, separates questions from defects, and makes no approval or edit decision.

Use Waza's behavior and action-sequence graders only for observable tool behavior. Use text or code graders for the output contract. Reserve model-judged or human review for reasoning quality. Run no-skill baselines for the pressure tasks before treating a skill as effective.

## Optional MCP

The default suites do not require MCP. A local, unauthenticated MCP server may be used when it adds a capability that is difficult to obtain from ordinary local tools. The optional `verification-before-completion` browser scenario can use a local Playwright MCP server against the locally running portal. Keep the server restricted to the local host and use an isolated browser profile; do not commit credentials, browser storage state, or a service configuration to this repository.

If the MCP server is unavailable, run the equivalent local command or file/DOM inspection. A missing optional MCP must not turn a skill run into a failure.

For deterministic MCP experiments, use Waza's local `mcp_mocks` in a separate evaluation variant. Do not connect an evaluation run to a live GitHub, Azure DevOps, or other platform server.

## Optional Azure DevOps Context

The release-notes and structured-pr-review skills can consume a local JSON export containing pull-request or work-item metadata. This is the recommended bridge for this repository because it works offline and does not expose credentials.

A developer who has Azure DevOps access may create an export with the Azure DevOps CLI or retrieve read-only context through the Azure DevOps MCP server, then save only the fields needed by the skill. Do not make `az repos`, REST calls, or Azure DevOps MCP a required step, and do not commit organization names, tokens, private URLs, or customer data.

The Azure DevOps MCP server requires an organization and authentication. It is an optional enrichment path, not part of the baseline evaluation contract.

## No Actions Dependency

These suites are local examples. They do not add or require a GitHub Actions workflow. Teams that use Azure Pipelines or another local runner can invoke the same Waza commands from their own pipeline configuration without changing the skill contracts.
