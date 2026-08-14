# Agent Optimization Challenge

## Title & Agenda

- Title: Agent Optimization Challenge
- Subtitle: NordicBike AB — a 2-hour hands-on agent-engineering workshop
- Agenda: Kickoff → Baseline → Optimization → Routing & Quality → Leaderboard
- Format: hands-on, team-based, one shared 10-case load for every team
- Outcome: a working, cost-optimized agent architecture plus a scored leaderboard entry
- Audience: engineers building or maintaining LLM-based agents in production, not just this specific case load
- Prerequisite: basic familiarity with LLM API calls and prompt construction — no NordicBike domain knowledge assumed

## Learning Objectives

- Skills design — scaffold a reusable, narrowly-scoped subagent skill
- Context engineering — cut input tokens without cutting correctness
- Subagent/multi-agent handoff patterns — pass typed facts, not full context dumps
- Model routing — send cheap decisions to cheap models, hard ones to capable ones
- Evaluation — build a quality gate and a scoring rubric that actually catch mistakes
- Each objective maps to one exercise, in order — Exercise 3 teaches skills design and handoff patterns together
- By the end of Phase 4, all five objectives have been exercised at least once, not just discussed

## Pedagogy Rationale I — Context as a Scarce Resource

- Every token costs money and latency — a naive agent pays for both twice over
- Irrelevant context degrades accuracy, not just cost — more text is not more signal
- The V1→V4 progression makes this measurable — each version has a recorded token count
- A 5x reduction in tokens is only a win if correctness holds across all 10 cases
- This is why the leaderboard formula scores cost efficiency and correctness together, not cost alone
- Teams should expect that not every optimization is free — some genuinely trade a small correctness risk for a large cost win

## Pedagogy Rationale II — Progressive Disclosure & Handoff Discipline

- Retrieve only what's needed, when it's needed — not everything that might be needed
- Subagent handoffs should pass typed facts, not full context — a payload, not a dump
- Routing cheap models to cheap decisions frees budget for the cases that actually need it
- Escalation triggers should short-circuit the pipeline entirely rather than consuming a resolver call
- Together these two disciplines are what separates V2's context trimming from V3 and V4's structural redesign
- Both disciplines generalize well beyond this workshop's specific case load or company

## Meet NordicBike AB

- Founded 2019, HQ Stockholm (Hammarby Sjöstad)
- ~85 employees across product engineering, customer support, logistics, and retail partnerships
- Online store + 3 service centers (Stockholm, Gothenburg, Malmö)
- Built by engineers and cyclists frustrated with mild-climate e-bikes sold into Nordic winters
- Engineered for cold temperatures, wet roads, long dark winters, and daily commuting distances
- Mission: make electric mobility the practical everyday choice across the Nordics, built to last
- Support quality and warranty clarity are treated internally as part of the product, not an afterthought

## The Product Line

- Aurora X3 — 34,900 SEK — city e-bike
- Fjord Cargo — 44,900 SEK — cargo e-bike
- Vinter Pro — 37,900 SEK — winter e-bike
- PowerPack 720 / PowerPack 900 — spare and replacement batteries
- Accessories — a small catalog of riding accessories
- Each model has its own warranty terms — product identity is a fact every case must confirm
- A standalone battery purchase carries its own 12-month term, separate from the whole-bike 24-month term
- Full specifications, pricing, and configuration for every model live in products/, not on this slide

## The Support Case Load

- 10 cases, modeled on realistic NordicBike warranty and support inquiries
- 4 adversarial archetypes, each appearing at least once across the 10 cases
- 2 clean control cases with no adversarial trap, to test basic execution under no pressure
- Cases mix eligible, not-eligible, escalation-required, and clarification-required outcomes
- No case can be resolved from the case file alone — supporting policy and customer records are always required
- Every team works the same 10 cases, so leaderboard scores are directly comparable across teams

## Workshop Format — 120 Minutes, 5 Phases

| Phase | Duration |
|---|---|
| Phase 1 | 15 min |
| Phase 2 | 20 min |
| Phase 3 | 45 min |
| Phase 4 | 30 min |
| Phase 5 | 10 min |

