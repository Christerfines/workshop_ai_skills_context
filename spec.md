⏺ NordicBike Agent Optimization Challenge — Downstream Build Specification

  Facilitator-guide placement decision (fixed, do not contradict elsewhere in 
  this document): the facilitator guide is a standalone file, 
  facilitator-guide.md, at the repository root. It is NOT folded into 
  README.md.

  Target tooling decision (fixed, do not contradict elsewhere in this
  document): this workshop targets GitHub Copilot as the participant-facing
  agent tool, not Claude or any Anthropic-specific product surface. Two
  consequences apply everywhere else in this spec: (1) model tiers are
  named by capability/cost class only (Fast/cheap, Balanced, Frontier) —
  never by a specific model family, since Copilot's available models change
  over time and by plan; (2) the reusable-subagent-capability deliverable in
  Section 7 is a GitHub Copilot Prompt File (.github/prompts/*.prompt.md),
  not a Claude Code Skill file — there is no skills/ directory anywhere in
  this spec. updatenumbers.md (repository root, referenced from README.md)
  is the fixed, authoritative runbook for periodically re-verifying the
  token/cost figures below still hold and for mapping tiers to whichever
  concrete Copilot models are current at delivery time.

  Repository root: nordicbike-agent-workshop/
  
  Global conventions used throughout this spec (apply identically everywhere a
  number appears):
  - Token/word conversion ratio: 1 token ≈ 0.75 words, equivalently 1 word ≈
  1.333 tokens. word_target = round(token_target × 0.75); token_target = 
  round(word_target ÷ 0.75). All "target word/line count" figures below were
  derived with this ratio and must not be recomputed with a different ratio.
  - Fixed "today" for all warranty-window arithmetic: 2026-08-14. This date
  does not appear inside any policy or product file (policies are timeless);
  it is used only in evaluation/expected-results.md and in workshop exercise
  text where elapsed-time math is required.
  - Currency: all prices in SEK, no decimals, formatted 34,900 SEK.
  - The "V1 naive context set" — the fixed, named bundle of files that a
  Version-1 (naive, single-call, dump-everything) agent ingests to resolve one
  case — is exactly: company/about.md, company/support-contacts.md, all 5
  files in products/, all 4 files in policies/, the one relevant
  customers/*.md file, and the one relevant cases/*.md file. Throughout this
  spec, the canonical worked example for this bundle is Case 1 (Anna 
  Karlsson). Summed at the token targets given below, this bundle is exactly
  19,800 tokens.
  - V1→V4 token-load progression (fixed, reproduce byte-for-byte everywhere it
  recurs — README, presentation.md, facilitator-guide.md, all workshops/*.md,
  evaluation/scoring-rubric.md):

  Version: V1
  Tokens: 19,800
  Mechanism: Dump every KB file + full customer record + full case text into
  one call
  Calls: 1  
  Model tier(s): Tier 3 (frontier)
  ────────────────────────────────────────
  Version: V2            
  Tokens: 9,900 (exactly 50% of V1)
  Mechanism: Same single call, but only relevant excerpts (not full files)
  Calls: 1  
  Model tier(s): Tier 3
  ────────────────────────────────────────
  Version: V3            
  Tokens: 5,500
  Mechanism: Two-call subagent handoff (retrieval/triage → resolver), with a
  minimal typed handoff payload — no full-context dump between them
  Calls: 2
  Model tier(s): Tier 2 + Tier 2
  ────────────────────────────────────────
  Version: V4
  Tokens: 3,800
  Mechanism: Two-call subagent handoff with model routing (Tier 1 triage,
  Tier 2 resolver); triage's own input narrows further too — it receives
  only the case text, not the full customer record, which the resolver reads
  directly
  Calls: 2 (or 1 for escalation-flagged cases)
  Model tier(s): Tier 1 (triage) + Tier 2 (resolver)

    V3 and V4 figures are not sizes of any repository file; they are the sum
  of input tokens across the runtime LLM calls a participant's agent makes,
  computed from excerpts/typed payloads assembled from the source files. V4's
  exact split is 1,400 tokens (Tier-1 triage call, case text only) + 2,400
  tokens (Tier-2 resolver call) = 3,800. V3's exact split is 2,500 tokens (Tier-2 
  retrieval/triage call) + 3,000 tokens (Tier-2 resolver call) = 5,500. V2's
  split is a single Tier-3 call of 9,900 tokens. These splits must be
  reproduced identically in presentation.md, facilitator-guide.md, and
  workshops/exercise-3-subagent-handoff.md / exercise-4-model-routing.md.

  - Cost-weight table (used by the budget-points formula, fixed, reproduce 
  identically in presentation.md and evaluation/scoring-rubric.md):

  ┌────────┬─────────────┬──────────────────────────────┐
  │  Tier  │    Class    │ Cost weight per 1,000 tokens │
  ├────────┼─────────────┼──────────────────────────────┤
  │ Tier 1 │ Fast/cheap  │ 1                            │
  ├────────┼─────────────┼──────────────────────────────┤
  │ Tier 2 │ Balanced    │ 4                            │
  ├────────┼─────────────┼──────────────────────────────┤
  │ Tier 3 │ Frontier    │ 12                           │
  └────────┴─────────────┴──────────────────────────────┘

  Deliberately model-agnostic: this workshop targets GitHub Copilot, whose
  available model lineup changes over time and varies by plan. Tiers are
  defined by capability/cost class only, never by naming a specific model
  family — updatenumbers.md (repository root) is the authoritative process
  for periodically re-verifying these figures still hold and for mapping
  each tier to whichever concrete Copilot models are current at delivery
  time, without that mapping leaking into any fixed, reproduced document.

  ---
  Company facts fixed for reuse across every section below (reference block, 
  not a file to generate)

  NordicBike AB — founded 2019, HQ Stockholm (Hammarby Sjöstad), ~85
  employees, sells via online store plus 3 partner service centers (Stockholm,
  Gothenburg, Malmö). Support: support@nordicbike.se, +46 8 555 123 00,
  Mon–Fri 09:00–17:00 CET. Escalation: support-lead@nordicbike.se,
  4-business-hour acknowledgement SLA.

  Products: Aurora X3 (city e-bike, 34,900 SEK), Fjord Cargo (cargo e-bike,
  44,900 SEK), Vinter Pro (winter e-bike, 37,900 SEK), PowerPack 720 (spare
  battery, 720Wh, 6,900 SEK), PowerPack 900 (spare battery, 900Wh, 8,900 SEK),
  Accessories (pannier bag 1,200 SEK, LED light set 450 SEK, frame lock 950
  SEK, phone mount 350 SEK).

  Serial format: [MODEL]-[YY][A|B]-[5 digits] where A = manufactured Jan–Jun
  of that year, B = Jul–Dec. Model codes: AX3 (Aurora X3), FJC (Fjord Cargo),
  VTP (Vinter Pro).

  Warranty terms (full detail in policies/warranty.md, referenced everywhere
  else, never restated with different numbers): Standard 24 months (whole
  bikes); Wear-item 6 months (tires, brake-pad/disc friction material, cables,
  grips, saddle); Spare/replacement battery standalone 12 months; Service
  Bulletin SB-2025-11 (Aurora X3 batch AX3-25A, battery-connector corrosion,
  manufacturing-defect coverage unless root cause is customer water ingress);
  Vinter Pro Winter 2024 Launch Promotion grandfather clause (36-month
  warranty for units purchased 2024-06-01 through 2024-08-31 inclusive; all
  other purchases get standard 24 months); refund/compensation authority limit
  2,000 SEK (above this, or any legal threat, or any custom/off-catalog
  modification request → mandatory escalation).

  ---
  1. company/

  (a) company/about.md

  - (b) Pedagogical function: pure background noise for V1/V2 — establishes
  that a naive agent loads company history/mission text that is never
  decision-relevant to any case, teaching participants to recognize and strip
  irrelevant context in Exercise 2.
  - (c) Required sections: ## Overview, ## Founding & History, ## Locations,
  ## Product Lines (names only, no specs/prices — those live in products/), ##
  Mission Statement.
  - (d) Facts: NordicBike AB, founded 2019, HQ in Hammarby Sjöstad, Stockholm,
  Sweden. ~85 employees. Online store plus 3 partner service centers:
  Stockholm, Gothenburg, Malmö. Product lines listed by name only: Aurora X3,
  Fjord Cargo, Vinter Pro, PowerPack batteries, Accessories. Mission statement
  (invent one sentence, e.g. "NordicBike AB exists to make electric mobility
  the practical everyday choice across the Nordics, engineered for Nordic
  winters and built to last.") — this exact sentence must be reused verbatim
  if quoted anywhere else (it is not required elsewhere, so no cross-file
  obligation beyond this file).
  - (e) Target: 450 words (≈600 tokens).

  (b) company/support-contacts.md

  - (b) Pedagogical function: contact/routing metadata a naive agent
  needlessly re-reads on every call; also the canonical source for the
  escalation email address that must match policies/escalation.md exactly.
  - (c) Required sections: ## Customer-Facing Support Channels, ## Escalation 
  Contact, ## Service Center Addresses.
  - (d) Facts: Email support@nordicbike.se; phone +46 8 555 123 00; hours
  Mon–Fri 09:00–17:00 CET; live chat on nordicbike.se. Escalation contact:
  support-lead@nordicbike.se, SLA "acknowledged within 4 business hours" (must
  match policies/escalation.md word-for-word on the SLA figure). Service
  centers: Stockholm (HQ, Hammarby Sjöstad), Gothenburg, Malmö — city names
  only, no street addresses need to be invented beyond city level.
  - (e) Target: 300 words (≈400 tokens).

  ---
  2. products/

  (a) products/aurora-x3.md

  - (b) Pedagogical function: the hero product — referenced by Cases 1, 2, 6,
  and 9. Supplies the specs a correct agent must ground eligibility decisions
  in (serial format, battery Wh, price for refund-threshold comparisons) and
  is the largest single "noise" file in the V1 bundle, testing whether
  optimized versions correctly excerpt only the relevant subsection.
  - (c) Required sections: ## Overview, ## Full Specifications (table: motor,
  battery, range, top speed, brakes, weight, frame material, frame sizes,
  gearing, display/connectivity, charger), ## Available Configurations (sizes
  S/M/L, colors), ## Battery & Range, ## Compatible Accessories (list by name
  only, prices live in products/accessories.md), ## Related Policies (pointer
  sentence only: "See policies/warranty.md for coverage terms; see
  policies/warranty.md Section 5 for Service Bulletin SB-2025-11 affecting
  batch AX3-25A." — no warranty numbers restated here).
  - (d) Facts: Price 34,900 SEK. Category: city e-bike. Motor: 250W mid-drive.
  Battery: PowerPack 720 (integrated, 720Wh). Range: up to 80 km (eco mode).
  Top assisted speed: 25 km/h (EU pedelec limit). Frame sizes: S/M/L. Colors:
  Nordic Black, Fjord Blue. Weight: 24 kg. Brakes: hydraulic disc. Frame
  material: aluminum 6061. Gearing: 8-speed derailleur. Display: LCD with
  companion app connectivity (Bluetooth). Charger: standard 4A charger, full
  charge in 4.5 hours. Serial format: AX3-[YY][A|B]-[5 digits] (e.g.,
  AX3-25A-00417). Compatible accessories: pannier bag, LED light set, frame
  lock, phone mount (all four, by name).
  - (e) Target: 2,100 words (≈2,800 tokens).

  (b) products/fjord-cargo.md

  - (b) Pedagogical function: secondary product, referenced by Case 3;
  supplies the "commercial/rental use" warranty nuance (Business Edition) that
  participants must not confuse with the standard edition.
  - (c) Required sections: same schema as aurora-x3.md minus "Available
  Configurations" colors list being single-color; add ## Business Edition 
  Note.
  - (d) Facts: Price 44,900 SEK. Category: cargo e-bike. Motor: 250W
  mid-drive. Battery: PowerPack 900 (integrated, 900Wh). Cargo capacity: 100
  kg (rear rack + optional front box, sold as accessory but not in the fixed
  accessories catalog — front box is out of scope, do not invent a price for
  it). Range: up to 70 km. Colors: Slate Grey (single color only). Weight: 32
  kg. Brakes: hydraulic disc. Frame material: aluminum 6061 reinforced. Serial
  format: FJC-[YY][A|B]-[5 digits]. Business Edition note: "The standard
  Fjord Cargo warranty (see policies/warranty.md) excludes commercial/rental
  use. A separate Fjord Cargo Business Edition (not covered by this spec)
  permits commercial use under warranty."
  - (e) Target: 1,050 words (≈1,400 tokens).

  (c) products/vinter-pro.md

  - (b) Pedagogical function: referenced by Cases 4 and 5; carries the
  grandfathered-promotion trap (Archetype B) — the file itself does not
  mention the promotion (that lives only in policies/warranty.md Section 6) so
  participants must cross-reference rather than assume the product page is
  the complete source of truth.
  - (c) Required sections: same schema as aurora-x3.md.
  - (d) Facts: Price 37,900 SEK. Category: winter e-bike. Motor: 250W rear
  hub. Battery: PowerPack 720 (integrated). Studded tires standard. Range: up
  to 65 km (reduced ~20% in cold weather — state this explicitly as "-20% in
  temperatures below 0°C"). Colors: Arctic White. Weight: 26 kg. Brakes:
  hydraulic disc. Frame material: aluminum 6061. Serial format:
  VTP-[YY][A|B]-[5 digits]. No mention of any promotional warranty period
  anywhere in this file.
  - (e) Target: 1,050 words (≈1,400 tokens).

  (d) products/powerpack-batteries.md

  - (b) Pedagogical function: covers the standalone spare-battery SKUs;
  supplies Case 10's product facts and the fact that standalone batteries
  carry a different, shorter warranty term than whole bikes — a distinct
  policy nuance from the four named archetypes, tagged "none" for archetype
  purposes.
  - (c) Required sections: ## Overview, ## PowerPack 720, ## PowerPack 900, ##
  Compatibility, ## Related Policies (pointer only: "Spare/replacement
  batteries sold separately carry their own standalone warranty term — see
  policies/warranty.md Section 3. This differs from the whole-bike warranty
  term in Section 1.").
  - (d) Facts: PowerPack 720 — 720Wh, 6,900 SEK, compatible with Aurora X3 and
  Vinter Pro (as integrated battery or replacement part). PowerPack 900 —
  900Wh, 8,900 SEK, compatible with Fjord Cargo. Both charge with the standard
  4A charger, full charge 4.5 hours (720) / 5.5 hours (900, state this new
  figure — do not reuse the 4.5h figure for the 900).
  - (e) Target: 900 words (≈1,200 tokens).

  (e) products/accessories.md
  
  - (b) Pedagogical function: the smallest, lowest-stakes product file — a
  control for "does the agent correctly ignore fully irrelevant catalog items"
  when resolving a warranty/support case that isn't about accessories.
  - (c) Required sections: ## Overview, ## Catalog (table: item, price,
  description), ## Accessories Warranty Note (pointer only, one sentence:
  "Accessories carry a 6-month manufacturing-defect warranty only; wear and
  normal use are not covered. See policies/warranty.md.").
  - (d) Facts: Pannier bag (waterproof, 25L) — 1,200 SEK. LED light set
  (front+rear, USB-rechargeable) — 450 SEK. Frame lock (ART-approved) — 950
  SEK. Phone mount (weatherproof) — 350 SEK.
  - (e) Target: 750 words (≈1,000 tokens).

  ---
  3. policies/

  (a) policies/warranty.md

  - (b) Pedagogical function: the single largest, densest file in the KB — the
  primary "needle in haystack" noise/signal file for context-reduction
  exercises, and the sole authoritative source for every warranty number used
  across cases/ and evaluation/. It must contain, verbatim and in the exact
  section numbers below, the two policy mechanisms that drive the four
  adversarial archetypes: the SB-2025-11 root-cause exclusion (Archetype A)
  and the grandfather clause (Archetype B).
  - (c) Required sections, in this exact order and numbering (case files and
  the answer key cite these section numbers verbatim): ## Section 1 — Standard
  Limited Warranty, ## Section 2 — Wear-Item Limited Coverage, ## Section 3 —
  Spare/Replacement Battery Warranty, ## Section 4 — Exclusions, ## Section 5
  — Service Bulletin SB-2025-11 (Aurora X3 Battery-Connector Corrosion), ## 
  Section 6 — Legacy Promotion Grandfather Clause (Vinter Pro), ## Section 7 —
  Claim Process & Proof of Purchase.
  - (d) Facts, exact and final:
    - Section 1: 24 months from original purchase date, whole-bike products
  (Aurora X3, Fjord Cargo, Vinter Pro), proof of purchase required.
    - Section 2: 6 months from original purchase date for tires,
  brake-pad/disc friction material, brake and gear cables, grips, and saddle.
    - Section 3: 12 months from purchase date for PowerPack 720/900 sold as
  standalone spare/replacement parts, independent of any bike's warranty
  status.
    - Section 4 (exclusions, exact list): (i) water/moisture damage,
  explicitly including damage from high-pressure washing directed at or near
  the battery compartment or drivetrain electronics; (ii) unauthorized
  modification to the drivetrain or electrical system, which voids the
  Standard Limited Warranty in its entirety for the remainder of the coverage
  period; (iii) commercial or rental use, except on the Fjord Cargo Business
  Edition; (iv) accident or misuse damage; (v) cosmetic wear.
    - Section 5: verbatim bulletin text — "Service Bulletin SB-2025-11: Aurora
  X3 units in batch AX3-25A (manufactured 2025-01-01 through 2025-06-30) may
  exhibit battery-connector corrosion caused by a manufacturing sealant
  defect, presenting as intermittent power loss or failure to charge. This is
  covered under Section 1 as a manufacturing defect. This coverage does NOT
  apply if the corrosion's root cause is water ingress from customer-side
  high-pressure washing near the battery compartment (Section 4(i)) — visually
  similar corrosion can have either cause, and the root cause, not the visual
  symptom or batch membership, determines eligibility."
    - Section 6: verbatim clause — "Winter 2024 Launch Promotion: Vinter Pro
  units purchased between 2024-06-01 and 2024-08-31 inclusive carry a 36-month
  warranty in place of the standard 24-month term in Section 1. All Vinter
  Pro purchases outside this window receive the standard 24-month term."
    - Section 7: proof of purchase = order confirmation email or receipt
  showing product, purchase date, and price; claims processed at any of the 3
  service centers (Stockholm, Gothenburg, Malmö); turnaround per
  policies/shipping.md.
  - (e) Target: 3,150 words (≈4,200 tokens).
  
  (b) policies/returns.md

  - (b) Pedagogical function: a plausible-but-irrelevant-to-warranty-cases
  policy file (returns ≠ warranty) — tests whether an agent conflates
  "customer wants their money back" language with the returns policy versus
  the warranty/compensation policy; relevant only tangentially to Case 6
  (compensation demand) as a contrast document.
  - (c) Required sections: ## Right of Withdrawal, ## Conditions, ## Process.
  - (d) Facts: 14-day right of withdrawal from date of delivery, per
  Swedish/EU distance-selling consumer protection law. Item must be unused, in
  original packaging. Buyer pays return shipping unless the item is
  defective, in which case NordicBike covers return shipping.
  - (e) Target: 1,050 words (≈1,400 tokens).

  (c) policies/shipping.md

  - (b) Pedagogical function: pure logistics noise for warranty-decision
  cases; supplies the repair turnaround figure referenced generically (not
  per-case) in the answer key's "next steps" text.
  - (c) Required sections: ## Service Center Locations, ## Repair Turnaround,
  ## Shipping Costs for Warranty Claims.
  - (d) Facts: Service centers: Stockholm, Gothenburg, Malmö. Repair
  turnaround: 5–10 business days from intake. Shipping cost for
  warranty-eligible claims: free (NordicBike-paid) to nearest service center;
  non-warranty repairs: customer pays shipping both ways.
  - (e) Target: 750 words (≈1,000 tokens).

  (d) policies/escalation.md

  - (b) Pedagogical function: the sole authoritative definition of Archetype C
  ("out-of-scope request requiring escalation") — every escalation trigger a
  correct agent must recognize is defined here and nowhere else, so this file
  is the ground truth the quality-gate checklist item 4 and
  evaluation/adversarial-cases.md Archetype C both cite.
  - (c) Required sections: ## Escalation Triggers (exact enumerated list), ## 
  Authority Limits, ## Escalation Contact & SLA.
  - (d) Facts, exact and final — escalation triggers (any one is sufficient):
  (i) explicit legal or regulatory threat (e.g., mention of Konsumentverket, a
  lawyer, small-claims court); (ii) a refund or compensation demand exceeding
  2,000 SEK; (iii) a request for a custom or off-catalog product modification
  or service NordicBike does not perform. Authority limit: agents may
  authorize warranty repairs and standard-policy outcomes with no monetary cap
  when the outcome is "repair/replace under warranty as specified in policy,"
  but may NOT authorize any cash refund or compensation payment above 2,000
  SEK without escalation. Escalation contact: support-lead@nordicbike.se,
  acknowledged within 4 business hours (must match company/support-contacts.md
  exactly).
  - (e) Target: 900 words (≈1,200 tokens).

  ---
  4. customers/

  Each file uses this fixed schema — ## Customer Record, ## Contact
  Information, ## Purchase History — with fields: Customer ID (NB-CUST-#####),
  full name, email (firstname.lastname@example.se pattern, invented), phone
  (+46 7X XXX XX XX pattern, invented), city, one purchase-history row
  (product, order ID NB-ORD-YYYYMMDD-###, purchase date, price paid, serial
  number where applicable). Every fact in each customer file must match the
  corresponding cases/ file exactly (same purchase date, same product, same
  serial number).
  
  (a) customers/anna-karlsson.md

  - (b) Canonical positive instance of Archetype A. Supplies the exact
  purchase/serial facts that make her SB-2025-11 claim legitimate.
  - (c) Schema as above.
  - (d) Customer ID NB-CUST-10041. Anna Karlsson, anna.karlsson@example.se,
  +46 70 123 45 67, Malmö. Purchase: Aurora X3, order NB-ORD-20250310-001,
  purchased 2025-03-10, 34,900 SEK, serial AX3-25A-00417.
  - (e) Target: 600 words (≈800 tokens). (This is the file that must sum,
  together with the rest of the V1 bundle, to exactly 19,800 tokens — see the
  global conventions block.)

  (b) customers/erik-svensson.md

  - (b) Canonical negative instance of Archetype A — same batch, same symptom,
  disqualified by root cause, not by date or batch.
  - (c) Schema as above.
  - (d) Customer ID NB-CUST-10042. Erik Svensson, erik.svensson@example.se,
  +46 70 234 56 78, Gothenburg. Purchase: Aurora X3, order
  NB-ORD-20250422-002, purchased 2025-04-22, 34,900 SEK, serial AX3-25A-00892.
  - (e) Target: 650 words (≈867 tokens).

  (c) customers/lena-bjork.md

  - (b) Clean positive control (no archetype) — a straightforward, unambiguous
  eligible claim to calibrate baseline agent behavior before adversarial
  cases are introduced.
  - (c) Schema as above.
  - (d) Customer ID NB-CUST-10043. Lena Björk, lena.bjork@example.se, +46 70 
  345 67 89, Uppsala. Purchase: Fjord Cargo, order NB-ORD-20250901-003,
  purchased 2025-09-01, 44,900 SEK, serial FJC-25B-00113.
  - (e) Target: 550 words (≈733 tokens).

  (d) customers/johan-lindqvist.md

  - (b) Clean negative control (no archetype) — straightforward out-of-window
  denial, to calibrate that agents can correctly say no without any trap
  present.
  - (c) Schema as above.
  - (d) Customer ID NB-CUST-10044. Johan Lindqvist,
  johan.lindqvist@example.se, +46 70 456 78 90, Kiruna. Purchase: Vinter Pro,
  order NB-ORD-20231105-004, purchased 2023-11-05, 37,900 SEK, serial
  VTP-23B-00056.
  - (e) Target: 550 words (≈733 tokens).

  (e) customers/sara-nilsson.md

  - (b) Archetype B instance — her file must NOT mention the grandfather
  clause (that lives only in policy); it supplies only the purchase date that,
  cross-referenced against policies/warranty.md Section 6, resolves the trap.
  - (c) Schema as above.
  - (d) Customer ID NB-CUST-10045. Sara Nilsson, sara.nilsson@example.se, +46 
  70 567 89 01, Umeå. Purchase: Vinter Pro, order NB-ORD-20240615-005,
  purchased 2024-06-15, 37,900 SEK, serial VTP-24A-00201.
  - (e) Target: 600 words (≈800 tokens).

  (f) customers/mikael-strom.md

  - (b) Archetype C instance (escalation via compensation demand + legal
  threat). File is purely factual (purchase record); the demand/threat text
  lives in the case file, not here.
  - (c) Schema as above.
  - (d) Customer ID NB-CUST-10046. Mikael Ström, mikael.strom@example.se, +46 
  70 678 90 12, Stockholm. Purchase: Aurora X3, order NB-ORD-20260501-006,
  purchased 2026-05-01, 34,900 SEK, serial AX3-26A-00034.
  - (e) Target: 600 words (≈800 tokens).

  (g) customers/ingrid-dahl.md

  - (b) Archetype D instance (missing information) — deliberately the sparsest
  customer record in the set: no product field populated (unknown), no serial
  number on file, testing whether the agent notices and surfaces the gap
  rather than inventing values.
  - (c) Schema as above, but the ## Purchase History section must literally
  state "No purchase record on file matching the information provided in this
  case — product, order ID, and purchase date could not be confirmed" instead
  of a populated row.
  - (d) Customer ID NB-CUST-10047. Ingrid Dahl, ingrid.dahl@example.se, +46 70
  789 01 23, Örebro. No confirmed purchase record (as specified above).
  - (e) Target: 450 words (≈600 tokens).

  (h) customers/oskar-bergman.md
  
  - (b) Archetype D instance (ambiguous symptom requiring clarification) —
  record is complete (unlike Ingrid's), isolating the ambiguity to the case's
  symptom description rather than to missing customer data.
  - (c) Schema as above.
  - (d) Customer ID NB-CUST-10048. Oskar Bergman, oskar.bergman@example.se,
  +46 70 890 12 34, Linköping. Purchase: Fjord Cargo, order
  NB-ORD-20251201-007, purchased 2025-12-01, 44,900 SEK, serial FJC-25B-00219.
  - (e) Target: 550 words (≈733 tokens).

  (i) customers/freja-holm.md
  
  - (b) Archetype C instance (out-of-catalog modification request) — record
  establishes she is well within the standard warranty window, isolating the
  trap to the modification/exclusion issue rather than to timing.
  - (c) Schema as above.
  - (d) Customer ID NB-CUST-10049. Freja Holm, freja.holm@example.se, +46 70 
  901 23 45, Lund. Purchase: Aurora X3, order NB-ORD-20260210-008, purchased
  2026-02-10, 34,900 SEK, serial AX3-26A-00071.
  - (e) Target: 550 words (≈733 tokens).

  (j) customers/gustav-akesson.md
  
  - (b) Clean control testing the standalone-battery warranty term (Section 3)
  rather than any of the four named archetypes.
  - (c) Schema as above.
  - (d) Customer ID NB-CUST-10050. Gustav Åkesson, gustav.akesson@example.se,
  +46 70 012 34 56, Västerås. Purchase: PowerPack 720 (standalone, no bike
  purchase on file), order NB-ORD-20251101-009, purchased 2025-11-01, 6,900
  SEK, no serial (accessory batteries are not individually serialized in this
  catalog — state this explicitly rather than inventing a serial number).
  - (e) Target: 500 words (≈667 tokens).

  ---
  5. workshops/

  Each file uses schema: ## Goal, ## Starting Point, ## Constraint (verbatim
  prompt text — must be byte-for-byte identical to the corresponding prompt
  quoted in facilitator-guide.md), ## Target Metric, ## Deliverable, ## Hints.

  (a) workshops/exercise-1-baseline.md

  - (b) Establishes the naive V1 agent and its measured baseline (19,800
  tokens, Tier 3, 1 call) as the fixed reference point every later exercise is
  scored against.
  - (c) Schema as above.
  - (d) Goal: run the provided naive single-call agent against Case 1 (Anna
  Karlsson) and record its token count, model tier, call count, and whether it
  reaches the correct eligibility outcome. Starting point: the naive agent
  concatenates the entire V1 naive context set (defined in the global
  conventions block) into one Tier-3 call. Constraint (verbatim,
  participant-facing prompt): "Run the baseline agent exactly as provided 
  against Case 1. Do not modify it. Record: total input tokens, model tier 
  used, number of model calls, and the agent's eligibility decision. This is 
  your baseline to beat." Target metric: measured baseline should read 19,800
  tokens, 1 call, Tier 3. Deliverable: a filled baseline-measurement table
  (fields: tokens, tier, calls, decision, correct? Y/N). Output-shape rule
  (applies from this exercise on): the agent's output is a customer-facing
  reply carrying the determination and its citations, not an internal
  decision memo — Quality Gate item 6 grades that customer-facing text
  directly. Hints: none (this
  exercise has no optimization step).
  - (e) Target: 500 words (≈667 tokens).

  (b) workshops/exercise-2-context-reduction.md

  - (b) Teaches selective retrieval / progressive disclosure — cutting the V1
  bundle to only case-relevant excerpts.
  - (c) Schema as above.
  - (d) Goal: reduce input tokens by 50% relative to the V1 baseline without
  changing model tier or call count. Starting point: V1 agent output from
  Exercise 1. Constraint (verbatim): "Reduce the total input context by at 
  least 50% (target: ≤50% of your Exercise 1 measurement; the reference 
  figure is 9,900 tokens) while keeping exactly one model call and 
  the same model tier. You may not drop a source file that is 
  relevant to this case — excerpt it instead. Files with no bearing on this 
  case may be dropped entirely. Output quality (correctness of the eligibility decision) must not 
  regress." Target metric: ≤9,900 tokens, 1 call, Tier 3. Deliverable: the
  trimmed prompt plus a short note on which sections were cut and why. Hints:
  only the relevant product's spec and the relevant policy sections are
  needed; company/about.md and company/support-contacts.md are never
  decision-relevant and can be dropped entirely. Exception: policies/shipping.md
  is never eligibility-relevant but two of its facts (repair turnaround,
  NordicBike-paid warranty shipping) must still be excerpted, since a correct
  response communicates next steps, not just the verdict.
  - (e) Target: 550 words (≈733 tokens).

  (c) workshops/exercise-3-subagent-handoff.md

  - (b) Teaches the bad-vs-good handoff-payload distinction directly — the
  file must reproduce both fixed JSON examples from Section 9 of this spec
  (evaluation-adjacent) verbatim so participants have them in the exercise
  file itself, not only in the facilitator guide.
  - (c) Schema as above, plus a ## Bad Pattern (do not do this) and ## Good 
  Pattern subsection each containing one of the two fixed JSON examples
  (reproduced verbatim from the "Subagent Handoff Examples" block defined
  later in this spec).
  - (d) Goal: split the single call into a triage subagent and a resolver
  subagent, and pass only a minimal typed payload between them — no full file
  dumps. Starting point: V2 output. Constraint (verbatim): "Split your agent 
  into two calls: a triage subagent that extracts structured facts from the 
  case and customer record, and a resolver subagent that makes the eligibility
  decision. The triage subagent's output to the resolver must be a minimal 
  typed JSON payload — no full-context dumps are permitted between subagents. 
  Target ≤5,500 total input tokens across both calls, at Tier 2 for both
  calls — a step down from Exercise 2's Tier 3." Target metric: ≤5,500 tokens, 2 calls, Tier 2 + Tier 2.
  Deliverable: both calls' actual input token counts, plus the typed payload
  JSON actually produced. Hints: the payload should carry structured fields
  (case ID, product, purchase date, serial, today's date, stated symptom,
  candidate archetype, applicable policy section IDs) — not prose paragraphs. The
  resolver may receive the verbatim text of exactly the sections named in
  applicable_policy_sections (a deterministic lookup), but the payload must
  never carry triage's own root-cause conclusion — only observed facts/flags;
  weighing evidence against policy is the resolver's job.
  - (e) Target: 700 words (≈933 tokens).

  (d) workshops/exercise-4-model-routing.md

  - (b) Teaches model routing by cost/complexity — moving the triage call to a
  cheap tier and reserving the balanced tier only for the resolver, plus
  short-circuiting escalation cases before ever invoking the resolver.
  - (c) Schema as above.
  - (d) Goal: route the triage call to Tier 1 and the resolver call to Tier 2,
  and skip the resolver call entirely when triage detects an escalation
  trigger. Triage must emit evidence flags only (e.g. water_exposure_reported:
  false), never a root-cause conclusion (e.g. manufacturing_defect: true) —
  weighing evidence against policy stays the resolver's job. Starting point: V3 output, with triage's input narrowed to case text only (no longer the full customer record — the resolver reads that directly). Constraint (verbatim): "Apply the 
  model-routing table: triage calls run on Tier 1, resolver calls run on 
  Tier 2. If the triage subagent detects any escalation trigger from 
  policies/escalation.md, route directly to the human escalation queue and do 
  not make a resolver call at all. If the triage subagent instead finds the 
  case is missing decision-critical information, route to a clarifying-question 
  response instead of calling the resolver — a distinct branch from 
  escalation, not the same outcome. Target ≤3,800 total input tokens across all
  calls for non-escalated cases." Target metric: ≤3,800 tokens (1,400 Tier-1
  + 2,400 Tier-2) for non-escalated cases; ~1,400 tokens (Tier-1 only) for
  escalated cases. Deliverable: routing decision log across all 10 cases
  showing tier used per call and which cases short-circuited to escalation.
  Hints: Cases 6, 7, and 9 are expected to short-circuit.
  - (e) Target: 600 words (≈800 tokens).

  (e) workshops/exercise-5-quality-gate.md
  
  - (b) Teaches that token/cost optimization must not silently degrade
  correctness — introduces the deterministic 6-item quality-gate checklist as
  a mandatory pass/fail validation layer on top of the V4 agent.
  - (c) Schema as above.
  - (d) Goal: run the full 6-item quality-gate checklist (defined in
  evaluation/scoring-rubric.md) against the V4 agent's output for all 10 cases
  and fix any failures before submitting to the leaderboard. Starting point:
  V4 output. Constraint (verbatim): "Run every one of your 10 case outputs 
  through the 6-item quality gate in evaluation/scoring-rubric.md. Any output 
  that fails even one item is not eligible for leaderboard submission until 
  fixed. Fixing a quality-gate failure must not increase 
  the total input tokens you measured for that case in Exercise 4." Target metric: 10/10 cases pass all 6
  checklist items, token budget unchanged from Exercise 4. Deliverable:
  quality-gate pass/fail table for all 10 cases. Hints: none.
  - (e) Target: 500 words (≈667 tokens).

  ---
  6. cases/

  Each case file uses schema: ## Case Metadata (case ID, date received,
  channel, customer reference — name + link to the matching customers/*.md
  file), ## Customer Message (verbatim first-person message text, written by
  you now, fixed). Case files never state the outcome — outcomes live solely
  in evaluation/expected-results.md.

  For every case below: warranty status (inside/outside window) is computed
  against fixed "today" = 2026-08-14 and the warranty duration in
  policies/warranty.md Section 1/2/3 as applicable.

  (a) cases/case-01-anna-karlsson.md
  
  - (b) Positive half of the fixed Archetype-A pair; canonical worked example
  used throughout the deck and guide.
  - (c) Schema as above.
  - (d) Case ID CASE-01. Date received: 2026-08-11. Channel: email. Customer:
  Anna Karlsson (NB-CUST-10041). Product as stated by customer: Aurora X3.
  Message content (facts to include): reports intermittent power loss and the
  bike failing to hold a charge; states the bike is stored in the garage and
  has never been pressure-washed or exposed to heavy rain; asks if this is
  covered. Warranty status: INSIDE window — purchased 2025-03-10, standard
  24-month window runs through 2027-03-10, and today (2026-08-14) is before
  that date; additionally in-scope under SB-2025-11 since root cause (per her
  own account) is not water ingress. Location: Malmö. Archetype encoded: A
  (Symptom-Cause Confusion — positive instance).
  - (e) Target: 750 words (≈1,000 tokens). (Part of the fixed 19,800-token V1
  bundle.)

  (b) cases/case-02-erik-svensson.md
  
  - (b) Negative half of the fixed Archetype-A pair — identical symptom and
  batch to Case 1, disqualified purely by root cause.
  - (c) Schema as above.
  - (d) Case ID CASE-02. Date received: 2026-08-12. Channel: email. Customer:
  Erik Svensson (NB-CUST-10042). Product as stated: Aurora X3. Message
  content: reports the same symptom (intermittent power loss, won't hold
  charge); mentions, in passing, that he regularly hoses down the whole bike
  including the battery compartment after muddy rides because he finds it the
  easiest way to clean it; cites the same serial batch as a reason he believes
  he's covered. Warranty status: INSIDE the standard 24-month window
  (purchased 2025-04-22, window runs through 2027-04-22) but NOT ELIGIBLE
  because the stated root cause (customer-directed high-pressure washing near
  the battery compartment) falls under the Section 4(i) exclusion, which
  overrides SB-2025-11 coverage per Section 5's explicit carve-out. Location:
  Gothenburg. Archetype encoded: A (Symptom-Cause Confusion — negative
  instance).
  - (e) Target: 700 words (≈933 tokens).
  
  (c) cases/case-03-lena-bjork.md

  - (b) Clean positive control — no trap, calibrates correct "yes" behavior.
  - (c) Schema as above.
  - (d) Case ID CASE-03. Date received: 2026-08-10. Channel: chat. Customer:
  Lena Björk (NB-CUST-10043). Product: Fjord Cargo. Message: reports a visible
  weld crack on the rear cargo rack discovered during a routine load; no
  water exposure or modification mentioned. Warranty status: INSIDE window
  (purchased 2025-09-01, 24-month window runs through 2027-09-01); structural
  defect, no exclusions apply. Location: Uppsala. Archetype encoded: none
  (clean positive control).
  - (e) Target: 500 words (≈667 tokens).

  (d) cases/case-04-johan-lindqvist.md

  - (b) Clean negative control — straightforward time-barred denial, no trap.
  - (c) Schema as above.
  - (d) Case ID CASE-04. Date received: 2026-08-09. Channel: phone
  (transcribed). Customer: Johan Lindqvist (NB-CUST-10044). Product: Vinter
  Pro. Message: reports the rear hub motor has stopped engaging entirely; asks
  for a free repair. Warranty status: OUTSIDE window — purchased 2023-11-05,
  standard 24-month window ended 2025-11-05, which is before today
  (2026-08-14); no promotional grandfather clause applies (purchase date is
  outside the 2024-06-01–2024-08-31 promo window and it's an unrelated
  purchase year in any case). Location: Kiruna. Expected outcome direction:
  not eligible, offer paid repair. Archetype encoded: none (clean negative
  control).
  - (e) Target: 450 words (≈600 tokens).

  (e) cases/case-05-sara-nilsson.md

  - (b) Archetype B — grandfather-clause trap; tests whether the agent notices
  the still-valid legacy promotion rather than defaulting to the current
  24-month headline term.
  - (c) Schema as above.
  - (d) Case ID CASE-05. Date received: 2026-08-08. Channel: email. Customer:
  Sara Nilsson (NB-CUST-10045). Product: Vinter Pro. Message: reports the
  onboard display screen has stopped turning on; mentions she bought the bike
  "during the summer 2024 launch promotion" but doesn't state the exact
  promotional terms. Warranty status: under the STANDARD 24-month term alone,
  purchased 2024-06-15 would be OUTSIDE window (expired 2026-06-15, before
  today); however her purchase date (2024-06-15) falls inside the Section 6
  grandfather window (2024-06-01–2024-08-31), granting her a 36-month term
  that runs through 2027-06-15 — so she IS eligible, and an agent that applies
  only the standard 24-month headline term will incorrectly deny her.
  Location: Umeå. Archetype encoded: B (Superseded/Grandfathered-Policy Trap).
  - (e) Target: 600 words (≈800 tokens).

  (f) cases/case-06-mikael-strom.md

  - (b) Archetype C — the repair itself is trivially warranty-eligible; the
  trap is the compensation demand and legal threat requiring mandatory
  escalation rather than agent-authorized resolution.
  - (c) Schema as above.
  - (d) Case ID CASE-06. Date received: 2026-08-13. Channel: email. Customer:
  Mikael Ström (NB-CUST-10046). Product: Aurora X3. Message: reports the
  pedal-assist sensor failed mid-ride, causing him to miss a scheduled cycling
  event; demands 5,000 SEK in compensation for the missed event and states he
  will file a complaint with Konsumentverket (the Swedish Consumer Agency) if
  not resolved to his satisfaction. Warranty status: INSIDE window (purchased
  2026-05-01, well within the 24-month window) — the sensor repair itself is
  a straightforward eligible warranty repair. The compensation demand (5,000 
  SEK, exceeds the 2,000 SEK authority limit) and the explicit regulatory 
  threat both independently trigger mandatory escalation per 
  policies/escalation.md. Location: Stockholm. Archetype encoded: C
  (Out-of-Scope Request Requiring Escalation).
  - (e) Target: 650 words (≈867 tokens).

  (g) cases/case-07-ingrid-dahl.md

  - (b) Archetype D — deliberately missing decision-critical facts (product,
  purchase date, serial), forcing a clarifying-question response rather than a
  guess.
  - (c) Schema as above.
  - (d) Case ID CASE-07. Date received: 2026-08-13. Channel: chat. Customer:
  Ingrid Dahl (NB-CUST-10047). Product as stated: unspecified — message says
  only "my e-bike." Message: "My e-bike stopped charging. I bought it last
  year sometime, I think. Can you help?" No product model, no purchase date,
  no serial number, no order reference given or on file. Warranty status: 
  CANNOT BE DETERMINED from the information given — this is the point of the
  case. Location: Örebro. Expected agent behavior direction: ask for product
  model, purchase date/receipt, and serial number before any eligibility
  decision. Archetype encoded: D (Incomplete/Ambiguous Information Requiring
  Clarification).
  - (e) Target: 300 words (≈400 tokens).
  
  (h) cases/case-08-oskar-bergman.md

  - (b) Archetype D, second instance — record is complete but the stated
  symptom is genuinely ambiguous between two policy branches with different,
  already-elapsed-vs-still-open windows, forcing a clarifying question rather
  than a coin-flip.
  - (c) Schema as above.
  - (d) Case ID CASE-08. Date received: 2026-08-12. Channel: email. Customer:
  Oskar Bergman (NB-CUST-10048). Product: Fjord Cargo. Message: "My brakes
  feel off lately, not as sharp as before. Is this covered?" — no further
  symptom detail (no mention of squealing, reduced bite, fluid leak, or lever
  travel). Warranty status: AMBIGUOUS by design — purchased 2025-12-01: if the
  cause is normal brake-pad wear (Section 2, 6-month wear-item coverage),
  that window expired 2026-06-01, before today, so NOT eligible; if the cause
  is a hydraulic/caliper defect (Section 1, 24-month standard coverage, window
  through 2027-12-01), it IS eligible. The two branches disagree, so the case
  cannot be resolved without a clarifying question distinguishing wear
  symptoms (squealing, reduced bite, worn pad indicator) from defect symptoms
  (fluid leak, spongy/soft lever, lever pulling to the handlebar). Location:
  Linköping. Archetype encoded: D (Incomplete/Ambiguous Information Requiring
  Clarification).
  - (e) Target: 400 words (≈533 tokens).

  (i) cases/case-09-freja-holm.md

  - (b) Archetype C, second instance — an out-of-catalog modification/service
  request combined with a warranty-voiding exclusion, testing that the agent
  both declines the warranty claim on policy grounds AND escalates the
  custom-service request, rather than just doing one.
  - (c) Schema as above.
  - (d) Case ID CASE-09. Date received: 2026-08-11. Channel: chat. Customer:
  Freja Holm (NB-CUST-10049). Product: Aurora X3. Message: states she
  installed an aftermarket higher-wattage motor kit on her Aurora X3 and asks
  NordicBike to confirm the bike's warranty still officially covers it going
  forward, and separately asks whether NordicBike can service/certify the
  modified build. Warranty status: purchased 2026-02-10, well INSIDE the
  standard 24-month window by date — but the unauthorized electrical-system
  modification triggers the Section 4(ii) exclusion, voiding the Standard
  Limited Warranty in its entirety for the remainder of the coverage period,
  regardless of date. The service/certification request for the modified build
  is also out of catalog and must be escalated per policies/escalation.md
  trigger (iii). Location: Lund. Archetype encoded: C (Out-of-Scope Request
  Requiring Escalation), compounded with a Section 4 exclusion.
  - (e) Target: 550 words (≈733 tokens).

  (j) cases/case-10-gustav-akesson.md

  - (b) Clean control isolating the standalone spare-battery warranty term
  (Section 3, 12 months) from the whole-bike term (Section 1, 24 months) —
  tests that the agent applies the correct, shorter, product-type-specific
  term rather than defaulting to the 24-month figure it has seen most often.
  - (c) Schema as above.
  - (d) Case ID CASE-10. Date received: 2026-08-07. Channel: email. Customer:
  Gustav Åkesson (NB-CUST-10050). Product: PowerPack 720 (standalone spare
  battery, not part of a bike purchase). Message: reports the battery no
  longer holds more than roughly 50% of its original capacity. Warranty 
  status: INSIDE window — purchased 2025-11-01 as a standalone spare part, the
  12-month standalone battery term (Section 3) runs through 2026-11-01, which
  is after today (2026-08-14); eligible for replacement under Section 3, not
  Section 1. Location: Västerås. Archetype encoded: none (clean control on the
  standalone-battery term).
  - (e) Target: 450 words (≈600 tokens).

  ---
  7. .github/prompts/

  This workshop targets GitHub Copilot, not a generic or Claude-specific
  agent runtime. Copilot has no "Skill" file concept; its equivalent
  reusable, invocable, tool-scoped unit is a Prompt File
  (.github/prompts/*.prompt.md in VS Code) — a markdown file with a fixed
  YAML frontmatter schema (mode, description, and optionally tools/model)
  that is invoked via its filename as a slash command in Copilot Chat. All
  three files below are empty starter scaffolds only — no solved content, no
  filled-in examples. Every stub section's body must contain exactly the
  literal placeholder comment shown below, verbatim, and nothing else.

  Fixed required frontmatter fields for all three files (YAML, top of file):
  mode (string, fixed "agent" in all three — each of these is meant to run
  autonomously as part of the pipeline, not as a plain ask/edit chat),
  description (string, pre-filled with the fixed one-line value given per
  file below). No "name" or "version" field — Copilot prompt files have no
  such convention; the file's basename (minus .prompt.md) is its identity
  and its slash-command name.

  Fixed required body section stubs, identical structure across all three 
  files, in this order: ## Purpose, ## When to Use This Prompt, ## Required 
  Inputs, ## Steps, ## Output Format, ## Examples. Each section's body, in all
  three files, must contain exactly this placeholder line and nothing else:
  <!-- TODO: participant fills this in during Exercise 3 (subagent design) -->

  (a) .github/prompts/warranty-triage.prompt.md

  - (b) Scaffold for the triage subagent participants build in Exercise 3/4 —
  the piece that extracts structured facts from a case and produces the
  minimal typed payload (the "good pattern" JSON).
  - (c) Schema as fixed above.
  - (d) Frontmatter values: mode: agent, description: "Extracts 
  structured case facts and produces a minimal typed handoff payload for the 
  resolver." All six body sections contain only the fixed placeholder comment
  — no other content.
  - (e) Target: 100 words (≈133 tokens) — frontmatter plus six one-line
  placeholder bodies.
  
  (b) .github/prompts/policy-lookup.prompt.md

  - (b) Scaffold for a policy-section-retrieval helper — participants wire
  this to fetch only the specific policies/warranty.md section(s) relevant to
  a flagged archetype, rather than the whole file.
  - (c) Schema as fixed above.
  - (d) Frontmatter values: mode: agent, description: "Retrieves only 
  the specific policy section(s) relevant to a case's flagged archetype." All
  six body sections contain only the fixed placeholder comment.
  - (e) Target: 100 words (≈133 tokens).

  (c) .github/prompts/escalation-router.prompt.md
  
  - (b) Scaffold for the Exercise 4 escalation short-circuit logic — decides
  whether a case routes to the Tier-2 resolver or directly to the human
  escalation queue.
  - (c) Schema as fixed above.
  - (d) Frontmatter values: mode: agent, description: "Decides 
  whether a case routes to the resolver model or directly to human 
  escalation." All six body sections contain only the fixed placeholder
  comment.
  - (e) Target: 100 words (≈133 tokens).
  
  ---
  8. evaluation/

  (a) evaluation/scoring-rubric.md

  - (b) The single authoritative source for both the per-case rubric and the
  leaderboard budget-points formula — every number in presentation.md's
  scoring slide and facilitator-guide.md's leaderboard procedure must be
  copied from here verbatim.
  - (c) Required sections: ## Per-Case Rubric (0–20 points), ## Quality Gate 
  (pass/fail, 6 items), ## Cost-Weight Table, ## Budget-Points Formula, ## 
  Reference Benchmarks.
  - (d) Facts, exact and final:
    - Per-Case Rubric — five categories, each scored 0–4: Correct Eligibility
  Decision; Root-Cause Grounding (not symptom pattern-matching); Policy
  Citation Accuracy (correct section number); Escalation/Scope Judgment;
  Clarity & Tone. Max 20 points/case. Pass threshold for the Quality Gate's 
  "Q" input: ≥16/20 (80%).
    - Quality Gate — exactly 6 items, each pass/fail: (1) Cites the specific
  policy section number used for the decision. (2) States the eligibility
  outcome explicitly using one of Eligible / Not Eligible / Escalate / Cannot
  Determine — Clarify (or wording unambiguously equivalent to one of these),
  with a one-sentence justification tied to root cause, not symptom text alone. (3)
  The output is grounded in purchase date and product identity extracted from
  the case/customer record — directly, or via a triage subagent's typed
  payload — with no facts assumed or invented at any stage. (4) Flags and escalates any request
  matching a policies/escalation.md trigger rather than resolving it directly.
  (5) If information needed for the decision is missing from the case file,
  asks a clarifying question instead of guessing. (6) Response tone is
  professional, empathetic, concise, and in the language the customer's
  message is primarily written in.
    - Cost-Weight Table: reproduced identically from the global conventions
  block (Tier 1 = 1, Tier 2 = 4, Tier 3 = 12 per 1,000 tokens).
    - Budget-Points Formula, exact and final:
        - BCP (Baseline Cost Points) = 12 × 19.8 = 237.6 (Tier-3 weight × V1's
  19,800 tokens ÷ 1,000, one call).
      - CostPoints(call) = TierWeight × (tokens_in_call ÷ 1000).
      - TotalCostPoints(case) = Σ CostPoints(call) over every model call used
  to resolve that case.
      - CostEfficiency(case) = max(0, 1 − TotalCostPoints(case) / BCP).
      - M = mean(CostEfficiency(case)) across all 10 cases.
      - Q = (number of cases scoring ≥16/20 on the rubric AND passing all 6 
  quality-gate items) ÷ 10.
      - Penalty = 10 × (number of cases with a critical adversarial-archetype 
  failure, as defined in evaluation/adversarial-cases.md "must not" clauses).
      - FinalScore = round((Q × 70) + (M × 30) − Penalty, 1), clamped to the
  range [0, 100].
    - Reference Benchmarks (illustrative, labeled as such, not pass/fail
  requirements): V1 naive baseline ≈ Q 0.6, M 0.0 → FinalScore ≈ 42.0. Fully
  optimized V4 reference ≈ Q 1.0, M ≈ 0.95 → FinalScore ≈ 98.5.
  - (e) Target: 900 words (≈1,200 tokens).

  (b) evaluation/expected-results.md

  - (b) Complete answer key for all 10 cases — the ground truth the
  facilitator uses to grade the leaderboard; must never be shown to
  participants during the exercise.
  - (c) Required structure: one ## Case NN — <Customer Name> subsection per
  case, each containing fields: Eligibility Outcome, Policy Section(s) Cited,
  Root-Cause Determination, Escalation Flag (Y/N), Clarifying Question
  Required (Y/N, and if Y, the exact question), Archetype Encoded.
  - (d) Exact answer key, all 10 cases:
    a. Anna Karlsson — Eligible. Sections 1, 5. Root cause: manufacturing
  sealant defect (no water exposure). Escalation: N. Clarifying question: N.
  Archetype: A.
    b. Erik Svensson — Not Eligible. Sections 4(i), 5. Root cause: customer
  high-pressure washing near battery compartment. Escalation: N. Clarifying
  question: N. Archetype: A.
    c. Lena Björk — Eligible. Section 1. Root cause: structural weld defect.
  Escalation: N. Clarifying question: N. Archetype: none.
    d. Johan Lindqvist — Not Eligible (out of window). Section 1. Root cause:
  motor failure, but 24-month window expired 2025-11-05. Escalation: N.
  Clarifying question: N. Archetype: none.
    e. Sara Nilsson — Eligible. Section 6 (grandfathered 36-month term;
  Section 1 alone would incorrectly deny). Root cause: display defect.
  Escalation: N. Clarifying question: N. Archetype: B.
    f. Mikael Ström — Repair Eligible (Section 1) but Escalation: Y
  (compensation demand 5,000 SEK exceeds 2,000 SEK limit; explicit
  Konsumentverket threat). Clarifying question: N. Archetype: C.
    g. Ingrid Dahl — Outcome: Cannot Determine — Escalation: N. Clarifying
  question: Y — exact question: "Could you tell me which NordicBike model you
  have (Aurora X3, Fjord Cargo, or Vinter Pro), your approximate purchase
  date, and if possible your order confirmation or serial number, so I can
  check your warranty coverage?" Archetype: D.
    h. Oskar Bergman — Outcome: Cannot Determine without clarification —
  Escalation: N. Clarifying question: Y — exact question: "Could you describe
  the brake issue a bit more — is it squealing or reduced stopping power
  (which may be normal pad wear), or is the brake lever feeling spongy/soft or
  pulling closer to the handlebar (which may indicate a hydraulic issue)?"
  Archetype: D.
    i. Freja Holm — Not Eligible (Section 4(ii), unauthorized modification
  voids warranty) AND Escalation: Y (out-of-catalog service/certification
  request). Clarifying question: N. Archetype: C.
    j. Gustav Åkesson — Eligible. Section 3 (standalone 12-month battery term,
  not Section 1). Root cause: capacity degradation. Escalation: N. Clarifying
  question: N. Archetype: none.
  - (e) Target: 1,800 words (≈2,400 tokens).

  (c) evaluation/adversarial-cases.md

  - (b) The authoritative definition of all four archetypes, each with exact
  required and forbidden agent behavior — this is what the "critical
  adversarial failure" penalty in the budget-points formula checks against.
  - (c) Required structure: one ## Archetype <Letter> — <Name> subsection per
  archetype, each with ### Definition, ### Instantiated In, ### Correct 
  Behavior (Must), ### Incorrect Behavior (Must Not).
  - (d) Exact and final:
    - Archetype A — Symptom-Cause Confusion. Definition: two cases present an
  identical visible symptom (and may share product batch/bulletin membership)
  but have different underlying root causes, and only one root cause qualifies
  under policy. Instantiated in: Cases 1, 2. Must: determine eligibility from
  the customer-stated or verified root cause, not from symptom text or batch
  membership alone; cite the specific exclusion or bulletin section. Must not:
  grant or deny eligibility based on symptom match or serial-batch match
  alone; must not treat Case 2's SB-2025-11 batch membership as sufficient for
  eligibility once a Section 4(i) root cause is present.
    - Archetype B — Superseded/Grandfathered-Policy Trap. Definition: a case's
  correct eligibility depends on a non-default, date-windowed policy clause
  (grandfather clause) that a shallow read of only the "current standard term"
  would miss. Instantiated in: Case 5. Must: check for and apply any
  applicable grandfather/legacy clause in policies/warranty.md Section 6
  before concluding based on the standard Section 1 term alone. Must not: deny
  a claim solely because it fails the standard 24-month term without checking
  whether a legacy promotional term also applies to that purchase date.
    - Archetype C — Out-of-Scope Request Requiring Escalation. Definition: a
  case contains a request or demand exceeding agent authority as defined in
  policies/escalation.md (legal/regulatory threat, compensation demand >2,000
  SEK, or custom/off-catalog modification/service request). Instantiated in:
  Cases 6, 9. Must: recognize the trigger and escalate to
  support-lead@nordicbike.se rather than resolving the demand directly; may
  still state the underlying warranty-repair eligibility determination (which
  is separable from the escalated demand) when applicable. Must not: authorize
  any refund/compensation payment above 2,000 SEK; must not attempt to
  resolve or promise resolution of a legal threat; must not agree to service
  or certify an out-of-catalog modification.
    - Archetype D — Incomplete/Ambiguous Information Requiring Clarification.
  Definition: a case is missing a decision-critical fact (product identity,
  purchase date/proof, serial number) or contains a symptom description that
  maps to two policy branches with different outcomes. Instantiated in: Cases
  7, 8. Must: ask a specific clarifying question naming exactly what is
  missing or ambiguous, and withhold a final eligibility determination until
  it is resolved. Must not: guess or assume a purchase date, product, or
  symptom category to force a determination; must not default to either
  "eligible" or "not eligible" without the missing fact.
  - (e) Target: 1,200 words (≈1,600 tokens).

  ---

  9. Subagent Handoff Examples (fixed reference block — reproduced verbatim in
  workshops/exercise-3-subagent-handoff.md, presentation.md slide 16–17, and 
  facilitator-guide.md)

  Bad pattern — full-context dump (do not build this):
  
  {
    "company_md": "<entire contents of company/about.md>",
    "support_contacts_md": "<entire contents of company/support-contacts.md>",
    "products": {
      "aurora_x3": "<entire contents of products/aurora-x3.md>",
      "fjord_cargo": "<entire contents of products/fjord-cargo.md>",
      "vinter_pro": "<entire contents of products/vinter-pro.md>",
      "powerpack_batteries": "<entire contents of 
  products/powerpack-batteries.md>",
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

  Good pattern — minimal typed payload (build this):
  {
    "handoff_type": "typed_decision_payload",
    "case_id": "CASE-01",
    "customer_id": "NB-CUST-10041",
    "product_sku": "AX3",
    "product_name": "Aurora X3",
    "serial_number": "AX3-25A-00417",
    "purchase_date": "2025-03-10",
    "warranty_window_end_standard": "2027-03-10",
    "today_date": "2026-08-14",
    "stated_symptom": "intermittent power loss, bike will not hold charge",
    "candidate_archetype": "symptom_cause_confusion",
    "applicable_policy_sections": ["warranty.md#section-5",
  "warranty.md#section-4"],
    "root_cause_flags": {
      "water_exposure_reported": false,
      "pressure_washed_near_battery": false
    },
    "recommended_model_tier": "tier_2"
  }

  ---
  10. README.md

  - (b) Pedagogical function: the single entry point; must let a facilitator
  or participant orient in under 2 minutes and find every other file.
  - (c) Required sections, exact order: ## Overview, ## Prerequisites, ## 
  Repository Map, ## How to Run Each Exercise, ## Facilitator-Only 
  Instructions (clearly marked, e.g. with a leading "⚠ Facilitators only — do
  not share this section with participants before the workshop" note; contains
  only a pointer sentence: "See facilitator-guide.md for setup, exact
  exercise wording, timing checkpoints, and the leaderboard procedure."), ## 
  Participant-Facing Instructions (pointer sentence: "Start with
  workshops/exercise-1-baseline.md and proceed in order through
  exercise-5-quality-gate.md.").
  - (d) Facts: Overview — restates workshop title "Agent Optimization
  Challenge," 2-hour hands-on session, NordicBike AB case, teaches skills
  design, context engineering, subagent handoff patterns, model routing,
  evaluation. Prerequisites: basic familiarity with LLM API calls and prompt
  construction; no NordicBike domain knowledge assumed. Repository Map: one
  line per top-level directory (company/, products/, policies/, customers/,
  workshops/, cases/, .github/prompts/, evaluation/) stating its purpose in ≤15 words
  each, plus presentation.md, facilitator-guide.md, updatenumbers.md (see
  Section 13 below), and export-participant-repo.sh (see Section 14 below).
  Every Repository Map bullet and paragraph describing facilitator-only or
  maintainer-only content, and the entire Facilitator-Only Instructions
  section, must be wrapped in a paired
  `<!-- PARTICIPANT-EXPORT:EXCLUDE-START -->` /
  `<!-- PARTICIPANT-EXPORT:EXCLUDE-END -->` HTML-comment marker so that
  export-participant-repo.sh (Section 14) can mechanically derive a
  participant-safe README from this one — see that section for the exact
  contract. How to Run Each
  Exercise: numbered list of the 5 exercises by filename, one line each,
  pointing to workshops/exercise-N-*.md. The Facilitator-Only and
  Participant-Facing sections must use the exact pointer sentences given above
  — no additional content duplicated from facilitator-guide.md.
  - (e) Target: 1,200 words (≈1,600 tokens).

  ---
  11. presentation.md

  - (b) Pedagogical function: the instructor deck driving live delivery of all
  5 phases; must be detailed enough to draft slides from with zero additional
  judgment calls.
  - Slide-delimiting convention (fixed): ##-per-slide. Each slide is a level-2
  heading; no --- delimiters are used anywhere in this file.
  - Exact total slide count: 24.
  - (c) Required schema: # Agent Optimization Challenge as the single H1 at
  the top, followed by exactly 24 ## slides in the order below. Each slide's
  body is bullet points only (no prose paragraphs).
  - (d) Per-slide content, exact and final:
    a. Title & Agenda — Title "Agent Optimization Challenge," subtitle
  "NordicBike AB — a 2-hour hands-on agent-engineering workshop," agenda
  bullets: Kickoff → Baseline → Optimization → Routing & Quality →
  Leaderboard.
    b. Learning Objectives — bullets: skills design; context engineering;
  subagent/multi-agent handoff patterns; model routing; evaluation.
    c. Pedagogy Rationale I — Context as a Scarce Resource — bullets: every
  token costs money and latency; irrelevant context degrades accuracy, not
  just cost; the V1→V4 progression makes this measurable.
    d. Pedagogy Rationale II — Progressive Disclosure & Handoff Discipline —
  bullets: retrieve only what's needed, when it's needed; subagent handoffs
  should pass typed facts, not full context; routing cheap models to cheap
  decisions frees budget for hard cases.
    e. Meet NordicBike AB — founded 2019, HQ Stockholm, ~85 employees, online
  + 3 service centers (Stockholm, Gothenburg, Malmö).
    f. The Product Line — Aurora X3 34,900 SEK, Fjord Cargo 44,900 SEK, Vinter
  Pro 37,900 SEK, PowerPack 720/900, accessories.
    g. The Support Case Load — 10 cases, 4 adversarial archetypes, each
  archetype appears ≥1 time.
    h. Workshop Format — 120 Minutes, 5 Phases — table reproducing the 5-phase
  timing (Phase 1: 15 min, Phase 2: 20 min, Phase 3: 45 min, Phase 4: 30 min,
  Phase 5: 10 min).
    i. Phase 1 — Kickoff & Case Introduction (15 min) — bullets: intro
  NordicBike case; state learning objectives; form teams.
    j. Phase 2 — Baseline Run & Diagnosis (20 min) — Exercise 1 — bullets: run
  naive V1 agent on Case 1; measure 19,800 tokens / 1 call / Tier 3; diagnose
  waste sources.
    k. Phase 3 — Context & Handoff Optimization (45 min) — Exercises 2–3 —
  bullets: build V2 (≤9,900 tokens, context trimming); build V3 (≤5,500
  tokens, subagent handoff with minimal payload).
    l. Phase 4 — Model Routing & Quality Gate (30 min) — Exercises 4–5 —
  bullets: build V4 (≤3,800 tokens, Tier 1 + Tier 2 routing, escalation
  short-circuit); run the 6-item quality gate on all 10 cases.
    m. Phase 5 — Leaderboard & Debrief (10 min) — bullets: final scoring run
  across all teams; leaderboard reveal; retro discussion.
    n. Meet the Naive Agent — V1 Baseline — 19,800 tokens, 1 call, Tier 3,
  dumps every KB file + full customer record + full case text.
    o. The V1→V4 Token-Load Progression — chart/table: V1 19,800 → V2 9,900 →
  V3 5,500 → V4 3,800 tokens (reproduce the full table from the global
  conventions block, including the mechanism/calls/tier columns).
    p. Subagent Handoff — The Bad Pattern — reproduce the fixed "bad pattern"
  JSON example verbatim from Section 9 of this spec.
    q. Subagent Handoff — The Good Pattern — reproduce the fixed "good
  pattern" JSON example verbatim from Section 9 of this spec.
    r. Model-Routing Table — reproduce the Tier 1/2/3 cost-weight table
  verbatim from the global conventions block, plus the routing rule: triage on
  Tier 1, resolver on Tier 2, escalation-flagged cases skip the resolver call
  entirely.
    s. The Anna & Erik Case — Same Fault, Different Outcome — side-by-side:
  Anna (eligible, no water exposure) vs. Erik (not eligible, pressure-washed
  battery compartment), same batch AX3-25A, same symptom, different root
  cause, Section 4(i) vs. Section 5.
  20. The Four Adversarial Archetypes — one line each for A (Symptom-Cause
  Confusion), B (Superseded/Grandfathered-Policy Trap), C (Out-of-Scope
  Escalation), D (Incomplete/Ambiguous Information), with which cases
  instantiate each.
  21. Quality-Gate Checklist — reproduce all 6 items verbatim from
  evaluation/scoring-rubric.md.
  22. Scoring Rubric & Budget-Points Formula — reproduce the 5-category 0–20
  rubric and the full FinalScore = (Q × 70) + (M × 30) − Penalty formula with
  the BCP = 237.6 constant, verbatim from evaluation/scoring-rubric.md.
  23. Leaderboard Mechanics — bullets: each team submits their V4 agent's
  output for all 10 cases; facilitator scores against
  evaluation/expected-results.md and the rubric; FinalScore computed per
  formula; ranked leaderboard displayed live.
  24. What You Learned — bullets restating the 5 learning objectives from
  Slide 2, each with a one-line "you now can..." statement (skills design →
  you can scaffold a reusable skill; context engineering → you can cut context
  5x without losing correctness; handoff patterns → you can design typed
  payloads instead of context dumps; model routing → you can route by
  cost/complexity; evaluation → you can build a quality gate and a scoring
  rubric).
  - (e) Target: 2,900 words total across all 24 slides (≈3,867 tokens), i.e.,
  ≈120 words average per slide.

  ---
  12. facilitator-guide.md
  
  - (b) Pedagogical function: the complete, standalone operating manual for
  whoever runs the session live — contains everything presentation.md and
  README.md deliberately omit (verbatim participant-facing prompts, timing
  checkpoints, leaderboard mechanics as an operational procedure rather than a
  slide summary).
  - (c) Required sections, exact order: ## Setup Instructions, ## The 5 
  Exercise Prompts (verbatim), ## Timing Checkpoints, ## Leaderboard Running 
  Procedure.
  - (d) Facts, exact and final:
    - Setup Instructions: clone the repo; confirm each team has access to Tier
  1/2/3 model endpoints; confirm each team can view
  workshops/exercise-1-baseline.md through exercise-5-quality-gate.md but NOT
  evaluation/expected-results.md or evaluation/adversarial-cases.md before
  Phase 5; project presentation.md slides 1–13 before Phase 2 begins; hold
  slides 14–24 until after Exercise 1's baseline measurement is complete (so
  teams measure the baseline themselves before seeing the reference figures).
    - The 5 Exercise Prompts (verbatim) — reproduce, byte-for-byte identical
  to the ## Constraint text in each corresponding workshops/exercise-N-*.md
  file, all 5 prompts in full:
        i. "Run the baseline agent exactly as provided against Case 1. Do not
  modify it. Record: total input tokens, model tier used, number of model
  calls, and the agent's eligibility decision. This is your baseline to beat."
      ii. "Reduce the total input context by at least 50% (target: ≤50% of
  your Exercise 1 measurement; the reference figure is 9,900 tokens) while
  keeping exactly one model call and the same model tier. You
  may not drop any relevant source file entirely if it is relevant to the case at
  hand — you must excerpt, not omit, relevant material. Output quality
  (correctness of the eligibility decision) must not regress."
      iii. "Split your agent into two calls: a triage subagent that extracts
  structured facts from the case and customer record, and a resolver subagent
  that makes the eligibility decision. The triage subagent's output to the
  resolver must be a minimal typed JSON payload — no full-context dumps are
  permitted between subagents. Target ≤5,500 total input tokens across both
  calls, same model tier as Exercise 2."
      iv. "Apply the model-routing table: triage calls run on Tier 1, resolver
  calls run on Tier 2. If the triage subagent detects any escalation trigger
  from policies/escalation.md, route directly to the human escalation queue
  and do not make a resolver call at all. Target ≤3,800 total input tokens
  across all calls for non-escalated cases."
      v. "Run every one of your 10 case outputs through the 6-item quality
  gate in evaluation/scoring-rubric.md. Any output that fails even one item is
  not eligible for leaderboard submission until fixed. Fixing a quality-gate
    - Timing Checkpoints — tied to the 5-phase, 120-minute schedule: 00:00
  Phase 1 starts; 00:15 Phase 2 starts (Exercise 1); 00:35 Phase 3 starts
  (Exercises 2–3, checkpoint at 00:57–01:00 for Exercise 2 complete, Exercise
  (Exercises 2–3, checkpoint at 00:57–01:00 for Exercise 2 complete, Exercise
  3 by 01:20); 01:20 Phase 4 starts (Exercises 4–5, checkpoint at 01:40 for
  Exercise 4 complete, Exercise 5 by 01:50); 01:50 Phase 5 starts (leaderboard
  + debrief); 02:00 close.
    - Leaderboard Running Procedure — exact steps: (1) collect each team's V4
  agent outputs for all 10 cases; (2) score each output against
  evaluation/expected-results.md for correctness and
  evaluation/scoring-rubric.md's 5-category rubric for the 0–20 per-case
  score; (3) run the 6-item quality gate per case to determine pass/fail for
  Q; (4) compute TotalCostPoints per case from the team's reported tier/token
  usage per call; (5) compute CostEfficiency per case and M as the mean across
  10 cases; (6) check each case against evaluation/adversarial-cases.md "must
  not" clauses to count critical failures for Penalty; (7) compute FinalScore
  = (Q × 70) + (M × 30) − Penalty, clamp to [0,100]; (8) rank teams
  descending by FinalScore and display live.
  - (e) Target: 2,500 words (≈3,333 tokens).

  ---
  13. updatenumbers.md

  - (b) Pedagogical function: none — this is a maintainer-only runbook, not
  participant- or facilitator-during-a-session material. It exists because
  every fixed token/cost figure in this spec (19,800 / 9,900 / 5,500 / 3,800
  tokens, BCP = 237.6, and everything derived from them) is deliberately
  reproduced byte-for-byte across many files for pedagogical consistency
  within a single session, but is not guaranteed to stay an honest
  reflection of the actual V1 bundle's real size as content elsewhere in the
  repository changes over time, and because this workshop's tier naming is
  deliberately model-agnostic (see the Target tooling decision at the top of
  this spec) — something still has to map "Tier 1/2/3" to concrete, current
  GitHub Copilot models for each actual delivery, and that mapping must not
  be written into any fixed, reproduced document.
  - (c) Required sections: ## What this file is for, ## When to run this,
  ## What this file does not cover, ## Step 1 through a final numbered step
  covering: re-measuring the V1 baseline from the real files in a GitHub
  Copilot session; deciding whether the drift is large enough to warrant
  re-deriving the downstream figures; recomputing V2/BCP (which are
  mathematical functions of V1) while treating V3's and V4's triage/resolver
  splits as independently fixed design choices, not V1-derived; an
  exhaustive location-by-location list of every file that must be edited in
  the same pass for each of the 19,800 / 9,900 / 5,500 / 3,800 / 237.6
  figures; refreshing the Tier 1/2/3 → concrete Copilot model mapping (this
  step is run every time, independent of whether any token figure changed);
  re-verifying cross-file consistency (Constraint-block/prompt matching,
  Cost-Weight Table matching, Subagent Handoff Examples matching); and a
  changelog table recording what changed and when.
  - (d) Facts, exact and final: this file is a checklist/runbook that a
  maintainer or agent works through by hand — it must not be framed as an
  auto-executing script, since the fixed pedagogical constants it governs
  are deliberately not meant to change on every content edit, only on a
  deliberate re-baselining decision. It must state explicitly that it is
  scoped only to token/cost/model-tier figures, not to any of NordicBike's
  fictional business data (prices, dates, names, serials), which never goes
  stale and is out of scope. README.md's Repository Map must reference this
  file per Section 10 above.
  - (e) Target: 1,600 words (≈2,133 tokens).

  ---
  14. export-participant-repo.sh

  - (b) Pedagogical function: none — maintainer-only tooling, like Section 13.
  This exists because this repo is a single git clone containing both
  participant-facing content and facilitator/maintainer-only content
  (evaluation/expected-results.md, evaluation/adversarial-cases.md,
  facilitator-guide.md, presentation.md, updatenumbers.md). Prior to this
  script, the boundary between the two was enforced only by documentation
  telling people not to open certain files — anyone who actually clones this
  repo has those files on disk regardless. This script produces a genuinely
  separate, fresh-history git repo containing only the Participant tier, so
  there is no facilitator-only file present for a participant to find, by
  accident or otherwise.
  - (c) Required behavior, exact and final: copies exactly the Participant
  tier (company/, products/, policies/, customers/, cases/, workshops/,
  .github/, evaluation/scoring-rubric.md — nothing else from evaluation/) to
  a target directory; derives README.md for the target by stripping every
  span between paired `<!-- PARTICIPANT-EXPORT:EXCLUDE-START -->` /
  `<!-- PARTICIPANT-EXPORT:EXCLUDE-END -->` HTML-comment markers in this
  repo's own README.md, rather than hand-maintaining a second README; runs
  `git init` at the target with no history carried over from this repo
  (never `git clone`-then-delete, since deleted files remain fully readable
  in git history); never touches any git remote or pushes anywhere itself.
  Must fail loudly (not silently) if the README's EXCLUDE-START and
  EXCLUDE-END marker counts are unbalanced, since an unpaired marker means a
  README edit broke the export contract without anyone noticing.
  - (d) Facts, exact and final: the participant-tier path list inside the
  script and the marker-delimited spans inside README.md are the two places
  that must be kept in sync whenever a new top-level file or directory is
  added to this repo — deciding a new file's tier (Participant /
  Facilitator-only / Maintainer-only) is a judgment call for whoever adds
  it, not something this script can infer. Re-running this script is a
  routine, repeatable pre-session step, not a one-time setup — the same
  operational posture as updatenumbers.md.
  - (e) Target: 550 words (≈733 tokens) — this is a script with a substantial
  header comment, not prose documentation; do not pad it with additional
  commentary purely to hit a word target.


