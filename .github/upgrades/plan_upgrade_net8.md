# Plan: Upgrade to .NET 8

Scenario Id: UpgradeToNewDotNetVersion

Goal: Convert the GameCaro project from .NET Framework 4.7.2 to .NET 8.0 (SDK-style), update packages, and fix breaking changes.

Steps:
1. Validate .NET 8 SDK is installed on machine and global.json compatibility.
2. Create a new git branch for the upgrade (user requested direct changes on main; proceed on `main` if confirmed).
3. Convert `GameCaro.csproj` to SDK-style per Plan §3.1.
4. Change TargetFramework to `net8.0`.
5. Update or remove NuGet packages per assessment recommendations.
6. Build solution and fix compilation errors.
7. Run and validate application manually (WinForms) — resolve runtime issues.
8. Commit changes with messages per task.

Notes:
- This plan will make code changes that may require manual adjustments for breaking API changes.
- The agent will stop and request user intervention if compilation errors occur that it cannot safely auto-fix.
