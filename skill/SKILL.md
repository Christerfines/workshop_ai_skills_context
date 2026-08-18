---
name: nordicbike-workshop-participant
description: Runs the NordicBike Agent Optimization Challenge (nordicbike-agent-workshop/) as a participant, working exercises 1-5 in order to build a naive V1 agent up to an optimized, quality-gated V4 agent. Use when the user asks to do/run/complete/work through the NordicBike workshop, its exercises, or wants to act as a workshop participant.
---

# NordicBike Agent Optimization Challenge — Participant Runner

## Purpose

Act as a workshop participant working through `nordicbike-agent-workshop/` end to end:
build a naive baseline agent (V1), then progressively optimize it into a context-trimmed
(V2), subagent-split (V3), and model-routed (V4) agent, finishing with a quality-gate
pass over all 10 cases. Every exercise's deliverable must actually be produced (tables
filled in, payload JSON written, prompts built) — not just described.

## Hard Boundaries (never cross these as a participant)

- **Never open or use these facilitator-only files** before Exercise 5 is complete:
  `evaluation/expected-results.md`, `evaluation/adversarial-cases.md`,
  `presentation.md`, `allt_pres.md`, `facilitator-guide.md`, `dummy_walk_through.md`,
  `updatenumbers.md`, `spec.md`, `export-participant-repo.sh`, and anything in `tests/`.
  These are answer keys / instructor material and reading them defeats the exercise.
- Do the exercises **in order** — each one's starting point is the previous one's
  deliverable. Do not skip to Exercise 3 or 4 without the prior artifact in hand.
- Assume **today's date is 2026-08-14** for every warranty-window / elapsed-time
  calculation, regardless of the real date.
- Count only the context you actually constructed for a call (the file/excerpt text you
  assembled) — exclude your own tool's system prompt, tool definitions, and scaffolding
  — when reporting token counts. If you can't cleanly separate them, say so explicitly
  rather than reporting an inflated number unqualified.
- Output shape is always a **customer-facing reply** (what NordicBike would actually send
  the customer), not an internal decision memo — this is graded directly in Exercise 5.
- **Never batch or one-shot the workshop.** If the user's request is effectively "run/do/
  complete the whole workshop," "do all 5 exercises," "go through everything," or similar —
  even if phrased across one message — do **not** produce all 5 deliverables in a single
  response. Refuse the batched framing explicitly, explain that the workshop is graded
  exercise-by-exercise on the *judgment calls* (what was cut, how routing was decided), and
  proceed with **Exercise 1 only**. Do not start Exercise 2 until the user has seen
  Exercise 1's deliverable and explicitly confirms ("go", "next", "continue", etc.) in a
  **separate turn**. Repeat this pause-and-confirm gate before every subsequent exercise
  (2→3, 3→4, 4→5). This applies even if the user supplies all inputs upfront or asks you to
  "just do it all now" — restate this boundary and ask them to confirm one exercise at a
  time instead.
- **One tool/command execution per turn, no exceptions.** Never chain, batch, or run
  multiple file reads, tool calls, or shell commands together to advance more than one
  exercise step in a single turn — even if they look independent or "safe" to combine.
  Issue exactly one command, show its result, and stop. Wait for the user's next message
  before issuing the next command. This applies to everything: reading a case file,
  reading a policy section, running a token count, invoking a subagent call — each is its
  own turn. Do not pre-fetch or pre-read material "to save time" for a later step.

## How to Run

