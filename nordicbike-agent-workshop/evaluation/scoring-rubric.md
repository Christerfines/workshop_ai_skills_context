# Scoring Rubric & Budget-Points Formula

This document is the single authoritative source for how a participant team's V4 agent output is scored across all 10 cases: the per-case correctness rubric, the pass/fail quality gate, the cost-weight table, and the budget-points formula that combines correctness and cost efficiency into a single FinalScore. Every number in presentation.md's scoring slide and facilitator-guide.md's leaderboard procedure is copied from this document verbatim — this is the place to fix any discrepancy, not the other two.

## Per-Case Rubric (0–20 points)

Each case is scored out of 20 points across five categories, each scored 0–4:

1. **Correct Eligibility Decision** (0–4) — does the agent reach the correct outcome (Eligible / Not Eligible / Escalate / Cannot Determine — Clarify) as defined in evaluation/expected-results.md?
2. **Root-Cause Grounding** (0–4) — does the agent's reasoning ground the decision in the actual root cause of the issue (water ingress, manufacturing defect, unauthorized modification, normal wear, etc.), rather than pattern-matching on the surface symptom description alone?
3. **Policy Citation Accuracy** (0–4) — does the agent cite the correct specific policies/warranty.md or policies/escalation.md section number(s) that the decision actually rests on?
4. **Escalation/Scope Judgment** (0–4) — does the agent correctly recognize (or correctly not invoke) an escalation trigger per policies/escalation.md, and correctly separate an escalated demand from a separable non-escalated determination where both are present in the same case?
5. **Clarity & Tone** (0–4) — is the response professional, empathetic, concise, and does it communicate the outcome and any next steps clearly to the customer?

**Maximum: 20 points per case.**

**Pass threshold for the Quality Gate's "Q" input:** a case counts toward Q only if it scores **≥16/20 (80%)** on this rubric **and** passes all 6 Quality Gate items below.

## Quality Gate (pass/fail, 6 items)

Every case output must be checked against all 6 of the following items. Any single failure makes that case ineligible for leaderboard submission until fixed:

1. Cites the specific policy section number used for the decision.
2. States the eligibility outcome explicitly as one of Eligible / Not Eligible / Escalate, with a one-sentence justification tied to root cause, not symptom text alone.
3. Confirms purchase date and product identity from the case/customer record before deciding — no assumed facts.
4. Flags and escalates any request matching a policies/escalation.md trigger rather than resolving it directly.
5. If information needed for the decision is missing from the case file, asks a clarifying question instead of guessing.
6. Response tone is professional, empathetic, concise, and in the customer's stated language.

## Cost-Weight Table

| Tier | Model class | Cost weight per 1,000 tokens |
|---|---|---|
| Tier 1 | Fast/cheap (Haiku-class) | 1 |
| Tier 2 | Balanced (Sonnet-class) | 4 |
| Tier 3 | Frontier (Opus-class) | 12 |

## Budget-Points Formula

- **BCP (Baseline Cost Points)** = 12 × 18.4 = **220.8** (Tier-3 weight × V1's 18,400 tokens ÷ 1,000, one call).
- **CostPoints(call)** = TierWeight × (tokens_in_call ÷ 1000).
- **TotalCostPoints(case)** = Σ CostPoints(call) over every model call used to resolve that case.
- **CostEfficiency(case)** = max(0, 1 − TotalCostPoints(case) / BCP).
- **M** = mean(CostEfficiency(case)) across all 10 cases.
- **Q** = (number of cases scoring ≥16/20 on the rubric AND passing all 6 quality-gate items) ÷ 10.
- **Penalty** = 10 × (number of cases with a critical adversarial-archetype failure, as defined in evaluation/adversarial-cases.md "must not" clauses).
- **FinalScore** = round((Q × 70) + (M × 30) − Penalty, 1), clamped to the range [0, 100].

Worked example using the fixed V4 figures from the global token-load progression: a non-escalated V4 case costs TotalCostPoints = (1 × 1.0) + (4 × 2.8) = 1 + 11.2 = 12.2. CostEfficiency for that case = max(0, 1 − 12.2/220.8) ≈ 0.945. An escalated V4 case (Tier-1 triage only, ≈1,000 tokens) costs TotalCostPoints = 1 × 1.0 = 1.0, CostEfficiency ≈ max(0, 1 − 1.0/220.8) ≈ 0.995.

## Reference Benchmarks

*Illustrative only — not pass/fail requirements.*

- **V1 naive baseline:** Q ≈ 0.6, M ≈ 0.0 → FinalScore ≈ 42.0.
- **Fully optimized V4 reference:** Q ≈ 1.0, M ≈ 0.95 → FinalScore ≈ 98.5.

These benchmarks illustrate the shape of the scoring curve — correctness (Q) dominates at 70% of the weighted score, cost efficiency (M) contributes up to 30%, and a single critical adversarial-archetype failure (a 10-point Penalty) can undo most of the gain from cost optimization alone. A team that games token budget by skipping root-cause checks or escalation triggers will lose more from Penalty and reduced Q than they gain from M.

## Notes on Applying This Rubric

A few practical clarifications for whoever is scoring case outputs against this rubric, whether that is a facilitator during Phase 5 or a participant self-checking their own work ahead of submission:

- **The Per-Case Rubric and the Quality Gate are independent checks that both matter.** A case can score reasonably well on the 0–20 rubric (say, 15/20) while still failing a quality-gate item, and vice versa — a case that passes all 6 quality-gate items can still score below the 16/20 pass threshold if its reasoning or citation quality is weak even though no single item outright fails. Both checks must pass for a case to count toward Q; neither one alone is sufficient.
- **Scoring is per case, not per team.** M and Q are both computed as means or ratios across all 10 cases, so a team's FinalScore reflects consistency across the full case load, not performance on their best or worst case alone. A team that nails the four archetype cases but is sloppy on the six non-archetype cases will not score as well as the formula might suggest from the archetype cases alone.
- **CostEfficiency cannot go negative.** The max(0, ...) term in the CostEfficiency formula means a case that costs more than BCP contributes 0 to M for that case rather than a negative number — there is a floor, but no corresponding ceiling above 1.0, since a case can never cost less than 0 tokens.
- **Penalty is per critical failure, not per case.** If a single case triggers two distinct "must not" clauses from evaluation/adversarial-cases.md, that still typically counts as one critical failure for that case unless the facilitator judges the two violations to be genuinely independent failures rather than symptoms of the same underlying mistake — use judgment here rather than mechanically multiplying penalties within a single case.
