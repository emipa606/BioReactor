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
    public static BioReactorMod instance;

    private static string currentVersion;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="content"></param>
    public BioReactorMod(ModContentPack content) : base(content)
    {
        instance = this;
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
        var listing_Standard = new Listing_Standard();
        listing_Standard.Begin(rect);
        listing_Standard.Label("BR.SelectItems".Translate());
        listing_Standard.CheckboxLabeled("BR.Carried".Translate(), ref Settings.Carried);
        listing_Standard.CheckboxLabeled("BR.Apparel".Translate(), ref Settings.Apparel);
        listing_Standard.CheckboxLabeled("BR.Inventory".Translate(), ref Settings.Inventory);
        if (currentVersion != null)
        {
            listing_Standard.Gap();
            GUI.contentColor = Color.gray;
            listing_Standard.Label("BR.ModVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listing_Standard.End();
    }
}