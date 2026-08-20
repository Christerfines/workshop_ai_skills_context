# Organizer Worksheet — Build an Issue-Diagnosis Skill

**Purpose:** Run this four-stage team activity during the two-hour workshop. Teams build and refine one shared `support-diagnosis` skill that turns a support report into an action-ready, structured diagnostic brief. The skill determines what is most likely happening and who should act next; it does not implement fixes, make promises, or authorise compensation.

**Set-up:** Teams of 4-5. Give every team the starter `.github/skills/support-diagnosis/SKILL.md`, the diagnostic brief schema, the public style guide, and one case pack. Keep answer keys and the private financial information out of the participant evidence pack. Use two cases per team: one straightforward and one noisy or ambiguous.

| Stage | Time | Team task | Organizer prompt / release | Evidence of progress |
|---|---:|---|---|---|
| **1. Establish a baseline** | 15 min | Read the initial support report. Optionally run the portal to understand or reproduce the reported behaviour; it is not required. Run the starter skill once without changing it. Individually note what the resulting brief knows, assumes, and fails to tell each consumer role. Then agree on the first skill changes. | Give only `initial/` evidence and portal access. Ask: “What must a support agent, developer, QA engineer, and product owner be able to act on from one brief?” | One baseline brief and a short list of missing or unsafe fields/instructions. |
| **2. Make evidence selection deliberate** | 20 min | Update the skill so it asks questions before opening material. Write two competing hypotheses and identify the smallest artifact that could distinguish them. Run the revised skill. | Release one requested artifact at a time. Before releasing it, require: question, expected signal, and what a negative result would mean. Withhold source and large logs unless the question justifies them. | Revised skill plus an evidence chain in which every artifact answers a named question. |
| **3. Make the brief actionable and bounded** | 25 min | Improve the structured output. Require evidence-backed root cause, confidence, remaining uncertainty, role-specific next actions, scope boundaries, and customer-safe facts in the company voice. Run the skill again. | Provide the schema and style guide. Introduce either conflicting evidence, a compensation demand, or irrelevant/private material. Ask: “What may this brief say, and what must it deliberately not decide or disclose?” | A complete JSON brief that lets each role find its next action without reading the full investigation. |
| **4. Stress-test and refine** | 25 min | Swap the refined skill with another team. The receiving team runs it on an unseen case and records failures: unnecessary context, missing questions, unsupported certainty, ambiguous actions, scope overreach, or unsafe customer wording. The authoring team fixes the skill instructions, not only the single output. | Give the second case, including a noisy log only after a correlation-linked question. Ask reviewers to identify the first place the skill either opened too much context or reached beyond the evidence. | Review sheet and final skill revision, demonstrated by a second structured brief. |

## Facilitation Notes

- Keep the activity on the skill, not on completing the most cases. A team that improves the workflow after one good and one hard case has succeeded.
- Use roles deliberately: customer/context advocate, evidence challenger, structured-output editor, and reviewer. Rotate after Stage 2 if time allows.
- For a large log, never hand it out as the default next step. Make its release conditional on a correlation ID or a falsifiable question.
- Ask teams to compare two designs in Stage 4: one that opens every artifact and one that stops after decisive evidence. They do not need token counts; they should explain why the narrower design is faster, cheaper, and less likely to be distracted.
- If a model returns `possible` or `insufficient_evidence`, treat that as a valid result when the brief names the exact missing artifact, question, or escalation path. Do not reward a confident guess.

## Organizer Check Before Debrief

Each team should leave with:

- A revised `.github/skills/support-diagnosis/SKILL.md`
- Two structured diagnostic briefs
- One evidence-request trail showing question-first context selection
- One peer review and a skill-level improvement made in response

Use the final 10-15 minutes to ask each team to share one question that changed its investigation and one skill instruction that prevented an unsafe or unsupported outcome.
