# Subagent Handoff: A Payload, Not a Dump

> Text version of [`preparation/04-subagent-handoff.md`](../04-subagent-handoff.md) — same content, written as an article instead of a podcast script. Preps Exercise 3 (subagent handoff and skill scoping). JSON examples reproduced verbatim from the participant-facing exercise file. ~5-minute read.

Quick recap of where the last article left off: you've got one call, trimmed down, doing everything — reading the case, reading the policy, making the decision. This one is about breaking that call into two, and why that's much easier to do badly than to do well.

## The instinct, and why it's not enough

Once you decide to split a pipeline, the instinct is usually just to draw a line down the middle: step one reads everything and figures out the facts, step two takes what step one found and makes the call. Sounds clean. It's also very easy to build in a way that accomplishes almost nothing.

The terminology matters here. You're building two subagents:

- A **triage** subagent, whose only job is to read the case and the customer record and extract structured facts — not to decide anything.
- A **resolver** subagent, whose only job is to take what triage found and make the actual eligibility judgment.

Two narrow, specific jobs instead of one broad one. That narrow scoping is the "skills design" piece people mention alongside this — a well-designed subagent is a reusable skill with a clear boundary: it does one specific thing, has a clear input and a clear output, and you could hand it a different case tomorrow and trust it to behave the same way. That's different from writing one long prompt that tries to do triage and resolution and everything else in a single undifferentiated pass.

## Where it goes wrong in practice

You split into two calls, sure — but then the triage subagent hands the resolver everything it read. The full text of every policy file, the full customer record, the full case file, re-serialized into a JSON blob and passed along. Technically two calls now. Functionally, you haven't saved anything, because the resolver has to read almost the same volume of material the single call did — it's just paying for it one hop later instead of up front.

Picture a handoff object with fields like `company_md` containing the entire contents of the company background file, `products` containing the entire text of every product specification, `policies` containing every policy document in full, plus the full customer record and the full case text, and sometimes even an open-ended conversation-history field tacked on for good measure. Every one of those fields is a full document dump wearing a JSON key as a costume. It looks structured because it's valid JSON — but structurally, it's the exact same bloat as before.

## What a minimal typed payload actually looks like

Compare that to a genuinely minimal payload: case ID, customer ID, product SKU and name, serial number, purchase date, the computed warranty window end date (already calculated, not left for the resolver to derive), the symptom as the customer stated it in a sentence or two — not a transcript — a candidate flag for what kind of situation this looks like, the specific policy sections that are likely relevant (section references, not the section text), and any structured yes-or-no signal flags that matter to the decision — something like whether water exposure was reported, or whether the customer mentioned pressure-washing near sensitive components.

Every field in that payload is a specific fact, not a document.

## The self-sufficiency test

The test worth carrying into the room: could the resolver make a correct, well-cited decision using only this payload, with zero access to the original source files? If the honest answer is no — if the resolver would have to go back and re-read the policy document or the customer record to fill in something the payload left out — the payload is missing a field. That's a gap, not a size problem. But if you look at a field and realize the resolver doesn't actually need it to decide, that field is a candidate for removal, no matter how reasonable it felt to include "just in case."

That "just in case" instinct is worth naming directly, because it's the single most common way these payloads bloat back up. You extract a fact, you're not fully sure whether the resolver will need it, so you paste in a whole paragraph of surrounding context to be safe. If you catch yourself doing that, it's usually a sign triage didn't finish the job — the extraction should have produced a specific fact, not a hedge.

One clarification on "zero access to the original source files," since it's easy to over-apply: the resolver may still receive the verbatim text of exactly the policy sections named in the payload's `applicable_policy_sections` field — that's a deterministic lookup keyed off a field triage already produced, not a re-read of the whole document. What the payload must never contain is triage's own *conclusion* about what those sections mean for the case — a root-cause determination is the resolver's job, not triage's. A field like `water_exposure_reported: false` is a fact triage observed; a field like `manufacturing_defect: true` is a conclusion triage isn't positioned to reach reliably. Keep the payload to observed facts and flags, not conclusions.

## What target you're aiming for

Meaningfully tighter than the single-call version — a fraction of what one trimmed call cost, split across the two calls together. The exact split matters less than the shape of it: a smaller triage call that's purely extraction, and a resolver call that's purely judgment, with a typed payload — not a document — crossing the seam between them.

## Why this matters beyond the token count

Once triage is a clean, narrow, reusable skill, you can reuse it — point it at a different case tomorrow and trust its output shape without rewriting it. That reusability is really the point of scoping a subagent tightly in the first place. A subagent that only works because it happens to have absorbed the whole knowledge base isn't really a skill — it's just the old monolithic prompt wearing a second hat.

## What's next

Once triage and resolver are cleanly separated, you've created an opportunity you didn't have before: you can send each of those two calls to a *different model*. Extraction and judgment are not equally hard tasks, and once they're separate calls, nothing says they have to run on the same tier of model — matching model tier to how hard each step actually is, and the specific case where the right move is to skip the second call entirely.
