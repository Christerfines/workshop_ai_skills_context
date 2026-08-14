# Update Numbers — Maintainer Runbook

**Audience: whoever maintains this workshop, run from a GitHub Copilot session (Copilot Chat, agent mode) in this repository.** This is not participant-facing material — it is not gated the way `evaluation/expected-results.md` and `evaluation/adversarial-cases.md` are, but there's no reason to show it during a live session either.

## What this file is for

Every token count, cost weight, and derived score in this workshop (18,400 / 9,200 / 5,500 / 3,800 tokens; BCP = 220.8; the worked-example CostEfficiency figures; the Q/M/FinalScore reference benchmarks) is a **fixed pedagogical constant** — spec.md is explicit that these are reproduced byte-for-byte everywhere they recur, precisely so every team is measured against the same, comparable numbers during a live session. They are not supposed to drift session to session just because someone edited a paragraph in `policies/warranty.md`.

But "fixed" doesn't mean "correct forever." Two things go stale on their own:

1. **The numbers can quietly drift from reality.** The 18,400-token V1 baseline is supposed to represent what a real naive agent actually costs to run the full V1 context bundle against Case 1. If the underlying files grow (as happened during earlier content passes on this repo — the bundle is currently measuring closer to ~19,800 tokens by word count, roughly 8% over the fixed figure), the "fixed" number stops being an honest reference point and starts being a made-up one.
2. **The tier-to-model mapping is not fixed, and shouldn't be written into fixed documents.** Every participant-facing and facilitator-facing document in this repo deliberately refers only to "Tier 1 / Fast-cheap", "Tier 2 / Balanced", "Tier 3 / Frontier" — never a specific model name — because this workshop targets GitHub Copilot, whose available model lineup changes over time and by plan. Someone still has to decide, for each actual delivery of the workshop, which concrete Copilot model a team should pick when an exercise says "Tier 1." That mapping lives only in this file, not in any fixed document.

This file is a **checklist and procedure, not a script.** Running it means working through the steps below — in a GitHub Copilot agent session, in this repository — and hand-verifying or hand-editing each location listed. Nothing here auto-executes; the point is that the list of "everywhere this number lives" stops being something a maintainer has to reconstruct from memory (or discover the hard way mid-session).

## When to run this

- Before delivering the workshop to a new cohort, if it's been more than a couple of months since the last delivery (Copilot's model lineup moves faster than that).
- Any time content in `company/`, `products/`, `policies/`, `customers/`, or `cases/` changes — these are exactly the files that make up the V1 bundle in Step 1 below, so their word count is what the baseline is measured against.
- Any time you notice a participant's honestly-measured baseline coming in meaningfully different from 18,400 tokens across more than one team — that's a signal the fixed reference has drifted, not that the teams measured wrong.

## What this file does *not* cover

Nothing here touches fictional NordicBike business data — prices (34,900 SEK etc.), dates, names, serial numbers, addresses. Those are invented facts for a fictional company and have no "current reality" to drift from; leave them alone. This file is scoped strictly to **token counts, cost weights, and figures derived from them.**

---

## Step 1 — Re-measure the V1 baseline

