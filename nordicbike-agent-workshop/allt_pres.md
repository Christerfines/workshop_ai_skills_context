# Agent Optimization Challenge — Plain-Language Deck

*A shorter, simpler companion to `presentation.md`, written for anyone who wants the "explain it to me like I'm new here" version — a first-time facilitator, or an audience that isn't deep into AI-engineering terms. Same course, same five exercises, same numbers — just the plain-English pitch, with full speaker notes under every slide instead of bullets alone. Each note says which of the five hands-on exercises that slide sets up, so you can present this and still land exactly on the real workshop.*

---

## Slide 1 — Today, in one sentence

- You're going to build a small AI helper, then make it faster and cheaper — four times — without ever letting it get things wrong.

**Speaker notes:** Here's the whole two hours in one line: we're going to build an AI assistant, measure exactly what it costs to run, and then improve it — not once, but four separate times, each time in a different way. By the end, your assistant does the same job for a fraction of the cost. The trick, and the whole point of today, is that "cheaper" is easy to fake and "still correct" is the hard part — so we're going to score both. This slide doesn't map to one exercise — it's the promise the other five slides deliver on.

---

## Slide 2 — Meet the company

- NordicBike: a made-up Swedish e-bike company.
- They sell bikes and batteries, and customers write in with warranty questions.
- Real questions, made-up company — no bike knowledge needed, everything you need is handed to you.

**Speaker notes:** NordicBike isn't real, but the situation is — a support inbox with ten real-sounding customer messages: a battery that won't charge, a bike that arrived scratched, a warranty question that's genuinely ambiguous. You don't need to know anything about bicycles. Every fact you'd ever need lives in the files you're given — company background, product specs, the actual warranty rules, and each customer's purchase history. If you can't find a fact, it's not hidden from you, you just haven't opened the right file yet. This is scene-setting for all five exercises — it's the constant backdrop everything else happens against.

---

## Slide 3 — The job: build a support agent

- Your AI helper reads the case, reads the rules, and decides: is this covered or not?
- It has to get that decision right — that's non-negotiable.
- Everything else is about how *expensively* it gets there.

**Speaker notes:** Think of your AI helper as a new support employee who has to make a judgment call — approve the warranty claim, deny it, or say "I need more information first." A good employee gets that right. A good AI helper has to get it right too — that part of the bar never moves. What *does* change, exercise by exercise, is how much it costs — in time and in money — to reach that same right answer. This is the setup for Exercise 1, where you'll build the very first, least clever version of this helper.

---

## Slide 4 — Version 1: the "just dump everything" agent

- The simplest possible approach: hand the AI *every* file, every time, and let it figure it out.
- It works — it gets the right answer.
- But it's expensive: about 19,800 "tokens" (units of text) for one single question.

**Speaker notes:** This is Exercise 1. You're not building anything clever yet — you're running an agent that's deliberately lazy: it doesn't try to guess what's relevant, it just reads everything — the whole rulebook, the whole customer file, everything — every single time. That's a completely normal way to build a first version of anything; speed beats elegance when you're just trying to get something working. Your job in Exercise 1 is just to measure it honestly: how many tokens did that cost, which model did it use, how many separate calls did it make. That number is your own personal baseline — every later exercise is judged against *your* number, not some number on a slide.

---

## Slide 5 — The core idea: don't hand over what you don't need

- Most of what you handed the AI in Version 1 didn't actually matter for that specific question.
- If you only give it the relevant pieces, it costs less — and can still be just as right.
- This is the single biggest idea in the whole workshop.

**Speaker notes:** Here's the "aha" moment the whole course is built around: an AI reading text pays for every word you give it, in both money and time — and most of what a "dump everything" agent reads is dead weight for the specific question in front of it. If you're careful — not lazy, *careful* — about only handing over what's actually needed, you can cut the cost dramatically without losing any accuracy. That's Exercise 2's whole job: take your Version 1 result and cut it down by at least half, while still getting the same right answer. The skill isn't deleting things randomly — it's judging, file by file, section by section, what this specific question actually depends on.

---

## Slide 6 — Version 3: split the job in two

- Instead of one AI doing everything, use two: one that just gathers the facts, one that makes the decision.
- The first one hands the second one a short, tidy list of facts — not a pile of documents.
- This sets up the next big idea: not every step needs the same amount of "brainpower."

**Speaker notes:** This is Exercise 3. Picture two coworkers instead of one: the first skims the case and the file and writes down the key facts on an index card — customer ID, which product, when they bought it, what's wrong. The second coworker only reads that index card, not the original pile of paperwork, and makes the actual decision. The whole skill here is discipline: that index card has to be *facts*, not a summary of facts and definitely not the second coworker's job done for them. A common mistake is writing down a *conclusion* ("this is a manufacturing defect") instead of an *observation* ("no water damage reported") — that's a mistake this exercise is specifically designed to catch you making.

