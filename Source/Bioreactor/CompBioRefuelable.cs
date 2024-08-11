using RimWorld;
using Verse;

namespace BioReactor;

public class CompBioRefuelable : CompRefuelable, IStoreSettingsParent
{
    private Building_BioReactor bioReactor;
    private CompFlickable flickComp;
    public StorageSettings inputSettings;

    private float ConsumptionRatePerTick => Props.fuelConsumptionRate / GenDate.TicksPerDay;

    public ThingFilter FuelFilter => inputSettings.filter;

    public StorageSettings GetStoreSettings()
    {
        return inputSettings;
    }

    public StorageSettings GetParentStoreSettings()
    {
        return parent.def.building.fixedStorageSettings;
    }

    public void Notify_SettingsChanged()
    {
    }

    public bool StorageTabVisible => true;

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        flickComp = parent.GetComp<CompFlickable>();
        if (inputSettings == null)
        {
            inputSettings = new StorageSettings(this);
            if (parent.def.building.defaultStorageSettings != null)
            {
                inputSettings.CopyFrom(parent.def.building.defaultStorageSettings);
            }
        }

        bioReactor = (Building_BioReactor)parent;

        var component = parent.Map.GetComponent<CompMapRefuelable>();

        component?.comps.Add(this);
    }

    public override void PostDeSpawn(Map map)
    {
        base.PostDeSpawn(map);
        var component = map.GetComponent<CompMapRefuelable>();

        component?.comps.Remove(this);
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Deep.Look(ref inputSettings, "inputSettings");
    }

    public override void CompTick()
    {
        if (!Props.consumeFuelOnlyWhenUsed && (flickComp == null || flickComp.SwitchIsOn) && bioReactor is
            {
                InnerPawn: not null
            })
        {
            ConsumeFuel(ConsumptionRatePerTick);
        }
    }

    public override void PostDestroy(DestroyMode mode, Map previousMap)
    {
    }
}