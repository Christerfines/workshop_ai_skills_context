# Reference Agent Skills

This catalog describes the completed reference skills in `.github/skills/`. They are original local examples inspired by public skill repositories and the Agent Skills specification. The existing `support-diagnosis` skill remains intentionally incomplete for the workshop exercise, and the three files in `.github/prompts/` remain participant scaffolds.

## Dependency Contract

Every reference skill works from a local checkout with VS Code and GitHub Copilot, Git, and the project toolchain relevant to the task. A GitHub-hosted repository, GitHub CLI/API, GitHub Actions, live Azure DevOps connection, or authenticated service is not required.

Optional integrations follow this order:

1. **Local baseline:** checked-out files, local Git history, local commands, and supplied fixtures or exports.
2. **Optional local or unauthenticated MCP:** use only when it adds a capability such as local browser inspection. Detect it first and preserve the baseline fallback.
3. **Optional authenticated Azure DevOps context:** use only when the developer has configured access. Read metadata; do not make platform operations part of the skill contract.

The Copilot license is the AI-runtime assumption. It is not a repository-hosting or work-management dependency.

## Curated Set

| Skill | Useful repository workflow | External pattern | Local demonstration |
| --- | --- | --- | --- |
| [`systematic-debugging`](../.github/skills/systematic-debugging/SKILL.md) | Investigate an unexpected result before changing code. | Superpowers systematic debugging and GitHub's bug-reproduction brief. | Reproduce one seeded portal or build failure using local files and commands. |
| [`test-driven-development`](../.github/skills/test-driven-development/SKILL.md) | Write a failing behavioral test, implement minimally, then refactor. | Superpowers TDD and Anthropic's skill-creator testing loop. | Use a small change in the local .NET project or a fixture-only example. |
| [`verification-before-completion`](../.github/skills/verification-before-completion/SKILL.md) | Gate completion claims on fresh, complete evidence. | Superpowers verification-before-completion and Waza result checks. | Verify a local build/test or inspect the locally running portal. |
| [`security-review`](../.github/skills/security-review/SKILL.md) | Review code and data flows without modifying the repository. | GitHub `awesome-copilot` security-review and public security-report patterns. | Review a bounded set of files under `src/` with no credentials or network. |
| [`release-notes`](../.github/skills/release-notes/SKILL.md) | Draft user-facing notes from local tags, commits, and diffs. | GitHub generated release notes and the `github-release` skill, narrowed to drafting. | Compare local Git tags or a checked-in fixture history. |
| [`structured-pr-review`](../.github/skills/structured-pr-review/SKILL.md) | Produce independent, line-grounded review findings for another developer. | GitHub pull-request review semantics and Superpowers requesting-code-review. | Review a local base/head range, staged diff, working-tree diff, or patch. |

The external material supplies patterns, not text to copy. The local versions deliberately remove hosted-repository assumptions, write side effects, and service-specific operations.

## How To Run Them Locally

Open the repository in VS Code and ask Copilot to use the relevant skill. The following prompts are intentionally ordinary developer requests:

```text
Use systematic-debugging on the failing local checkout path. Reproduce it first and report the evidence before changing anything.

Use test-driven-development to add the smallest behavior change described here. Show the RED command and expected failure before implementation.

Use verification-before-completion to verify the local build and focused tests after the last change. Do not rely on an earlier run.

Use security-review on the current local diff. Do not modify files, expose secret values, or use a remote service.

Use release-notes to draft notes for the local range v0.1.0..HEAD. Use local Git evidence and mark missing PR metadata as unknown.

Use structured-pr-review on the local base..head diff as an independent reviewer. Return JSON findings with line evidence and do not edit or approve anything.
```

## Optional MCP

MCP is an enhancement, not a hidden prerequisite.

- A local Playwright MCP server can add browser inspection for `verification-before-completion` when the portal is running. Keep it restricted to the local portal and use ordinary local commands or DOM inspection when it is unavailable.
- Waza evaluation suites may use local `mcp_mocks` for deterministic tool behavior. They must not call a live platform server.
- Azure DevOps MCP can provide read-only pull-request or work-item context when explicitly configured and authenticated. The skills must fall back to local Git or a local JSON export, and no Azure DevOps MCP configuration is committed by default.

The repository should not commit credentials, storage state, or a service-specific `.vscode/mcp.json` merely to demonstrate a skill.

## Local Metadata Export

Release notes and PR review may accept a neutral local JSON export. An Azure DevOps-shaped export is useful, but the skill should not depend on the provider name:

```json
{
  "pull_requests": [
    {
      "id": "PR-42",
      "title": "Improve checkout retry handling",
      "description": "",
      "labels": ["bug"],
      "work_items": ["WI-1001"],
      "commit_ids": ["abc1234"]
    }
  ],
  "work_items": [
    {
      "id": "WI-1001",
      "title": "Prevent duplicate checkout attempts"
    }
  ]
}
```

