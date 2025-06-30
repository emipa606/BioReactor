using Mlie;
using UnityEngine;
using Verse;

namespace BioReactor;

[StaticConstructorOnStartup]
internal class BioReactorMod : Mod
{
    /// <summary>
    ///     The instance of the settings to be read by the mod
    /// </summary>
    public static BioReactorMod Instance;

    private static string currentVersion;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="content"></param>
    public BioReactorMod(ModContentPack content) : base(content)
    {
        Instance = this;
        Settings = GetSettings<BioReactorSettings>();
        currentVersion = VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    /// <summary>
    ///     The instance-settings for the mod
    /// </summary>
    internal BioReactorSettings Settings { get; }

    /// <summary>
    ///     The title for the mod-settings
    /// </summary>
    /// <returns></returns>
    public override string SettingsCategory()
    {
        return "BioReactor";
    }

    /// <summary>
    ///     The settings-window
    ///     For more info: https://rimworldwiki.com/wiki/Modding_Tutorials/ModSettings
    /// </summary>
    /// <param name="rect"></param>
    public override void DoSettingsWindowContents(Rect rect)
    {
        var listingStandard = new Listing_Standard();
        listingStandard.Begin(rect);
        listingStandard.Label("BR.SelectItems".Translate());
        listingStandard.CheckboxLabeled("BR.Carried".Translate(), ref Settings.Carried);
        listingStandard.CheckboxLabeled("BR.Apparel".Translate(), ref Settings.Apparel);
        listingStandard.CheckboxLabeled("BR.Inventory".Translate(), ref Settings.Inventory);
        if (currentVersion != null)
        {
            listingStandard.Gap();
            GUI.contentColor = Color.gray;
            listingStandard.Label("BR.ModVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listingStandard.End();
    }
}