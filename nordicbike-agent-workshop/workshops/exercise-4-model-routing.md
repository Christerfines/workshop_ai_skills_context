# Exercise 4 — Model Routing

## Goal

Route the triage call to Tier 1 and the resolver call to Tier 2, and skip the resolver call entirely when triage detects an escalation trigger. This exercise teaches model routing by cost and complexity: not every call in a pipeline needs the same model tier, and some cases can be resolved — or routed to a human — without ever reaching the more expensive resolver call at all.

The underlying idea is that triage — extracting structured facts and flagging archetypes from a case and customer record — is a comparatively easy task that does not need a frontier or even a balanced-tier model to do reliably, while the actual eligibility judgment made by the resolver benefits more from a stronger model's reasoning. Matching model tier to task difficulty, rather than using your strongest available model for every call regardless of what that call is actually doing, is the core cost lever this exercise introduces on top of the context and handoff work from Exercises 2 and 3.

## Starting Point

The V3 output from Exercise 3: a two-call, Tier 2 + Tier 2 pipeline totaling ≤5,500 tokens, using a minimal typed handoff payload between triage and resolver. You will run this same two-subagent structure again in this exercise — the change here is which model tier each subagent runs on, and adding the short-circuit branch that skips the resolver call entirely for escalation-flagged cases, not a change to the subagents' responsibilities themselves.

## Constraint

"Apply the model-routing table: triage calls run on Tier 1, resolver calls run on Tier 2. If the triage subagent detects any escalation trigger from policies/escalation.md, route directly to the human escalation queue and do not make a resolver call at all. Target ≤3,800 total input tokens across all calls for non-escalated cases."

## Target Metric

- **Non-escalated cases:** ≤3,800 total input tokens (1,000 Tier-1 triage + 2,800 Tier-2 resolver).
- **Escalated cases:** ~1,000 tokens (Tier-1 triage only — no resolver call).

## Deliverable

A routing decision log across all 10 cases, showing which model tier was used for each call made, and which cases short-circuited straight to escalation without a resolver call.

| Case | Triage Tier | Triage Tokens | Resolver Called? | Resolver Tier | Resolver Tokens | Total Tokens | Escalated? |
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

Cases 6, 7, and 9 are expected to short-circuit — but for different reasons, so don't route on a single pattern-matched signal. Case 6 and Case 9 short-circuit because triage detects an actual escalation trigger from policies/escalation.md (compensation demand / legal threat for Case 6; out-of-catalog service request for Case 9). Case 7 does not contain an escalation trigger at all — it short-circuits because it's missing decision-critical information, which is a different kind of "don't call the resolver yet" than an escalation. Make sure your routing logic distinguishes "escalate to a human" from "ask the customer a clarifying question" — they are not the same outcome and should not share the same routing branch, and conflating them is a common mistake at this stage.

Watch out for Case 9 in particular: it both has an escalation trigger (the out-of-catalog service/certification request) and a separate, resolvable warranty determination (the Section 4(ii) modification exclusion). Your routing logic should still short-circuit to escalation for this case, but your triage subagent's output should retain enough information that the escalation queue — or a human reviewer — can see both the escalation reason and the warranty-relevant fact, rather than losing the warranty context entirely just because the case routed away from the resolver.
