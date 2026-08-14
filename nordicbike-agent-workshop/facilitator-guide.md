# Facilitator Guide — Agent Optimization Challenge

This is the standalone operating manual for whoever runs the NordicBike Agent Optimization Challenge live. It contains everything presentation.md and README.md deliberately omit: verbatim participant-facing exercise prompts, timing checkpoints, and the leaderboard mechanics as an operational procedure rather than a slide summary. This file is not folded into README.md and should not be shared with participants before the workshop, since it links directly to the answer key and adversarial-case definitions that participants must not see in advance.

## Setup Instructions

Before the session begins:

1. Clone the repository.
2. Confirm each team has access to Tier 1, Tier 2, and Tier 3 model endpoints — the workshop requires all three tiers to be reachable from Exercise 1 onward, since the naive baseline agent runs on Tier 3 and later exercises route across all three.
3. Confirm each team can view `workshops/exercise-1-baseline.md` through `workshops/exercise-5-quality-gate.md`, but **NOT** `evaluation/expected-results.md` or `evaluation/adversarial-cases.md`, before Phase 5. These two files are the answer key and the adversarial-archetype definitions respectively — showing either before scoring defeats the exercise.
4. Project `presentation.md` slides 1–13 before Phase 2 begins.
5. Hold slides 14–24 until after Exercise 1's baseline measurement is complete, so that teams measure the 18,400-token baseline themselves before seeing the reference V1→V4 progression table and later material. Revealing the progression table early gives away the target numbers for every subsequent exercise before teams have earned them through measurement.

A few additional setup details worth confirming ahead of the session rather than discovering live, in roughly the order they matter:

- **Team composition and size.** This workshop is designed around small teams of 2–4 participants sharing one set of model-endpoint credentials and one working repository checkout. Groups larger than 4 tend to have participants disengage during the hands-on exercises; groups of 1 lose the discussion value of having to justify a context-reduction or routing decision to a teammate before implementing it. If registration numbers don't divide cleanly, it's better to have one team of 5 than two teams of 2 and 3 that finish Exercise 1 at very different times.
- **Repository access.** Every team needs their own clone or fork of this repository rather than a shared read-only copy, since Exercises 2 through 5 each produce a deliverable (a modified agent, a set of case outputs) that the team needs to keep and eventually submit for Phase 5 scoring. Decide before the session whether teams commit their work to individual branches of a shared repository or to entirely separate clones, and communicate that choice in the pre-workshop email so teams aren't figuring out git workflow during Phase 1.
- **Model endpoint verification.** Do this well before participants arrive, not as the first live action of Phase 1 — a Tier 1 or Tier 3 endpoint that turns out to be misconfigured or rate-limited is far easier to fix with 30 minutes of buffer than with a room of participants waiting on it. A simple pre-flight check (one trivial call per tier, from a machine on the same network participants will use) catches most of these issues.
- **Materials for in-person delivery.** If running this in person, prepare a visible countdown timer for each phase and printed or projected copies of the Timing Checkpoints below, since teams that can see the clock self-correct their pace far more often than teams that have to ask the facilitator how much time is left.
- **A backup plan for a down model tier.** If a tier becomes unreachable mid-session, the fastest recovery is usually to let affected teams substitute the next-available tier temporarily and note the substitution rather than pausing the whole room — a team's cost numbers will be off for that run, but their correctness work and their understanding of the exercise are not lost. Decide this contingency in advance so it doesn't need to be improvised while a room is waiting.
- **Pre-workshop communication.** Send participants the Prerequisites section of README.md and the repository link at least a day ahead, so no one arrives needing to set up API access or clone the repository during Phase 1's 15-minute window. Do not send `presentation.md`'s slides 14–24 or anything from `evaluation/` ahead of time, for the same reason those are held back live.

## The 5 Exercise Prompts (verbatim)

These prompts are reproduced byte-for-byte identical to the `## Constraint` text in each corresponding `workshops/exercise-N-*.md` file. Read them to participants exactly as written, or point them to the corresponding exercise file — do not paraphrase.

i. "Run the baseline agent exactly as provided against Case 1. Do not modify it. Record: total input tokens, model tier used, number of model calls, and the agent's eligibility decision. This is your baseline to beat."

ii. "Reduce the total input context by at least 50% (target ≤9,200 tokens) while keeping exactly one model call and the same model tier. You may not drop any file category entirely if it is relevant to the case at hand — you must excerpt, not omit, relevant material. Output quality (correctness of the eligibility decision) must not regress."

iii. "Split your agent into two calls: a triage subagent that extracts structured facts from the case and customer record, and a resolver subagent that makes the eligibility decision. The triage subagent's output to the resolver must be a minimal typed JSON payload — no full-context dumps are permitted between subagents. Target ≤5,500 total input tokens across both calls, same model tier as Exercise 2."

iv. "Apply the model-routing table: triage calls run on Tier 1, resolver calls run on Tier 2. If the triage subagent detects any escalation trigger from policies/escalation.md, route directly to the human escalation queue and do not make a resolver call at all. Target ≤3,800 total input tokens across all calls for non-escalated cases."

v. "Run every one of your 10 case outputs through the 6-item quality gate in evaluation/scoring-rubric.md. Any output that fails even one item is not eligible for leaderboard submission until fixed. Fixing a quality-gate failure must not increase your token budget above the Exercise 4 targets."

