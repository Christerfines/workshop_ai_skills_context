# Support Diagnosis Skill Workshop Plan

## Outcome and Scope

Run the 70 participants in teams of four or five (about 14-18 teams), deliberately mixing developers, QA, business/requirements analysts, product owners, and architects. Each team authors and exercises one shared-repository `support-diagnosis` skill. The skill investigates a support report, establishes an evidence-backed likely root cause, records uncertainty, and prepares customer-ready facts in NordicBike's public voice. It does **not** change code, promise a fix, authorize compensation, or send the final customer response.

Use a workspace skill at `.github/skills/support-diagnosis/SKILL.md` for this session. Give participants a small starter template, but make the question process, evidence order, stopping conditions, and output contract their own work.

## Repository Assessment

The portal is a good fit. It has eight seeded faults from easy UI/API mismatches through authorization, internal-note leakage, and checkout idempotency. Its correlation IDs, audit events, Diagnostics page, API responses, and reproducible portal workflows offer realistic evidence sources. For example, `portal.js` redirects a successful checkout to `/orders/order-{id}` even though the API returns the canonical order ID.

Do not use the current Support Issues records directly as participant cases. Their `Change`, `Investigation`, and `Acceptance` fields reveal the intended diagnosis. Create a participant case pack that contains only reports and evidence; retain the diagnosis, discriminating evidence, and scoring notes in facilitator-only material. A few cases should contain small reports, while others include a large noisy log extract, screenshots/network captures, and irrelevant correlation IDs.

## Skill Contract

The skill should: classify the report; state what is known and unknown; ask for the smallest next piece of evidence; correlate UI, API, audit, and source evidence; form and test competing hypotheses; and stop at a bounded diagnosis. Its output is a short diagnostic brief with: customer impact, likely root cause and confidence, cited evidence, ruled-out alternatives, remaining uncertainty, next internal action, and a customer-language fact summary. Require evidence references rather than unsupported claims, and a clear `insufficient evidence` result where appropriate.

Teach a question ladder as the shared context-engineering method: (1) What happened, to whom, and when? (2) What changed or differs from the expected path? (3) Which identifier or correlation can connect the report to evidence? (4) What single artifact would most reduce uncertainty? (5) What observation would disprove the leading hypothesis? Add this rule to the skill: request or open evidence only when it answers a named question; record why any large log, source file, or screenshot was needed.

## Two-Hour Agenda

| Time | Activity | Team artifact |
|---|---|---|
| 00:00-00:10 | Frame diagnosis versus resolution. Demo one portal flow, correlation ID, and the expected brief. | Shared definition of done. |
| 00:10-00:25 | Individual then team drill: read a short case, write three questions before opening evidence, and rank them by information gained. Reveal one API response only after a team names the question it answers. | Ranked question ladder. |
| 00:25-00:40 | Author `SKILL.md`: triggers, boundaries, question ladder, evidence order, stop conditions, and output schema. Assign rotating roles: customer/context, evidence challenger, and skill editor. | First runnable skill. |
| 00:40-01:00 | Run it on an easy/moderate fixture such as checkout redirect, stale cart total, or Malmo service request. Work individually for five minutes, compare investigations, then refine the workflow rather than a case-specific answer. | Diagnostic brief, evidence-request log, skill revision. |
| 01:00-01:25 | Run a hard noisy fixture: status projection, escalation audit gap, internal-note leakage, or duplicate checkout. Give the large log only when teams state a falsifiable question and the correlation they will trace. | Evidence-backed diagnosis with confidence and a disproof test. |
| 01:25-01:45 | Cross-team swap: a new team uses the skill on an unseen fixture. They mark vague questions, unnecessary context, unsupported leaps, and unsafe resolution language. | Review notes and final revision. |
| 01:45-02:00 | Share one question that changed the investigation and one uncertainty retained. Debrief how the skill made context selection repeatable. | Shared-repository `SKILL.md` and two briefs. |

## Case Pack and Facilitation

Prepare six cases, not all eight: two easy, two moderate, and two hard. Give each a customer/support report plus tiered evidence folders: `initial/`, `on-request/`, and optionally `large-log/`. The last folder should contain a deliberately noisy, generated thousand-line log with a small number of relevant correlation-linked events. Add an evidence-request card to every case: `question`, `expected signal`, `why this is the next best artifact`, and `what would change our mind`. Limit each team to two cases during the session so skill design, not case completion volume, remains the learning goal.

Facilitators keep an answer key with root cause, minimum decisive evidence, acceptable alternative hypotheses, and red flags. Review using six checks: correct scope, question quality, evidence traceability, competing-hypothesis handling, honest uncertainty, and public-facing language that explains impact without diagnosing beyond the evidence. No leaderboard or formal submission is needed; the cross-team run is the practical test.

## Decisions Before Build

Confirm NordicBike's external response language for the exercise (the portal is English, while the fictional company is Swedish). Also decide how much source access each role receives: equal access keeps the activity simple, while staged source access makes the evidence-request discipline more visible.