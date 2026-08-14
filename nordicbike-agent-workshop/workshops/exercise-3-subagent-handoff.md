# Exercise 3 — Subagent Handoff

## Goal

Split your agent into two calls: a triage subagent that extracts structured facts from the case and customer record, and a resolver subagent that makes the eligibility decision. The triage subagent's output to the resolver must be a minimal typed JSON payload — no full-context dumps are permitted between subagents. This exercise teaches the bad-vs-good handoff-payload distinction directly: it is easy to "split into two calls" and still pass all the same bloat across the seam, which defeats the purpose of splitting at all.

## Starting Point

Your V2 output from Exercise 2: a single Tier-3 call at ≤9,200 tokens using trimmed, case-relevant excerpts.

## Constraint

"Split your agent into two calls: a triage subagent that extracts structured facts from the case and customer record, and a resolver subagent that makes the eligibility decision. The triage subagent's output to the resolver must be a minimal typed JSON payload — no full-context dumps are permitted between subagents. Target ≤5,500 total input tokens across both calls, same model tier as Exercise 2."

## Target Metric

**≤5,500 tokens, 2 calls, Tier 2 + Tier 2.**

## Deliverable

Both calls' actual input token counts, plus the typed payload JSON your triage subagent actually produced when handing off to the resolver.

## Bad Pattern (do not do this)

Full-context dump — reproduced verbatim from the fixed reference examples:

```json
{
  "company_md": "<entire contents of company/about.md>",
  "support_contacts_md": "<entire contents of company/support-contacts.md>",
  "products": {
    "aurora_x3": "<entire contents of products/aurora-x3.md>",
    "fjord_cargo": "<entire contents of products/fjord-cargo.md>",
    "vinter_pro": "<entire contents of products/vinter-pro.md>",
    "powerpack_batteries": "<entire contents of products/powerpack-batteries.md>",
    "accessories": "<entire contents of products/accessories.md>"
  },
  "policies": {
    "warranty": "<entire contents of policies/warranty.md>",
    "returns": "<entire contents of policies/returns.md>",
    "shipping": "<entire contents of policies/shipping.md>",
    "escalation": "<entire contents of policies/escalation.md>"
  },
  "customer_record_full": "<entire contents of customers/anna-karlsson.md>",
  "case_full_text": "<entire contents of cases/case-01-anna-karlsson.md>",
  "conversation_history": "<unbounded prior reasoning trace>"
}
```

This is what "splitting into two calls" looks like when it accomplishes nothing: every file the triage subagent read is simply re-serialized and handed to the resolver wholesale, plus an open-ended conversation history field. The resolver now has to read (and pay for) roughly the same context the single V2 call did, just one hop later.

## Good Pattern (build this)

Minimal typed decision payload — reproduced verbatim from the fixed reference examples:

```json
{
  "handoff_type": "typed_decision_payload",
  "case_id": "CASE-01",
  "customer_id": "NB-CUST-10041",
  "product_sku": "AX3",
  "product_name": "Aurora X3",
  "serial_number": "AX3-25A-00417",
  "purchase_date": "2025-03-10",
  "warranty_window_end_standard": "2027-03-10",
  "stated_symptom": "intermittent power loss, bike will not hold charge",
  "candidate_archetype": "symptom_cause_confusion",
  "applicable_policy_sections": ["warranty.md#section-5", "warranty.md#section-4"],
  "root_cause_flags": {
    "water_exposure_reported": false,
    "pressure_washed_near_battery": false
  },
  "recommended_model_tier": "tier_2"
}
```

Notice what is absent: no file contents, no prose paragraphs, no conversation history. Every field is a specific typed fact the resolver actually needs to make a decision — case and customer identifiers, product identity, the serial number needed to check batch membership, the purchase date and computed warranty window end date, the symptom as reported, a candidate archetype flag, the specific policy sections that are likely relevant (not the full policy text), and structured root-cause signal flags extracted from the customer's own account. The resolver still has to reason over these facts and apply policy — but it does not have to re-read source files to reconstruct what the triage subagent already determined.

## Hints

The payload should carry structured fields — case ID, product, purchase date, serial, stated symptom, candidate archetype, applicable policy section IDs — not prose paragraphs. If you find yourself putting a full sentence of unstructured text into a payload field "just in case," that's usually a sign the triage subagent didn't finish its job of extracting a specific fact.

A useful test for whether your payload is actually minimal: could the resolver subagent make a correct, well-cited decision using only the payload, with zero access to any of the original source files? If the answer is no — if the resolver would need to go back and re-read policies/warranty.md or the customer record to fill in something the payload left out — the payload is missing a field, not too large. Conversely, if a field in your payload duplicates information the resolver doesn't actually need to make its decision, that field is a candidate for removal. Getting this balance right, field by field, is the actual skill this exercise is teaching, more so than hitting the 5,500-token number itself.