A note on delivery: read each prompt to the room exactly as written above at the start of its phase, and keep it visible (on a slide or a shared document) for the duration of that exercise, since teams will want to re-check the exact wording of their constraint partway through rather than relying on memory. Do not answer "does X count as relevant material" questions by expanding the prompt's wording on the spot — point the team back to the exact constraint text and let them make the judgment call themselves; that judgment call is part of what each exercise is teaching, particularly for Exercise 2's "excerpt, don't omit" instruction.

## Timing Checkpoints

Tied to the 5-phase, 120-minute schedule:

- **00:00** — Phase 1 starts.
- **00:15** — Phase 2 starts (Exercise 1).
- **00:35** — Phase 3 starts (Exercises 2–3). Checkpoint at **00:57–01:00**: Exercise 2 should be complete. Exercise 3 should be complete by **01:20**.
- **01:20** — Phase 4 starts (Exercises 4–5). Checkpoint at **01:40**: Exercise 4 should be complete. Exercise 5 should be complete by **01:50**.
- **01:50** — Phase 5 starts (leaderboard + debrief).
- **02:00** — Close.

If a team is meaningfully behind at a checkpoint (e.g., still on Exercise 1 at 00:35), the facilitator should help them fix their baseline measurement quickly rather than let them fall further behind — Exercises 2–5 each build directly on the previous exercise's output, so an incorrect or incomplete Exercise 1 compounds through the rest of the session.

A few practical notes on running these checkpoints in the room rather than just posting them:

- **Treat each checkpoint as a walk-around, not an announcement.** At 00:57, 01:20, and 01:40 specifically, walk to each team rather than asking the room generally "is everyone on track?" — teams that are behind rarely self-report it unprompted, especially early in the session before rapport is established, and a quiet team in the corner still stuck on Exercise 1 at 00:57 needs to be caught by observation, not by them raising a hand.
- **Distinguish "behind on time" from "stuck on a concept."** A team that is behind because they're being thorough (for example, carefully checking each of the 10 cases against the Exercise 2 constraint rather than skimming) is in a different situation than a team stuck because they've misunderstood what "excerpt, not omit" means. The first team may just need a nudge to move faster; the second needs a clarifying conversation, and giving the first team a concept explanation they don't need wastes time better spent circulating.
- **Build in slack rather than announcing a hard cutoff.** The checkpoint times above assume some teams will run slightly over on any given exercise; this is normal and does not require the whole room to stop. What does require intervention is a team still meaningfully behind at the start of the *next* phase, since that is when the compounding problem described above actually starts to bite.
- **Teams that finish early are not idle time.** A team that clears Exercise 3 well before 01:20 can be encouraged to re-run their V3 agent against a second or third case from the case load (beyond Case 1) to sanity-check that their context-reduction and handoff logic generalizes, rather than being purely tuned to the one case they've been testing against. This also produces a better-prepared team for Exercise 5's full 10-case quality-gate run.
- **Phase 5's 10 minutes is tight by design.** It is meant to be a reveal and a retro, not a full scoring session run live in front of the room — see the Leaderboard Running Procedure below for why scoring itself should largely happen before Phase 5's clock starts, using outputs teams submit as they finish Exercise 5.

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

Expanding on why this procedure is structured the way it is, and how to run it well with more than two or three teams in the room:

- **Start scoring before Phase 5's clock starts.** With more than a handful of teams, scoring all 10 cases for every team from a cold start at 01:50 will not fit in Phase 5's 10-minute window. In practice, begin scoring each team's outputs as soon as that team submits their completed Exercise 5 deliverable, which for most teams will be sometime during Phase 4. By the time Phase 5 formally begins, most or all teams' scores should already be computed and ready to reveal — Phase 5 itself is then genuinely just the reveal and the retro discussion, not a live scoring session under time pressure.
- **A second scorer materially improves consistency, if available.** Steps 2, 3, and 6 all involve judgment calls — is a response's root-cause reasoning strong enough for a 3 versus a 4 on that rubric category, does a borderline phrasing count as a passed or failed quality-gate item. If a second facilitator or teaching assistant is available, having them score independently and reconcile differences on close calls produces more consistent and more defensible results across teams than a single scorer working alone, especially across a long list of teams where scoring fatigue can creep in on the later teams.
- **Handling disputes.** Some teams will push back on a specific score, most often on Root-Cause Grounding or Escalation/Scope Judgment, since those categories require the most judgment. Rather than re-litigating the specific number live in front of the room, note the disputed case and offer to review it individually after the session with the specific `evaluation/expected-results.md` reasoning that drove the score — this keeps Phase 5's short window moving and still gives the team a real answer.
- **A tie at the FinalScore level is rare but not impossible**, particularly with small numbers of teams. There is no formal tie-breaking rule defined by this workshop; facilitators should use judgment (for example, favoring the team with the higher Q, since correctness is the harder half of the score to fake) or simply present tied teams as co-ranked, whichever fits the room's tone better.
- **Use the scored outputs as debrief material.** Once every team is ranked, the retro discussion in the remaining Phase 5 time is far more useful if it references specific patterns you noticed while scoring — for example, "several teams lost points on Case 2 by citing SB-2025-11's batch match without engaging with Erik's own account of his washing habits" is a much more actionable takeaway than a generic reminder to "read carefully." This is also the natural moment to reveal `evaluation/adversarial-cases.md`'s archetype definitions to participants, now that scoring is complete and the answer key no longer needs to stay hidden.
- **Keep a record of each team's per-case breakdown, not just their FinalScore**, even after the session ends — a team's Q and M values broken out by case are far more useful for a written follow-up or a "what to try next time" note than the single ranked number is, and this breakdown is only easy to reconstruct if you keep it during scoring rather than trying to recompute it afterward from memory.
