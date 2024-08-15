using Verse;

namespace BioReactor;

/// <summary>
///     Definition of the settings for the mod
/// </summary>
internal class BioReactorSettings : ModSettings
{
    public bool Apparel = true;
    public bool Carried = true;
    public bool Inventory = true;

    /// <summary>
    ///     Saving and loading the values
    /// </summary>
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref Carried, "Carried", true);
        Scribe_Values.Look(ref Apparel, "Apparel", Apparel);
        Scribe_Values.Look(ref Inventory, "Inventory", Inventory);
    }
}