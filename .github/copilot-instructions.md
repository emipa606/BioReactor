# GitHub Copilot Instructions for RimWorld Mod: BioReactor (Continued)

## Mod Overview and Purpose
The BioReactor (Continued) mod is an updated version of NukaFrog's original mod, designed to enhance gameplay by integrating new functionalities and systems. The mod focuses on introducing a bio-reactor system where living things, such as pawns, can be used to produce electricity. It supports compatibility with the Vanilla Nutrient Paste Expanded mod and provides mod-settings for selecting what items are dropped when a pawn enters a reactor.

## Key Features and Systems
- **Living Battery - BioReactor**: The core feature of this mod is the ability to produce electricity from living things. The bio-reactor uses different food types as fuel, and if desired, stored things can be converted into chemfuel.
- **Perservation**: Stored things (e.g., living pawns) are preserved semi-permanently, similar to the cryo caskets in the game.
- **Power Production Mechanism**: 
  - Basic power output ranges from 1000 to 1400 watts per reactor size variant (2x2, 3x3, 4x4).
  - Multipliers are applied based on body size and race type, with human-like entities providing 100% power, non-human-like providing 50%, and non-bio types providing 0%.
- **Fuel Usage**: The reactor uses food as fuel, defaulting to raw food.
- **Research Prerequisite**: Players must research Biofuel Refining to access the bio-reactor technology.

## Coding Patterns and Conventions
- Classes are grouped logically by their purpose, such as handling bio-reactor definitions, patches, settings, and components.
- Methods are primarily private within classes unless they need to interact with other parts of the system.
- C# naming conventions are followed, including PascalCase for class and method names, and camelCase for local variables.

## XML Integration
- The mod uses XML for defining game objects, relationships, and settings that RimWorld can read and execute.
- XML files categorize data for building and modifying in-game entities, with a focus on compatibility and extending existing game mechanics.

## Harmony Patching
- The mod uses Harmony for patching game methods to inject additional functionality or alter existing behavior without modifying the original game code. This approach ensures compatibility and reduces potential conflicts with other mods.

## Suggestions for Copilot
1. **Class and Method Expansion**: When adjusting power generation logic or adding new types of storables, explore additional methods in classes like `Building_BioReactor` and `CompBioPowerPlant`.
2. **Integrate XML Definitions**: Use Copilot to generate XML snippets that align with the existing object definitions used in the game, focusing on new entities or resources.
3. **Enhance Compatibility**: Work with Copilot to draft Harmony patches that allow the mod to work seamlessly with other popular mods or newer versions of RimWorld.
4. **Refactor and Optimize Code**: Utilize Copilot's refactoring suggestions to optimize performance, especially in areas related to WorkGiver optimization and refuel systems.
5. **User Interface Improvements**: Leverage Copilot to enhance the UI components within the mod, such as improving tab layouts in `ITab_CustomRefuel` or enhancing float menu options.

By following these guidelines, you can seamlessly contribute to the development and improvement of the BioReactor (Continued) mod for RimWorld, enhancing both gameplay and compatibility within the modding community.