Missing URLs, authors, labels, or work-item descriptions remain missing. The export is input data, not authority to publish or approve anything.

## Smoke-Test Matrix

These checks are designed to run without a hosted repository or authenticated service.

| Skill | Local scenario | Required invariant | Near-miss or boundary probe |
| --- | --- | --- | --- |
| `systematic-debugging` | Give it a reproducible local failure with an obvious-looking cause. | It records expected/actual, reproduces, forms one hypothesis, and runs a discriminating check before a fix. | Failure cannot be reproduced; it reports `blocked` or `not_reproduced` instead of guessing. |
| `test-driven-development` | Request a small behavior change in the local project. | It shows a meaningful RED failure before production code and a minimal GREEN change. | Ask to skip the test because the change is “obvious”; it asks for an explicit exception. |
| `verification-before-completion` | Claim the current build is complete after an earlier passing run. | It runs fresh evidence against the current checkout and reports the exact scope. | A lint-only pass is offered as proof of a build; it rejects the stronger claim. |
| `security-review` | Review a bounded local diff containing a real risk signal and a benign look-alike. | It stays read-only, redacts values, gives evidence and confidence, and filters the false positive. | Ask it to apply all fixes or post findings; it returns a handoff only. |
| `release-notes` | Provide a local tag range and a metadata export with one missing field. | It produces categorized, traceable user-facing notes and marks uncertainty. | No tags or remote exist; it asks for or uses an explicit local commit range and does not publish. |
| `structured-pr-review` | Review a local patch with one actionable issue, one unproven concern, and unrelated noise. | It prioritizes the actionable issue, separates questions, anchors findings to the patch, and makes no edits. | No patch/range is supplied; it asks for local scope rather than contacting a platform. |

## Evaluation Levels

1. **Basic:** Run the static validator and the smoke-test matrix manually in VS Code.
2. **Portable specification:** Run `skills-ref validate` against each completed skill when the optional validator is installed.
3. **Repeatable evaluation:** Run the Waza suites under `evals/` with local fixtures. Start with no live MCP, Azure DevOps, or repository-host connection.
4. **Optional enhancement:** Run the local MCP or Waza mock variants separately and compare the result with the local-only path.

## Research Sources

- [Agent Skills specification](https://agentskills.io/specification): portable `SKILL.md` structure, metadata, progressive disclosure, local references, optional resources, and validation.
- [Anthropic skills](https://github.com/anthropics/skills): self-contained skills across development, documents, and web testing.
- [Anthropic skill-creator](https://github.com/anthropics/skills/blob/main/skills/skill-creator/SKILL.md): intent capture, progressive disclosure, baseline testing, and iteration.
- [Superpowers systematic-debugging](https://github.com/obra/superpowers/blob/main/skills/systematic-debugging/SKILL.md): evidence before fixes and one hypothesis at a time.
- [Superpowers test-driven-development](https://github.com/obra/superpowers/blob/main/skills/test-driven-development/SKILL.md): RED-GREEN-REFACTOR and verification of the expected failure.
- [Superpowers verification-before-completion](https://github.com/obra/superpowers/blob/main/skills/verification-before-completion/SKILL.md): fresh evidence before completion claims.
- [Superpowers writing-skills](https://github.com/obra/superpowers/blob/main/skills/writing-skills/SKILL.md): skill-specific baseline tests and iterative improvement.
- [GitHub security-review](https://github.com/github/awesome-copilot/blob/main/skills/security-review/SKILL.md): scoped security analysis, self-verification, severity, and human approval.
- [GitHub bug-reproduction-brief](https://github.com/github/awesome-copilot/blob/main/skills/bug-reproduction-brief/SKILL.md): expected/actual separation, minimal reproduction, repeatability, and stopping before repair.
- [GitHub generated release notes](https://docs.github.com/en/repositories/releasing-projects-on-github/automatically-generated-release-notes): merged changes, categories, exclusions, and review before publishing.
- [GitHub github-release](https://github.com/github/awesome-copilot/blob/main/skills/github-release/SKILL.md): diff-first classification and review gates, adapted here without release-side effects.
- [GitHub pull-request reviews](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/reviewing-changes-in-pull-requests/about-pull-request-reviews): review decisions, line comments, and suggestions.
- [Superpowers requesting-code-review](https://github.com/obra/superpowers/blob/main/skills/requesting-code-review/SKILL.md): precise review context and severity-based handoff.
- [Microsoft Waza](https://github.com/microsoft/waza): local skill evaluation, fixtures, graders, baselines, and MCP mocks.
- [Microsoft Playwright MCP](https://github.com/microsoft/playwright-mcp): optional local browser automation without platform credentials.
- [Microsoft Azure DevOps MCP](https://github.com/microsoft/azure-devops-mcp): optional authenticated work-item and pull-request context, not a baseline dependency.
