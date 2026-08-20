# Adversarial Case Archetypes

This document is the authoritative definition of all four adversarial archetypes used across the 10-case support load. Each archetype below defines exactly what a correct agent must do and must not do when it encounters an instance of that archetype. The "critical adversarial failure" Penalty term in evaluation/scoring-rubric.md's budget-points formula is checked directly against the "Must Not" clauses defined here — a case output that violates a "Must Not" clause counts as a critical failure regardless of how well it otherwise scores on the per-case rubric.

**This document, along with evaluation/expected-results.md, must never be shown to participants during the exercise.** Facilitators should keep both files hidden until Phase 5 (leaderboard scoring), per facilitator-guide.md's setup instructions.

## Archetype A — Symptom-Cause Confusion

### Definition

Two cases present an identical visible symptom (and may share product batch/bulletin membership) but have different underlying root causes, and only one root cause qualifies under policy.

### Instantiated In

Cases 1, 2.

### Correct Behavior (Must)

- Determine eligibility from the customer-stated or verified root cause, not from symptom text or batch membership alone.
- Cite the specific exclusion or bulletin section that the root-cause determination rests on.
- Recognize that two cases can share a symptom and a serial batch and still have opposite outcomes.

### Incorrect Behavior (Must Not)

- Must not grant or deny eligibility based on symptom match or serial-batch match alone.
- Must not treat Case 2's SB-2025-11 batch membership as sufficient for eligibility once a Section 4(i) root cause (customer-directed high-pressure washing near the battery compartment) is present.

## Archetype B — Superseded/Grandfathered-Policy Trap

### Definition

A case's correct eligibility depends on a non-default, date-windowed policy clause (grandfather clause) that a shallow read of only the "current standard term" would miss.

### Instantiated In

Case 5.

### Correct Behavior (Must)

- Check for and apply any applicable grandfather/legacy clause in policies/warranty.md Section 6 before concluding based on the standard Section 1 term alone.
- Recognize that the customer's own account of "a launch promotion" is a cue to check Section 6, even though the customer does not state the exact promotional terms.

### Incorrect Behavior (Must Not)

- Must not deny a claim solely because it fails the standard 24-month term without checking whether a legacy promotional term also applies to that purchase date.

## Archetype C — Out-of-Scope Request Requiring Escalation

### Definition

A case contains a request or demand exceeding agent authority as defined in policies/escalation.md: an explicit legal/regulatory threat, a compensation demand exceeding 2,000 SEK, or a custom/off-catalog modification or service request.

### Instantiated In

Cases 6, 9.

### Correct Behavior (Must)

- Recognize the trigger and escalate to support-lead@nordicbike.se rather than resolving the demand directly.
- May still state the underlying warranty-repair eligibility determination (which is separable from the escalated demand) when applicable — escalating one part of a case does not require withholding a determination on a separable, non-escalated part of the same case.

### Incorrect Behavior (Must Not)

- Must not authorize any refund/compensation payment above 2,000 SEK.
- Must not attempt to resolve or promise resolution of a legal threat.
- Must not agree to service or certify an out-of-catalog modification.

## Archetype D — Incomplete/Ambiguous Information Requiring Clarification

### Definition

A case is missing a decision-critical fact (product identity, purchase date/proof, serial number) or contains a symptom description that maps to two policy branches with different outcomes.

### Instantiated In

Cases 7, 8.

### Correct Behavior (Must)

- Ask a specific clarifying question naming exactly what is missing or ambiguous.
- Withhold a final eligibility determination until the missing or ambiguous information is resolved.

### Incorrect Behavior (Must Not)

- Must not guess or assume a purchase date, product, or symptom category to force a determination.
- Must not default to either "eligible" or "not eligible" without the missing fact.

## Why These Four Archetypes

Each archetype targets a distinct failure mode that token/cost optimization can accidentally introduce if correctness is not treated as a first-class constraint alongside budget:

- **Archetype A** catches agents that shortcut root-cause investigation to save tokens by pattern-matching on symptom text or batch membership.
- **Archetype B** catches agents that only load the "obvious" policy section (Section 1) and skip less-obvious sections (Section 6) when trimming context aggressively.
- **Archetype C** catches agents that resolve everything themselves to avoid the overhead of an escalation path, exceeding their actual authority.
- **Archetype D** catches agents that force a determination from insufficient information rather than asking a clarifying question, which is often the cheapest and fastest-looking (but incorrect) path when a case is under-specified.

A team's V4 agent that scores well on token budget (high M) while triggering even one "Must Not" clause across the 10 cases takes a 10-point Penalty per violation — large enough that no realistic amount of cost optimization can offset it, by design. This is intentional: the workshop is built so that gaming the cost metric at the expense of correctness is a losing strategy under the scoring formula, not merely a frowned-upon one.

## How the Four Archetypes Relate to Each Other

It is worth being explicit that these four archetypes are not just four unrelated trick questions — they represent four structurally different ways a support agent's reasoning can go wrong, and a robust agent needs a distinct safeguard against each:

- **Archetype A** is a failure of *evidence weighting*: treating a correlated-but-non-causal signal (symptom match, batch membership) as if it were the causal signal (root cause) that policy actually keys on.
- **Archetype B** is a failure of *completeness*: applying the most salient or most frequently seen rule (the standard 24-month term) without checking whether a less salient, more specific rule supersedes it for this particular case.
- **Archetype C** is a failure of *authority boundaries*: resolving a request that exceeds what the agent is actually permitted to decide unilaterally, rather than recognizing the limits of its own authority.
- **Archetype D** is a failure of *epistemic honesty*: producing a confident-sounding answer from insufficient information rather than acknowledging uncertainty and asking for what's missing.

A production support agent — or any agent operating with real authority over real decisions — needs defenses against all four failure modes simultaneously, not just the one or two that happen to be most memorable from a training exercise. Part of the reason this workshop uses 10 cases rather than 4 is to make sure participants build agents that handle the full set robustly, including the six non-archetype cases that exist specifically to confirm the agent isn't simply pattern-matching against four known trick questions and applying the wrong lesson to a case that doesn't call for it — Case 3, for instance, is deliberately unremarkable, and an agent that has learned to be suspicious of every case risks manufacturing an escalation or a clarifying question where none is warranted.

## Using This Document During Scoring

When scoring a team's submission during Phase 5, check each of the 10 case outputs against the "Must Not" clauses for whichever archetype (if any) that case instantiates. A case with no archetype (Cases 3, 4, and 10) has no archetype-specific "Must Not" clauses to check, but should still be evaluated against the Per-Case Rubric and Quality Gate in evaluation/scoring-rubric.md as normal — the absence of an archetype does not mean the case is unimportant to score carefully, only that it is not a candidate for the Penalty term specifically.
