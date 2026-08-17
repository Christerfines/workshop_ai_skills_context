#!/usr/bin/env bash
#
# tests/copilot/run.sh — semi-automated playtest. GitHub Copilot has no local
# unattended multi-step CLI, so this script drives YOU: it copies the
# workshop, prints each exercise's prompt in turn, and waits for you to paste
# back what Copilot Chat (in VS Code, opened on the printed working-copy
# path) produced. Same deliverable format as the Claude lane, so the same
# grader works on either.
#
# Usage:
#   tests/copilot/run.sh                        # Case 1 only, smoke test
#   tests/copilot/run.sh --cases all             # full 10-case Exercise 5
#   tests/copilot/run.sh --target /some/path     # override the working copy location

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"

CASE_SCOPE_FLAG="case1"
TARGET_DIR="$REPO_ROOT/../test_workshop/copilot"

while [ $# -gt 0 ]; do
  case "$1" in
    --cases) CASE_SCOPE_FLAG="$2"; shift 2 ;;
    --target) TARGET_DIR="$2"; shift 2 ;;
    *) echo "Unknown argument: $1" >&2; exit 1 ;;
  esac
done

if [ "$CASE_SCOPE_FLAG" = "all" ]; then
  REPORT_SCOPE="all 10 cases (full run)"
else
  REPORT_SCOPE="Case 1 only (smoke test)"
fi

echo "== Copying participant-tier content to $TARGET_DIR =="
rm -rf "$TARGET_DIR"
mkdir -p "$(dirname "$TARGET_DIR")"
"$REPO_ROOT/export-participant-repo.sh" "$TARGET_DIR"
mkdir -p "$TARGET_DIR/deliverables"

echo
echo "Before continuing:"
echo "  1. Open $TARGET_DIR in VS Code with GitHub Copilot Chat (agent mode)."
echo "  2. Fill in tier -> Copilot model choices in: $SCRIPT_DIR/tier_mapping.md"
echo "     (mirrors updatenumbers.md Step 5 — there's no fixed mapping to assume)."
read -r -p "Press Enter once you're ready to start Exercise 1... " _

for N in 1 2 3 4 5; do
  echo
  echo "############################################################"
  echo "  EXERCISE $N of 5"
  echo "############################################################"
  echo
  echo "1) In Copilot Chat, tell it: \"Read workshops/exercise-$N-*.md in this"
  echo "   repository and follow its Goal, Starting Point, Constraint, Target"
  echo "   Metric, and Deliverable exactly as written. Use the tier -> model"
  echo "   mapping in tier_mapping.md for any per-tier call the exercise"
  echo "   requires.\""
  if [ "$N" != "1" ]; then
    echo "   Also tell it to read its own deliverables/exercise-$((N-1)).md"
    echo "   (and earlier) as its starting point, same as a real handoff."
  fi
  if [ "$N" = "5" ] && [ "$CASE_SCOPE_FLAG" != "all" ]; then
    echo "   SCOPE THIS RUN TO CASE 1 ONLY (smoke test) rather than all 10 cases."
  fi
  echo
  echo "2) When it's done, ask it to also write, at the end of its own answer:"
  echo "   a '## Notes & Friction' section — anything about the exercise's"
  echo "   wording that was ambiguous or hard to satisfy exactly as written."
  echo
  echo "3) Paste the COMPLETE deliverable (including Notes & Friction) below."
  echo "   Finish with a line containing only: EOF"
  echo

  OUT="$TARGET_DIR/deliverables/exercise-$N.md"
  : > "$OUT"
  while IFS= read -r line; do
    [ "$line" = "EOF" ] && break
    printf '%s\n' "$line" >> "$OUT"
  done

  if [ ! -s "$OUT" ]; then
    echo "WARNING: nothing captured for exercise $N — deliverables/exercise-$N.md is empty." >&2
  fi
done

echo
echo "== Grading the run =="
REPORT_PATH="$("$REPO_ROOT/tests/common/grade.sh" copilot "$TARGET_DIR" "$REPORT_SCOPE")"

echo
echo "Done. Report: $REPORT_PATH"
echo "Working copy left at: $TARGET_DIR (safe to delete: rm -rf $TARGET_DIR)"