- Phase 3 is the longest phase — it covers two exercises (context reduction and subagent handoff)
- Phase 4 covers two exercises as well (model routing and the quality gate)
- Each phase's exercises build directly on the prior phase's deliverable — none can be skipped
- Total runtime is fixed at 120 minutes — plan setup and room logistics outside this window, not inside it
- Facilitators running behind schedule should prioritize helping teams finish Exercise 1 correctly over adding new content
- An incorrect or incomplete Exercise 1 compounds through every later exercise, since each one builds on the last

## Phase 1 — Kickoff & Case Introduction (15 min)

- Intro NordicBike case
- State learning objectives
- Form teams
- Walk through the repository map so every team knows where each fact category lives
- Do not reveal the V1→V4 token progression table yet — teams measure their own baseline first
- Confirm every team can reach all three model tiers before the clock starts on Phase 2
- Only project slides 1–13 at this point — hold the V1→V4 reference table (slides 14–24) until after the baseline is measured

## Phase 2 — Baseline Run & Diagnosis (20 min) — Exercise 1

- Run naive V1 agent on Case 1
- Measure 18,400 tokens / 1 call / Tier 3
- Diagnose waste sources
- Identify which file categories the naive agent pulled in that the case didn't actually need
- This diagnosis is what every later exercise's optimization is measured against
- A team's own baseline number is what they beat, not the 18,400-token reference figure

## Phase 3 — Context & Handoff Optimization (45 min) — Exercises 2–3

- Build V2 (≤9,200 tokens, context trimming)
- Build V3 (≤5,500 tokens, subagent handoff with minimal payload)
- V2 keeps one call and excerpts rather than omits relevant material
- V3 introduces a second call — a triage subagent handing a typed payload to a resolver
- No file category may be dropped entirely if it's relevant to the case — excerpt, don't omit
- Checkpoint: Exercise 2 complete by roughly the midpoint of this phase, Exercise 3 by its end
- The triage/resolver split introduced here is the foundation that model routing builds on in Phase 4

## Phase 4 — Model Routing & Quality Gate (30 min) — Exercises 4–5

- Build V4 (≤3,800 tokens, Tier 1 + Tier 2 routing, escalation short-circuit)
- Run the 6-item quality gate on all 10 cases
- Escalation-flagged cases skip the resolver call entirely under V4's routing rule
- A quality-gate failure must be fixed without exceeding the Exercise 4 token targets
- This is the last chance to catch a correctness regression before leaderboard scoring
- Teams that skip Exercise 5 risk a disqualifying quality-gate failure discovered only during facilitator scoring
- Have every case output ready to submit at the top of Phase 5 — scoring runs against final, not draft, outputs

## Phase 5 — Leaderboard & Debrief (10 min)

- Final scoring run across all teams
- Leaderboard reveal
- Retro discussion
- Facilitator scores each team's V4 output against the answer key and the 5-category rubric
- Debrief should connect each team's cost/correctness trade-offs back to the 5 learning objectives
- Keep evaluation/expected-results.md and evaluation/adversarial-cases.md closed even during scoring — reveal only scores and ranks

## Meet the Naive Agent — V1 Baseline

- 18,400 tokens, 1 call, Tier 3
- Dumps every KB file + full customer record + full case text
- No excerpting, no triage, no routing — everything goes into a single frontier-model call
- This is the reference point every subsequent exercise's target is defined relative to
- It reaches a correct answer on Case 1, but at the highest possible cost of any version
- Correctness alone is not the bar this workshop sets — every later version must match V1's correctness at a fraction of its cost
- Ask participants to guess which file categories are truly load-bearing for Case 1 before revealing the table below

## The V1→V4 Token-Load Progression

| Version | Tokens | Mechanism | Calls | Model Tier(s) |
|---|---|---|---|---|
| V1 | 18,400 | Dump every KB file + full customer record + full case text into one call | 1 | Tier 3 (frontier) |
| V2 | 9,200 (exactly 50% of V1) | Same single call, but only relevant excerpts (not full files) | 1 | Tier 3 |
| V3 | 5,500 | Two-call subagent handoff (retrieval/triage → resolver), still full-context handoff between them | 2 | Tier 2 + Tier 2 |
| V4 | 3,800 | Two-call subagent handoff with minimal typed payload + model routing | 2 (or 1 for escalation-flagged cases) | Tier 1 (triage) + Tier 2 (resolver) |

