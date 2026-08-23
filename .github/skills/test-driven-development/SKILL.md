---
name: test-driven-development
description: "Use when implementing a feature, fixing a bug, refactoring behaviour, or changing a contract where a test can express the expected result before production code is written."
---

# Test-Driven Development

## Purpose

Turn an intended behaviour into a small executable contract, prove that the contract can fail, make the smallest change that satisfies it, and then refactor without changing behaviour.

## Boundaries

This skill governs implementation work. It does not replace investigation of an unknown failure, code review, release preparation, or verification of an already completed change. Do not write production code first and retrofit a test. Do not broaden the feature, refactor unrelated code, or add framework machinery that the requested behaviour does not need. For prototypes, generated files, and configuration-only changes, explain the exception and ask before skipping the cycle.

## Inputs

Before writing code, identify:

- the behaviour a user or caller should observe;
- the smallest existing test project, test runner, or executable check;
- the relevant local fixture and boundary;
- constraints such as public API, error shape, persistence, or performance;
- the command that will run the focused test.

If the desired behaviour is ambiguous, ask one question that would change the test rather than guessing an implementation.

## Workflow

1. **Name one behaviour.** State the smallest observable input and expected result. A test name containing multiple independent behaviours is a signal to split the test.
2. **Write RED.** Add one focused test against real code where practical. Assert behaviour, not private implementation details or mock call counts. Keep the fixture minimal.
3. **Verify RED.** Run the focused test before changing production code. It must fail for the expected reason: the behaviour is missing or wrong. If it passes, repair the test or choose a case that proves the requested change. If it errors during setup, fix the test setup and run it again until it fails meaningfully.
4. **Write GREEN.** Make the smallest production change that satisfies the failing test. Do not add speculative options, unrelated cleanup, or a second behaviour in the same change.
5. **Verify GREEN.** Run the focused test and then the relevant existing suite. Read the complete result, including warnings and skipped tests. If a test fails, change the implementation or test fixture based on evidence; do not weaken the assertion merely to obtain a pass.
6. **Refactor.** Only after green, remove duplication, improve names, or simplify structure while preserving the contract. Do not add new behaviour during refactoring.
7. **Record the cycle.** Leave a concise record of the red failure, green command, refactor scope, and final verification so another developer can understand why the test exists.

## Exceptions

Ask before using an exception. An exception is acceptable only when the user explicitly agrees that the output is disposable or that no executable test surface exists. Even then, write a concrete manual check and state what remains unverified. Never call a test "covered" when the red failure was not observed.

## Output

Return a structured cycle record:

```json
{
  "behaviour": "",
  "test": {
    "path": "",
    "name": "",
    "red_command": "",
    "red_exit_code": 1,
    "red_failure_reason": ""
  },
  "implementation": {
    "files_changed": [],
    "summary": ""
  },
  "green": {
    "focused_command": "",
    "focused_exit_code": 0,
    "suite_command": "",
    "suite_exit_code": 0
  },
  "refactor": "none | described change",
  "remaining_gaps": []
}
```

Do not report a completed cycle if `red_exit_code` was not observed as a meaningful failure or if the final relevant checks did not run.

## Examples

- In scope: "Add a regression test for the duplicate checkout response, watch it fail, then implement the smallest fix."
- In scope: "Change the parser so malformed input returns the documented error. Start with one failing test."
- Out of scope: "Why does this existing test fail?" without a requested behaviour change. Investigate with systematic debugging first.
