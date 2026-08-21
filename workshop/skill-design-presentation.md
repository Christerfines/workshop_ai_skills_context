---
title: "Designing Agent Skills in GitHub Copilot"
subtitle: "NordicBike AB — Support-Diagnosis Skill Workshop"
date: "2026-08-21"
---

## Page 1 of 4 — Know What You're Actually Authoring

- An agent skill is not a single prompt — it's a standing set of instructions the agent (Copilot Chat / agent mode) reloads every time its trigger matches, session after session.
- In GitHub Copilot the reusable, invocable unit is a **Prompt File** (`.github/prompts/*.prompt.md`): fixed YAML frontmatter (`mode: agent`, `description`, optionally `tools`/`model`), invoked by filename as a slash command in Copilot Chat. There is no separate `name` field — the filename *is* the identity.
- This repo also uses a shared, whole-repository **`SKILL.md`** convention (`name` + `description` frontmatter) for the main exercise skill, `support-diagnosis` — same authoring discipline, different host file.
- The frontmatter `description` is what decides *when* the skill loads. Write it as a trigger condition — *"Use when investigating a NordicBike support report to establish root cause"* — not a feature summary. A vague description means the skill fires at the wrong time, or never.
- Treat the skill file as versioned source code: it lives in the repo, gets reviewed, and improves across sessions — not a one-off chat instruction that vanishes when the window closes.

::: notes
Before we open the skill file, let's be clear about what we're actually building. This isn't a prompt you type once — it's a standing instruction set the agent reloads every time its trigger matches. In GitHub Copilot, that's a Prompt File: a markdown file with fixed frontmatter — mode, description, optionally tools and model — invoked by its filename as a slash command in Copilot Chat. Our repo also uses a SKILL.md convention with name and description frontmatter for the shared support-diagnosis skill we'll all build on today. The single most important field is description — that's what the agent matches against to decide when to load the skill, so write it as a trigger condition, not a feature list. And treat this file like source code: it lives in the repo, gets reviewed, and gets better across sessions — it's not disposable chat instructions.
:::

## Page 2 of 4 — Scope It to One Job

- Give the skill exactly one job and say so in writing. `support-diagnosis` diagnoses — it does **not** fix code, promise a resolution date, authorise compensation, or draft the customer reply.
- Put the boundary in two places: the frontmatter `description` *and* a dedicated `## Boundaries` section in the body. The model needs the "does not" list as much as the "does" list.
- Prefer several narrow skills over one that does everything — this repo's `.github/prompts/` scaffolds (`warranty-triage`, `policy-lookup`, `escalation-router`) split extraction, lookup, and routing into separate, individually reviewable units.
- Narrow scope is what makes the `description` trigger reliable. A skill that "does everything" has a description too vague to match correctly and too broad to review.
- Narrow scope is also what makes evaluation possible at all — you cannot cross-team review a skill against a clear pass/fail bar if its job was never pinned down.

::: notes
Scope is the next decision, and it's the one teams get wrong first. Give the skill exactly one job. Ours diagnoses — it does not fix code, promise a date, authorise compensation, or write the customer reply. Say that twice: once in the description, once in a dedicated Boundaries section, because the model needs the 'does not' list as much as the 'does' list. Notice the prompts folder already models this — warranty-triage, policy-lookup, and escalation-router are three separate narrow files, not one skill trying to do everything. Narrow scope isn't just tidiness — it's what makes the description trigger reliably, and it's what makes today's cross-team review possible, because you can't grade a skill against a job that was never pinned down.
:::

## Page 3 of 4 — Engineer the Context It Sees, Not Just the Prompt

- Most agent failures are context failures, not model failures. The skill's real job is assembling the right evidence at the right moment — not being clever with wording.
- Four levers, all present in `support-diagnosis`: **write** (evidence log and reasoning go in output fields, not inline narration), **select** (open the API response before the source file), **compress** (excerpt three log lines, not the thousand-line file), **isolate** (separate "what happened" reasoning from "what to do next").
- Require a named question before any artifact is opened: what question does this evidence answer, what signal is expected, what would a negative result mean. Instructions that allow speculative context-opening invite context distraction and context poisoning.
- Every extra token opened costs latency and money, and dilutes signal. A skill scoped to what a named question requires is cheaper *and* more accurate than one that opens everything "to be safe."
- Write the four context failure modes into the skill's own review checklist: **poisoning** (a wrong fact treated as ground truth), **distraction** (signal buried in volume), **confusion** (off-topic material shifts the conclusion), **clash** (two sources disagree and the model silently picks one).

::: notes
Here's the part that actually determines whether your diagnosis is any good: context engineering. Most agent failures are context failures, not model failures. Four levers to use deliberately — write your evidence log into output fields instead of narrating it inline, select the API response before you reach for source code, compress a thousand-line log down to the three lines that matter, and isolate 'what happened' reasoning from 'what to do next' generation. The discipline underneath all four is the same: require a named question before you open any artifact. What question does this answer, what signal do you expect, what would a negative result tell you. Skip that discipline and you get the four failure modes on this slide — poisoning, distraction, confusion, and clash — and every one of them produces a confidently wrong brief.
:::

