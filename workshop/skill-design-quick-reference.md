---
title: "Support-Diagnosis Skill — Quick Reference"
subtitle: "Print and keep at the table"
date: "2026-08-21"
---

# Support-Diagnosis Skill — Quick Reference

*One skill, four stages, no leaderboard — the workflow is the deliverable.*

---

## 5 Things to Get Right Before You Touch the Skill File

1. **Scope** — one job. State the "does not" list as clearly as the "does" list.
2. **Description = trigger, not summary** — it decides when the skill loads.
3. **Question before evidence** — no artifact opens without a named question behind it.
4. **Structured output only** — every field must map to one role's next decision.
5. **Confidence has rules** — `confirmed` needs two independent sources; unsure means `insufficient_evidence`, never a guess.

---

## The Question Ladder — Ask Before Opening Anything

1. What happened, to whom, and when?
2. What differs from the expected path?
3. Which identifier links the report to evidence?
4. Which single artifact would reduce uncertainty the most, right now?
5. What observation would disprove the leading hypothesis?

---

## Confidence Levels

| Level | Use it when |
|---|---|
| `confirmed` | Two independent sources agree; no alternative remains |
| `likely` | Primary evidence supports it; one alternative not yet ruled out |
| `possible` | Evidence fits the conclusion *and* at least one alternative |
| `insufficient_evidence` | Say so. Name the missing artifact instead of guessing |

---

## 4 Context Failure Modes to Guard Against

- **Poisoning** — a wrong fact gets treated as ground truth
- **Distraction** — real signal buried under irrelevant volume
- **Confusion** — off-topic material shifts the conclusion
- **Clash** — two sources disagree and the model silently picks one

\newpage

## Per-Stage Suggestions

**Stage 1 — Establish a baseline** *(15 min)*
Run the starter skill unchanged first. Don't fix anything yet — write down what's missing for each role: support agent, developer, QA, product owner.

**Stage 2 — Make evidence selection deliberate** *(20 min)*
Write your two competing hypotheses before touching the skill wording. Only request an artifact once you can say which hypothesis it would separate.

**Stage 3 — Make the brief actionable and bounded** *(25 min)*
Fill the schema field by field, not prose-first. If a field would need another document to act on, rewrite it until it doesn't.

**Stage 4 — Stress-test and refine** *(25 min)*
Review in two passes: first hunt for unnecessary context, then for unsupported certainty. Fix the *skill instructions*, not just the one bad output you found.

---

## Non-Negotiables (from the diagnostic brief schema)

- `scope_boundary` is never empty
- `customer_communication` is facts-only — no technical terms, no promises, no compensation, matches the customer's language
- `alternatives_ruled_out` has at least one entry for any non-trivial case
- `next_actions` must be executable from the brief alone — no "investigate further"

## Reference Skills

The repository also includes six completed, generic engineering skills for local demonstrations: debugging, test-driven development, verification, security review, release-note drafting, and structured pull-request review. See [Reference Agent Skills](./reference-skills.md) for external sources, local-only run instructions, optional MCP guidance, and evaluation tasks. The starter `support-diagnosis` skill and prompt-file scaffolds remain intentionally incomplete for the workshop.
