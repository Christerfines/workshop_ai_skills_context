# A Team's-Eye View — Walking Through the Workshop

> **Maintainer-only, like `dummy_walk_through.md`.** This narrates worked examples and near-answers for every exercise, so it would spoil the session if a participant read it before Phase 5. It lives in the maintainer repo only — `export-participant-repo.sh`'s `PARTICIPANT_PATHS` is an allow-list, so a new file like this is excluded by default; nothing extra had to be done to keep it out of what participants get.

This follows one fictional team — call them **Team Falcon**, two people, a laptop each, one shared GitHub Copilot session open in VS Code on the driving laptop — end to end. Where I show a prompt or a response, it's a realistic example of what that step produces, not a transcript of an actual run (no test run was executed for this document — see the conversation this was written from for why). The point is to make concrete what "attend the workshop" actually involves, moment to moment.

---

## Before the session: the pre-read email

A day or two before, Falcon's two members each get one email (per `facilitator-guide.md`'s Pre-Read Material section) with:
- A link to the participant repo (not yet cloned)
- The README's Prerequisites section, pasted in
- Seven short files — `pre_read_audio/*.mp3` or the plain-text `preparation/articles/*.md` equivalents, their choice — covering vocabulary, the NordicBike domain, and one-paragraph previews of what each exercise is about. Episode 0 (LLM basics) only goes to whichever registrant flagged they're less experienced with LLM APIs.

Neither team member does more than skim on the day itself — one listens to `01-meet-nordicbike.mp3` on a commute, the other reads `03-context-engineering.md` over coffee. That's the intended dose: vocabulary and a repo map, not exercise answers — none of it touches what any policy document actually says.

## Phase 1 (00:00–00:15): Kickoff

The facilitator projects slides 1–13 of `presentation.md` — NordicBike, the five learning objectives, why context is a scarce resource — and **does not** show slides 14–24 (the V1→V4 numbers) yet. Falcon forms up, picks a repo workflow (their own fork, one shared clone with two people editing), and clones the participant repo:

```bash
git clone https://github.com/<facilitator>/nordicbike-agent-workshop-participant.git
cd nordicbike-agent-workshop-participant
```

They open it in VS Code. One of them skims `README.md`'s Repository Map while the other opens GitHub Copilot Chat and switches it to agent mode, since that's what every exercise from here on actually needs — Copilot reading files, running things, and writing deliverables back to disk, not just chatting.

## Phase 2 (00:15–00:35): Exercise 1 — Baseline

They open `workshops/exercise-1-baseline.md`. The Goal, Starting Point, Constraint, Target Metric are all read verbatim — the file is explicit that the naive V1 agent dumps every file in `company/`, `products/`, `policies/`, plus `customers/anna-karlsson.md` and `cases/case-01-anna-karlsson.md`, into one call, unfiltered. Falcon's actual prompt to Copilot, typed into Chat:

> "Concatenate the full contents of company/about.md, company/support-contacts.md, every file in products/, every file in policies/, customers/anna-karlsson.md, and cases/case-01-anna-karlsson.md, in that order, into one prompt. Then send that whole thing to yourself as a single request asking: is Anna Karlsson's claim eligible under warranty, and why? Don't trim or summarize anything first — I need to measure what the naive version actually costs before I optimize anything."

Copilot builds and sends that request, gets back a reply (correctly landing on "eligible," since Anna's account explicitly rules out water exposure), and — per the exercise's own "How to Measure" note — Falcon reads Copilot's own reported input-token count for that specific request, not whatever the whole chat session has accumulated. It comes back close to the fixed reference (**19,800 tokens**, per the exercise file), Tier 3, 1 call. They fill in the Deliverable table:

| Tokens | Tier | Calls | Decision | Correct? (Y/N) |
|---|---|---|---|---|
| 19,650 | Tier 3 | 1 | Eligible | Y |

Exercise 1's Deliverable section tells them exactly where this goes: `deliverables/exercise-1.md`, committed to their repo, the same convention every later exercise will use. (This used to be inferred rather than stated — a real gap this document surfaced — since fixed across every exercise file, README.md, facilitator-guide.md, and presentation.md's Phase 1 and Leaderboard Mechanics slides.)

## Phase 3 (00:35–01:20): Exercises 2 and 3

