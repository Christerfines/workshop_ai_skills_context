# Meet NordicBike: What This Workshop Actually Is

> Text version of [`preparation/01-meet-nordicbike.md`](../01-meet-nordicbike.md) — same content, written as an article instead of a podcast script. Required reading before the session; ~7-minute read.

If you're reading this, you've got a two-hour workshop coming up called the Agent Optimization Challenge, and this is enough to make sure you show up knowing what you're walking into. No slides, no laptop needed yet — just context.

## The company behind the case load

Everything in this workshop hangs off one fictional business: NordicBike AB. Stockholm-based e-bike company, founded 2019, about 85 people. Three product lines — a city e-bike called the Aurora X3, a cargo e-bike called the Fjord Cargo, and a winter-specific e-bike called the Vinter Pro — plus replacement batteries and accessories. The pitch: most e-bikes are designed for mild European climates and then sold, unmodified, into a market with actual winters. NordicBike builds for the winter case first.

Why does that matter for a workshop that's really about building an AI agent? Because it's not a toy dataset. NordicBike sells through an online store and three service centers, and every sale comes with a warranty — and warranties generate exactly the kind of support cases that are genuinely annoying to resolve correctly. A purchase date has to line up with the right coverage window for that specific product. A symptom has to be traced back to an actual root cause, not just pattern-matched. Sometimes the honest answer is "I don't have enough information yet, let me ask." That's realistic support work, and it's exactly the kind of work people are trying to hand to LLM agents right now, often badly.

## The support inbox you'll work from

The workshop hands you ten cases. Real-sounding customers, real-sounding problems — a battery that won't hold charge, a frame issue, a shipping question, that kind of thing. Every fact you need to resolve any one of them is written down somewhere in the repository: the company background, the product specs, the policy documents, the customer's own purchase record, and the case file itself. Nothing is missing, and no outside knowledge of e-bikes or Swedish warranty law is assumed. If you can't find a fact, it's not because it's hidden — it's because you haven't looked in the right file yet.

That last point is worth sitting with, because it's the whole workshop in miniature. You're not graded on whether you know e-bikes. You're graded on how well you handle a pile of information — most of which, for any given case, you don't actually need.

## Why you measure before you touch anything

Which sets up the thing everyone does first and nobody enjoys: the baseline. Before anyone optimizes anything, you run a provided agent — unmodified, on purpose — against the first case. It doesn't try to be smart about what's relevant. It reads every single file in scope and dumps the whole thing into one call to the most capable, most expensive model tier available. You just measure what that costs: token count, model tier, how many calls, whether it even got the right answer.

That "dump everything into one big call" agent isn't a strawman. It's genuinely how a lot of first working prototypes get built in the real world, because it's the fastest way to get something that runs. The point of measuring it isn't to mock it — it's to put a number on the instinct, so you have something concrete to improve against for the rest of the session.

## The four exercises that follow

From there, the workshop is a straight progression. Four more exercises, each building directly on the last — you genuinely cannot skip ahead, because Exercise 4 assumes you've already done the work in Exercise 3. Over those four exercises you'll cut context you don't need, split your one big call into a smaller triage step and a decision step, send each of those steps to a model tier that actually matches how hard the step is, and then run everything through a quality checklist before it counts as done.

Each of those is a real skill, not busywork. Cutting context without cutting correctness is genuinely hard — it's easy to trim blindly and quietly break something. Splitting a pipeline into steps is only a win if what passes between the steps is small and structured, not just the same bloat handed off one call later. Matching model tier to task difficulty is a real cost lever in production systems. And the quality checklist exists because a fast, cheap agent that gets the warranty decision wrong isn't actually a win — it's just a cheap way to be wrong.

The next four episodes (or articles) go deeper on each of those, one per skill — meant to be worked through across the days before the workshop, not all at once. This one is just the map.

## A heads-up about the ten cases

Some of these ten cases are built specifically to be tricky. Not unfair — every fact you need is there — but a few are designed to catch an agent that's been optimized purely for token count and stopped actually reading the record. If you hit every budget target across the exercises and your agent still gets tripped up by one of those cases, that's not bad luck. That's the exercise working as intended.

Speed and cost are easy to optimize for, because they're easy to measure. Correctness under pressure is the harder thing — and it's the thing that actually matters once an agent like this is running against real customers instead of a workshop case load.

## What to actually expect walking in

Two hours, five phases, teams of two to four people sharing one working repository. A short kickoff, then you measure your baseline, then a longer stretch building the two middle versions of your agent, then routing and the quality gate, and it closes with a live leaderboard where every team's scored on the same ten cases — correctness and cost together, not cost alone.

Bring a laptop, make sure you've got access to the model endpoints beforehand if that's been set up for you, and don't stress about domain knowledge — there's no bicycle-trivia component to this. What's being tested is judgment about information: what's relevant, what's structure, what's safe to compress, and what absolutely is not.

One thing to actually take from this before the session: resist the urge to start optimizing before you've measured. It's tempting, especially if you've done this kind of work before, to walk in already trimming context in your head. The first exercise is deliberately not that — it's just look, measure, write the number down. Everything after that is earned against your own number, not somebody else's guess.

## What's next

The next episode/article is the one worth calling required: the rulebook itself — the product line and the full warranty and escalation policy, the same reference material you'll be working from live. Knowing it cold before the session buys you real time in the room. After that comes the engineering side — cutting context, splitting calls, routing by model tier, and the quality gate at the end.
