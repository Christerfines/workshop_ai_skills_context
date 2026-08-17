# Model Routing: Not Every Call Deserves Your Best Model

> Text version of [`preparation/05-model-routing.md`](../05-model-routing.md) — same content, written as an article instead of a podcast script. Preps Exercise 4 (model routing and escalation short-circuiting). Deliberately does not name which of the ten cases short-circuit — that's for you to discover live. ~5-minute read.

By the end of the last article you had two calls — a triage subagent extracting facts, a resolver subagent making the decision — both still running on the same model tier. This one is about why that's about to change.

## The question underneath the exercise

Is extracting structured facts from a case file actually the same difficulty of task as weighing those facts against policy and reaching a defensible judgment? Most people, asked directly, will say no. And yet it's extremely common in real systems to see every call in a pipeline running on the same top-tier model, regardless of what that particular call is actually doing.

Think about how you'd staff this if it were a human team instead of an AI pipeline. Triage — pulling out the purchase date, the product, the serial number, what the customer said their symptom is — is a task you'd hand to whoever's fastest and most consistent at reading a form and lifting facts off it. You wouldn't put your most senior, most expensive case reviewer on that step. You'd save that person for the part that actually needs judgment: weighing a borderline case against policy, deciding what's covered and what isn't, catching the case that looks straightforward but isn't.

So the routing table is: cheap, fast tier for triage, a stronger tier for the resolver — a lighter tier for extraction, a more capable tier for judgment. This is on top of everything from the previous two articles, not a replacement for it. You're still trimming context, you're still passing a minimal typed payload — you're now also choosing, per call, how much model you actually need to pay for.

## The second piece: some cases shouldn't reach the resolver at all

The triage step isn't just extracting facts — it's also watching for signals that a case is outside what an automated resolver should be deciding on its own. A request that's really a legal or compensation demand, for instance, or something that falls outside what support is authorized to grant. When triage detects a trigger like that, the right move isn't to hand it to the resolver anyway and let a stronger model take a swing at it. It's to route straight to a human queue and skip the resolver call entirely.

That's a genuine cost win on top of everything else — if triage can confidently say "this doesn't belong with an automated resolver," you've saved the entire resolver call, not just made it cheaper. But the reason this matters more than the token savings is what happens if you get it backwards.

## Two ways this can fail

1. **Escalating a case a resolver could genuinely have handled** — a cost miss, mildly annoying, not dangerous.
2. **Letting a resolver make a confident, well-cited decision on something that should have gone to a human** — far worse. A confident wrong answer, dressed up with a policy citation, is more dangerous than an honest "I can't decide this," because it looks resolved.

## A distinction easy to blur under time pressure

Escalating to a human and asking the customer a clarifying question are not the same outcome, even though both mean "the resolver doesn't get to decide this yet." An escalation trigger means this shouldn't be automated at all. A missing-information case means the resolver *could* decide — it just doesn't have a fact it needs yet, so the honest move is to ask, not to guess. If your routing logic sends both down the same branch, you'll end up either escalating things a human didn't need to see, or guessing on things that needed a real question asked first.

## When both situations show up in the same case

Without getting into which specific case: yes, a case can have both an escalation-worthy piece and a separate, resolvable piece sitting side by side — worth thinking about in advance rather than discovering live under time pressure. When a case has an escalation trigger and a genuinely separate, decidable fact both present at once, the routing decision should still send it to escalation — but the information triage extracted about the resolvable part shouldn't just vanish because the case took the escalation branch. A human reviewer picking that case up afterward should be able to see both what triggered the escalation and whatever else triage had already figured out, rather than starting from zero. That's a good general habit for any routing system — routing a case away from full automation doesn't mean you're allowed to throw away the partial work already done on it.

## What's left once this is wired up

Cheaper tier for triage, stronger tier for resolution, a short-circuit branch for anything that shouldn't be automated — what's left is the part that's easy to skip when you're excited about hitting a small token number: checking that everything you built still gets the right answer, consistently, across every case, not just the one you've been iterating against this whole time.

## What's next

That check is the next article — and it's the one that actually determines whether anything discussed so far counts for anything.
