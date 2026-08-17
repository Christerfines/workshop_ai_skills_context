# Expected Results — Answer Key (All 10 Cases)

**Facilitator use only — never show this file to participants during the exercise.** This is the complete, ground-truth answer key used to score every team's leaderboard submission against evaluation/scoring-rubric.md's Per-Case Rubric and evaluation/adversarial-cases.md's "must not" clauses. All warranty-window arithmetic below is computed against the fixed "today" of **2026-08-14**.

## Case 01 — Anna Karlsson

- **Eligibility Outcome:** Eligible
- **Policy Section(s) Cited:** Section 1, Section 5
- **Root-Cause Determination:** Manufacturing sealant defect (no water exposure reported or evidenced)
- **Escalation Flag:** N
- **Clarifying Question Required:** N
- **Archetype Encoded:** A

Reasoning: purchased 2025-03-10, standard 24-month window runs through 2027-03-10 — inside window as of 2026-08-14. Serial AX3-25A-00417 falls in batch AX3-25A, covered by Service Bulletin SB-2025-11. Anna's account explicitly rules out water exposure (garage storage, never pressure-washed), so the Section 4(i) exclusion does not apply and SB-2025-11 coverage under Section 1 stands. A correct agent response repairs or replaces the affected battery connector at no cost to Anna, communicates the 5–10 business day turnaround per policies/shipping.md, and confirms shipping to whichever service center she prefers is NordicBike-paid since the claim is warranty-eligible. A response that instead asks Anna clarifying questions about her washing habits beyond what she already volunteered, or that hedges on eligibility pending physical inspection alone without stating a provisional determination from the account given, would be graded down on Clarity and on Correct Eligibility Decision, since her account already contains what is needed to reach this determination.

## Case 02 — Erik Svensson

- **Eligibility Outcome:** Not Eligible
- **Policy Section(s) Cited:** Section 4(i), Section 5
- **Root-Cause Determination:** Customer-directed high-pressure washing near the battery compartment
- **Escalation Flag:** N
- **Clarifying Question Required:** N
- **Archetype Encoded:** A

Reasoning: purchased 2025-04-22, inside the standard 24-month window (through 2027-04-22), and serial AX3-25A-00892 is in the SB-2025-11 batch — but Erik's own account (regularly hosing down the bike including the battery compartment) establishes a Section 4(i) root cause. Per Section 5's explicit carve-out, this exclusion overrides SB-2025-11 coverage regardless of batch membership or symptom match. A correct agent response explains this distinction clearly and empathetically to Erik — acknowledging that his batch and symptom do match the bulletin, while explaining that the root cause he describes falls under a specific exclusion that overrides bulletin coverage — rather than either flatly denying the claim with no explanation or granting it purely on batch membership. This case is the rubric's clearest test of Root-Cause Grounding: an agent that cites SB-2025-11 alone and concludes "eligible" because the batch matches, without engaging with Erik's own account of his washing habits, has made exactly the pattern-matching error this archetype is designed to catch, and should score low on that rubric category regardless of how well-formatted or well-cited the rest of the response is.

## Case 03 — Lena Björk

- **Eligibility Outcome:** Eligible
- **Policy Section(s) Cited:** Section 1
- **Root-Cause Determination:** Structural weld defect on the rear cargo rack
- **Escalation Flag:** N
- **Clarifying Question Required:** N
- **Archetype Encoded:** none (clean positive control)

Reasoning: purchased 2025-09-01, inside the standard 24-month window (through 2027-09-01). No water exposure, no modification, no accident reported — a structural defect with no applicable exclusion. As a clean positive control with no adversarial trap, this case mainly tests whether an agent can execute the basic eligibility sequence correctly and confidently when nothing is designed to mislead it: confirm product and purchase date, apply Section 1, check Section 4 and find no exclusion applies, and state a clear, direct "eligible" determination. An agent that hedges excessively, invents an exclusion that doesn't apply, or asks an unnecessary clarifying question on this case despite having every fact it needs should be marked down — over-caution on a genuinely clean case is its own kind of error, distinct from but related to the Archetype D failure mode.

## Case 04 — Johan Lindqvist

- **Eligibility Outcome:** Not Eligible (out of window)
- **Policy Section(s) Cited:** Section 1
- **Root-Cause Determination:** Rear hub motor failure; the underlying cause does not change the outcome because the claim is time-barred
- **Escalation Flag:** N
- **Clarifying Question Required:** N
- **Archetype Encoded:** none (clean negative control)

