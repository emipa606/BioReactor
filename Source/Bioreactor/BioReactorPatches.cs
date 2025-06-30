using UnityEngine;
using Verse;

namespace BioReactor;

[StaticConstructorOnStartup]
public static class BioReactorPatches
{
    public static readonly Texture2D PawnInfoTexture = ContentFinder<Texture2D>.Get("UI/Commands/ViewQuest");
}