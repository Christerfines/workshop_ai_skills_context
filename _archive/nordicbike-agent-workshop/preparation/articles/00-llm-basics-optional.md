# Five Words You'll Hear All Session (Optional)

> Text version of [`preparation/00-llm-basics-optional.md`](../00-llm-basics-optional.md) — same content, written as an article instead of a podcast script. Pick whichever format you'd rather use; there's no difference in what's covered. ~4-minute read.

**Optional.** Skip this one entirely if you already work with LLM APIs regularly — you know all of it. It's for anyone joining the workshop who's comfortable with AI tools generally but hasn't personally written code that calls a model API before. The workshop's one real prerequisite is being comfortable constructing a prompt from a few source documents, sending it to a model, and reading back a structured response. If that sentence already makes sense, skip ahead to Episode 1. If any part of it felt fuzzy, the five terms below are all you need.

## Model call

One request-response round trip to an AI model: you send it some text (the input, or prompt), and it sends back some text (the output). When this workshop says "one call" versus "two calls," that's exactly what it means — did the work happen in a single round trip, or was it split into two separate requests, maybe to two different models, with the first one's output feeding into the second one's input?

Think of it like sending someone a written question and getting a written answer back, versus asking a first person a question and handing what they told you to a second person to act on. Same basic shape as a phone call or an email, just automated and fast.

## Token

A small chunk of text — sometimes a whole word, often a piece of a word, sometimes just punctuation. You don't need the exact rule for how text splits into tokens; the useful intuition is: more text means more tokens, and models are priced and measured by token count, both for what you send in and what you get back. When this workshop says a prompt is "19,800 tokens," that's just a precise, cost-relevant way of saying "this much text, measured in the model's own units" — more precise than counting words or characters.

The practical reason it matters: tokens cost money, and more tokens generally means a slower response too. So "reduce tokens" always means the same thing in this workshop — send the model less text — while "don't lose correctness" means being careful about *which* text you cut.

## Context (or context window)

The context is everything you put in front of the model for a given call — instructions, source documents, any prior conversation, all of it, bundled into one request. The context window is the upper limit on how much of that a given model can accept at once. In this workshop, "context" mostly just means "everything in the prompt" — and a recurring theme is that it's very easy to put more into that prompt than the task actually needs.

## Model tier

Different models aren't interchangeable: some are faster and cheaper but less capable of complex reasoning; some are slower and more expensive but handle harder judgment calls more reliably. A "tier" is just a label for where a model sits on that spectrum. This workshop uses three tiers, numbered by capability and cost. A big part of what it teaches is sending each piece of work to the tier that actually matches how hard that work is, rather than defaulting to the strongest tier for everything out of habit or caution. You don't need to know anything about specific models to follow along — just that "tier" means "how capable, and how expensive, is the model doing this particular piece of work."

## JSON, and "typed payload"

JSON is a plain-text way of writing structured data — key-value pairs, the way a form has labeled fields instead of a blank paragraph. Instead of writing "the customer's name is Anna and she bought the bike on March 10th" as a sentence, you'd write a field called *customer name* with the value *Anna*, and a separate field called *purchase date* with the value *March 10th*. You don't need to already know how to write it — you'll see concrete examples once the workshop starts. Just know the shape: labeled fields instead of prose.

A "typed payload" is JSON used a specific way: a small, deliberate set of labeled fields passed from one step of a pipeline to the next, instead of dumping a wall of text across that same handoff. You'll hear "payload, not a dump" a lot in this series — now you know what both halves of that phrase mean.

## That's the glossary

Model call, token, context, tier, and JSON as a typed payload. Every other episode in this series uses all five without re-explaining them. If none of this was new, you've lost four minutes and gained nothing — which is exactly why skipping this one is the right call for most people. Either way, Episode 1 is next, and that's where the actual workshop content starts.