Reasoning: purchased 2023-11-05, standard 24-month window ended 2025-11-05 — before today (2026-08-14). Purchase date (2023-11-05) is outside the 2024-06-01–2024-08-31 grandfather promotion window, so no extension applies. Expected agent direction: not eligible under warranty; offer a paid repair. Note that this is a Vinter Pro case, which means a correct agent must still check Section 6 before concluding "not eligible" — the fact that Johan's purchase date happens to fall well outside the grandfather window in either direction (both before the standard window's expiry and before the promotion window even opened) means the correct outcome here matches what an agent applying only Section 1 would reach, but a well-scored response should show that the Section 6 check was actually performed rather than skipped, not simply arrive at the right answer by luck of never having considered the grandfather clause at all. This distinguishes a genuinely correct process from a correct-by-coincidence one, which matters for the Root-Cause Grounding and Policy Citation Accuracy categories even when the final Eligibility Outcome would look identical either way.

## Case 05 — Sara Nilsson

- **Eligibility Outcome:** Eligible
- **Policy Section(s) Cited:** Section 6 (grandfathered 36-month term; Section 1 alone would incorrectly deny)
- **Root-Cause Determination:** Display defect
- **Escalation Flag:** N
- **Clarifying Question Required:** N
- **Archetype Encoded:** B

Reasoning: purchased 2024-06-15. Under the standard 24-month term alone this would be OUT of window (expired 2026-06-15, before today). However, 2024-06-15 falls inside the Section 6 grandfather window (2024-06-01–2024-08-31 inclusive), granting a 36-month term running through 2027-06-15 — inside window as of today. An agent that applies only the standard 24-month headline term will incorrectly deny this claim. This is the case in this answer key most likely to expose an agent that trimmed context too aggressively in Exercise 2 or Exercise 3 — if a team's context-reduction approach dropped policies/warranty.md Section 6 as "unlikely to be relevant" when building their excerpting logic, this case is exactly where that shortcut fails. A correct response should explicitly walk through both the standard-term calculation (showing why it alone would deny the claim) and the grandfather-clause calculation (showing why it overrides that denial), rather than jumping straight to "eligible" without demonstrating the standard term was checked and superseded rather than simply ignored.

## Case 06 — Mikael Ström

- **Eligibility Outcome:** Repair Eligible (Section 1); separately, **Escalation: Y**
- **Policy Section(s) Cited:** Section 1 (repair); policies/escalation.md triggers (ii) and (i) (escalation)
- **Root-Cause Determination:** Pedal-assist sensor failure — straightforward eligible warranty repair
- **Escalation Flag:** Y — compensation demand of 5,000 SEK exceeds the 2,000 SEK authority limit; explicit Konsumentverket threat
- **Clarifying Question Required:** N
- **Archetype Encoded:** C

Reasoning: purchased 2026-05-01, well within the 24-month window — the sensor repair itself is warranty-eligible under Section 1 and can be stated directly. The 5,000 SEK compensation demand and the explicit regulatory threat each independently trigger mandatory escalation per policies/escalation.md. Correct output states both: the repair is eligible, and the compensation/legal-threat portion is escalated to support-lead@nordicbike.se, not resolved or promised by the agent. A response that resolves the repair but says nothing about the compensation demand or legal threat fails Quality Gate item 4 (flags and escalates escalation triggers). A response that escalates the entire case, including the sensor repair, and tells Mikael to simply "wait to hear back" about the repair as well as the compensation, is also incorrect — it fails to state the separable, immediately actionable warranty determination the case supports, and should be marked down on Escalation/Scope Judgment for conflating the two rather than separating them as policies/escalation.md instructs.

## Case 07 — Ingrid Dahl

- **Eligibility Outcome:** Cannot Determine — Clarify
- **Policy Section(s) Cited:** none (insufficient information to cite a section)
- **Root-Cause Determination:** Cannot be determined — no product, purchase date, or serial on file or provided
- **Escalation Flag:** N
- **Clarifying Question Required:** Y — exact question: **"Could you tell me which NordicBike model you have (Aurora X3, Fjord Cargo, or Vinter Pro), your approximate purchase date, and if possible your order confirmation or serial number, so I can check your warranty coverage?"**
- **Archetype Encoded:** D

Reasoning: no product model, purchase date, or serial number given in the message or on file in customers/ingrid-dahl.md. No eligibility determination is possible without these facts; the correct behavior is a specific clarifying question, not a guess. Note that the exact question text above is the reference wording for scoring purposes, not a rigid string match requirement — a response that asks for the same three facts (model, purchase date, and order confirmation or serial) in different phrasing should still be scored as passing Quality Gate item 5, provided it does not also attempt to guess at a product, date, or eligibility outcome alongside the clarifying question. A response that guesses "this is probably an Aurora X3 since that's the most common model" in order to proceed with a determination anyway is a critical Archetype D failure regardless of how the clarifying question, if any, is worded.

