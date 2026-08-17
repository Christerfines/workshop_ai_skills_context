# Context Engineering: Cutting Tokens Without Cutting Correctness

> Text version of [`preparation/03-context-engineering.md`](../03-context-engineering.md) — same content, written as an article instead of a podcast script. Preps Exercise 1 (baseline) and Exercise 2 (context reduction). ~5-minute read.

You now have the company and the full rulebook in your head — product line, seven-section warranty policy, escalation triggers. Here's the number that should sting a little now that you know how much of that rulebook exists: **19,800 tokens, one model call, the most expensive tier available, for a single support case.** This article is about where that number comes from and what you actually do about it.

## Where the number comes from

Nineteen thousand eight hundred tokens is what you get when an agent has no way to know in advance which of the source material is relevant, so it includes all of it — company background, every product's full specification, every policy document, the customer's whole record, the case file — one prompt, everything in it.

An analogy: imagine someone asks you a specific question about one clause in one contract, and instead of pulling that contract off the shelf, you photocopy every folder in the filing cabinet and hand them the whole stack. Somewhere in there is the paragraph they need. You've technically given them everything required to answer — and also made the job much harder and much slower than it had to be.

And it's expensive twice over: you pay in dollars, because bigger prompts cost more per call, and you pay in latency, because bigger prompts take longer to process. There's also a subtler cost people underrate: irrelevant context doesn't just sit there neutrally — it can actively degrade accuracy. A model wading through material that isn't decision-relevant is more likely to latch onto the wrong signal than a model looking at exactly what it needs. More text is not more signal.

## The skill isn't "cut" — it's judgment

The fix sounds obvious: cut the context. But "cut the context" is not the skill. The skill is judgment about what's actually relevant to the decision in front of you, applied file by file, section by section.

Take a single product file — it typically has a full specification, a list of available configurations like frame sizes and colors, and a list of compatible accessories. If you're resolving a warranty claim, the configuration options and the accessory list usually aren't doing any work for you. What matters is the handful of facts the eligibility decision actually depends on — things like what kind of battery the bike has and which warranty sections apply to it. Same idea with a policy document: it might have seven sections, and for a given case maybe three of them are load-bearing and the rest are noise — *for that case*. Not for every case. A section that's irrelevant today might be exactly what a different case turns on tomorrow.

That last point is the trap. It's very easy to hit a token-reduction target by truncating aggressively or dropping a section because it looked unimportant on a skim. That gets you a smaller prompt fast, and it's exactly the kind of blind cut that can quietly break a decision — either on the case you're looking at right now, or silently, on a case you haven't gotten to yet.

## The actual discipline: two questions, per file

1. **Is this file relevant to this case at all?** Some files are relevant to essentially no eligibility decisions — general company background, for instance, almost never changes a warranty outcome, so it can often be dropped entirely rather than trimmed.
2. **If yes: which specific facts or sections within this file does the decision actually depend on?** This is where you excerpt rather than omit — keep the load-bearing sentence or section at full fidelity, and let go of everything around it, rather than keeping a diluted, trimmed-down version of the whole file.

That distinction — excerpt versus omit — is the whole exercise, honestly. Omitting something you needed is a correctness failure disguised as a cost win. Excerpting is the actual skill: high-fidelity on what matters, nothing at all on what doesn't.

## Your target: relative, not absolute

Once you're in the room, the target is roughly a fifty percent cut from whatever you measure as *your own* baseline, while keeping the same single call and the same model tier — not the 19,800 reference figure specifically, your own number, whatever it comes out to. If your measurement in the first exercise comes out a little different from 19,800 — small tokenizer-level variation is normal — the fifty percent target scales to your number, not the reference one. The whole point is comparing your own before-and-after, honestly measured.

## The lesson beyond this workshop

Context is a scarce resource, the same way compute or memory is in any other system you'd build. Treating it that way — asking "does this specific fact need to be in front of the model for this specific decision" instead of "let's just include the source documents to be safe" — is a habit that transfers directly to production agent work, regardless of what company or case load you're actually working with.

One practical tip before you're doing this live: think about the two questions — is it relevant, and if so, which part — *as you go*, not after. Skimming a file and deciding "I'll trim this at the end" tends to produce shallower cuts than reasoning through it section by section the first time you touch it.

## What's next

Once you've got a smaller, sharper single call working, the next move isn't to trim further — it's to change the shape of the pipeline entirely: splitting one call into two, and the very specific way that split can go wrong if you're not careful about what crosses the seam.
