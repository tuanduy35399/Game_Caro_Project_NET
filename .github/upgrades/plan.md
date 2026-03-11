# Plan: Cleanup Designer files

Scenario: Cleanup unused using directives and minor tidying for Designer files in the GameCaro project.

Goals:
- Remove unused `using` directives from `*.Designer.cs` files.
- Ensure Designer files compile unchanged (no logic changes).
- Run a build to verify no compilation errors.

Tasks:
- TASK-001: Remove unused using directives from the following files:
  - GameCaro\MatchHistoryForm.Designer.cs
  - GameCaro\LoginForm.Designer.cs
  - GameCaro\RegisterForm.Designer.cs
  - GameCaro\TopScoresForm.Designer.cs
  - GameCaro\Form1.Designer.cs

- TASK-002: Build solution and verify successful build.

Notes:
- Changes will be applied directly to the `main` branch as requested by the user.
- No functional changes are planned; only removal of unused `using` statements and whitespace formatting.