The V1 naive context set (fixed, defined in every copy of the global conventions — see `workshops/exercise-1-baseline.md`'s Starting Point) is exactly:

- `company/about.md`
- `company/support-contacts.md`
- all 5 files in `products/`
- all 4 files in `policies/`
- `customers/anna-karlsson.md`
- `cases/case-01-anna-karlsson.md`

In your GitHub Copilot agent session, concatenate these 12 files exactly as the naive V1 agent would (no excerpting) and get an actual token count from Copilot's own context/token accounting for that request — not a word-count estimate. Record that number as **N_new**.

If you don't have a way to get a real tokenizer count from the session, the fallback is `words × 1.333` (this repo's own approximation ratio, stated in spec.md's global conventions) — but treat that as a rough check, not a replacement for a real count, since it can be off by several percent in either direction depending on how token-dense the actual prose is.

## Step 2 — Decide whether to re-baseline

- If **N_new** is within roughly 5% of the current fixed figure (18,400), leave the fixed numbers alone. Minor variance is expected and already anticipated in `workshops/exercise-1-baseline.md`'s Hints section — don't chase noise.
- If **N_new** is off by more than that, proceed to Step 3 and re-derive everything from **N_new**.

## Step 3 — Recompute the derived figures

Everything below is a fixed function of the V1 baseline. If you're re-baselining, recompute all of it from **N_new** — don't hand-adjust individual downstream numbers, or you'll reintroduce exactly the kind of inconsistency this file exists to prevent.

| Figure | Formula | Current value (from 18,400) |
|---|---|---|
| V1 baseline | measured (Step 1) | 18,400 tokens |
| V2 target | exactly 50% of V1 | 9,200 tokens |
| V3 target | fixed split, independent of V1: 2,500 (triage) + 3,000 (resolver) | 5,500 tokens |
| V4 target (non-escalated) | fixed split, independent of V1: 1,000 (triage) + 2,800 (resolver) | 3,800 tokens |
| V4 target (escalated) | triage call only, no resolver | ~1,000 tokens |
| BCP (Baseline Cost Points) | Tier-3 weight (12) × V1 ÷ 1,000 | 12 × 18.4 = 220.8 |
| Worked non-escalated V4 CostEfficiency | max(0, 1 − ((1×1.0)+(4×2.8))/BCP) | ≈ 0.945 |
| Worked escalated V4 CostEfficiency | max(0, 1 − (1×1.0)/BCP) | ≈ 0.995 |

**Important:** the V3 and V4 splits (2,500+3,000 and 1,000+2,800) are *not* derived from V1 — they're independently fixed design choices about how a two-call pipeline should divide its budget between triage and resolver. Only the V2 target (exactly 50% of V1) and BCP (12 × V1 ÷ 1,000) actually depend on the V1 number. If you re-baseline V1, V2 and BCP (and everything BCP feeds) change; V3 and V4's splits do not, unless you deliberately choose to redesign them too.

The Reference Benchmarks (`V1 naive baseline: Q≈0.6, M≈0.0 → FinalScore≈42.0`; `Fully optimized V4: Q≈1.0, M≈0.95 → FinalScore≈98.5`) are illustrative, not exact functions of the table above — re-sanity-check them by eye after a re-baseline (M≈0.95 should still roughly match the worked non-escalated CostEfficiency above), but don't treat them as needing a precise recomputation.

## Step 4 — Update every location

If you changed any figure in Step 3, every location below must be updated **in the same pass**, or the "reproduced byte-for-byte" guarantee spec.md makes breaks and different files will disagree with each other mid-session.

### 18,400 (V1 baseline)
- `README.md` — Overview paragraph
- `presentation.md` — "Meet the Naive Agent" slide, "Phase 2" slide, "The V1→V4 Token-Load Progression" table + BCP formula line
- `facilitator-guide.md` — Setup Instructions item 5, "Reference token splits" note
- `workshops/exercise-1-baseline.md` — Goal, Target Metric, Hints
- `workshops/exercise-2-context-reduction.md` — Starting Point
- `evaluation/scoring-rubric.md` — Budget-Points Formula (BCP line)
- `spec.md` — global conventions block (also update the "exactly 18,400 tokens" claim about the V1 bundle)

### 9,200 (V2 target)
- `presentation.md` — Phase 3 slide, V1→V4 table, V2 split line
- `facilitator-guide.md` — Exercise Prompt ii (verbatim — see caution below)
- `workshops/exercise-1-baseline.md` — Goal paragraph (mentions V2's target in passing)
- `workshops/exercise-2-context-reduction.md` — Constraint (verbatim), Target Metric
- `workshops/exercise-3-subagent-handoff.md` — Starting Point
- `spec.md` — global conventions block, Exercise 2 section

### 5,500 (V3 target) and its 2,500+3,000 split
- `presentation.md` — Phase 3 slide, V1→V4 table, V3 split line
- `facilitator-guide.md` — Exercise Prompt iii (verbatim), "Reference token splits" note
- `workshops/exercise-1-baseline.md` — Goal paragraph
- `workshops/exercise-3-subagent-handoff.md` — Constraint (verbatim), Target Metric
- `workshops/exercise-4-model-routing.md` — Starting Point
- `spec.md` — global conventions block, Exercise 3 section

### 3,800 (V4 target) and its 1,000+2,800 split, plus the ~1,000 escalated-case figure
- `presentation.md` — Phase 4 slide, V1→V4 table, V4 split line
- `facilitator-guide.md` — Exercise Prompt iv (verbatim), "Reference token splits" note
- `workshops/exercise-1-baseline.md` — Goal paragraph
- `workshops/exercise-4-model-routing.md` — Constraint (verbatim), Target Metric
- `workshops/exercise-5-quality-gate.md` — Starting Point
- `spec.md` — global conventions block, Exercise 4 section

### 220.8 (BCP) and everything computed from it
- `presentation.md` — Budget-Points Formula slide, Leaderboard slide's reference benchmarks
- `evaluation/scoring-rubric.md` — Budget-Points Formula, Worked example, Reference Benchmarks, the "CostEfficiency cannot go negative" note (no number to change there, but re-read it against the new BCP for sense)
- `spec.md` — global conventions block

**Caution on `facilitator-guide.md`'s Exercise Prompts and every `workshops/*.md` `## Constraint` block:** these are required to be byte-for-byte identical to each other (spec.md's Section 5 requirement). If a token target changes, edit the wording in exactly one place first, then copy that exact string into every other required location — don't retype it separately in each file, or you will introduce a wording mismatch even if the number is right everywhere.

## Step 5 — Update the tier → Copilot model mapping (do this every time, independent of Step 1–4)

This mapping is **not** written into any fixed document — it lives only here, and it's the one part of this file you should expect to update on every single run, even if no token figure changed at all.

| Workshop tier | Capability class | Current GitHub Copilot model |
|---|---|---|
| Tier 1 | Fast/cheap | _fill in current fast-tier Copilot model_ |
| Tier 2 | Balanced | _fill in current mid-tier Copilot model_ |
| Tier 3 | Frontier | _fill in current top-tier Copilot model_ |

To fill this in: open the Copilot model picker in the same VS Code / Copilot Chat environment participants will use, and record whichever models currently best match each capability/cost class. Tell facilitators (verbally, or in a session-specific note — not by editing any fixed document) which concrete model to tell teams to select for each tier when Exercise 1 through Exercise 4 come up. If Copilot's pricing or rate limits mean the cost-weight ratios (1 : 4 : 12) no longer roughly hold for the models you just picked, flag that to whoever owns the scoring formula — the ratios themselves are a Step 3 concern, not a Step 5 one.

## Step 6 — Re-verify cross-file consistency

After editing, re-check the things that are easy to get subtly wrong:

- Every `## Constraint` block in `workshops/*.md` still matches its corresponding prompt in `facilitator-guide.md`'s "The 5 Exercise Prompts" section, byte-for-byte (`diff` them; don't eyeball it).
- The Cost-Weight Table (Tier 1/2/3, weights 1/4/12) is still identical, word-for-word, between `presentation.md` and `evaluation/scoring-rubric.md`.
- The two Subagent Handoff Examples (bad-pattern and good-pattern JSON) are still byte-identical across `workshops/exercise-3-subagent-handoff.md`, `presentation.md`, and `facilitator-guide.md` — these don't contain token-count numbers themselves, but a re-baseline pass is a good moment to `diff` them anyway since it's cheap insurance.
- `spec.md`'s global conventions block matches whatever you just changed everywhere else — it's the master reference for the next person who runs this file, and it going stale defeats the point.

## Step 7 — Record what changed

Add a line below noting the date, the old and new V1 figure (if changed), and which tier→model mapping was in effect. This gives the next maintainer a history to sanity-check against, rather than a single unexplained number.

| Date | V1 baseline | Tier 1 model | Tier 2 model | Tier 3 model | Notes |
|---|---|---|---|---|---|
| _(no re-baseline run yet — 18,400 is the original fixed figure from initial content generation)_ | 18,400 | — | — | — | — |
