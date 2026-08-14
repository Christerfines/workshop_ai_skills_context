# NordicBike Agent Optimization Challenge

## Overview

**Agent Optimization Challenge** is a 2-hour hands-on workshop built around a fictional company, NordicBike AB, and a realistic customer-support case load of 10 warranty and support inquiries. Participants build and progressively optimize an LLM-based support agent that resolves NordicBike warranty cases — starting from a naive, single-call, dump-everything baseline and ending with a two-call, model-routed, quality-gated agent running at a fraction of the original cost.

Along the way, the workshop teaches five practical agent-engineering skills: **skills design**, **context engineering**, **subagent handoff patterns**, **model routing**, and **evaluation**. Every exercise is measured against a fixed baseline (18,400 tokens, 1 call, Tier 3) so improvement is concrete and comparable across teams, and a leaderboard at the end scores both correctness and cost efficiency — not cost alone — because a fast, cheap agent that gets warranty decisions wrong is not actually a win.

## Prerequisites

- Basic familiarity with LLM API calls and prompt construction.
- No NordicBike domain knowledge assumed — everything needed to resolve every case is contained in this repository.

## Repository Map

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

## How to Run Each Exercise

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
