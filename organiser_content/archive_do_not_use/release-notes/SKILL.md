---
name: release-notes
description: "Use when preparing user-facing release notes, a changelog entry, or a version summary from local Git history, a tag range, a diff, commits, or supplied pull-request metadata."
---

# Release Notes

## Purpose

Create a reviewable release-notes draft from local version-control evidence. Describe user-visible changes in plain language, preserve traceability to commits and files, and make missing metadata explicit.

## Boundaries

This skill drafts notes only. It does not create or move tags, create branches, push changes, publish a release, edit a changelog without explicit file instructions, or infer user impact that the evidence does not support. Local Git is the required evidence source. Azure DevOps pull-request or work-item metadata may be supplied as a local export or read through an explicitly configured read-only MCP, but authentication and a live service are optional. Do not call a platform CLI or API as a prerequisite.

## Inputs

Collect:

- the release identifier or comparison range, such as `v1.2.0..HEAD`;
- the repository checkout and its local tags, commits, and diff;
- the intended audience and release-note format, if known;
- optional local PR/work-item metadata export;
- any labels, paths, authors, or generated files to exclude.

If no range is supplied, inspect local tags and ask before choosing a baseline. If no tags exist, use an explicitly confirmed commit range; never silently compare against an arbitrary remote.

## Workflow

1. **Resolve the range.** Identify the previous reference and target revision from local Git. Record whether each reference is a tag, branch, or commit and whether the range is empty.
2. **Read primary evidence.** Inspect the diff/stat for the relevant product or public paths, then read commit subjects and bodies. Use commit messages as context, not as proof when the code says otherwise.
3. **Enrich optionally.** If a local metadata export or configured read-only Azure DevOps MCP is available, associate PR titles, work items, labels, and authors with commits. Treat absent or conflicting metadata as an uncertainty, not a reason to invent a category.
4. **Classify changes.** Group supported user-facing changes under Added, Changed, Deprecated, Removed, Fixed, and Security as appropriate. Exclude tests, internal refactors, generated output, and dependency noise unless they change user-visible behaviour or release risk.
5. **Write for the audience.** Use plain language and explain the effect for a user or operator. Keep exact product names, version identifiers, and known limitations. Do not copy commit prefixes as prose.
6. **Trace and qualify.** Attach commit IDs, changed paths, or supplied PR/work-item identifiers to each entry. Mark entries as `needs_review` when the user impact, breaking nature, or category is not established by local evidence.
7. **Stop for review.** Return the draft and a review checklist. Do not write or publish release state unless the user separately requests a specific local file edit.

## Local-Only Operation

The skill must work when the checkout has no usable remote and no platform metadata. In that case use `git tag`, `git log`, `git diff`, and `git show` against local objects. A local JSON export can carry optional fields such as `id`, `title`, `url`, `labels`, `work_items`, and `commit_ids`; absent fields remain unknown.

## Output

Return a reviewable Markdown draft plus structured provenance:

```json
{
  "range": {
    "from": "",
    "to": "",
    "source": "local git"
  },
  "draft": "## [version] - date\n\n### Added\n- ...",
  "entries": [
    {
      "category": "Added | Changed | Deprecated | Removed | Fixed | Security",
      "text": "",
      "evidence": ["commit or path"],
      "confidence": "high | medium | low",
      "needs_review": false
    }
  ],
  "omitted": [
    {
      "reason": "internal-only | duplicate | unsupported-impact | unknown",
      "evidence": []
    }
  ],
  "assumptions": [],
  "side_effects": "No tags, branches, pushes, or releases were created."
}
```

## Examples

- In scope: "Draft release notes for the local range v2.3.0..HEAD using the commit history and diff. Do not publish anything."
- In scope: "Turn these local commits and exported Azure DevOps PR titles into a Keep a Changelog draft, flag uncertain user impact."
- Out of scope: "Cut and publish the release." Hand off the draft; release operations are separate and require explicit human action.
