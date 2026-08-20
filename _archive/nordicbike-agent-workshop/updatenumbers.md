# Update Numbers — Maintainer Runbook

**Audience: whoever maintains this workshop, run from a GitHub Copilot session (Copilot Chat, agent mode) in this repository.** This is not participant-facing material — it is not gated the way `evaluation/expected-results.md` and `evaluation/adversarial-cases.md` are, but there's no reason to show it during a live session either.

## What this file is for

Every token count, cost weight, and derived score in this workshop (19,800 / 9,900 / 5,500 / 3,800 tokens; BCP = 237.6; the worked-example CostEfficiency figures; the Q/M/FinalScore reference benchmarks) is a **fixed pedagogical constant** — spec.md is explicit that these are reproduced byte-for-byte everywhere they recur, precisely so every team is measured against the same, comparable numbers during a live session. They are not supposed to drift session to session just because someone edited a paragraph in `policies/warranty.md`.

But "fixed" doesn't mean "correct forever." Two things go stale on their own:

1. **The numbers can quietly drift from reality.** The V1 baseline is supposed to represent what a real naive agent actually costs to run the full V1 context bundle against Case 1. If the underlying files grow, the "fixed" number stops being an honest reference point and starts being a made-up one — this is exactly what had happened by the time of the 2026-08-17 re-baseline logged in Step 7 below: the bundle had drifted to ~19,800 tokens by the words×1.333 fallback against a fixed figure that still read 18,400, roughly 8% over threshold.
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

In your GitHub Copilot agent session, concatenate these 13 files exactly as the naive V1 agent would (no excerpting) and get an actual token count from Copilot's own context/token accounting for that request — not a word-count estimate. Record that number as **N_new**.

If you don't have a way to get a real tokenizer count from the session, the fallback is `words × 1.333` (this repo's own approximation ratio, stated in spec.md's global conventions) — but treat that as a rough check, not a replacement for a real count, since it can be off by several percent in either direction depending on how token-dense the actual prose is.

**A footnote on measurement methodology, worth knowing before you trust N_new:** if the "session" doing the counting is a coding agent's own headless or agentic mode (Claude Code, Copilot's agent mode, or similar) rather than a plain chat completion, its reported input-token count typically bundles in that tool's own system prompt and tool/function definitions — which can inflate the number well above what a raw API call against just the V1 bundle would show. This is a real, observed effect, not a hypothetical: `tests/claude/run.sh`'s playtest harness measured Exercise 1's baseline via headless `claude -p` and got 45,225 tokens against an (at-the-time) 18,400 reference — part of that gap was this overhead, part was the bundle's own genuine growth (the reason for this re-baseline). If your counting method is tool-based rather than a raw API call, note that explicitly next to N_new so the next maintainer doesn't mistake tool overhead for genuine content drift.

## Step 2 — Decide whether to re-baseline

- If **N_new** is within roughly 5% of the current fixed figure (19,800, as of the 2026-08-17 re-baseline logged in Step 7), leave the fixed numbers alone. Minor variance is expected and already anticipated in `workshops/exercise-1-baseline.md`'s Hints section — don't chase noise.
- If **N_new** is off by more than that, proceed to Step 3 and re-derive everything from **N_new**.

## Step 3 — Recompute the derived figures

Everything below is a fixed function of the V1 baseline. If you're re-baselining, recompute all of it from **N_new** — don't hand-adjust individual downstream numbers, or you'll reintroduce exactly the kind of inconsistency this file exists to prevent.

| Figure | Formula | Current value (from 19,800, as of the 2026-08-17 re-baseline) |
|---|---|---|
| V1 baseline | measured (Step 1) | 19,800 tokens |
| V2 target | exactly 50% of V1 | 9,900 tokens |
| V3 target | fixed split, independent of V1: 2,500 (triage) + 3,000 (resolver) | 5,500 tokens |
| V4 target (non-escalated) | fixed split, independent of V1: 1,400 (triage) + 2,400 (resolver) | 3,800 tokens |
| V4 target (escalated) | triage call only, no resolver | ~1,400 tokens |
| BCP (Baseline Cost Points) | Tier-3 weight (12) × V1 ÷ 1,000 | 12 × 19.8 = 237.6 |
| Worked non-escalated V4 CostEfficiency | max(0, 1 − ((1×1.4)+(4×2.4))/BCP) | ≈ 0.954 |
| Worked escalated V4 CostEfficiency | max(0, 1 − (1×1.4)/BCP) | ≈ 0.994 |

**Important:** the V3 and V4 splits (2,500+3,000 and 1,400+2,400) are *not* derived from V1 — they're independently fixed design choices about how a two-call pipeline should divide its budget between triage and resolver. Only the V2 target (exactly 50% of V1) and BCP (12 × V1 ÷ 1,000) actually depend on the V1 number. If you re-baseline V1, V2 and BCP (and everything BCP feeds) change; V3 and V4's splits do not, unless you deliberately choose to redesign them too.

