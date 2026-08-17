# Preparation — Pre-Workshop Podcast Series

Seven short pieces meant to be consumed before the Agent Optimization
Challenge, so participants arrive with shared vocabulary, domain knowledge,
and mental models rather than meeting everything cold in the room. Each
topic comes in **two formats covering identical content** — listen or
read, participants' choice:

- **Podcast** — the `.md` files at this level are the podcast *manus*
  (scripts), rendered to audio in [`../pre_read_audio/`](../pre_read_audio/).
  Three-voice dialogue; see "The cast" below.
- **Article** — [`articles/`](articles/) has one plain-prose write-up per
  topic, same numbering, same information, no dialogue framing. For anyone
  who'd rather read than listen, or wants to skim/search instead of playing
  a 5–10 minute file.

Point participants at both options in the pre-workshop email — "listen to
these, or read the text versions, whichever you'd rather" — rather than
defaulting to just one.

## The cast

Three recurring voices carry every episode — the same named cast (and the
same Kokoro voice IDs) as the author's other local-podcast project, *The AI
Reality Check*:

| Persona | Plays the role of... | Lens |
|---|---|---|
| **Mira Chen** | Facilitator | moderates, keeps pace, asks the question a participant would ask |
| **Marcus Webb** | Expert | the practitioner view — real-world stakes, production experience, common failure modes |
| **Professor Iris Vance** | Teacher | breaks the mechanism down, uses analogies, checks understanding |

Every episode opens with a short spoken disclosure, in Mira Chen's voice,
that the voices, script, and analysis are AI-generated — the same practice
and near-identical wording as *The AI Reality Check*.

## The finished audio

The rendered MP3s live in [`../pre_read_audio/`](../pre_read_audio/), one
file per episode below, and **are committed to this repo** — they're what
actually gets emailed to participants. See
`facilitator-guide.md`'s opening section for the send-out instructions.

## Generating the audio

The generator is **a separate project, deliberately not part of this repo**
(`~/proj/workshop_pre_read_audio` — it pulls in a real ML dependency stack
that has no business in a repo handed to participants). It reads the manus
files here and writes finished MP3s straight into
[`../pre_read_audio/`](../pre_read_audio/):

```bash
cd ~/proj/workshop_pre_read_audio
.venv/bin/python generate_podcast.py --all
# then, back in this repo: review + commit pre_read_audio/
```

Entirely local — Kokoro-82M neural TTS via `mlx-audio`, no cloud service, no
API key. Full details, engine fallbacks, and how it works in that project's
own `README.md`.

## Episode order

Each line links both formats — 🎧 the podcast manus, 📄 the article.

0. 🎧 [`00-llm-basics-optional.md`](00-llm-basics-optional.md) · 📄 [`articles/00-llm-basics-optional.md`](articles/00-llm-basics-optional.md) — **optional.** A vocabulary primer (model call, token, context, model tier, JSON/typed payload) for anyone attending who hasn't personally written code that calls a model API before. Skip it if the workshop's stated prerequisite — "comfortable constructing a prompt from multiple source documents, sending it to a model, and reading back a structured response" — already describes you. Send this link specifically to registrants you know are less LLM-experienced, rather than broadcasting it to everyone.
1. 🎧 [`01-meet-nordicbike.md`](01-meet-nordicbike.md) · 📄 [`articles/01-meet-nordicbike.md`](articles/01-meet-nordicbike.md) — the case, the format, what's expected of you. Required; start here if you skipped Episode 0.
2. 🎧 [`02-the-domain.md`](02-the-domain.md) · 📄 [`articles/02-the-domain.md`](articles/02-the-domain.md) — a repository map, not a content walkthrough: what each of the 5 top-level folders (`company/`, `products/`, `policies/`, `customers/`, `cases/`) holds and what kind of question it answers, so you're not discovering the folder structure live. Deliberately stops short of what any document actually says — reading the real content, and judging what's relevant to a case, is Exercise 2's job.
3. 🎧 [`03-context-engineering.md`](03-context-engineering.md) · 📄 [`articles/03-context-engineering.md`](articles/03-context-engineering.md) — why the naive baseline is so expensive, and how to cut context without cutting correctness.
4. 🎧 [`04-subagent-handoff.md`](04-subagent-handoff.md) · 📄 [`articles/04-subagent-handoff.md`](articles/04-subagent-handoff.md) — splitting one call into a triage/resolver pair, and why a typed payload isn't the same as a smaller dump.
5. 🎧 [`05-model-routing.md`](05-model-routing.md) · 📄 [`articles/05-model-routing.md`](articles/05-model-routing.md) — matching model tier to task difficulty, and when a pipeline should short-circuit to a human instead of a resolver call.
6. 🎧 [`06-evaluation-quality-gates.md`](06-evaluation-quality-gates.md) · 📄 [`articles/06-evaluation-quality-gates.md`](articles/06-evaluation-quality-gates.md) — why cost savings can't buy back correctness, and how the leaderboard actually scores you.

## Who needs Episode 0

Registration for this workshop draws a mixed crowd — comfortable-with-LLM-APIs engineers alongside people who use AI tools daily but have never called a model from code. Episode 0 exists for the second group. It's deliberately narrow: five terms, no NordicBike content, nothing that overlaps with any other episode, so listening to it costs nothing for someone who already knows the material and skips it. If you know your registrant list well enough to tell who's who, it's worth pointing Episode 0 at specific people rather than presenting it as episode 1 of 7 to everyone — treating it as mandatory for an already-fluent audience reads as condescending, and treating it as nonexistent for someone who needed it means they're decoding "token" and "tier" for the first time while also trying to follow Episode 2's warranty policy.

## What these are (and aren't)

The guiding line for this series: knowing the **vocabulary** and the
**repository map** in advance is preparation, knowing what the **documents
actually say** — let alone the **exercise answers** — in advance is
spoiling. Episode 0 covers the vocabulary; Episode 2 covers the map: which
of the 5 top-level folders holds which kind of fact, so nobody's discovering
the folder structure live. It deliberately does not walk through what any
policy document says — no terms, no numbers, no categories — because that
content, and the judgment about which parts of it apply to a given case, is
exactly what Exercise 2 (context reduction) and the Phase 5 leaderboard are
built to test. Neither episode walks through any of the ten cases, and
neither states or implies which case needs what.

Everything else used across the series — token counts, the JSON handoff
payload shapes, the quality-gate checklist items — is likewise drawn from
material participants already see in `README.md` and the `workshops/`
exercise files. None of these scripts touch `evaluation/expected-results.md`
or `evaluation/adversarial-cases.md`, and none of them reveal which specific
cases contain an escalation trigger, a missing-information gap, or an
adversarial archetype — working that out live, against the rulebook, is the
exercise itself. Safe to hand to participants any time before the session.
