# Diagnostic Brief — Output Schema

This document defines the structured output that the `support-diagnosis` skill must produce. Every field is mandatory unless marked optional. The brief is the *only* output — the skill does not produce prose, resolution steps, or code changes.

Participants use this as the output contract when authoring `SKILL.md`. Facilitators use it as the scoring reference for cross-team review.

---

## Why Structured Output

A brief written as prose forces every reader to extract different facts from the same text. A structured brief gives each consumer role exactly the field they need without reading the whole document. The schema below is designed so that:

- A **developer** reads `root_cause` and `evidence_chain` to know where to look
- A **QA engineer** reads `alternatives_ruled_out` and `next_actions[role=QA]` to know what to test
- A **product owner** reads `customer_impact`, `root_cause.confidence`, and `next_actions[blocks_release]` to make a prioritisation decision
- A **support agent** reads `customer_communication` to draft a response without introducing technical speculation

---

## Schema

```json
{
  "brief_id": "DIAG-YYYYMMDD-NNN",
  "case_reference": "ISSUE-NNN",
  "generated_at": "ISO-8601 timestamp",

  "scope": {
    "investigated": "One sentence: what report, which reported symptom, which system area.",
    "not_investigated": "One sentence: what was explicitly out of scope for this brief."
  },

  "customer_impact": "Plain-language statement of who is affected and what they cannot do. No technical terms. No cause — only observable effect.",

  "root_cause": {
    "statement": "One sentence naming the cause, not the symptom.",
    "confidence": "confirmed | likely | possible | insufficient_evidence",
    "confidence_rationale": "One sentence explaining why this confidence level was assigned, naming the specific evidence gap if below confirmed.",
    "area": "The system area or layer where the fault originates: e.g. client, API, validation, auth, data projection."
  },

  "evidence_chain": [
    {
      "question": "The investigation question this artifact was opened to answer.",
      "artifact": "Name or path of the evidence opened: e.g. API 400 response, /api/diagnostics/events extract, portal.js line 12.",
      "excerpt": "The specific value, line, or payload that answered the question. Exact text where possible.",
      "implication": "One sentence: what this excerpt tells us about the root cause."
    }
  ],

  "alternatives_ruled_out": [
    {
      "hypothesis": "Competing explanation that was considered.",
      "ruling_evidence": "The artifact or observation that made this hypothesis inconsistent with the facts."
    }
  ],

  "remaining_uncertainty": "What is still unknown that could change the diagnosis. Use 'None — diagnosis is confirmed by two independent sources' only when that is literally true.",

  "scope_boundary": [
    "Explicit statement of each thing this brief does not address. Use one entry per out-of-scope item.",
    "Examples: 'This brief does not propose a fix.', 'Compensation demand is out of scope — see next_actions.'"
  ],

  "next_actions": [
    {
      "role": "Developer | QA | Support agent | Product owner | Support lead | Architect",
      "action": "Specific, verb-first instruction. Must be actionable from this brief alone without reading additional documents.",
      "priority": "High | Normal | Low",
      "blocks_release": true
    }
  ],

  "customer_communication": {
    "language": "en | sv | [ISO 639-1 code matching the customer's message]",
    "facts_only": true,
    "text": "Two to four sentences. State what the customer cannot do, confirm the team is aware, and give the next concrete step the customer should expect. No technical terms. No root cause. No promises about timing or compensation."
  }
}
```

---

## Field Rules

### `root_cause.confidence`

| Level | Required evidence |
|---|---|
| `confirmed` | Two independent artifacts agree; no remaining alternative hypothesis |
| `likely` | One primary artifact supports the conclusion; one alternative not ruled out |
| `possible` | Evidence is consistent with the conclusion but also consistent with at least one alternative |
| `insufficient_evidence` | A clarifying question or additional artifact is required; do not produce a `root_cause.statement` |

When confidence is `insufficient_evidence`, the brief must still be complete. Set `root_cause.statement` to the specific question that must be answered and assign a `next_action` to the role who can answer it.

### `evidence_chain`

- Minimum one entry. No upper limit, but every entry must answer a named question.
- An artifact opened speculatively — without a question — must not appear.
- If a large log file was opened, the `excerpt` field must contain only the relevant lines. The full file must not be reproduced.

### `alternatives_ruled_out`

- Minimum one entry for any non-trivial case.
- A brief with no alternatives listed, and confidence above `possible`, will not pass cross-team review.
- A ruled-out hypothesis with no ruling evidence is not ruled out — it is speculated away.

### `next_actions`

- Every role who needs to act must have a corresponding entry.
- `blocks_release` must be a boolean. Do not leave it absent — absent is treated as false by reviewers.
- The `action` field must be executable from reading this brief alone. If it requires reading a separate document, cite the document in the action text.

