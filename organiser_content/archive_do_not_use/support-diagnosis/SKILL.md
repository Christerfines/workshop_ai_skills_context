---
name: support-diagnosis
description: "Use when investigating a NordicBike support report to establish root cause. Produces a structured diagnostic brief with evidence chain, confidence level, ruled-out alternatives, next actions by role, and customer-safe facts. Does NOT fix code, propose a resolution timeline, authorise compensation, or respond to the customer."
---

# Support Diagnosis Skill

## Purpose

Investigate a NordicBike support report and return an action-ready diagnostic brief. Use the report and evidence available in this repository. Running the portal is optional: use it only when it helps answer a specific investigation question.

## Boundaries

This skill diagnoses and routes work. It does not implement code changes, promise a resolution date, authorise compensation, or send a customer response. Do not invent facts that are absent from the available evidence.

## Initial Output

Return a JSON object with the following fields:

```json
{
  "case_reference": "",
  "customer_impact": "",
  "likely_cause": "",
  "confidence": "confirmed | likely | possible | insufficient_evidence",
  "evidence_used": [],
  "remaining_questions": [],
  "next_actions": [],
  "customer_facts": ""
}
```

## Workshop Challenge

This is intentionally incomplete. Improve this skill as you work through the cases. Decide how it should select evidence, test alternative explanations, stop when it has enough information, express uncertainty, protect confidential information, and make the output useful to the people who must act on it.
