# GitHub Copilot Instructions for BioReactor (Continued) Mod

This repository contains the source code for the BioReactor (Continued) mod for RimWorld. This mod is an update of the NukaFrogs mod and adds several new features and systems to enhance the gameplay experience.

## Mod Overview and Purpose

The BioReactor (Continued) mod aims to introduce a new energy production system to RimWorld. The mod allows players to produce electricity using living things as fuel, providing a unique and innovative way to power their colonies.

## Key Features and Systems

### Living Battery - BioReactor

- **Energy Production**: The BioReactor produces electricity from living things, using food as fuel.
- **Storage Thing**: Living things can be stored semi-permanently, similar to crypto caskets.
- **Power Generation**: Power production is calculated based on the body size and race of the living thing.
- **Fuel Type**: The default fuel type is raw food, but it can be customized through mod settings.
- **Research Prerequisite**: The BiofuelRefining research is required to unlock the BioReactor.

### Mod Settings

- **Fuel Selection**: Players can customize which items are used as fuel for the BioReactor.
- **Compatibility**: The mod is compatible with Vanilla Nutrient Paste Expanded and Alien mods.

## Coding Patterns and Conventions

- **C#**: The mod is written in C# and follows the standard RimWorld coding conventions.
- **Harmony Patching**: The mod uses Harmony patching to modify existing RimWorld code.
- **XML Integration**: XML files are used to define new game elements such as buildings, job definitions, and thoughts.

## XML Integration

The BioReactor mod uses XML files to define new game elements. The following are the key XML files used in the mod:

- `Buildings_Power.xml`: Defines new building types and job definitions.
- `SoundDef.xml`: Defines new sound effects.
- `VPNE.xml`: Contains Harmony patches for compatibility with Vanilla Nutrient Paste Expanded.
- `About.xml`: Contains metadata about the mod, including dependencies.

## Harmony Patching

The BioReactor mod uses Harmony patching to modify existing RimWorld code. Harmony is a powerful tool for modders to hook into and modify game behavior without altering the original source code. The following are the main Harmony patches used in the mod:

- **Building_BioReactor_VNPE.cs**: Contains patches to integrate the BioReactor with Vanilla Nutrient Paste Expanded.
- **Bio_JobDefOf.cs**: Contains patches to define new job definitions for the BioReactor.

## Suggestions for Copilot

1. **C# Files**: When writing C# files, consider the following patterns:
   - Use `Building_` prefix for building-related classes.
   - Use `ThoughtDef` suffix for thought-related classes.
   - Use `JobDef` suffix for job-related classes.
   - Use `Def` suffix for definition-related classes.
   - Use `Mod` suffix for mod-related classes.

2. **XML Files**: When writing XML files, consider the following patterns:
   - Use `Def` suffix for definition-related elements.
   - Use `ThingDef` for defining new game elements.
   - Use `JobDef` for defining new job definitions.
   - Use `ThoughtDef` for defining new thoughts.

3. **Harmony Patching**: When writing Harmony patches, consider the following patterns:
   - Use `Postfix` and `Prefix` attributes for modifying method behavior.
   - Use `Transpiler` attribute for modifying method IL code.
   - Use `StaticConstructorOnStartup` attribute for executing code at startup.

4. **Mod Settings**: When implementing mod settings, consider the following patterns:
   - Use `DoSettingsWindowContents` method for creating settings UI.
   - Use `Settings` class for storing and managing mod settings.

## Example Code Snippets

Here are some example code snippets that demonstrate the coding patterns and conventions used in the BioReactor mod:

### C# Example

csharp
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BioReactor
{
    public class Building_BioReactor_VNPE : Building_BioReactor
    {
        public override void VNPE_Check()
        {
            // Check if the BioReactor is compatible with Vanilla Nutrient Paste Expanded
        }
    }
}


### XML Example

xml
<Defs>
    <ThingDef>
        <defName>Building_BioReactor</defName>
        <label>BioReactor</label>
        <description>A device that produces electricity from living things.</description>
        <thingClass>Building_BioReactor</thingClass>
        <statBases>
            <PowerOutput>1000</PowerOutput>
        </statBases>
    </ThingDef>
</Defs>


### Harmony Patch Example

csharp
using HarmonyLib;
using RimWorld;
using Verse;

namespace BioReactor
{
    [HarmonyPatch(typeof(Building_BioReactor), "SomeMethod")]
    public static class Building_BioReactor_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Building_BioReactor __instance)
        {
            // Postfix code to modify the original method behavior
        }
    }
}


## Conclusion

The BioReactor (Continued) mod is a powerful and innovative addition to the RimWorld modding community. By following the coding patterns and conventions outlined in this document, you can help ensure that your contributions to the mod are consistent and maintainable.

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
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