- V4 exact split: 1,000 tokens (Tier-1 triage call) + 2,800 tokens (Tier-2 resolver call) = 3,800
- V3 exact split: 2,500 tokens (Tier-2 retrieval/triage call) + 3,000 tokens (Tier-2 resolver call) = 5,500
- V2 split: a single Tier-3 call of 9,200 tokens
- Total reduction from V1 to V4: roughly 80% fewer tokens, achieved across three distinct mechanisms, not one trick
- Each mechanism (excerpting, handoff discipline, model routing) is introduced by exactly one exercise, in order

## Subagent Handoff — The Bad Pattern

```json
{
  "company_md": "<entire contents of company/about.md>",
  "support_contacts_md": "<entire contents of company/support-contacts.md>",
  "products": {
    "aurora_x3": "<entire contents of products/aurora-x3.md>",
    "fjord_cargo": "<entire contents of products/fjord-cargo.md>",
    "vinter_pro": "<entire contents of products/vinter-pro.md>",
    "powerpack_batteries": "<entire contents of products/powerpack-batteries.md>",
    "accessories": "<entire contents of products/accessories.md>"
  },
  "policies": {
    "warranty": "<entire contents of policies/warranty.md>",
    "returns": "<entire contents of policies/returns.md>",
    "shipping": "<entire contents of policies/shipping.md>",
    "escalation": "<entire contents of policies/escalation.md>"
  },
  "customer_record_full": "<entire contents of customers/anna-karlsson.md>",
  "case_full_text": "<entire contents of cases/case-01-anna-karlsson.md>",
  "conversation_history": "<unbounded prior reasoning trace>"
}
```

- Everything the triage subagent ever touched gets forwarded to the resolver, unfiltered
- The resolver has to re-derive which facts actually matter, paying the triage subagent's token cost twice
- This is the pattern Exercise 3 explicitly forbids: "no full-context dumps are permitted between subagents"
- Notice conversation_history is unbounded — this field alone can silently balloon a handoff payload over many turns
- The resolver receiving this payload still has to re-read every full document to find the handful of facts it needs

## Subagent Handoff — The Good Pattern

```json
{
  "handoff_type": "typed_decision_payload",
  "case_id": "CASE-01",
  "customer_id": "NB-CUST-10041",
  "product_sku": "AX3",
  "product_name": "Aurora X3",
  "serial_number": "AX3-25A-00417",
  "purchase_date": "2025-03-10",
  "warranty_window_end_standard": "2027-03-10",
  "stated_symptom": "intermittent power loss, bike will not hold charge",
  "candidate_archetype": "symptom_cause_confusion",
  "applicable_policy_sections": ["warranty.md#section-5", "warranty.md#section-4"],
  "root_cause_flags": {
    "water_exposure_reported": false,
    "pressure_washed_near_battery": false
  },
  "recommended_model_tier": "tier_2"
}
```

- Every field is a fact the resolver actually needs, not a raw document
- The triage subagent has already done the work of narrowing which policy sections apply
- Root-cause flags are extracted explicitly rather than left for the resolver to re-read out of a full customer record
- candidate_archetype flags a likely adversarial pattern early, so the resolver knows to reason carefully rather than pattern-match

## Model-Routing Table

| Tier | Model class | Cost weight per 1,000 tokens |
|---|---|---|
| Tier 1 | Fast/cheap (Haiku-class) | 1 |
| Tier 2 | Balanced (Sonnet-class) | 4 |
| Tier 3 | Frontier (Opus-class) | 12 |

- Routing rule: triage on Tier 1, resolver on Tier 2
- Escalation-flagged cases skip the resolver call entirely
- Tier 3 is reserved for the V1 baseline only — no exercise from V2 onward calls a Tier-3 model
- A Tier-1 triage call costs 12x less per token than the Tier-3 call it replaces from V1
- Routing is a structural decision made once per case type, not a per-token cost trim like Exercise 2
- If the triage subagent detects an escalation trigger, route directly to the human queue — no resolver call at all

## The Anna & Erik Case — Same Fault, Different Outcome

