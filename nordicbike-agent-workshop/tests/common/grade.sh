#!/usr/bin/env bash
#
# common/grade.sh — grades a completed playtest run and writes a report.
# Run from the real repo (needs evaluation/expected-results.md and
# evaluation/adversarial-cases.md, never present in a working copy).
#
# Usage: common/grade.sh <lane> <target_dir> <case_scope_description>
#   lane                 "claude" or "copilot" — just a label for the report
#   target_dir           the working copy containing deliverables/exercise-N.md
#   case_scope_description  e.g. "Case 1 only (smoke test)" or "all 10 cases"
#
# Writes tests/reports/<UTC-timestamp>-<lane>.md and prints its path on success.

set -euo pipefail

LANE="${1:?usage: grade.sh <lane> <target_dir> <case_scope_description>}"
TARGET_DIR="${2:?usage: grade.sh <lane> <target_dir> <case_scope_description>}"
CASE_SCOPE="${3:-Case 1 only (smoke test)}"

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
GRADER_MODEL="claude-opus-5"

TARGET_DIR="$(cd "$TARGET_DIR" && pwd)"
DELIVERABLES_DIR="$TARGET_DIR/deliverables"
if [ ! -d "$DELIVERABLES_DIR" ]; then
  echo "Error: no deliverables/ found at $DELIVERABLES_DIR — did the run actually produce output?" >&2
  exit 1
fi

WORK="$(mktemp -d)"
trap 'rm -rf "$WORK"' EXIT

echo "Assembling exercise specs (Target Metric + Constraint, verbatim) ..." >&2
EXERCISE_SPECS_FILE="$WORK/exercise_specs.md"
: > "$EXERCISE_SPECS_FILE"
for f in "$REPO_ROOT"/workshops/exercise-*.md; do
  {
    echo "### $(basename "$f")"
    awk '/^## Constraint/,/^## Target Metric/' "$f" | sed '$d'
    awk '/^## Target Metric/,/^## Deliverable/' "$f" | sed '$d'
    echo
  } >> "$EXERCISE_SPECS_FILE"
done

echo "Assembling answer key (evaluation/) ..." >&2
ANSWER_KEY_FILE="$WORK/answer_key.md"
cat "$REPO_ROOT/evaluation/expected-results.md" \
    "$REPO_ROOT/evaluation/adversarial-cases.md" \
    "$REPO_ROOT/evaluation/scoring-rubric.md" > "$ANSWER_KEY_FILE" 2>/dev/null \
  || { echo "Error: evaluation/*.md not found — this script must run from the maintainer repo, not a participant export." >&2; exit 1; }

echo "Assembling test-agent deliverables ..." >&2
DELIVERABLES_FILE="$WORK/deliverables.md"
: > "$DELIVERABLES_FILE"
TOTAL_COST="0"
for f in "$DELIVERABLES_DIR"/exercise-*.md; do
  [ -e "$f" ] || continue
  { echo "### $(basename "$f")"; cat "$f"; echo; } >> "$DELIVERABLES_FILE"
done
# Sum real cost across every captured tiered-model call, if present.
if compgen -G "$DELIVERABLES_DIR"/exercise-*-calls/*.json > /dev/null 2>&1; then
  TOTAL_COST="$(jq -s '[.[].total_cost_usd] | add' "$DELIVERABLES_DIR"/exercise-*-calls/*.json 2>/dev/null || echo "0")"
fi
echo "### Measured cost across all tiered-model calls: \$${TOTAL_COST}" >> "$DELIVERABLES_FILE"

echo "Building the grading prompt ..." >&2
PROMPT_FILE="$WORK/grade_prompt.md"
python3 - "$SCRIPT_DIR/grade_prompt_template.md" "$PROMPT_FILE" \
         "$LANE" "$CASE_SCOPE" "$EXERCISE_SPECS_FILE" "$ANSWER_KEY_FILE" "$DELIVERABLES_FILE" <<'PY'
import sys
tpl_path, out_path, lane, case_scope, specs_path, key_path, deliv_path = sys.argv[1:8]
tpl = open(tpl_path).read()
specs = open(specs_path).read()
key = open(key_path).read()
deliv = open(deliv_path).read()
out = (tpl.replace("{LANE}", lane)
          .replace("{CASE_SCOPE}", case_scope)
          .replace("{EXERCISE_SPECS}", specs)
          .replace("{ANSWER_KEY}", key)
          .replace("{DELIVERABLES}", deliv))
open(out_path, "w").write(out)
PY

echo "Grading with $GRADER_MODEL (this is one more real API call) ..." >&2
RAW_JSON="$WORK/raw_grade.json"
claude -p "$(cat "$PROMPT_FILE")" --model "$GRADER_MODEL" --output-format json \
  --dangerously-skip-permissions > "$RAW_JSON"

TS="$(date -u +%Y%m%dT%H%M%SZ)"
REPORT_PATH="$REPO_ROOT/tests/reports/${TS}-${LANE}.md"
mkdir -p "$REPO_ROOT/tests/reports"

GRADE_COST="$(jq -r '.total_cost_usd // 0' "$RAW_JSON")"
{
  echo "# Playtest Report — ${LANE} lane"
  echo
  echo "- Generated: $TS"
  echo "- Working copy: $TARGET_DIR"
  echo "- Case scope: $CASE_SCOPE"
  echo "- Measured cost across tiered-model calls: \$${TOTAL_COST}"
  echo "- Grading-pass cost ($GRADER_MODEL): \$${GRADE_COST}"
  echo
  echo "---"
  echo
  jq -r '.result' "$RAW_JSON"
} > "$REPORT_PATH"

echo "$REPORT_PATH"
