# NordicBike Agent Optimization Challenge

## Overview

**Agent Optimization Challenge** is a 2-hour hands-on workshop built around a fictional company, NordicBike AB, and a realistic customer-support case load of 10 warranty and support inquiries. Participants build and progressively optimize an LLM-based support agent that resolves NordicBike warranty cases — starting from a naive, single-call, dump-everything baseline and ending with a two-call, model-routed, quality-gated agent running at a fraction of the original cost.

Along the way, the workshop teaches five practical agent-engineering skills: **skills design**, **context engineering**, **subagent handoff patterns**, **model routing**, and **evaluation**. Every exercise is measured against a fixed baseline (18,400 tokens, 1 call, Tier 3) so improvement is concrete and comparable across teams, and a leaderboard at the end scores both correctness and cost efficiency — not cost alone — because a fast, cheap agent that gets warranty decisions wrong is not actually a win.

NordicBike AB was chosen deliberately as the workshop's subject rather than a generic or abstract dataset, because a realistic support case load exposes the kinds of failure modes that toy examples tend to hide. A warranty claim is rarely a single clean fact lookup — it usually depends on cross-referencing a purchase date against the right coverage window, distinguishing a covered root cause from a superficially similar excluded one, recognizing when a request exceeds what the agent is authorized to resolve on its own, and knowing when the honest answer is "I need one more fact before I can tell you." The 10 cases in this repository are built specifically to exercise all of those situations, and four of them are constructed as adversarial archetypes — patterns deliberately designed to catch an agent that has optimized for token budget at the expense of actually reading the record it was given. A team that hits every token target in Exercises 2 through 4 but trips one of these adversarial patterns will see that reflected directly in their leaderboard score, because the scoring formula in evaluation/scoring-rubric.md applies a real penalty for exactly this failure mode.

The progression from V1 to V4 is the spine of the workshop, and each exercise builds directly on the deliverable from the one before it — there is no way to skip ahead to Exercise 4 without first working through what a subagent handoff payload should and should not contain in Exercise 3. This is intentional: the point is not merely to arrive at a small, fast agent, but to understand, exercise by exercise, exactly which optimization produced which reduction in token load, and what each optimization cost or risked in terms of correctness. Teams that treat each exercise's constraint as a genuine design problem, rather than a box to check on the way to the leaderboard, tend to get the most out of the two hours.

Facilitators running this workshop should read facilitator-guide.md in full before the session — it contains setup steps, the exact exercise wording, a timing schedule tied to the 5 phases, and the operational leaderboard-scoring procedure that this README deliberately does not duplicate. Participants should not need to read anything beyond this README and the workshops/ directory to get started; everything else in the repository exists to be discovered and referenced as each exercise requires it, which is itself part of what Exercise 2 (context engineering) is teaching.

## Prerequisites

- Basic familiarity with LLM API calls and prompt construction.
- No NordicBike domain knowledge assumed — everything needed to resolve every case is contained in this repository.

Beyond these two points, no special background is required. "Basic familiarity with LLM API calls and prompt construction" means participants should be comfortable constructing a prompt from multiple source documents, sending it to a model, and reading back a structured or semi-structured response — the workshop does not assume prior experience with subagent orchestration, context-window optimization, or evaluation-rubric design, since those are precisely the skills the five exercises are meant to build from scratch over the course of the session. Teams do not need to have used a multi-tier model routing setup before; Exercise 4 is where that concept is introduced and applied for the first time, with the model-routing table provided rather than assumed.

Domain knowledge about bicycles, e-bikes, or warranty law is deliberately not assumed and is not needed: every fact required to resolve any of the 10 cases — product specifications, coverage windows, exclusions, escalation thresholds, and customer purchase history — is written down somewhere in company/, products/, policies/, or customers/. Part of what Exercise 1's baseline measurement is designed to reveal is just how much of that material a naive agent pulls in when it has no way to know in advance which of it is actually relevant to the case at hand; participants are not expected to have that judgment themselves before the workshop starts; building it is the point of the session.

One practical prerequisite worth flagging separately even though it is a facilitator setup responsibility rather than a participant one: each team will need working access to all three model tiers referenced throughout the workshop (Tier 1, Tier 2, and Tier 3) before Exercise 4, since the model-routing exercise specifically requires routing different call types to different tiers. Facilitators should confirm this access as part of session setup per facilitator-guide.md; participants do not need to arrange this themselves.

## Repository Map

This repository contains everything needed to run and participate in the workshop, organized into the following top-level directories, each scoped to a single kind of content so that an agent (or a participant) can retrieve only what a given case actually requires:

- **company/** — NordicBike AB's background, mission, and support contact information.
- **products/** — Full specifications for all NordicBike products: Aurora X3, Fjord Cargo, Vinter Pro, PowerPack batteries, and accessories.
- **policies/** — Warranty, returns, shipping, and escalation policy documents; the ground truth for every eligibility decision.
- **customers/** — Customer records (contact info and purchase history) for the 10 customers whose cases appear in this workshop.
- **cases/** — The 10 customer support case files participants' agents must resolve.
- **workshops/** — The 5 exercise files that structure the workshop, in order.
- **skills/** — Empty scaffold files for the subagent skills participants build in Exercise 3.
- **evaluation/** — The scoring rubric, quality-gate definition, and (facilitator-only) answer key and adversarial-case definitions.
- **presentation.md** — The instructor slide deck driving live delivery of all 5 workshop phases.
- **facilitator-guide.md** — The standalone facilitator operating manual: setup, exact exercise prompts, timing, and leaderboard procedure.

Two of these directories deserve a specific note on access: evaluation/expected-results.md and evaluation/adversarial-cases.md are facilitator-only and must not be shown to participants before Phase 5, since they contain the answer key and the definitions of the adversarial patterns the leaderboard scoring checks for — see the Facilitator-Only Instructions section below for where the operational detail on this lives.

## How to Run Each Exercise

Work through the five exercises in order — each one's starting point is the previous exercise's output, so skipping ahead is not possible without first producing the deliverable the next exercise assumes you already have:

1. `workshops/exercise-1-baseline.md` — measure the naive V1 baseline.
2. `workshops/exercise-2-context-reduction.md` — cut context to build V2.
3. `workshops/exercise-3-subagent-handoff.md` — split into a triage/resolver subagent pair to build V3.
4. `workshops/exercise-4-model-routing.md` — route by model tier and short-circuit escalations to build V4.
5. `workshops/exercise-5-quality-gate.md` — validate V4 output against the 6-item quality gate before leaderboard submission.

## Facilitator-Only Instructions

⚠ **Facilitators only — do not share this section with participants before the workshop.**

See facilitator-guide.md for setup, exact exercise wording, timing checkpoints, and the leaderboard procedure.

## Participant-Facing Instructions

Start with workshops/exercise-1-baseline.md and proceed in order through exercise-5-quality-gate.md.