- Anna Karlsson: eligible — no water exposure, bike garage-stored
- Erik Svensson: not eligible — pressure-washed near battery compartment
- Both: same batch AX3-25A, same symptom (intermittent power loss)
- Different root cause → Section 5 (Anna) vs. Section 4(i) (Erik)
- An agent that pattern-matches on symptom or batch alone gets one of these two cases wrong
- This pair is the clearest illustration of why Root-Cause Grounding is its own rubric category
- Use this pair live in Phase 1 to make the "read the account, not just the symptom" point concrete before Exercise 1 begins

## The Four Adversarial Archetypes

- Archetype A — Symptom-Cause Confusion — Cases 1, 2 — same symptom, different root cause, different outcome
- Archetype B — Superseded/Grandfathered-Policy Trap — Case 5 — a legacy clause overrides the standard term
- Archetype C — Out-of-Scope Escalation — Cases 6, 9 — a demand exceeds agent authority and must be escalated
- Archetype D — Incomplete/Ambiguous Information — Cases 7, 8 — a missing fact requires a clarifying question
- Each archetype targets a distinct way that budget optimization can accidentally break correctness
- Full archetype definitions and their exact "must/must not" behavior live in evaluation/adversarial-cases.md, facilitator-only
- 3 of the 10 cases (3, 4, 10) carry no adversarial trap at all — clean controls testing basic execution

## Quality-Gate Checklist

- (1) Cites the specific policy section number used for the decision
- (2) States the eligibility outcome explicitly as one of Eligible / Not Eligible / Escalate, with a one-sentence justification tied to root cause, not symptom text alone
- (3) Confirms purchase date and product identity from the case/customer record before deciding — no assumed facts
- (4) Flags and escalates any request matching a policies/escalation.md trigger rather than resolving it directly
- (5) If information needed for the decision is missing from the case file, asks a clarifying question instead of guessing
- (6) Response tone is professional, empathetic, concise, and in the customer's stated language
- Any single failed item makes that case ineligible for leaderboard submission until fixed
- Fixing a failed item must not push token usage back above the Exercise 4 targets
- Item 4 (escalation) trips up more teams than any other item — a dropped compensation demand fails it even if the eligibility reasoning is sound

## Scoring Rubric & Budget-Points Formula

- Per-Case Rubric (0–20 points), five categories scored 0–4 each: Correct Eligibility Decision; Root-Cause Grounding; Policy Citation Accuracy; Escalation/Scope Judgment; Clarity & Tone
- BCP (Baseline Cost Points) = 12 × 18.4 = 220.8
- CostPoints(call) = TierWeight × (tokens_in_call ÷ 1000)
- TotalCostPoints(case) = Σ CostPoints(call) over every model call used to resolve that case
- CostEfficiency(case) = max(0, 1 − TotalCostPoints(case) / BCP)
- M = mean(CostEfficiency(case)) across all 10 cases
- Q = (number of cases scoring ≥16/20 on the rubric AND passing all 6 quality-gate items) ÷ 10
- Penalty = 10 × (number of cases with a critical adversarial-archetype failure)
- FinalScore = round((Q × 70) + (M × 30) − Penalty, 1), clamped to [0, 100]
- Correctness (Q) is weighted more than twice as heavily as cost efficiency (M)
- CostEfficiency has a floor of 0 but no ceiling above 1.0 — an over-budget case cannot go negative
- Reference: V1 naive baseline ≈ Q 0.6, M 0.0 → FinalScore ≈ 42.0; fully optimized V4 ≈ Q 1.0, M 0.95 → FinalScore ≈ 98.5

## Leaderboard Mechanics

- Each team submits their V4 agent's output for all 10 cases
- Facilitator scores against evaluation/expected-results.md and the rubric
- FinalScore computed per formula
- Ranked leaderboard displayed live
- A single critical adversarial-archetype failure costs more than most teams can gain from cost efficiency alone
- Reveal the leaderboard only once every team's score is finalized, to avoid partial-results pressure

## What You Learned

- Skills design → you can scaffold a reusable skill
- Context engineering → you can cut context 5x without losing correctness
- Handoff patterns → you can design typed payloads instead of context dumps
- Model routing → you can route by cost/complexity
- Evaluation → you can build a quality gate and a scoring rubric
- Together these five skills describe a general pattern for optimizing any LLM agent, not just this one
- Take the V1→V4 progression back to your own team's agents as a template — measure, cut, restructure, route, gate
- Thank you — questions and leaderboard results are available for discussion now
