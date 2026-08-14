# Exercise 5 — Quality Gate

## Goal

Run the full 6-item quality-gate checklist (defined in evaluation/scoring-rubric.md) against your V4 agent's output for all 10 cases, and fix any failures before submitting to the leaderboard. This exercise teaches that token and cost optimization must not silently degrade correctness — the quality gate is a mandatory pass/fail validation layer sitting on top of whatever budget you've achieved.

This is the first exercise in the workshop that asks you to run your agent against all 10 cases rather than Case 1 alone. That matters: everything you built in Exercises 2 through 4 was tuned and measured against a single, relatively clean case, and an approach that works well on Case 1 does not automatically generalize to the other nine, several of which are specifically designed to break agents that took shortcuts earlier. This is where that gap, if one exists in your pipeline, becomes visible.

## Starting Point

Your V4 agent's output for all 10 cases: routed per the Exercise 4 model-routing table, targeting ≤3,800 tokens for non-escalated cases and ~1,000 tokens for escalated cases.

## Constraint

"Run every one of your 10 case outputs through the 6-item quality gate in evaluation/scoring-rubric.md. Any output that fails even one item is not eligible for leaderboard submission until fixed. Fixing a quality-gate failure must not increase your token budget above the Exercise 4 targets."

The "must not increase your token budget" clause is the sharpest constraint in this exercise, and it is intentional: it is easy to fix a correctness problem by throwing more context at the resolver, but that is exactly the kind of trade-off this entire workshop has been building toward avoiding. A genuine fix — tightening the triage subagent's extraction logic, adjusting the resolver's instructions, correcting a routing rule — should not require a larger prompt than the one you already built in Exercise 4.

## Target Metric

**10/10 cases pass all 6 checklist items, token budget unchanged from Exercise 4.**

## Deliverable

A quality-gate pass/fail table for all 10 cases, one row per case, one column per checklist item.

| Case | 1. Cites section | 2. States outcome + why | 3. Confirms facts first | 4. Escalates when triggered | 5. Clarifies when missing info | 6. Tone/language | All 6 Pass? |
|---|---|---|---|---|---|---|---|
| CASE-01 | | | | | | | |
| CASE-02 | | | | | | | |
| CASE-03 | | | | | | | |
| CASE-04 | | | | | | | |
| CASE-05 | | | | | | | |
| CASE-06 | | | | | | | |
| CASE-07 | | | | | | | |
| CASE-08 | | | | | | | |
| CASE-09 | | | | | | | |
| CASE-10 | | | | | | | |

## Hints

None. If you find a failure, fix the prompt or the agent's instructions rather than hand-editing the output — a fix that only patches the visible output for one case, without changing the underlying logic, will likely resurface on a similar case elsewhere in a real deployment. Pay particular attention to items 4 and 5 of the checklist across Cases 6 through 9 specifically, since those are the cases most likely to expose a routing or clarification gap that Case 1 alone would never have surfaced.