**On the V4 split specifically (updated 2026-08-17, second playtest):** the original 1,000+2,800 split was arithmetically impossible — Case 1's case file alone is ~1,006 tokens before any triage schema or instructions, so no case could ever hit the 1,000-token triage line-item honestly. Re-derived to 1,400 (triage, case-text-only per Exercise 4's Starting Point) + 2,400 (resolver), same 3,800 aggregate. Checked against all 10 case files' actual word counts, not just Case 1: Case 1 is the largest at 755 words (~1,006 tokens), every other case is smaller (311–700 words), so 1,400 leaves comfortable headroom for schema and instructions across the full case load, not just the one case teams tune against.

The Reference Benchmarks (`V1 naive baseline: Q≈0.6, M≈0.0 → FinalScore≈42.0`; `Fully optimized V4: Q≈1.0, M≈0.95 → FinalScore≈98.5`) are illustrative, not exact functions of the table above — re-sanity-check them by eye after a re-baseline (M≈0.95 should still roughly match the worked non-escalated CostEfficiency above), but don't treat them as needing a precise recomputation.

## Step 4 — Update every location

If you changed any figure in Step 3, every location below must be updated **in the same pass**, or the "reproduced byte-for-byte" guarantee spec.md makes breaks and different files will disagree with each other mid-session.

### V1 baseline (currently 19,800)
- `README.md` — Overview paragraph
- `presentation.md` — "Meet the Naive Agent" slide, "Phase 2" slide, "The V1→V4 Token-Load Progression" table + BCP formula line
- `facilitator-guide.md` — Setup Instructions item 5, "Reference token splits" note
- `workshops/exercise-1-baseline.md` — Goal, How to Measure note, Target Metric, Hints
- `workshops/exercise-2-context-reduction.md` — Starting Point
- `evaluation/scoring-rubric.md` — Budget-Points Formula (BCP line)
- `spec.md` — global conventions block (also update the "exactly N tokens" claim about the V1 bundle)

### V2 target (currently 9,900)
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

### 3,800 (V4 target) and its 1,400+2,400 split, plus the ~1,400 escalated-case figure
- `presentation.md` — Phase 4 slide, V1→V4 table, V4 split line
- `facilitator-guide.md` — Exercise Prompt iv (verbatim), "Reference token splits" note
- `workshops/exercise-1-baseline.md` — Goal paragraph
- `workshops/exercise-4-model-routing.md` — Constraint (verbatim), Target Metric
- `workshops/exercise-5-quality-gate.md` — Starting Point
- `spec.md` — global conventions block, Exercise 4 section

### BCP (currently 237.6) and everything computed from it
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
| _(initial content generation)_ | 18,400 | — | — | — | Original fixed figure, never re-verified |
| 2026-08-17 | 19,800 | _(not evaluated this pass)_ | _(not evaluated this pass)_ | _(not evaluated this pass)_ | Re-baseline driven by `tests/claude/run.sh`'s playtest audit (see `tests/reports/20260817T085835Z-claude.md`, Finding #3). N_new measured via the words×1.333 fallback (14,889 words → 19,847, rounded to 19,800) — the actual V1 bundle is 13 files, not 12 (also fixed in Step 1's file list above); no live tokenizer session was run. V2/BCP recomputed per Step 3 (9,900 / 237.6); V3/V4 splits left untouched per Step 3's own "independently fixed" note. All Step 4 locations updated in this same pass, plus Decision-1 Mechanism-column fix to the V1→V4 table (V3 now correctly shows the minimal typed payload; V4's mechanism is model routing + triage narrowing to case-text-only). Next real delivery should still get a live tokenizer count per Step 1's primary method, not rely on this fallback indefinitely. |

## Step 8 — Re-verify date-window boundaries (do this every time, independent of Step 1–4)

`evaluation/expected-results.md` pins every warranty-window calculation to a single fixed "today" (declared once, in its opening paragraph), and `workshops/exercise-1-baseline.md`'s "How to Measure, and What Date to Assume" note mirrors that same date into participant-visible material. Both must agree, and both go stale on their own as real time passes — a case whose eligibility depends on a boundary close to the pinned date can silently flip from what the answer key says once enough real time has elapsed, even though nothing in the repository's content changed.

Before every delivery:

1. Decide the date to pin for this delivery (usually just today's actual date, unless you have a reason to pin something else).
2. Update that date in both `evaluation/expected-results.md`'s opening paragraph and `workshops/exercise-1-baseline.md`'s "How to Measure, and What Date to Assume" note — these two must match.
3. Re-check every case in `evaluation/expected-results.md` whose eligibility reasoning depends on a date-window boundary against the newly-pinned date. Case 10 (Gustav Åkesson) is the known example — a PowerPack purchased 2025-11-01, with a 12-month standalone-battery term under Section 3, is only Eligible while today is before 2026-11-01; deliver after that date without re-checking and the key will say Eligible when the honest answer is Not Eligible.
4. If any case's outcome flips, update that case's `evaluation/expected-results.md` entry and note the flip here in Step 7's changelog table (add a column or a Notes entry) so the history shows when and why a "fixed" answer actually changed.
