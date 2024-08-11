using RimWorld;
using UnityEngine;
using Verse;

namespace BioReactor;

public class ITab_CustomRefuel : ITab
{
    private const float TopAreaHeight = 40f;

    private static readonly Vector2 WinSize = new Vector2(300f, 480f);

    private readonly ThingFilterUI.UIState thingFilterState = new ThingFilterUI.UIState();

    public ITab_CustomRefuel()
    {
        size = WinSize;
        labelKey = "RefuelTab";
    }

    private IStoreSettingsParent SelStoreSettingsParent => ((ThingWithComps)SelObject).GetComp<CompBioRefuelable>();

    public override bool IsVisible => SelStoreSettingsParent.StorageTabVisible;

    public override void OnOpen()
    {
        base.OnOpen();
        thingFilterState.quickSearch.Reset();
    }

    protected override void FillTab()
    {
        var selStoreSettingsParent = SelStoreSettingsParent;
        var storeSettings = selStoreSettingsParent.GetStoreSettings();
        var rect = new Rect(0f, 0f, WinSize.x, WinSize.y).ContractedBy(10f);
        GUI.BeginGroup(rect);
        Text.Font = GameFont.Medium;
        Text.Anchor = TextAnchor.MiddleCenter;
        Widgets.Label(new Rect(rect)
        {
            height = 32f
        }, "RefuelTitle".Translate());
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.UpperLeft;
        ThingFilter thingFilter = null;
        if (selStoreSettingsParent.GetParentStoreSettings() != null)
        {
            thingFilter = selStoreSettingsParent.GetParentStoreSettings().filter;
        }

        var rect2 = new Rect(0f, TopAreaHeight, rect.width, rect.height - TopAreaHeight);
        ThingFilterUI.DoThingFilterConfigWindow(rect2, thingFilterState, storeSettings.filter, thingFilter, 8);
        PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.StorageTab, KnowledgeAmount.FrameDisplayed);
        GUI.EndGroup();
    }

    public override void Notify_ClickOutsideWindow()
    {
        base.Notify_ClickOutsideWindow();
        thingFilterState.quickSearch.Unfocus();
    }
}