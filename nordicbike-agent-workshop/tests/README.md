# Workshop Playtest — Automated End-to-End Exercise Testing

Answers one question this repo couldn't otherwise answer without a live human cohort: **if a competent AI agent follows only the participant-facing instructions, can it actually build the V1→V4 agent, hit the stated targets, and get the cases right — and where does the workshop's own wording cause friction?**

This is a maintainer tool, not participant-facing — it reads `evaluation/expected-results.md` and `evaluation/adversarial-cases.md` to grade results, so it's excluded from `export-participant-repo.sh` the same way `facilitator-guide.md` is. Nobody running this is a participant.

## The two lanes

Both lanes copy the repo's **participant tier only** (via `export-participant-repo.sh` — the same script that produces what a real team gets) into a fresh working directory outside this repo, then work through Exercises 1–5 against that copy, uncontaminated by the answer key.

- **`claude/`** — fully automated. Shells out to the `claude` CLI in headless mode (`claude -p`) once per exercise; the agent reads the exercise file itself, builds/runs the pipeline, and writes its deliverable to disk, exactly like a real participant handing off between exercises via git.
- **`copilot/`** — semi-automated. GitHub Copilot has no equivalent local, unattended multi-step CLI (its automation surface is either interactive Chat, or the async cloud "coding agent" tied to a real GitHub repo/PR). Instead, `copilot/run.sh` copies the workshop, then **drives a human**: it prints each exercise's exact prompt and prior deliverables in turn and waits for you to paste back what Copilot Chat (in VS Code) produced. Same deliverable format either way, so...

...both lanes feed the same grader (`common/grade.sh`), which is run **from this repo** (not from the working copy) so it has access to the answer key, and produces one report in `reports/`.

## Running it

```bash
# Claude lane — fully automated, real API spend, ~20-40 min for a Case-1-only smoke run
tests/claude/run.sh                      # default target: ../../test_workshop/claude
tests/claude/run.sh --cases all          # full 10-case Exercise 5 pass (much slower/costlier)
tests/claude/run.sh --target /some/path  # override the working-copy location

# Copilot lane — semi-automated, needs you at the keyboard in VS Code
tests/copilot/run.sh
```

Default working copies land at `../../test_workshop/<lane>/` relative to this file — i.e. `<repo-root>/../test_workshop/claude` and `.../copilot`, siblings of this repo, never inside it. `--target` overrides that.

Reports land in `tests/reports/<UTC-timestamp>-<lane>.md`, committed to this repo — that's the artifact worth keeping; the working copies are scratch and safe to delete after a run (`rm -rf ../../test_workshop`).

## Tier → model mapping used by the Claude lane

Fixed for this tool (unlike the workshop's own Copilot mapping, which is deliberately not fixed — see `updatenumbers.md`):

| Workshop tier | Model |
|---|---|
| Tier 1 — Fast/cheap | `claude-haiku-4-5-20251001` |
| Tier 2 — Balanced | `claude-sonnet-5` |
| Tier 3 — Frontier | `claude-opus-5` |

The coding/driving session itself (the thing playing "the participant team," as opposed to the fictional NordicBike agent it's building) always runs on `claude-sonnet-5`, regardless of which tier a given exercise's fictional agent is targeting — see `claude/run.sh`'s `DRIVER_MODEL`.

The Copilot lane has no equivalent fixed table — fill in `copilot/tier_mapping.md` per session, the same way `updatenumbers.md` Step 5 asks a facilitator to, since Copilot's model lineup isn't fixed either.

## What "grading" actually checks

`common/grade.sh` sends the full set of deliverables plus the answer key (`evaluation/expected-results.md`, `evaluation/adversarial-cases.md`, `evaluation/scoring-rubric.md`) to one more `claude -p` call, asked to grade every deliverable — per-exercise Target Metric and Constraint compliance, correctness against the answer key, quality-gate pass/fail, adversarial "must not" violations — **and** to separately flag anything that looks like a workshop-wording problem rather than an agent mistake (an ambiguous constraint, an unreachable target, a Constraint/Deliverable mismatch). That second half is the actual point of this tool: a wrong *answer* means the test agent made a mistake; a wrong *target* or unreachable *constraint* means the workshop material needs a fix.

## Honest scope of a smoke run

The default run uses **Case 1 only** for Exercises 1–4 (matching how those exercises are scoped in the real workshop) and Exercise 5's quality gate is applied to that same single case rather than all 10, to keep a routine run fast and cheap. This is a smoke test — "does the pipeline as specified actually work at all" — not a full correctness audit. Pass `--cases all` for the real thing (much slower: Exercise 5 alone becomes a 10-case run, and worth doing before trusting the workshop's V1→V4 progression numbers for a new cohort).
