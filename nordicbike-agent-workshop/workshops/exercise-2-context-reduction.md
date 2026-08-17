# Exercise 2 — Context Reduction

## Goal

Reduce input tokens by at least 50% relative to the V1 baseline, without changing model tier or call count, while keeping the eligibility decision correct. This exercise teaches selective retrieval and progressive disclosure: instead of dumping every file in the knowledge base into the prompt, pull only the excerpts that are actually decision-relevant to the case at hand.

The core skill here is judgment about relevance, not compression for its own sake. It would be easy to hit a 50% token reduction by truncating files arbitrarily or dropping a policy section at random — but that kind of blind cut is exactly what this exercise is designed to catch, since it risks silently breaking the eligibility decision on this case or on a different case later. The right approach is to reason explicitly, file by file and section by section, about whether each piece of content is something the eligibility decision for Case 1 actually depends on, and to keep full fidelity on whatever you keep rather than keeping everything at reduced fidelity.

## Starting Point

The V1 agent output from Exercise 1: 19,800 tokens, 1 call, Tier 3, built from the full V1 naive context set. You should already have this number, its model tier, and its call count recorded from Exercise 1 — this exercise's 50% target is defined directly against whatever you measured there, so refer back to your own Exercise 1 deliverable rather than assuming the reference figure quoted here.

## Constraint

"Reduce the total input context by at least 50% (target: ≤50% of your Exercise 1 measurement; the reference figure is 9,900 tokens) while keeping exactly one model call and the same model tier. You may not drop any relevant source file entirely if it is relevant to the case at hand — you must excerpt, not omit, relevant material. Output quality (correctness of the eligibility decision) must not regress."

## Target Metric

**≤9,900 tokens (or ≤50% of your own Exercise 1 measurement, whichever this is defined against), 1 call, Tier 3.**

## Deliverable

The trimmed prompt you actually send to the model, plus a short note listing which sections were cut from each file and why they were judged not decision-relevant for Case 1.

## Hints

Only the relevant product's full specification section and the relevant policy sections are needed in full — most other content can be excerpted down to a sentence or dropped from the excerpt entirely if it plays no role in the eligibility decision. company/about.md and company/support-contacts.md are never decision-relevant to a warranty eligibility call and can be dropped entirely, not merely trimmed. Be careful, though: "irrelevant" is case-specific — a file that is noise for Case 1 (for example, most of policies/returns.md) may still need to stay in scope if a different case actually turns on it.

Think in terms of two separate questions for every file: first, is this file relevant to this case at all; second, if it is relevant, which specific sections or facts within it does the decision actually depend on. products/aurora-x3.md, for example, is relevant to Case 1 — but the eligibility decision does not depend on its Available Configurations section (frame sizes and colors) or most of its Compatible Accessories section, only on facts like the integrated battery and the pointer to the applicable warranty sections. The same two-question approach applies to policies/warranty.md: Sections 1, 4, and 5 are directly relevant to Case 1; Sections 2, 3, 6, and 7 are not, for this particular case, even though they matter enormously for other cases later in this workshop.

One deliberate exception to the "relevant to the eligibility decision" test: policies/shipping.md is never relevant to *whether* a claim is eligible, by its own explicit statement — but two facts from it (the 5–10 business-day repair turnaround, and that warranty-eligible shipping is NordicBike-paid) still belong in your trimmed context, because a correct response communicates next steps, not just an eligibility verdict, and those two facts are what a customer needs to hear once eligibility is settled. Don't apply the eligibility-relevance test to the whole document and conclude it can be dropped entirely — excerpt those two facts, drop the rest.