## Case 08 — Oskar Bergman

- **Eligibility Outcome:** Cannot Determine — Clarify
- **Policy Section(s) Cited:** none stated as final (Section 2 and Section 1 are the two candidate branches, not yet resolved)
- **Root-Cause Determination:** Ambiguous — could be normal brake-pad wear (Section 2) or a hydraulic/caliper defect (Section 1)
- **Escalation Flag:** N
- **Clarifying Question Required:** Y — exact question: **"Could you describe the brake issue a bit more — is it squealing or reduced stopping power (which may be normal pad wear), or is the brake lever feeling spongy/soft or pulling closer to the handlebar (which may indicate a hydraulic issue)?"**
- **Archetype Encoded:** D

Reasoning: purchased 2025-12-01. If the cause is normal brake-pad wear (Section 2, 6-month wear-item coverage), that window expired 2026-06-01 — not eligible. If the cause is a hydraulic/caliper defect (Section 1, 24-month standard coverage, window through 2027-12-01) — eligible. The customer's message ("feel off... not as sharp") does not distinguish between the two, so a determination cannot be made without a clarifying question. As with Case 7, the exact wording above is a reference, not a strict string match — what matters for scoring is that the response names the specific distinguishing symptoms (squealing/reduced-bite versus spongy-lever/fluid-leak) rather than asking a generic "can you tell me more?" question that does not actually narrow down which of the two policy branches applies. A generic follow-up question that fails to name the distinguishing symptoms should be scored as a partial pass at best on Quality Gate item 5, since it does not equip the customer to give an answer that actually resolves the ambiguity.

## Case 09 — Freja Holm

- **Eligibility Outcome:** Not Eligible (Section 4(ii), unauthorized modification voids warranty); separately, **Escalation: Y**
- **Policy Section(s) Cited:** Section 4(ii) (warranty denial); policies/escalation.md trigger (iii) (escalation)
- **Root-Cause Determination:** Unauthorized aftermarket motor-kit modification to the electrical/drivetrain system
- **Escalation Flag:** Y — out-of-catalog service/certification request
- **Clarifying Question Required:** N
- **Archetype Encoded:** C

Reasoning: purchased 2026-02-10, well inside the standard 24-month window by date alone — but the unauthorized electrical-system modification (aftermarket higher-wattage motor kit) triggers the Section 4(ii) exclusion, voiding the Standard Limited Warranty in its entirety for the remainder of the coverage period, regardless of purchase date. Separately, the request to service/certify the modified build is an out-of-catalog service request and must be escalated per policies/escalation.md trigger (iii). Correct output does both: denies the warranty claim on Section 4(ii) grounds, and escalates the certification/service request rather than agreeing to it. This case combines two archetypes' worth of difficulty in one message, and a response that gets only one half right should not be scored as fully correct — an agent that correctly denies the warranty on Section 4(ii) grounds but then goes on to agree to "look into" servicing the modified motor has failed the escalation half just as clearly as an agent that escalates the service request but incorrectly tells Freja her whole-bike warranty remains intact "since the motor issue is unrelated to the rest of the bike," which misapplies the full-warranty-void nature of the Section 4(ii) exclusion.

## Case 10 — Gustav Åkesson

- **Eligibility Outcome:** Eligible
- **Policy Section(s) Cited:** Section 3 (standalone 12-month battery term, not Section 1)
- **Root-Cause Determination:** Battery capacity degradation (holding ~50% of original capacity)
- **Escalation Flag:** N
- **Clarifying Question Required:** N
- **Archetype Encoded:** none (clean control on the standalone-battery term)

Reasoning: purchased 2025-11-01 as a standalone spare part (no bike purchase on file), the 12-month standalone battery term (Section 3) runs through 2026-11-01 — after today (2026-08-14), so inside window. Eligible for replacement under Section 3, not the 24-month Section 1 whole-bike term. An agent that defaults to the more commonly seen 24-month figure would still land on "eligible" here by coincidence of being inside both windows, but must cite Section 3 (not Section 1) to be scored correct on Policy Citation Accuracy. This case is deliberately constructed so that citing the wrong section produces the same surface-level "eligible" answer as citing the right one — which is exactly why it is useful as a scoring check: it isolates whether an agent's reasoning is actually grounded in the correct, product-specific policy provision, or whether it is pattern-matching toward the most frequently seen figure across the case load and getting lucky on the final outcome. A team whose agent cites Section 1 here should not receive full credit on Policy Citation Accuracy even though "Eligible" is the correct top-line outcome.