## Page 4 of 4 — Make the Output Structured, Bounded, and Honest

- Specify output as a schema, not prose. A structured brief lets each consumer role — developer, QA, product owner, support agent — read exactly the field it needs without reading the whole document.
- Require a confidence level with rules attached: `confirmed` only with two independent agreeing sources, `insufficient_evidence` whenever a guess would otherwise be produced. Don't let the skill hedge in prose instead of picking a level.
- Force explicit `alternatives_ruled_out` and `remaining_uncertainty` fields. A skill that never names a competing hypothesis is pattern-matching, not diagnosing.
- Bake enforcement into the skill itself: it should reject its own draft if a scope boundary is missing, if `confirmed` appears with a single source, or if customer-facing text leaks technical language or a promise.
- Iterate the skill *file* after every failure, not just the one bad output. When review finds a gap, the fix belongs in the instructions so the next run doesn't repeat it.

::: notes
Last piece: what comes out the other end. Specify it as a schema, not prose, so a developer, a QA engineer, and a product owner can each read one field and know what to do without reading the whole document. Confidence levels need rules attached — confirmed only with two independent sources, insufficient_evidence whenever you'd otherwise be guessing. Don't let the skill hedge in paragraphs instead of picking a level. Push for alternatives_ruled_out and remaining_uncertainty every time — a brief with no competing hypothesis is pattern-matching, not diagnosis. And when review finds a gap this afternoon, fix it in the skill file, not just in the one output that was wrong, so the next run doesn't make the same mistake.
:::

## Meet NordicBike AB

- Founded 2019 in Stockholm, by engineers and cyclists frustrated with mild-climate e-bikes sold, unmodified, into the Nordic market.
- HQ in Hammarby Sjöstad, Stockholm, since founding; ~85 employees across product engineering, customer support, logistics, and retail partnerships — the whole operational footprint stays inside Sweden.
- Sells direct-to-consumer online at nordicbike.se; no owned retail storefronts. Physical service runs through three partner centers — Stockholm (co-located with HQ), Gothenburg, Malmö — any of which can handle warranty work regardless of where the bike was bought.
- Product line is deliberately split by use case, not one do-everything bike: **Aurora X3** (city), **Fjord Cargo** (cargo), **Vinter Pro** (winter), plus **PowerPack** batteries and a small accessories catalog.
- Mission: make electric mobility the practical everyday choice across the Nordics — engineered for Nordic winters, built to last.
- Support quality and warranty clarity are treated internally as part of the product, not an afterthought — which is exactly why this workshop is built around a support-diagnosis skill.

::: notes
Quick grounding in who we're doing this for. NordicBike is a Stockholm e-bike company, founded 2019, about 85 people, still entirely Swedish in its footprint. They sell direct online and support customers through three partner centers — Stockholm, Gothenburg, and Malmö — any of which can handle warranty work regardless of where the bike was bought. Three model lines plus batteries and accessories, each engineered around Nordic winters rather than one do-everything bike. The one line worth remembering for today: NordicBike treats support quality and warranty clarity as part of the product, not an afterthought — which is exactly why we're spending two hours building a diagnosis skill instead of a marketing chatbot.
:::

## About This Repository

- A self-contained workshop repo: a working NordicBike support portal (`src/`) plus everything needed to run the skill-building session (`workshop/`) and its GitHub Copilot scaffolding (`.github/`).
- **`src/`** — the portal itself (.NET 10, deterministic in-memory data): customer, support-agent, support-lead, and business-operations views; JSON APIs under `/api`; correlation IDs and a Diagnostics page as real evidence sources; eight seeded faults from simple UI/API mismatches through authorisation and checkout-idempotency bugs.
- **`.github/skills/support-diagnosis/SKILL.md`** — the shared, intentionally-incomplete starter skill every team builds on. **`.github/prompts/*.prompt.md`** — GitHub Copilot prompt-file scaffolds (`mode: agent`) for narrower, subagent-style helpers.
- **`workshop/`** — facilitator and participant material: the company knowledge base (`company/`), a ten-case support pack (`support-cases/`), the diagnostic brief schema, the workshop plan and agenda, learning outcomes, and the evaluation rubric.
- **`_archive/`** — an earlier, larger iteration of this workshop (a tiered, leaderboard-based agent-optimization format); kept for reference, not part of the current session.
- Everything here is deliberately unfinished by design: the repo ships evidence and structure, not answers — participants author the judgment.

::: notes
And finally, orientation to the repo itself. Src is a real, working portal — customer, support-agent, support-lead, and business-operations views, JSON APIs, correlation IDs, a Diagnostics page, and eight seeded faults ranging from simple UI mismatches to authorisation bugs. That's your evidence source all afternoon. The skill you'll edit lives at dot-github slash skills slash support-diagnosis slash SKILL dot md, deliberately incomplete. Workshop has your case pack, the brief schema, the agenda, and the rubric. Archive is an earlier, bigger version of this workshop — ignore it unless you're curious. One closing point: everything here is unfinished on purpose. We shipped evidence and structure, not answers. The judgment is what you're here to build.
:::