### `customer_communication`

- `facts_only` is always `true`. A brief that contains a promise, a timeline estimate, a compensation offer, or a technical root-cause explanation in the customer communication section fails the scope boundary check.
- The language must match the language of the original customer report, not the default operating language of the portal.

---

## Anti-Patterns

These patterns will be flagged in cross-team review as context or scope failures.

| Anti-pattern | Why it fails |
|---|---|
| `root_cause.confidence: "confirmed"` with only one artifact | Confidence is overstated; violates the two-source rule |
| `evidence_chain` entry with no `question` field | Evidence was opened speculatively — context distraction risk |
| `alternatives_ruled_out` is an empty array | Competing hypotheses were not formed; diagnosis may be pattern-matched rather than reasoned |
| `customer_communication` contains the word "bug", "defect", "error", or any code reference | Technical language not appropriate for customer-facing facts |
| `next_actions` contains "investigate further" as the action | Not actionable; must name the specific artifact or question to pursue |
| `scope_boundary` is absent or empty | Scope overreach risk; reviewers cannot tell what the brief does not cover |
| `remaining_uncertainty: "None"` with `confidence: "likely"` or `"possible"` | Contradictory; uncertainty must be expressed consistently across both fields |

---

## Example Brief (ISSUE-004 — Malmo Service Request)

```json
{
  "brief_id": "DIAG-20260820-001",
  "case_reference": "ISSUE-004",
  "generated_at": "2026-08-20T10:15:00Z",

  "scope": {
    "investigated": "Support agent report that creating a service request for the Malmo centre returns a 400 error.",
    "not_investigated": "Whether other service request types are affected. Whether the issue reproduces in production."
  },

  "customer_impact": "Support agents are unable to route cases to the Malmö service centre. Affected cases remain unassigned until the issue is resolved.",

  "root_cause": {
    "statement": "The validation list for service centre names contains a trailing space after 'Malmo', causing a string equality mismatch against the value sent by the form.",
    "confidence": "confirmed",
    "confidence_rationale": "The 400 response payload names the validation failure, and the source validation set confirms the trailing space. Two independent sources agree.",
    "area": "API validation"
  },

  "evidence_chain": [
    {
      "question": "Does the server reject the submitted value, or does the client send an incorrect value?",
      "artifact": "API POST /api/cases/{id}/service-requests — 400 response body",
      "excerpt": "{\"detail\": \"A valid service center is required.\", \"status\": 400}",
      "implication": "The server rejects the value, ruling out a client-side form serialisation error."
    },
    {
      "question": "What values does the server accept, and does the submitted value match exactly?",
      "artifact": "Program.cs — ServiceInput validation, line ~95",
      "excerpt": "if (input is null || !new[] { \"Stockholm\", \"Gothenburg\", \"Malmo \" }.Contains(input.Center))",
      "implication": "The accepted value is 'Malmo ' (trailing space); the form submits 'Malmo' (no space). The mismatch is in the validation list, not the form."
    }
  ],

  "alternatives_ruled_out": [
    {
      "hypothesis": "Client-side form serialisation mangles the selected value before submission.",
      "ruling_evidence": "API 400 response confirmed the server received a value and evaluated it against the list. The client is not at fault."
    },
    {
      "hypothesis": "The issue affects all three service centres, not only Malmo.",
      "ruling_evidence": "Stockholm and Gothenburg entries in the validation list contain no trailing space. Not ruled out for production; ruled out in source."
    }
  ],

  "remaining_uncertainty": "Whether the trailing space was introduced during initial seeding or later editing. Does not change the diagnosis.",

  "scope_boundary": [
    "This brief does not propose or implement a fix.",
    "Production environment not investigated — findings are based on source and local API behaviour only."
  ],

  "next_actions": [
    {
      "role": "Developer",
      "action": "Remove the trailing space from 'Malmo ' in the service-centre validation list in Program.cs, or normalise both sides of the comparison with Trim().",
      "priority": "High",
      "blocks_release": true
    },
    {
      "role": "QA",
      "action": "Add a regression test that submits service requests for Stockholm, Gothenburg, and Malmo via the API and asserts a 201 response for each.",
      "priority": "High",
      "blocks_release": true
    },
    {
      "role": "Support agent",
      "action": "Manually assign Malmo cases to the Malmo centre via direct database update or support lead until the fix is deployed.",
      "priority": "High",
      "blocks_release": false
    }
  ],

  "customer_communication": {
    "language": "en",
    "facts_only": true,
    "text": "We are aware of an issue that prevents cases from being routed to our Malmö service centre. Our team has identified the cause and is working on a resolution. We will update you as soon as routing is restored and can confirm your case has been assigned."
  }
}
```
