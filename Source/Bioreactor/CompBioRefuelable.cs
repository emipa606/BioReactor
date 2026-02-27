using System.Reflection;
using RimWorld;
using Verse;

namespace BioReactor;

public class CompBioRefuelable : CompRefuelable, IStoreSettingsParent
{
    private static readonly PropertyInfo BaseFuelFilterProperty = typeof(CompRefuelable)
        .GetProperty("FuelFilter", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    private static readonly FieldInfo BaseFuelFilterField = typeof(CompRefuelable)
                                                                .GetField("fuelFilter",
                                                                    BindingFlags.Instance | BindingFlags.Public |
                                                                    BindingFlags.NonPublic) ??
                                                            typeof(CompRefuelable).GetField("allowedFuelFilter",
                                                                BindingFlags.Instance | BindingFlags.Public |
                                                                BindingFlags.NonPublic);

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

        syncFuelFilter();

        bioReactor = (Building_BioReactor)parent;

        var component = parent.Map.GetComponent<CompMapRefuelable>();

        component?.comps.Add(this);
    }

    public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
    {
        base.PostDeSpawn(map, mode);
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
        if (!Props.consumeFuelOnlyWhenUsed && (flickComp == null || flickComp.SwitchIsOn) &&
            bioReactor is { InnerPawn: not null })
        {
            ConsumeFuel(ConsumptionRatePerTick);
        }
    }

    public override void PostDestroy(DestroyMode mode, Map previousMap)
    {
    }

    private void syncFuelFilter()
    {
        inputSettings ??= new StorageSettings(this);

        var baseFilter = getBaseFuelFilter();
        if (baseFilter == null)
        {
            return;
        }

        if (inputSettings.filter != baseFilter && inputSettings.filter != null)
        {
            baseFilter.CopyAllowancesFrom(inputSettings.filter);
        }

        inputSettings.filter = baseFilter;
    }

    private ThingFilter getBaseFuelFilter()
    {
        if (BaseFuelFilterProperty != null)
        {
            return BaseFuelFilterProperty.GetValue(this) as ThingFilter;
        }

        return BaseFuelFilterField?.GetValue(this) as ThingFilter;
    }
}