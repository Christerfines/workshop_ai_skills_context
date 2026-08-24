---
name: security-review
description: "Use when reviewing a repository, diff, dependency set, or application flow for security vulnerabilities, secrets exposure, authorization failures, unsafe input handling, or trust-boundary problems."
---

# Security Review

## Purpose

Perform a scoped, read-only security review that follows data and authority across the local codebase. Report evidence-backed risks and safe remediation directions for human review without turning a suspicion into a vulnerability claim.

## Boundaries

The default review is local and read-only. It does not require GitHub, Azure DevOps, a live scanner, a production connection, or credentials. Do not edit files, run destructive commands, probe systems that were not explicitly placed in scope, reveal secret values, or publish a finding to an issue tracker. Do not infer exploitability from a keyword match or claim that a repository is secure because no obvious pattern was found. Recommendations are for human review; they are not automatic patches.

## Inputs

Establish:

- the exact path, diff, component, or repository scope;
- the runtime and dependency manifests that are present locally;
- trust boundaries, entry points, sensitive operations, and data classes;
- the threat or compliance question, if one was supplied;
- local fixtures or test commands that can validate a suspected issue safely.

When no scope is supplied, ask whether to review the current diff, a named directory, or the entire checkout. Exclude generated and build output unless it is itself part of the risk. If generated or build output is identified as a risk source (e.g., a committed artifact containing credentials), include it in scope, note it explicitly in the `excluded` override field, and explain why it was included.

### Review Category Reference

| Category |
| --- |
| Injection |
| Broken object-level authorization |
| Privilege escalation |
| Secrets exposure |
| Unsafe deserialization |
| Path traversal |
| Cross-site scripting |
| Request forgery |
| Weak cryptography |
| Dependency risk |
| Sensitive data leakage |
| Business-logic abuse |

## Workflow

1. **Resolve scope.** Record the files, revision, and environment reviewed. Check the current diff and repository layout before reading unrelated code.
2. **Inventory exposure.** Locate authentication and authorization boundaries, input sources, output sinks, file and process operations, dependency manifests, configuration, logs, and likely secret material. Redact values in notes and output. If a finding contains what appears to be a live secret, set the evidence field to "[REDACTED — possible live credential at <location>]" and add a limitations entry warning the reviewer to treat the report as sensitive before sharing.
3. **Trace flows.** Follow untrusted input to storage, queries, commands, templates, redirects, logs, network requests, and privileged actions across file boundaries. Check both the validation and the authorization decision at each sensitive boundary.
4. **Check categories.** Check each category in the reference table if and only if the code reviewed in step 3 contains a direct signal for that category (e.g., user-controlled input reaching the relevant sink).
5. **Self-verify each finding.** Re-read the source, identify sanitization or framework protections, reproduce only with a safe local fixture when the finding's confidence is medium or low and a fixture already exists in the repository, and distinguish confirmed evidence from a plausible concern. Discard findings that rely only on naming or correlation.
6. **Prioritize.** Assign severity based on reachable impact and exploit conditions, and confidence based on evidence quality. Group duplicate symptoms into one finding and include the strongest counterevidence.
7. **Hand off safely.** Provide a focused remediation direction, validation idea, and human review requirement. Do not apply the patch or send the report externally.

## Optional Tools

A local, unauthenticated analysis MCP may be used when it adds a read-only capability such as local browser inspection or a syntax-aware scan. Detect its availability first, keep the tool scope narrow, and use the local files and standard commands when it is absent. Never make an authenticated platform service a prerequisite for this skill.

## Output

Return a structured report:

```json
{
  "scope": {
    "revision": "",
    "paths": [],
    "excluded": [],
    "limitations": []
  },
  "summary": {
    "critical": 0,
    "high": 0,
    "medium": 0,
    "low": 0,
    "info": 0,
    "conclusion": ""
  },
  "findings": [
    {
      "id": "SEC-001",
      "severity": "critical | high | medium | low | info",
      "confidence": "high | medium | low",
      "category": "",
      "location": "path and line or symbol",
      "evidence": "redacted, checkable excerpt",
      "impact": "",
      "exploit_conditions": "",
      "counterevidence": "",
      "recommendation": "",
      "validation": ""
    }
  ],
  "review_boundary": "No files changed; findings require human validation."
}
```

If no actionable finding is supported, say so and list the scope and limitations. Do not use "clean" as a synonym for "secure."

## Examples

- In scope: "Review the current local diff for authorization and secret-handling risks. Do not modify files and redact any values you find."
- In scope: "Trace the request parameter from the local API entry point to its database or HTML sink and report only evidence-backed issues."
- Out of scope: "Apply the security fixes automatically." Produce the report and request human approval instead.
