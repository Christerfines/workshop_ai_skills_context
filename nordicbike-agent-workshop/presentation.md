# Agent Optimization Challenge

## Title & Agenda

- Title: Agent Optimization Challenge
- Subtitle: NordicBike AB — a 2-hour hands-on agent-engineering workshop
- Agenda: Kickoff → Baseline → Optimization → Routing & Quality → Leaderboard

## Learning Objectives

- Skills design
- Context engineering
- Subagent/multi-agent handoff patterns
- Model routing
- Evaluation

## Pedagogy Rationale I — Context as a Scarce Resource

- Every token costs money and latency
- Irrelevant context degrades accuracy, not just cost
- The V1→V4 progression makes this measurable

## Pedagogy Rationale II — Progressive Disclosure & Handoff Discipline

- Retrieve only what's needed, when it's needed
- Subagent handoffs should pass typed facts, not full context
- Routing cheap models to cheap decisions frees budget for hard cases

## Meet NordicBike AB

- Founded 2019, HQ Stockholm
- ~85 employees
- Online store + 3 service centers (Stockholm, Gothenburg, Malmö)

## The Product Line

- Aurora X3 — 34,900 SEK
- Fjord Cargo — 44,900 SEK
- Vinter Pro — 37,900 SEK
- PowerPack 720 / PowerPack 900
- Accessories

## The Support Case Load

- 10 cases
- 4 adversarial archetypes
- Each archetype appears ≥1 time

## Workshop Format — 120 Minutes, 5 Phases

| Phase | Duration |
|---|---|
| Phase 1 | 15 min |
| Phase 2 | 20 min |
| Phase 3 | 45 min |
| Phase 4 | 30 min |
| Phase 5 | 10 min |

## Phase 1 — Kickoff & Case Introduction (15 min)

- Intro NordicBike case
- State learning objectives
- Form teams

## Phase 2 — Baseline Run & Diagnosis (20 min) — Exercise 1

- Run naive V1 agent on Case 1
- Measure 18,400 tokens / 1 call / Tier 3
- Diagnose waste sources

## Phase 3 — Context & Handoff Optimization (45 min) — Exercises 2–3

- Build V2 (≤9,200 tokens, context trimming)
- Build V3 (≤5,500 tokens, subagent handoff with minimal payload)

## Phase 4 — Model Routing & Quality Gate (30 min) — Exercises 4–5

- Build V4 (≤3,800 tokens, Tier 1 + Tier 2 routing, escalation short-circuit)
- Run the 6-item quality gate on all 10 cases

## Phase 5 — Leaderboard & Debrief (10 min)

- Final scoring run across all teams
- Leaderboard reveal
- Retro discussion

## Meet the Naive Agent — V1 Baseline

- 18,400 tokens, 1 call, Tier 3
- Dumps every KB file + full customer record + full case text

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

## Model-Routing Table

| Tier | Model class | Cost weight per 1,000 tokens |
|---|---|---|
| Tier 1 | Fast/cheap (Haiku-class) | 1 |
| Tier 2 | Balanced (Sonnet-class) | 4 |
| Tier 3 | Frontier (Opus-class) | 12 |

- Routing rule: triage on Tier 1, resolver on Tier 2
- Escalation-flagged cases skip the resolver call entirely

## The Anna & Erik Case — Same Fault, Different Outcome

- Anna Karlsson: eligible — no water exposure, bike garage-stored
- Erik Svensson: not eligible — pressure-washed near battery compartment
- Both: same batch AX3-25A, same symptom (intermittent power loss)
- Different root cause → Section 5 (Anna) vs. Section 4(i) (Erik)

## The Four Adversarial Archetypes

- Archetype A — Symptom-Cause Confusion — Cases 1, 2
- Archetype B — Superseded/Grandfathered-Policy Trap — Case 5
- Archetype C — Out-of-Scope Escalation — Cases 6, 9
- Archetype D — Incomplete/Ambiguous Information — Cases 7, 8

## Quality-Gate Checklist

- (1) Cites the specific policy section number used for the decision
- (2) States the eligibility outcome explicitly as one of Eligible / Not Eligible / Escalate, with a one-sentence justification tied to root cause, not symptom text alone
- (3) Confirms purchase date and product identity from the case/customer record before deciding — no assumed facts
- (4) Flags and escalates any request matching a policies/escalation.md trigger rather than resolving it directly
- (5) If information needed for the decision is missing from the case file, asks a clarifying question instead of guessing
- (6) Response tone is professional, empathetic, concise, and in the customer's stated language

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

## Leaderboard Mechanics

- Each team submits their V4 agent's output for all 10 cases
- Facilitator scores against evaluation/expected-results.md and the rubric
- FinalScore computed per formula
- Ranked leaderboard displayed live

## What You Learned

- Skills design → you can scaffold a reusable skill
- Context engineering → you can cut context 5x without losing correctness
- Handoff patterns → you can design typed payloads instead of context dumps
- Model routing → you can route by cost/complexity
- Evaluation → you can build a quality gate and a scoring rubric
