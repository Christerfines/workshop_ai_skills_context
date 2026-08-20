# Evaluation: Why Cheap and Wrong Still Loses

> Text version of [`preparation/06-evaluation-quality-gates.md`](../06-evaluation-quality-gates.md) — same content, written as an article instead of a podcast script. Preps Exercise 5 (quality gate) and the Phase 5 leaderboard. Uses only rubric categories, checklist items, and scoring shape already published in the participant-facing scoring rubric. ~5-minute read.

The last article left you with a routed, short-circuiting, two-model pipeline hitting a small fraction of the original token count. This one is about the step that decides whether any of that actually counts.

## Why this gets its own exercise

Everything built so far was tuned and measured against a single case — the same one from the very first baseline measurement. An approach that works cleanly on one case does not automatically generalize to nine others, some of which are built specifically to expose shortcuts that a single clean case would never surface. This is the exercise where you finally run against all ten — for the first time in the workshop. And that matters, because it's genuinely possible to have done every prior exercise well — hit every token target, built a clean typed handoff, routed correctly — and still discover here that your pipeline has a gap that only shows up on cases you hadn't been testing against.

This is really the thesis of the whole workshop, stated as plainly as it gets: **a fast, cheap agent that gets the decision wrong isn't a win. It's just a cheap way to be wrong.** Everything from context trimming through model routing has been in service of cost — this exercise is where correctness gets checked, deliberately, as a separate concern that cost savings cannot buy its way out of.

## How it's actually checked: two independent layers

**The quality gate** — six specific pass-or-fail checks applied to every case output:

1. Does it cite the specific policy section the decision actually rests on?
2. Does it state the outcome explicitly, with a one-sentence justification tied to the actual root cause rather than just the surface symptom?
3. Does it confirm the purchase date and product identity from the record before deciding, instead of assuming anything?
4. Does it correctly flag and escalate anything that matches an escalation trigger, rather than resolving it directly?
5. If information needed for the decision is missing, does it ask a clarifying question instead of guessing?
6. Is the tone professional, empathetic, concise, and in the language the customer's message is primarily written in?

If even one of those six fails, that case isn't eligible for submission until it's fixed. Not partially credited — genuinely blocked.

And here's the sharpest constraint in this whole exercise, worth knowing before you're in the room facing it: fixing a quality-gate failure is not allowed to blow your token budget back up past what you measured in the previous exercise. It's tempting, when a case fails, to just throw more context at it — paste in more of the policy document, more of the customer record, hope the extra material papers over whatever went wrong. That's exactly the trade-off the entire workshop has been building toward avoiding. A genuine fix tightens the triage extraction, or adjusts the resolver's instructions, or corrects a routing rule — it doesn't require a bigger prompt than the one you already built.

**A more granular per-case rubric** sits alongside the gate — twenty points per case across five categories, four points each: whether the eligibility decision itself is correct, whether the reasoning is actually grounded in root cause rather than surface symptom, whether the policy citation is specific and accurate, whether escalation and scope judgment are handled correctly, and clarity of tone. A case only counts as a genuine pass if it clears a solid threshold on that rubric *and* passes all six gate items — neither check alone is sufficient on its own.

## How this feeds the leaderboard

The shape of the scoring formula is worth knowing in advance, because it changes how you should think about trade-offs during the whole session, not just at the end. Correctness — how many of your ten cases genuinely pass — is weighted more heavily than cost efficiency. Cost efficiency contributes, but it's the smaller share. And there's a real penalty on top of both for a case that trips a critical failure — enough that a single bad miss can undo most of what you gained from optimizing cost aggressively.

Put concretely: a team that games the token budget by skipping root-cause checks or cutting corners on escalation judgment loses more from that penalty and from reduced correctness than they gain from being cheap. The formula is built so that the fast, cheap, wrong path is not the winning path, even on paper.

## The mindset for this last stretch

Treat the quality gate the way you'd treat a real pre-launch check on a support agent you were about to point at actual customers, not as a formality between you and the leaderboard. If you find a failure, resist the temptation to patch just the one visible output — fix the underlying instruction or logic, because a fix that only touches one case's output tends to resurface on a similar case somewhere else you haven't checked yet.

And go in expecting some of the ten cases to be more demanding than the one you've been building against all session. That's by design, not bad luck — the case load is built to reward an agent that's actually reading the record in front of it, at whatever tier and whatever token budget, over one that's learned to look right on a single familiar example.

## The full arc

Measure honestly, cut context with judgment, split calls with a clean typed handoff, route by task difficulty, and check correctness as its own non-negotiable layer at the end. Five topics, five habits — see you in the room.
