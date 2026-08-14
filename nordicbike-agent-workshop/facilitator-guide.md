# Facilitator Guide — Agent Optimization Challenge

This is the standalone operating manual for whoever runs the NordicBike Agent Optimization Challenge live. It contains everything presentation.md and README.md deliberately omit: verbatim participant-facing exercise prompts, timing checkpoints, and the leaderboard mechanics as an operational procedure rather than a slide summary. This file is not folded into README.md and should not be shared with participants before the workshop, since it links directly to the answer key and adversarial-case definitions that participants must not see in advance.

## Setup Instructions

Before the session begins:

1. Clone the repository.
2. Confirm each team has access to Tier 1, Tier 2, and Tier 3 model endpoints — the workshop requires all three tiers to be reachable from Exercise 1 onward, since the naive baseline agent runs on Tier 3 and later exercises route across all three.
3. Confirm each team can view `workshops/exercise-1-baseline.md` through `workshops/exercise-5-quality-gate.md`, but **NOT** `evaluation/expected-results.md` or `evaluation/adversarial-cases.md`, before Phase 5. These two files are the answer key and the adversarial-archetype definitions respectively — showing either before scoring defeats the exercise.
4. Project `presentation.md` slides 1–13 before Phase 2 begins.
5. Hold slides 14–24 until after Exercise 1's baseline measurement is complete, so that teams measure the 18,400-token baseline themselves before seeing the reference V1→V4 progression table and later material. Revealing the progression table early gives away the target numbers for every subsequent exercise before teams have earned them through measurement.

## The 5 Exercise Prompts (verbatim)

These prompts are reproduced byte-for-byte identical to the `## Constraint` text in each corresponding `workshops/exercise-N-*.md` file. Read them to participants exactly as written, or point them to the corresponding exercise file — do not paraphrase.

i. "Run the baseline agent exactly as provided against Case 1. Do not modify it. Record: total input tokens, model tier used, number of model calls, and the agent's eligibility decision. This is your baseline to beat."

ii. "Reduce the total input context by at least 50% (target ≤9,200 tokens) while keeping exactly one model call and the same model tier. You may not drop any file category entirely if it is relevant to the case at hand — you must excerpt, not omit, relevant material. Output quality (correctness of the eligibility decision) must not regress."

iii. "Split your agent into two calls: a triage subagent that extracts structured facts from the case and customer record, and a resolver subagent that makes the eligibility decision. The triage subagent's output to the resolver must be a minimal typed JSON payload — no full-context dumps are permitted between subagents. Target ≤5,500 total input tokens across both calls, same model tier as Exercise 2."

iv. "Apply the model-routing table: triage calls run on Tier 1, resolver calls run on Tier 2. If the triage subagent detects any escalation trigger from policies/escalation.md, route directly to the human escalation queue and do not make a resolver call at all. Target ≤3,800 total input tokens across all calls for non-escalated cases."

v. "Run every one of your 10 case outputs through the 6-item quality gate in evaluation/scoring-rubric.md. Any output that fails even one item is not eligible for leaderboard submission until fixed. Fixing a quality-gate failure must not increase your token budget above the Exercise 4 targets."

## Timing Checkpoints

Tied to the 5-phase, 120-minute schedule:

- **00:00** — Phase 1 starts.
- **00:15** — Phase 2 starts (Exercise 1).
- **00:35** — Phase 3 starts (Exercises 2–3). Checkpoint at **00:57–01:00**: Exercise 2 should be complete. Exercise 3 should be complete by **01:20**.
- **01:20** — Phase 4 starts (Exercises 4–5). Checkpoint at **01:40**: Exercise 4 should be complete. Exercise 5 should be complete by **01:50**.
- **01:50** — Phase 5 starts (leaderboard + debrief).
- **02:00** — Close.

If a team is meaningfully behind at a checkpoint (e.g., still on Exercise 1 at 00:35), the facilitator should help them fix their baseline measurement quickly rather than let them fall further behind — Exercises 2–5 each build directly on the previous exercise's output, so an incorrect or incomplete Exercise 1 compounds through the rest of the session.

## Leaderboard Running Procedure

Exact steps for scoring and ranking teams in Phase 5:

1. Collect each team's V4 agent outputs for all 10 cases.
2. Score each output against `evaluation/expected-results.md` for correctness and `evaluation/scoring-rubric.md`'s 5-category rubric for the 0–20 per-case score.
3. Run the 6-item quality gate per case to determine pass/fail for Q.
4. Compute TotalCostPoints per case from the team's reported tier/token usage per call.
5. Compute CostEfficiency per case and M as the mean across all 10 cases.
6. Check each case against `evaluation/adversarial-cases.md`'s "must not" clauses to count critical failures for Penalty.
7. Compute FinalScore = (Q × 70) + (M × 30) − Penalty, clamp to [0, 100].
8. Rank teams descending by FinalScore and display live.

A few practical notes for running this procedure smoothly:

- Steps 2–3 and step 6 all require the facilitator to read each team's actual case outputs, not just their reported token counts — a team can hit every budget target and still fail correctness or trip a "must not" clause, and the scoring procedure is designed to catch that rather than reward budget alone.
- It's fastest to score all 10 cases for one team before moving to the next team, since the facilitator will be holding the same section of `evaluation/expected-results.md` and `evaluation/adversarial-cases.md` in mind across all 10 cases for a given team.
- Have a simple spreadsheet or scoring sheet ready ahead of time with columns for each rubric category, the quality-gate items, and the cost/token fields per case, so the arithmetic in steps 4–7 is quick rather than done by hand live in front of the room.
- Reveal the leaderboard only after every team's score is finalized, to avoid partial-results pressure affecting a team still being scored.
- Keep `evaluation/expected-results.md` and `evaluation/adversarial-cases.md` closed to participant view even during Phase 5 scoring — reveal only the resulting scores and ranks, not the underlying answer key, unless you intend to use it as debrief material afterward.
