---
name: structured-pr-review
description: "Use when another developer needs an independent, structured review of a local pull-request change, base/head diff, working-tree diff, staged diff, or supplied patch for correctness, security, maintainability, and test risk."
---

# Structured Pull-Request Review

## Purpose

Give another developer a focused, repeatable review of a change set. Prioritize actionable risks, ground every finding in the local diff and surrounding code, and separate evidence from questions or preferences.

## Boundaries

This is an advisory review. It does not edit files, apply suggestions, approve or reject a change, vote, post comments to GitHub or Azure DevOps, or declare a change correct because no finding was produced. It does not require a hosted pull request. Review a local base/head range, current working-tree or staged diff, or supplied patch. Optional read-only Azure DevOps metadata may add business context, but the review must work without it.

## Inputs

Request one of:

- a local base and head revision;
- the current working-tree or staged diff;
- a patch file or pasted diff;
- optional local PR metadata export containing title, description, linked work items, labels, or reviewers;
- explicit review priorities such as security, data integrity, compatibility, performance, or test coverage.

If the change set is unspecified, ask which local range or patch to inspect. Record the exact scope and revision before forming findings.

## Workflow

1. **Establish the review scope.** Identify the base and head, or record that the input is a working-tree/staged patch. Inspect status and diff statistics. Note unreviewed files, generated output, and missing context.
2. **Understand intent.** Read the local change description, commit messages, and relevant tests. Use optional exported or read-only Azure DevOps context only to clarify intent; never treat a work-item title as proof that the implementation is safe.
3. **Read changed code.** Inspect each changed hunk and enough surrounding code to understand data flow, error handling, authorization, concurrency, compatibility, and resource lifetime. Follow important calls across files.
4. **Test review risks.** Check whether the change preserves existing contracts, handles failure paths, protects sensitive data, and adds or updates a meaningful test for changed behaviour. Run safe, focused local checks only when requested or clearly necessary; record commands and results.
5. **Separate finding types.** Report an actionable defect only when the evidence shows a concrete incorrect, unsafe, or likely-to-break outcome. Put plausible but unproven concerns in `questions`. Put style preferences in `notes` or omit them. Do not report unchanged code unless the change makes it newly relevant.
6. **Self-verify.** Re-read every proposed finding, check for guards or tests that invalidate it, and confirm the file and line are in the reviewed scope. Downgrade confidence or remove the finding when evidence is incomplete.
7. **Hand off.** Order findings by severity, include a precise next step and validation idea, and state that a human developer owns the decision. A no-finding result means no actionable issue was supported in this review, not approval.

## Output

Return JSON first, followed by no additional prose unless requested:

```json
{
  "review_scope": {
    "source": "base..head | working tree | staged | patch",
    "base": "",
    "head": "",
    "files": [],
    "limitations": []
  },
  "decision": "findings | no_actionable_findings | blocked",
  "summary": "",
  "findings": [
    {
      "id": "PR-001",
      "severity": "blocker | major | minor | note",
      "confidence": "high | medium | low",
      "file": "relative/path",
      "line": 0,
      "title": "",
      "evidence": "exact local code or diff excerpt",
      "why_it_matters": "",
      "suggested_next_step": "",
      "validation": ""
    }
  ],
  "questions": [
    {
      "topic": "",
      "evidence": "",
      "question": ""
    }
  ],
  "notes": [],
  "checks_run": [],
  "human_review_required": true,
  "side_effects": "No files changed and no review decision or platform comment was posted."
}
```

Use `line: 0` only for a finding that cannot be anchored to one line; explain the limitation. Keep findings concise enough for a developer to act without reconstructing the entire review.

## Examples

- In scope: "Review the local base..head diff for correctness and security. Return JSON findings with file and line evidence; do not edit anything."
- In scope: "Review the staged patch as a second developer. Treat missing tests as a risk only when the changed behaviour needs coverage, and distinguish questions from defects."
- Out of scope: "Apply the review suggestions and approve the PR." Return the findings and leave the change and decision to the developer.
