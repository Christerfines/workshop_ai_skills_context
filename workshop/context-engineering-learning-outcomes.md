# Context Engineering — Learning Outcomes

This document defines what participants should be able to understand, judge, and do by the end of the session. It is intended for facilitators planning exercises and for participants reviewing what they are trying to build.

---

## What Context Engineering Is

Context engineering is the discipline of designing dynamic systems that give an LLM exactly the right information, in the right format, at the right time — so that the task it is asked to perform is plausibly solvable. It is distinct from writing a single prompt.

> "Context engineering is the delicate art and science of filling the context window with just the right information for the next step."
> — Andrej Karpathy

A context window is the model's working memory. Everything the model sees before it generates a response is context: the system instructions, the user message, conversation history, retrieved documents, tool definitions, and the output schema. The quality of that assembled window — not the sophistication of the model — is what separates a useful agent from an unhelpful one. **Most agent failures are context failures.**

---

## The Four Strategies

Research and practice group context engineering decisions into four strategies. Every choice made while authoring a diagnostic skill falls into one of these.

| Strategy | What it means | Diagnostic example |
|---|---|---|
| **Write** | Save information outside the context window so the skill can refer to it later | Record the investigation scratchpad and evidence log as output fields, not inline narration |
| **Select** | Pull only the relevant artifact into the window at each step | Open the API response first; only open the source file if the response is ambiguous |
| **Compress** | Retain only the tokens needed for the next decision | Excerpt the three relevant log lines, not the thousand-line file |
| **Isolate** | Separate concerns so each step has a focused context | Separate "what happened" reasoning from "what to do next" generation |

---

## Learning Outcomes

### 1 — Understand context as a system, not a string

Participants will be able to list the components of a context window (instructions, user input, history, retrieved knowledge, tools, output schema) and explain why assembling them correctly matters more than choosing a more powerful model.

**Practical test:** Given two skill invocations on the same case — one that dumps the full log file and one that excerpts three lines — explain which will produce a more reliable diagnosis and why, without running either.

---

### 2 — Apply question-first evidence selection

Participants will be able to write a ranked investigation question before opening any evidence artifact, stating the expected signal and what a negative result would tell them.

The five-question ladder:

1. What happened, to whom, and when?
2. What path or state differs from what was expected?
3. Which identifier or correlation links the report to an artifact?
4. Which single artifact would most reduce uncertainty right now?
5. What observation would disprove the leading hypothesis?

**Practical test:** Given a one-paragraph support report, write all five questions before looking at any evidence. Rank them. Open only the artifact that answers question 4. If it resolves the case, stop. If not, explain which question still requires an answer and which artifact answers it.

---

### 3 — Diagnose context failure modes

Participants will be able to identify the four ways too much or wrong context degrades a diagnosis:

- **Context poisoning** — a hallucinated or incorrect fact is included and the model treats it as ground truth
- **Context distraction** — the volume of irrelevant material buries the signal
- **Context confusion** — off-topic artifacts shift the model toward the wrong conclusion
- **Context clash** — two included sources contradict each other; the model picks one silently

**Practical test:** Read a peer team's diagnostic brief and identify which failure mode, if any, the evidence selection produced. Name the specific artifact and explain which failure it caused.

---

### 4 — Form and separate competing hypotheses

Participants will be able to state at least two competing hypotheses for any support report and identify the minimum evidence needed to separate them. They will write a brief that names ruled-out alternatives, not just the accepted root cause.

**Practical test:** For a case with an ambiguous symptom (e.g. "brakes feel off"), state two policy-distinct hypotheses and name one artifact that would confirm each. Do not merge them into a single vague explanation.

---

### 5 — Define explicit stop conditions and recognise context cost

Participants will be able to state in the skill body when the investigation is complete: what evidence is sufficient, what confidence level is required before the brief can be finalised, and when the honest output is `insufficient_evidence` rather than a guess.

Context has a cost in both directions. Every artifact opened adds tokens to the request; more tokens means higher latency and higher cost. The question-first discipline in outcome 2 is also a cost-management discipline: a skill that opens only the API response for a straightforward case uses a fraction of the context a skill that dumps the full source repository uses. Participants do not need to count tokens manually, but they should be able to describe which of two skill designs is cheaper to run and why.

**Practical test:** Modify a skill that currently over-investigates (opens source code for every case) to stop at API evidence when that evidence is unambiguous. Confirm the diagnosis does not change and describe why the revised skill is cheaper to run.

---

### 7 — Design structured, actionable skill output

Participants will be able to produce a diagnostic brief whose fields directly map to the decisions that each consumer role needs to make. The brief must be structured data, not prose. Every claim must reference the evidence that supports it. See [diagnostic-brief-schema.md](./diagnostic-brief-schema.md) for the full output contract.

**Practical test:** Read another team's brief and answer: Which field tells a developer what to fix? Which field tells QA what to test? Which field tells a product owner whether this blocks a release? If any answer is "you have to read the whole brief to find out," the structure needs revision.

---

### 8 — Express confidence and uncertainty precisely

Participants will be able to assign one of four confidence levels to a root cause determination and write a brief that makes uncertainty visible rather than burying it in hedged prose.

| Level | Meaning |
|---|---|
| `confirmed` | At least two independent evidence sources agree; no alternative hypothesis remains |
| `likely` | Primary evidence supports the conclusion; one alternative not yet ruled out |
| `possible` | Evidence is consistent with the conclusion but also consistent with at least one alternative |
| `insufficient_evidence` | A clarifying question or additional artifact is required before any determination can be made |

**Practical test:** Given a case with one supporting artifact and no ruling-out artifact, the brief must use `likely`, not `confirmed`. A brief that uses `confirmed` without two independent sources fails this outcome.

---

### 9 — Enforce scope boundaries in the skill

Participants will be able to write skill instructions that prevent the model from: proposing a code fix, promising a resolution timeline, authorising compensation, or resolving an out-of-scope escalation trigger. The brief's scope boundary field must state explicitly what the brief does not address.

**Practical test:** Run the skill on a case that includes a compensation demand. Confirm the brief contains a scope_boundary entry for the compensation demand and a next_action entry assigning it to an appropriate role, rather than resolving it inline.

---



## Role-Specific Notes

These outcomes apply across all roles. The emphasis differs by background.

| Role | Primary emphasis |
|---|---|
| Developer | Outcomes 2, 3, 5, 6 — evidence selection, context failure modes, stop conditions, model routing |
| QA | Outcomes 4, 10 — competing hypotheses, audit criteria |
| Business analyst / requirements | Outcomes 1, 7, 9 — what context is, brief structure, scope boundaries |
| Product owner | Outcomes 7, 8, 9 — actionable fields, confidence levels, scope |
| Architect | Outcomes 3, 5, 6, 9 — failure modes, stopping conditions, model routing, systemic risk boundaries |

No role is expected to understand all outcomes at equal depth in a two-hour session. The cross-team review (outcome 9) is the moment when these perspectives combine — a QA participant spotting context distraction while a product owner spots an unsafe promise in customer communication.

---

## What These Outcomes Are Not

Participants do not need to:
- Count tokens manually or configure model API parameters
- Know how to build or wire a multi-agent orchestration pipeline in code
- Write code or use a specific IDE
- Have prior experience with any specific LLM API

The conceptual awareness in outcomes 5 and 6 — that context has a cost, and that a skill can signal when a more capable model is needed — is sufficient for this session. Implementation detail is not.
