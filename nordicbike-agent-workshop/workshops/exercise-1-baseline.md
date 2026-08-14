# Exercise 1 — Baseline

## Goal

Run the provided naive V1 agent against Case 1 (Anna Karlsson) and record its token count, model tier, call count, and whether it reaches the correct eligibility outcome. This establishes the fixed reference point — 18,400 tokens, Tier 3, 1 call — that every later exercise in this workshop is measured against. Every subsequent exercise's target (V2's ≤9,200 tokens, V3's ≤5,500, V4's ≤3,800) is defined as a reduction relative to whatever number you record here, so getting an accurate, honest baseline measurement in this exercise matters more than it might first appear — a sloppy or inflated baseline makes every later exercise look like a bigger win than it actually is, and a deflated one makes later exercises unfairly hard to hit.

## Starting Point

The naive V1 agent concatenates the entire V1 naive context set into one single Tier-3 model call. The V1 naive context set is fixed and named: company/about.md, company/support-contacts.md, all 5 files in products/, all 4 files in policies/, the one relevant customers/*.md file (customers/anna-karlsson.md), and the one relevant cases/*.md file (cases/case-01-anna-karlsson.md). No excerpting, no filtering — every byte of every one of those files goes into the prompt. This is intentionally wasteful; that is the point of this exercise.

This is a "dump everything" agent in the most literal sense: it does not attempt to judge which files are relevant to Case 1 before including them, it does not summarize or excerpt any file, and it makes exactly one call to a single, frontier-tier model to both read all of that context and produce a final eligibility decision. Nothing about this design is a strawman — this is a realistic, if unoptimized, way many teams' first working agent prototype actually gets built when speed to a first working version is prioritized over context discipline. Recognizing your own instinct to build something like this, and measuring what it actually costs, is the entire point of doing this exercise before touching any optimization technique.

## Constraint

"Run the baseline agent exactly as provided against Case 1. Do not modify it. Record: total input tokens, model tier used, number of model calls, and the agent's eligibility decision. This is your baseline to beat."

## Target Metric

Measured baseline should read **18,400 tokens, 1 call, Tier 3**.

## Deliverable

A filled baseline-measurement table with the following fields: tokens, tier, calls, decision, correct? (Y/N).

| Tokens | Tier | Calls | Decision | Correct? (Y/N) |
|---|---|---|---|---|
| | | | | |

## Hints

None — this exercise has no optimization step. Your job here is purely to measure and record, not to improve anything yet. Resist the urge to start trimming context; that begins in Exercise 2. If your measured token count comes out noticeably different from 18,400, double-check that you have included every file in the V1 naive context set exactly once, in full, with nothing excerpted or summarized — a mismatch is more often a bundling mistake than a meaningful difference in how your particular model or tokenizer counts tokens, though minor tokenizer-level variation of a few percent either way is expected and not a cause for concern.