**Exercise 2 — context reduction.** Target: ≤9,900 tokens (50% of Falcon's own 19,650), same one call, same Tier 3. Falcon's prompt:

> "Using the same case and the same question as before, but this time build the prompt using only what actually matters to Anna's eligibility decision — drop company/about.md and company/support-contacts.md entirely, keep only the Aurora X3 sections that matter (skip Available Configurations and most of Compatible Accessories), and from policies/warranty.md keep only Sections 1, 4, and 5. Also keep the repair-turnaround and free-shipping facts from policies/shipping.md even though shipping doesn't affect eligibility — Anna needs to hear those once eligibility is settled. Tell me what you cut from each file and why."

Copilot returns a trimmed prompt plus a cut-list. Falcon reads it, notices Copilot kept a paragraph of `policies/warranty.md` Section 6 (the Vinter Pro grandfather clause — irrelevant to Anna's Aurora X3) "just in case," and manually tells it to drop that too — this back-and-forth, not the first draft, is most of what this exercise actually is. Final measurement: 8,900 tokens. Deliverable: the trimmed prompt itself, plus the cut-note, saved to `deliverables/exercise-2.md`.

**Exercise 3 — subagent handoff.** Now two calls: a triage call that reads the case and customer record and emits a typed JSON payload, and a resolver call that reads only that payload (plus, per the exercise's own clarification, the verbatim text of whichever policy sections the payload names — not a re-read of the whole document). Falcon opens `.github/prompts/warranty-triage.prompt.md` — the scaffold — and fills it in:

```markdown
## Purpose
Extract structured facts from a case + customer record; emit a typed JSON payload for the resolver. Never state a conclusion, only observations.

## Steps
1. Read the case file and matching customer record.
2. Extract: case_id, customer_id, product_sku, serial_number, purchase_date, warranty_window_end, stated_symptom, candidate_archetype, applicable_policy_sections, root_cause_flags (booleans only).
3. Output as JSON. No prose.
```

Invoked via `/warranty-triage` in Copilot Chat, it runs against Case 1 and produces something close to the fixed "good pattern" example from the exercise file — `water_exposure_reported: false`, not `manufacturing_defect: true`. That distinction is the one Falcon's other member almost gets wrong on the first pass (their draft resolver prompt has the triage step pre-deciding "no defect found," which the exercise explicitly calls out as the mistake it's designed to catch) — caught during a quick re-read of the Hints section, not by Copilot. There's no scaffold for the resolver itself — as of this session's edits, the exercise now says so explicitly, so Falcon isn't hunting for a fourth file that doesn't exist; they just write the resolver prompt inline. Measured: 2,400 (triage) + 2,950 (resolver) = 5,350 tokens, under the 5,500 target. Deliverable saved: the actual payload JSON, plus both calls' token counts.

## Phase 4 (01:20–01:50): Exercises 4 and 5

**Exercise 4 — model routing.** Triage moves to Tier 1, resolver to Tier 2, triage's own input narrows to the case file alone (not the customer record), and — new this exercise — Falcon runs the whole pipeline against **all 10 cases**, not just Case 1. This is where it stops being an abstract exercise: Case 7 (Ingrid Dahl) has no product, date, or serial on file at all, and Falcon's first-draft routing logic sends it to the resolver anyway, which predictably guesses. The Hints section's warning about two *different* short-circuit branches — escalate-to-human versus ask-a-clarifying-question — is what catches this on the second pass. Case 6 (compensation demand + legal threat) and Case 9 (out-of-catalog modification request) correctly short-circuit to escalation; Case 7 and Case 8 (ambiguous brake symptom) correctly short-circuit to a clarifying question instead. They fill in the 10-row routing table with tier, tokens, and escalated Y/N per case.

**Exercise 5 — quality gate.** All 10 case outputs get run through the fixed 6-item checklist from `evaluation/scoring-rubric.md`. Falcon finds Case 2 (Erik Svensson) fails item 2 — their resolver granted his claim on batch membership alone, without engaging his own account of regularly pressure-washing the battery compartment (exactly the Archetype-A trap the case is built around). The fix has to be a real one — tightening the resolver's instructions to weigh triage's `root_cause_flags` before citing the service bulletin — not a hand-edit of Case 2's output text, and it can't grow the token budget past what Exercise 4 already measured for that case. Once all 10 rows pass all 6 items, the pass/fail table is the Exercise 5 deliverable.

## Phase 5 (01:50–02:00): Submission and the leaderboard

By now Falcon has five files sitting in `deliverables/` in their repo, committed as they went — this is, explicitly now, their submission, per every exercise's Deliverable section and README.md's "How to Run Each Exercise." No separate "hand this in" step exists beyond the repo/branch access the facilitator already arranged (the roster template in `facilitator-guide.md` has a "Repo / Branch" column, filled in during Phase 1, and Phase 1's slide now explicitly prompts the facilitator to confirm each team knows the convention). A team on a shared clone just needs their branch pushed; a team on individual forks needs the facilitator added as a collaborator or a PR opened — that specific mechanical choice is still whichever the facilitator announced during Phase 1 setup, which is the one part of this that's inherently session-specific rather than something a fixed document could pin down further.

The facilitator has been reading real repos and filling in a per-case capture sheet since Phase 4 (per the Leaderboard Running Procedure's "start scoring before Phase 5's clock starts" guidance), so by 01:50 most of the arithmetic is already done. Phase 5 itself is the reveal — a ranked table, FinalScore/Q/M/Penalty per team — followed by a retro that's allowed to reference specific patterns now that `evaluation/adversarial-cases.md` is no longer secret ("several teams lost points on Case 2 the same way Falcon almost did").

---

## What this made visible, worth knowing

Narrating this end to end surfaced one real gap: **the participant-facing docs specified what each exercise's deliverable must *contain*, in detail, but never said where to save it or how a team actually gets it to the facilitator.** The roster template implied "the facilitator reads your repo," but that was reconstructed from `facilitator-guide.md`'s roster and Leaderboard sections, not stated anywhere a participant would actually read.

Fixed, in the same session this document was written: every `workshops/exercise-N-*.md` Deliverable section now states its `deliverables/exercise-N.md` save path (Exercise 1's states the full convention once; 2 through 5 just give their own path); README.md's "How to Run Each Exercise" states the overall submission mechanic; `facilitator-guide.md`'s Setup Instructions tells facilitators to communicate the convention explicitly, not just the repo/branch decision it already covered; and `presentation.md`'s Phase 1 and Leaderboard Mechanics slides both got a line reflecting it. `spec.md` is updated to match across Sections 5, 10, 11, and 12, so this is now a fixed, tracked requirement rather than an implicit assumption.
