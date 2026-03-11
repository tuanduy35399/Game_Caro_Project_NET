# Game_Caro_Project_NET .NET 8 Upgrade Tasks

## Overview

This document tracks the execution of the atomic upgrade of the GameCaro solution from .NET Framework 4.7.2 to .NET 8.0 by converting project files to SDK-style, updating package references, and resolving compilation issues. The All-At-Once approach will update all projects simultaneously, then run automated tests (if present).

**Progress**: 1/3 tasks complete (33%) ![0%](https://progress-bar.xyz/33)

---

## Tasks

### [✓] TASK-001: Verify prerequisites (SDK and configuration) *(Completed: 2026-03-11 06:55)*
**References**: Plan §Steps (Step 1)

- [✓] (1) Verify .NET 8.0 SDK is installed on the machine per Plan §Steps
- [✓] (2) Runtime/SDK version meets minimum requirements (**Verify**)
- [✓] (3) Check for `global.json` and verify compatibility with .NET 8.0 per Plan §Steps (update if required)
- [✓] (4) `global.json` (if present) is compatible with .NET 8.0 (**Verify**)

### [▶] TASK-002: Atomic framework and package upgrade (convert projects, update packages, build)
**References**: Plan §Steps (Steps 3–6), Plan §3.1

- [▶] (1) Convert `GameCaro.csproj` and any other project files listed in Plan §Steps to SDK-style project format per Plan §3.1 and update `TargetFramework` to `net8.0` across all projects per Plan §Steps
- [ ] (2) All project files updated to SDK-style and `TargetFramework=net8.0` (**Verify**)  
- [ ] (3) Update or remove NuGet packages per assessment recommendations in Plan §Steps (use centralized package management if present)  
- [ ] (4) All package references updated or removed as specified (**Verify**)  
- [ ] (5) Restore dependencies (`dotnet restore`) for the solution per Plan §Steps  
- [ ] (6) Restore completes successfully (**Verify**)  
- [ ] (7) Build the entire solution and fix compilation errors caused by framework/package changes (apply fixes referenced in Plan §Breaking Changes / Plan §Steps)  
- [ ] (8) Solution builds with 0 errors (**Verify**)  
- [ ] (9) Commit changes with message: "TASK-002: Atomic upgrade to .NET 8 — convert projects and update packages"

### [ ] TASK-003: Run automated tests and validate upgrade
**References**: Plan §Steps (Steps 6–7)

- [ ] (1) Run all test projects in the solution (`dotnet test`) per Plan §Steps (if test projects exist)  
- [ ] (2) Fix any test failures that are automatable (reference Plan §Breaking Changes for common issues)  
- [ ] (3) Re-run tests after fixes  
- [ ] (4) All tests pass with 0 failures (**Verify**)  
- [ ] (5) Commit test fixes with message: "TASK-003: Complete testing and validation"

---


