# Course Walkthrough — Private Facilitator Cheat Sheet

> **This file lives and is tracked in this repo, same tier as `facilitator-guide.md`** — it's excluded from `export-participant-repo.sh`'s output (see its `PARTICIPANT_PATHS` list and the "What is deliberately NOT copied" note at the top of that script), so when a facilitator runs that script to generate the stripped repo handed to attendees, this file simply never makes it into that copy. It stays in git history and on GitHub in the maintainer repo, same as `facilitator-guide.md` and `presentation.md` — it's just not polished documentation, it's a faster on-ramp through the real manual for the first time you run this course.

One paragraph of what this actually is: a fictional e-bike company's warranty-support case load, used to teach five agent-engineering skills (context trimming, subagent handoff, model routing, evaluation, skills design) by having teams progressively rebuild the same support agent from a wasteful single-call version (V1) down to a routed, quality-gated version (V4). Two hours, five phases, teams of 2–4. Everything below is the mechanical "who does what, in what order" — the actual pedagogy lives in `facilitator-guide.md` and `spec.md`, read those for the *why*.

---

## 0. One-time setup (do this once, not per-session)

1. Clone this repo: `git clone git@github.com:Christerfines/workshop_ai_skills_context.git` (note the capital-C — GitHub redirected from the old lowercase URL as of Aug 2026).
2. Read `facilitator-guide.md` top to bottom once. It's the real manual; this file is just a faster path through it.
3. Fill in `updatenumbers.md` Step 5 — the Tier 1/2/3 → concrete Copilot model mapping. This is **not** written anywhere fixed on purpose (Copilot's lineup drifts); you decide it fresh, verbally tell teams, never edit it into a shipped file.
4. If it's been a couple of months since the last delivery, run `updatenumbers.md` Steps 1–4 (re-baseline check) — the fixed token figures (currently 19,800 / 9,900 / 5,500 / 3,800, BCP 237.6) can drift if `company/`, `products/`, `policies/`, `customers/`, or `cases/` content changed.

---

## 1. Before every session (per-delivery checklist)

1. **Generate the participant distribution:**
   ```bash
   cd nordicbike-agent-workshop
   ./export-participant-repo.sh ../nordicbike-agent-workshop-participant
   ```
   This strips everything facilitator-only (`facilitator-guide.md`, `presentation.md`, `updatenumbers.md`, `tests/`, `evaluation/expected-results.md`, `evaluation/adversarial-cases.md`) and git-inits a fresh repo with no history.
2. **If teams need it via GitHub** (not just a local clone): `cd` into that new distribution dir, create a GitHub repo (`gh repo create`), `git remote add origin ...`, `git push -u origin main`. Do this per-cohort if you don't want old cohorts' branches/forks piling up on one shared repo.
3. **Generate the pre-read audio:**
   ```bash
   cd ~/proj/workshop_pre_read_audio
   .venv/bin/python generate_podcast.py --all
   ```
   Writes straight into `nordicbike-agent-workshop/pre_read_audio/*.mp3`. This is a **separate project on purpose** — don't go looking for the generator inside the workshop repo.
4. **Email the pre-read material** — Prerequisites section of README.md, the repo link, and `pre_read_audio/*.mp3` (skip Episode 0 unless you know specific registrants need it) — at least a day ahead. **Send it to yourself first as a dry run** before blasting the real participant list, so you catch a broken attachment or a wrong link on your own inbox, not theirs.
5. **Verify model endpoints** — one trivial call per tier, from a machine on the network participants will use. Do this well before anyone arrives.
6. **Re-check `updatenumbers.md` Step 8** (date-window boundaries) — the answer key's eligibility math is pinned to a fixed "today"; Case 10 in particular flips answer after 2026-11-01. Re-pin the date in `workshops/exercise-1-baseline.md`'s "How to Measure, and What Date to Assume" note if you're delivering after that.

---

## 2. Optional but genuinely worth it the first time: watch the course run itself

You don't have to imagine what a session looks like — there's a harness that actually plays a participant team through all 5 exercises and grades the result:

```bash
cd nordicbike-agent-workshop
./tests/claude/run.sh          # Case-1 smoke test, ~20-40 min, real (small) API spend
```

Read the resulting `tests/reports/<timestamp>-claude.md`. This is the fastest way to internalize what a real team's deliverable at each exercise actually looks like, and where they're likely to get stuck — better than reading the exercise files cold. (`--cases all` for the full 10-case version — much slower/costlier, do it once before a big delivery, not every time.)

If it errors out mid-run with **"Your computer went to sleep"** or a subscription session-limit message, that's not a bug — just re-run the same command; it skips whatever exercises already have a deliverable on disk and picks up where it stopped.

---

## 3. Live session, phase by phase — what happens and what's tricky

### Phase 1 — Kickoff (15 min)
- Intro the case, form teams, walk the repo map, confirm tier access.
- Project slides 1–13 only. **Do not** reveal slides 14–24 (the V1→V4 table) yet — that's the whole point of Phase 2.

### Phase 2 — Exercise 1: Baseline (20 min)
- Teams run the naive agent, record tokens/tier/calls/decision. Nothing to build yet.
- **Tricky part:** "how do I actually measure tokens" isn't obvious — that's why `exercise-1-baseline.md` now has a "How to Measure" note (use your tool's own reported count if it exposes one, exclude your own tool's system-prompt/scaffolding overhead). Point confused teams there rather than re-explaining verbally.
- Expected reading: **19,800 tokens, 1 call, Tier 3.** A team's own number is what they beat — not necessarily this exact figure.

### Phase 3 — Exercises 2–3 (45 min)
- **Exercise 2 (context trim):** cut to ≤50% of *their own* Ex1 number (reference: 9,900). Tricky part: judgment about what's relevant is per-file *and* per-section — `company/about.md` drops entirely, most of `policies/warranty.md` doesn't. One deliberate exception: `policies/shipping.md` is never eligibility-relevant but its turnaround/free-shipping facts still belong in the response — that's now called out explicitly in the Hints so don't let a team drop it just because it's not eligibility-relevant.
- **Exercise 3 (subagent split):** triage → resolver, minimal typed JSON payload, Tier 2 + Tier 2. Tricky part: "minimal" doesn't mean "resolver gets nothing from source files" — the resolver may get the verbatim text of exactly the sections triage named, just not triage's own conclusion about what those sections mean. Watch for a payload field that states a conclusion (`manufacturing_defect: true`) instead of an observed fact (`water_exposure_reported: false`) — that's the mistake this exercise exists to catch, and it's an easy one to miss just eyeballing token counts.

### Phase 4 — Exercises 4–5 (30 min)
- **Exercise 4 (routing):** triage → Tier 1, resolver → Tier 2, plus escalation short-circuit *and* a separate clarifying-question short-circuit (missing info ≠ escalation — two different branches, easy to conflate). Triage's own input also narrows further here (case text only, not the full customer record) — that's the actual new token lever this exercise introduces, on top of routing.
- **Exercise 5 (quality gate):** run the 6-item checklist across all 10 cases, fix failures without growing the budget past *their own* Exercise 4 number. Tricky part: a "fix" that just adds more context back in defeats the exercise — point teams at tightening the prompt/logic instead.

### Phase 5 — Leaderboard (10 min)
1. Collect each team's V4 outputs for all 10 cases as they finish (ideally starting during Phase 4, not cold at 01:50).
2. Fill in the per-case capture sheet from `facilitator-guide.md` (rubric categories, gate pass/fail, tokens by tier).
3. Run the **"Prompt: generate the leaderboard"** template from `facilitator-guide.md` — paste in each team's capture sheet, get back a ranked table with FinalScore/Q/M/Penalty. This is what actually gets projected — not a live scoring session in front of the room.
4. Reveal only after every team is scored. Use the scored outputs as debrief material — this is also the first moment it's safe to show `evaluation/adversarial-cases.md`.

---

## 4. Quick reference — where everything actually lives

- Participant distribution generator: `nordicbike-agent-workshop/export-participant-repo.sh`
- Pre-read audio generator (separate project): `~/proj/workshop_pre_read_audio`
- Finished pre-read MP3s (committed, emailed out): `nordicbike-agent-workshop/pre_read_audio/`
- Answer key (never shown before Phase 5): `nordicbike-agent-workshop/evaluation/expected-results.md`, `evaluation/adversarial-cases.md`
- Leaderboard formula + rubric: `nordicbike-agent-workshop/evaluation/scoring-rubric.md`
- Tier→model mapping (session-specific, never committed): `nordicbike-agent-workshop/updatenumbers.md` Step 5
- Full playtest harness + past reports: `nordicbike-agent-workshop/tests/`
