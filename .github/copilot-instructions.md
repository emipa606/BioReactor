# GitHub Copilot Instructions for BioReactor (Continued)

## Mod Overview and Purpose

**Mod Name**: BioReactor (Continued)  
**Author**: NukaFrog  
**PackageId**: Mlie.BioReactor  

The BioReactor mod for RimWorld enables colonies to generate electrical power by storing living beings in a bioreactor. The mod enhances sustainability by utilizing food types as fuel and allowing living creatures to be preserved while contributing to power generation.

## Key Features and Systems

1. **Living Battery - BioReactor**: 
   - Produces electricity from living things.
   - Utilizes food types (default: raw food) as fuel.
   - Optionally converts the storage being into chemfuel.
   - Preserves stored creatures semi-permanently, akin to a crypto casket.

2. **Storable Entities**: 
   - Capable of storing any entity that can move.
   - Storage capacity is limited to one entity.

3. **Power Generation Mechanism**:
   - Computed as Basic Power x Body Size Multiplier x Race Multiplier.
   - Basic power generation ranges from 1000/1200/1400 based on reactor size (2x2/3x3/4x4).
   - Body size and race type affect power output (HumanLike: 100%, NonHumanLike: 50%, NonBioType: 0%).

4. **Research Requirement**:
   - Requires completing BiofuelRefining research to gain access.

5. **User Interaction**:
   - Colonists can be directed to store entities manually.
   - Entities can be instructed to carry unconscious beings to the reactor.

6. **Compatibility Enhancements**:
   - Mod settings allow users to customize dropped items when pawns enter the reactor.
   - Compatibility with Vanilla Nutrient Paste Expanded and Alien mods.

## Coding Patterns and Conventions

- Adheres to C# coding conventions prevalent in RimWorld mods.
- Structuring code in components, ensuring encapsulation and separation of concerns.
- Follows naming conventions that align types with their conceptual function (e.g., `BioReactorSettings`, `BioReactorDef`).

## XML Integration

- **Def Files**: Defined in XML to integrate with RimWorld.
  - `Buildings_Power.xml`, `SoundDef.xml`, etc., are used to define building behaviors, job definitions, sound settings, etc.

## Harmony Patching

- Uses Harmony (`brrainz.harmony`) to inject code into existing methods for enhanced mod functionality.
- `BioReactorPatches.cs` contains the necessary Harmony patches to extend or modify game behavior relevant to the bioreactor mechanics.

## Suggestions for Copilot

- **Intelligent Suggestions**: Leverage Copilot to develop complex calculations for power output and additions to logic that handles interactions with other mods.
- **Code Optimization**: Utilize Copilot to refactor and optimize existing code, especially around computational loops and data-handling methods.
- **Error Handling**: Prompt Copilot to suggest robust error-checking mechanisms for user inputs and mod interaction.
- **Compatibility Checks**: Use Copilot for creating condition checks and modular code to ensure broad compatibility with other RimWorld mods.
- **UI Enhancements**: Encourage Copilot to propose enhancements to the user interface for a smoother user experience, such as better interaction prompts for storing entities in the reactor.

---

Ensure to activate and test this mod in a controlled environment, without other mods initially, to avoid discords in behavior and compatibility. For error reporting, use the provided Discord channel and adhere to the suggestion guidelines for effective problem resolution.

For any code contributions or bug-fixes, use RimSort to organize your mods and post solutions directly to the GitHub repository.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).