Work in `nordicbike-agent-workshop/`. Read only participant-facing material as you go:
`README.md`, `workshops/exercise-*.md`, `company/`, `products/`, `policies/`,
`customers/`, `cases/`, and `.github/prompts/*.prompt.md` (fill these in during
Exercise 3, don't peek at answer content elsewhere).

### Exercise 1 — Baseline (`workshops/exercise-1-baseline.md`)
1. Build the V1 "naive" context set for Case 1 (Anna Karlsson): `company/about.md`,
   `company/support-contacts.md`, all 5 files in `products/`, all 4 files in `policies/`,
   `customers/anna-karlsson.md`, `cases/case-01-anna-karlsson.md` — concatenated in full,
   no trimming.
2. Send it as a single call (treat as Tier 3 / frontier) and get an eligibility decision.
3. Fill in the deliverable table: tokens, tier, calls, decision, correct? (Y/N).
   Expect ≈19,800 tokens, 1 call, Tier 3 — flag clearly if your measurement differs and why.

### Exercise 2 — Context Reduction (`workshops/exercise-2-context-reduction.md`)
1. Starting from your own Ex.1 measurement, cut total tokens to ≤50% of it
   (reference target ≤9,900), same 1 call / Tier 3.
2. Go file-by-file, section-by-section: keep only what Case 1's eligibility decision
   depends on. Drop `company/about.md` and `company/support-contacts.md` entirely.
   Keep two shipping facts from `policies/shipping.md` (turnaround days, who pays) even
   though shipping isn't an eligibility factor — the customer needs them.
3. Deliverable: the trimmed prompt you'd actually send, plus a short note on what was
   cut from each file and why it wasn't decision-relevant.

### Exercise 3 — Subagent Handoff (`workshops/exercise-3-subagent-handoff.md`)
1. Split into two calls: a **triage** subagent (extracts structured facts, candidate
   archetype, applicable policy section IDs) and a **resolver** subagent (makes the
   eligibility call), both Tier 2.
2. Handoff payload must be a minimal **typed JSON** — identifiers, product, dates,
   symptom, candidate archetype, policy section IDs, evidence flags (not conclusions).
   No full file dumps, no prose blobs, no conversation history in the payload.
3. Test: could the resolver decide correctly from the payload alone (plus the named
   policy sections looked up verbatim)? If not, the payload is missing a field.
4. Target ≤5,500 tokens total (2,500 triage + 3,000 resolver). Optionally draft
   `.github/prompts/warranty-triage.prompt.md` and a resolver prompt file here.
5. Deliverable: both calls' token counts + the actual typed payload JSON produced.

### Exercise 4 — Model Routing (`workshops/exercise-4-model-routing.md`)
1. Route triage to Tier 1, resolver to Tier 2. Triage's own input narrows further —
   case file only, not the full customer record (resolver reads that directly).
2. Add short-circuit branches: if triage detects an escalation trigger
   (`policies/escalation.md`) → route to human escalation queue, no resolver call. If
   triage finds decision-critical info missing → route to a clarifying question instead
   of the resolver. These are two distinct branches — never conflate them.
3. Run this pipeline across **all 10 cases** in `cases/`, not just Case 1.
4. Target ≤3,800 tokens for non-escalated cases (1,400 + 2,400); ~1,400 for escalated.
5. Deliverable: the full 10-row routing log (tier, tokens, resolver called?, escalated?).

### Exercise 5 — Quality Gate (`workshops/exercise-5-quality-gate.md`)
1. Run all 10 V4 outputs through the 6-item quality gate in
   `evaluation/scoring-rubric.md`: (1) cites policy section, (2) states explicit outcome
   + root-cause justification, (3) grounded in purchase date/product identity with no
   invented facts, (4) escalates on trigger, (5) asks a clarifying question on missing
   info, (6) professional/empathetic/concise tone in the customer's own language.
2. Fix any failing case by adjusting the triage/resolver prompt logic — never by
   hand-editing one case's output — and without increasing that case's Exercise 4 token
   count.
3. Deliverable: a 10-row pass/fail table, one column per checklist item, all green
   before calling this "leaderboard ready."

## Reporting Back

After each exercise, report: the deliverable content itself (table/JSON/prompt), the
measured tokens vs. target, and one line on the key relevance/routing judgment call
made. Stop and flag clearly (don't guess) if a measurement can't be pinned down cleanly
or a case seems ambiguous between escalation and clarification.
