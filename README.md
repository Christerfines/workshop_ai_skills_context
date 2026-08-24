# AI Agent Skills Workshop Overview

## Introduction & Concept
AI agent skills are modular, multi-step workflows designed to handle repeatable procedures, package domain expertise, and be discovered progressively by Copilot.

## Key Learning Outcomes
* **Skill Construction:** Hands-on experience structuring skills with optional reference files and execution scripts in Visual Studio Code.
* **Eval Validation:** Understanding how to write automated test cases (Evals) using static inputs to systematically verify skill outputs.

## Workshop Agenda
Hands-on Development (Participants select and build one of the five challenge options).
### Part 1: Skills development (10:30 - 11:30)
* **10 Mins:** Introduction
* **10 Mins:** Demo: Skill creation (Creating a skill from scratch in VS Code and demonstrating the completed reference skill).
* **30 Mins:** Group activity: Create your own skill
* **10 Mins (just before lunch):** Introduction to skill evaluators

### Part 2: Skills Eval development (12:30 - 13:30)
* **10 Mins:** Demo: Create a skill evaluator
* **30 Mins:** Group activity: Create a skill evaluator
* **20 Mins:** Open discussion – AI in general


## Completed Reference Skill
* **Automated Code Review Skill:** A fully implemented reference skill that executes a linter script and dynamically loads external security guidelines on demand to run Evals.

## Participant Challenge Options
1. **[Support Case Triage](supplementary_material/support-case-triage/README.md):** Analyze raw customer emails/logs from Nordic Bikes to output standardized bug or warranty reports.
2. **[Documentation Generation](supplementary_material/documentation-generation/README.md):** Scan C# controllers and API endpoints to auto-generate Markdown documentation.
3. **[PR Release Notes](supplementary_material/pr-release-notes/README.md):** Process git commit logs to draft human-readable release notes.
4. **[ADO Planning](supplementary_material/ado-planning/README.md):** Break down product backlog requirements into epics, features, and user stories within Azure DevOps.
5. **[Controller Creation](supplementary_material/controller-creation/README.md):** Scaffold new C# API controllers following project-specific architectural standards.