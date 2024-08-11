using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace BioReactor;

public class WorkGiver_CustomRefuel : WorkGiver_Scanner
{
    public override PathEndMode PathEndMode => PathEndMode.Touch;

    protected virtual JobDef JobStandard => JobDefOf.Refuel;

    protected virtual JobDef JobAtomic => JobDefOf.RefuelAtomic;

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
    {
        return pawn.Map.GetComponent<CompMapRefuelable>().comps.Select(x => x.parent);
    }

    protected virtual bool CanRefuelThing(Thing t)
    {
        return t is not Building_Turret;
    }

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        return CanRefuelThing(t) && RefuelWorkGiverUtility.CanRefuel(pawn, t, forced);
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        return RefuelWorkGiverUtility.RefuelJob(pawn, t, forced, JobStandard, JobAtomic);
    }
}