---

## Slide 7 — Version 4: use a cheaper brain where you can

- Not every step of the job needs your smartest, most expensive AI model.
- Reading and jotting down facts is easy — use a cheap, fast model for that.
- Making the actual judgment call is harder — save your stronger model for that part.

**Speaker notes:** This is Exercise 4. Once you've split the work into "gather facts" and "make the decision" in Exercise 3, you can also split *which AI model* does each part. Gathering facts is the easy half of the job — a small, fast, cheap model handles it fine. Making the actual warranty call deserves your better model. This exercise also adds one more move: sometimes, the facts alone tell you this case shouldn't be decided by AI at all — a legal threat, an unusually large money request — and in those cases, you skip straight to a human instead of even bothering the decision-making step. Knowing the difference between "this needs a human" and "I just need one more piece of information from the customer" is the specific trap this exercise sets.

---

## Slide 8 — Before you celebrate: check your work

- Cheaper and faster is only a win if it's still *right*.
- Version 4 gets checked against a strict 6-point checklist, on all 10 customer cases — not just the one you've been practicing on.
- This is where shortcuts get caught.

**Speaker notes:** This is Exercise 5, and it's the reality check for everything before it. Up to now you've mostly been working against one clean, friendly case. This exercise runs your finished agent against all ten real cases — several of which are deliberately built to expose an agent that got fast by getting sloppy. You'll run a strict yes/no checklist against every single response: did it cite the right rule, did it correctly recognize when to escalate, did it ask a question instead of guessing when information was missing. If you find a problem, the fix has to be a *real* fix — tightening your instructions — not just quietly feeding it more text again, since that undoes all the savings you just earned.

---

## Slide 9 — A few of the ten cases are traps, on purpose

- Some cases look almost identical to each other but have opposite right answers.
- Some cases are missing a fact on purpose — the right move is to ask, not guess.
- Some cases hide a legal or money request inside an otherwise normal message.

**Speaker notes:** Not all ten cases are meant to be easy, and that's deliberate — a workshop where every case is straightforward doesn't actually test whether your agent is *thinking* or just pattern-matching. A couple of cases look the same on the surface but come out differently underneath, once you look at *why* something happened, not just *what* happened. A couple are missing a fact on purpose, and the honest, correct move is a clarifying question, not a confident guess. And at least one case is a normal-sounding request with a legal or compensation demand tucked inside it, which should get escalated to a human rather than resolved by the AI. This is the payoff of Exercise 5 specifically — these are exactly the cases that separate an agent that's actually careful from one that just got lucky on the easy case.

---

## Slide 10 — How you're actually scored

- Two things count: did you get it right, and did you do it cheaply.
- Getting it right matters *more* than twice as much as being cheap.
- One serious mistake (like missing a legal threat) costs you more than any amount of cost-cutting can make up for.

**Speaker notes:** The scoring is built to reward exactly the balance this whole course teaches. Correctness carries about seventy percent of your score, cost-efficiency about thirty — so a fast, cheap, wrong answer never beats a correct one. On top of that, there's a penalty for a genuinely serious mistake — the kind covered in Exercise 5's checklist — big enough that no amount of clever cost-cutting can buy it back. The message underneath the formula is simple: this whole exercise is trying to make you *feel* what happens in a real product, where a cheap-but-wrong AI decision is a much bigger problem than a slightly expensive one.

---

## Slide 11 — The finish line

- Every team worked the same ten cases, so scores are genuinely comparable.
- Scores get revealed together, all at once, at the end.
- Then we talk about what actually made the difference between teams.

**Speaker notes:** By the end of Exercise 5, every team has a finished agent and a full set of scored results, and because everyone worked the exact same ten cases, the leaderboard is a fair comparison, not a guess. We reveal every team's score together rather than one at a time, so nobody's watching their result land under pressure. The most useful part of this last stretch isn't actually the ranking — it's the debrief, where we talk through *why* certain teams handled the tricky cases well and others didn't, since that's the part people actually remember afterward.

---

## Slide 12 — What you actually take away

- This isn't really about bicycles.
- It's about a habit: don't hand an AI more than it needs, don't skip the "did I get it right" check, and know which parts of a task need a strong model and which don't.
- Those three habits apply to *any* AI system you build after today.

**Speaker notes:** Close on the thing that actually matters beyond today: NordicBike and its warranty cases are a made-up practice ground, but the four moves you just practiced — trimming what you hand an AI, splitting a job into a clean handoff between steps, matching model strength to task difficulty, and never skipping a real correctness check — are exactly the habits that separate a cheap AI demo from something you could actually trust in production. That's the whole reason this course exists, and it's the one line worth leaving people with as they head out.
