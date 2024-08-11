using RimWorld;
using Verse;

namespace BioReactor;

public class CompBioPowerPlant : CompPowerPlant
{
    public Building_BioReactor building_BioReactor;
    public CompRefuelable compRefuelable;

    protected override float DesiredPowerOutput => -Props.PowerConsumption;

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        building_BioReactor = (Building_BioReactor)parent;
        compRefuelable = parent.GetComp<CompRefuelable>();
    }

    public override void CompTick()
    {
        base.CompTick();
        UpdateDesiredPowerOutput();
    }

    public new void UpdateDesiredPowerOutput()
    {
        if (building_BioReactor != null && building_BioReactor.state != Building_BioReactor.ReactorState.Full ||
            breakdownableComp is { BrokenDown: true } ||
            refuelableComp is { HasFuel: false } || flickableComp is { SwitchIsOn: false } ||
            !PowerOn)
        {
            PowerOutput = 0f;
        }
        else
        {
            if (building_BioReactor?.ContainedThing is not Pawn pawn)
            {
                return;
            }

            if (pawn.Dead || pawn.RaceProps.FleshType == FleshTypeDefOf.Mechanoid)
            {
                PowerOutput = 0;
                return;
            }

            if (pawn.RaceProps.Humanlike)
            {
                PowerOutput = DesiredPowerOutput;
            }
            else
            {
                PowerOutput = DesiredPowerOutput * 0.50f;
            }

            PowerOutput *= pawn.BodySize;
        }
    }
}