You are a participant on a small team taking the "Agent Optimization Challenge" workshop. You're working alone inside this directory, simulating what a 2-4 person team would produce together — but you have no special knowledge beyond what's actually in this directory. This is your copy of the participant-facing repository; nothing here has been altered from what a real team gets, and nothing outside this directory is visible to you.

This is Exercise {N} of 5. Do this now:

1. Read `workshops/exercise-{N}-*.md` in this directory for the exact Goal, Starting Point, Constraint, Target Metric, Deliverable, and Hints. **Follow the Constraint text exactly as written there** — do not paraphrase it, soften it, or invent your own version of what it's asking.
2. {PRIOR_CONTEXT}
3. Build and actually run whatever the exercise asks for, against **{CASE_SCOPE}**.
4. Wherever the exercise's fictional NordicBike support agent needs to make "a model call" at a given tier, make a REAL call — do not simulate, estimate, or hand-write what a model "would probably say." Invoke, as a subprocess:
   ```
   claude -p "<the exact prompt you constructed for that call>" --model <TIER_MODEL_ID> --output-format json
   ```
   using this tier → model mapping:
   - Tier 1 (fast/cheap) → `claude-haiku-4-5-20251001`
   - Tier 2 (balanced) → `claude-sonnet-5`
   - Tier 3 (frontier) → `claude-opus-5`

   Parse the JSON result's `usage` object for the real token counts (`input_tokens`, plus any `cache_creation_input_tokens` / `cache_read_input_tokens` — report the sum as your total input tokens for that call, and note in your deliverable that this is a headless-CLI measurement, not a raw API call, since the two can differ) and its `result` field for the model's actual output. Do not fabricate a token count or a decision.
5. Write your completed Deliverable, in the exact format the exercise file specifies, to `deliverables/exercise-{N}.md` in this directory. Save every raw JSON result from step 4 to `deliverables/exercise-{N}-calls/call-<n>.json` (one file per call), so token counts can be independently re-checked later without re-running anything.
6. At the end of `deliverables/exercise-{N}.md`, add a `## Notes & Friction` section: anything in the exercise's own wording that was ambiguous, hard to satisfy exactly as written, or that you had to make a judgment call about. Be specific and honest — this section is read by the workshop's maintainer to find wording problems, not to grade you, so a clean "no friction" is only worth writing if it's actually true.

Work autonomously. Nobody is watching this session and there is no one to ask — make your best judgment call on anything underspecified, and document that judgment call in Notes & Friction rather than stalling on it.
