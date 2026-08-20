#!/usr/bin/env bash
#
# tests/claude/run.sh — fully automated playtest: an unattended headless
# Claude Code session works through Exercises 1-5 using ONLY the
# participant-facing repo (never sees evaluation/expected-results.md or
# evaluation/adversarial-cases.md), then a separate grading pass (run from
# this repo, which does have the answer key) produces a report.
#
# Usage:
#   tests/claude/run.sh                        # Case 1 only, smoke test
#   tests/claude/run.sh --cases all             # full 10-case Exercise 5
#   tests/claude/run.sh --target /some/path     # override the working copy location
#   tests/claude/run.sh --fresh                 # wipe the working copy and start over
#                                                 (default: resume — skip any
#                                                 exercise whose deliverable
#                                                 already exists on disk)
#
# Real Anthropic API spend happens here — five (or more) headless `claude -p`
# calls playing the participant, plus nested per-tier calls the test agent
# makes itself, plus one grading call at the end. This can run 20-40+
# minutes unattended, so it re-execs itself under `caffeinate` (if available)
# to stop the machine sleeping mid-run and silently truncating a response —
# that's the #1 way an unattended run actually fails.

set -euo pipefail

if command -v caffeinate >/dev/null 2>&1 && [ -z "${_PLAYTEST_CAFFEINATED:-}" ]; then
  export _PLAYTEST_CAFFEINATED=1
  exec caffeinate -dimsu "$0" "$@"
fi

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
DRIVER_MODEL="claude-sonnet-5"

CASE_SCOPE_FLAG="case1"
TARGET_DIR="$REPO_ROOT/../test_workshop/claude"
FRESH=0

while [ $# -gt 0 ]; do
  case "$1" in
    --cases) CASE_SCOPE_FLAG="$2"; shift 2 ;;
    --target) TARGET_DIR="$2"; shift 2 ;;
    --fresh) FRESH=1; shift ;;
    *) echo "Unknown argument: $1" >&2; exit 1 ;;
  esac
done

if [ "$CASE_SCOPE_FLAG" = "all" ]; then
  CASE_SCOPE_1_4="Case 1 (cases/case-01-anna-karlsson.md) — Exercises 1-4 are always single-case, per their own Starting Point sections"
  CASE_SCOPE_5="all 10 cases in cases/ — the full quality-gate pass"
  REPORT_SCOPE="all 10 cases (full run)"
else
  CASE_SCOPE_1_4="Case 1 (cases/case-01-anna-karlsson.md)"
  CASE_SCOPE_5="Case 1 only (cases/case-01-anna-karlsson.md) — NOT the full 10-case set; this is a smoke test, note that explicitly in your Notes & Friction"
  REPORT_SCOPE="Case 1 only (smoke test)"
fi

if [ "$FRESH" = "1" ] || [ ! -d "$TARGET_DIR" ]; then
  echo "== Copying participant-tier content to $TARGET_DIR ==" >&2
  rm -rf "$TARGET_DIR"
  mkdir -p "$(dirname "$TARGET_DIR")"
  "$REPO_ROOT/export-participant-repo.sh" "$TARGET_DIR" >&2
else
  echo "== Reusing existing working copy at $TARGET_DIR (pass --fresh to start over) ==" >&2
fi

mkdir -p "$TARGET_DIR/deliverables"

for N in 1 2 3 4 5; do
  if [ -s "$TARGET_DIR/deliverables/exercise-$N.md" ]; then
    echo "== Exercise $N: already have a deliverable on disk — skipping (pass --fresh to redo) ==" >&2
    continue
  fi
  echo "== Exercise $N: running headless $DRIVER_MODEL session in $TARGET_DIR ==" >&2

  if [ "$N" = "1" ]; then
    PRIOR_CONTEXT="This is the first exercise — there is no prior deliverable. Start from the workshop's naive V1 baseline agent as described in the exercise file."
  else
    PRIOR_CONTEXT="Read your own prior deliverable(s) at deliverables/exercise-$((N-1)).md (and earlier, if referenced) in this directory as your starting point — exactly as the exercise file's own Starting Point section describes."
  fi

  if [ "$N" = "5" ]; then
    CASE_SCOPE="$CASE_SCOPE_5"
  else
    CASE_SCOPE="$CASE_SCOPE_1_4"
  fi

  PROMPT="$(sed \
    -e "s/{N}/$N/g" \
    -e "s|{PRIOR_CONTEXT}|$PRIOR_CONTEXT|" \
    -e "s|{CASE_SCOPE}|$CASE_SCOPE|" \
    "$SCRIPT_DIR/task_prompt_template.md")"

  OUT_JSON="$TARGET_DIR/deliverables/exercise-$N.session.json"
  (
    cd "$TARGET_DIR"
    claude -p "$PROMPT" --model "$DRIVER_MODEL" --output-format json \
      --dangerously-skip-permissions > "$OUT_JSON"
  )

  if [ ! -s "$TARGET_DIR/deliverables/exercise-$N.md" ]; then
    echo "WARNING: exercise-$N.md was not written — the session's own output is in $OUT_JSON for debugging." >&2
  fi
done

echo "== Grading the run ==" >&2
REPORT_PATH="$("$REPO_ROOT/tests/common/grade.sh" claude "$TARGET_DIR" "$REPORT_SCOPE")"

echo >&2
echo "Done. Report: $REPORT_PATH" >&2
echo "Working copy left at: $TARGET_DIR (safe to delete: rm -rf $TARGET_DIR)" >&2
echo "$REPORT_PATH"
