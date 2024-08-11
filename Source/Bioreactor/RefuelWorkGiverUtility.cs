using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace BioReactor;

public static class RefuelWorkGiverUtility
{
    public static bool CanRefuel(Pawn pawn, Thing t, bool forced = false)
    {
        var compRefuelable = t.TryGetComp<CompBioRefuelable>();
        if (compRefuelable == null || compRefuelable.IsFull)
        {
            return false;
        }

        if (!forced && !compRefuelable.ShouldAutoRefuelNow)
        {
            return false;
        }

        if (t.IsForbidden(pawn))
        {
            return false;
        }

        LocalTargetInfo target = t;
        if (!pawn.CanReserve(target, 1, -1, null, forced))
        {
            return false;
        }

        if (t.Faction != pawn.Faction)
        {
            return false;
        }

        if (FindBestFuel(pawn, t) == null)
        {
            var fuelFilter = t.TryGetComp<CompBioRefuelable>().FuelFilter;
            JobFailReason.Is("NoFuelToRefuel".Translate(fuelFilter.Summary));
            return false;
        }

        if (!t.TryGetComp<CompBioRefuelable>().Props.atomicFueling || FindAllFuel(pawn, t) != null)
        {
            return true;
        }

        var fuelFilter2 = t.TryGetComp<CompBioRefuelable>().FuelFilter;
        JobFailReason.Is("NoFuelToRefuel".Translate(fuelFilter2.Summary));
        return false;
    }

    public static Job RefuelJob(Pawn pawn, Thing t, bool forced = false, JobDef customRefuelJob = null,
        JobDef customAtomicRefuelJob = null)
    {
        if (!t.TryGetComp<CompBioRefuelable>().Props.atomicFueling)
        {
            var t2 = FindBestFuel(pawn, t);
            return new Job(customRefuelJob ?? JobDefOf.Refuel, t, t2);
        }

        var source = FindAllFuel(pawn, t);
        var job = new Job(customAtomicRefuelJob ?? JobDefOf.RefuelAtomic, t)
        {
            targetQueueB = (from f in source
                select new LocalTargetInfo(f)).ToList()
        };
        return job;
    }

    private static Thing FindBestFuel(Pawn pawn, Thing refuelable)
    {
        var filter = refuelable.TryGetComp<CompBioRefuelable>().FuelFilter;
        var position = pawn.Position;
        var map = pawn.Map;
        var bestThingRequest = filter.BestThingRequest;
        var peMode = PathEndMode.ClosestTouch;
        var traverseParams = TraverseParms.For(pawn);
        return GenClosest.ClosestThingReachable(position, map, bestThingRequest, peMode, traverseParams, 9999f,
            Predicate);

        bool Predicate(Thing x)
        {
            return !x.IsForbidden(pawn) && pawn.CanReserve(x) && filter.Allows(x);
        }
    }

    private static List<Thing> FindAllFuel(Pawn pawn, Thing refuelable)
    {
        var quantity = refuelable.TryGetComp<CompBioRefuelable>().GetFuelCountToFullyRefuel();
        var filter = refuelable.TryGetComp<CompBioRefuelable>().FuelFilter;
        var position = refuelable.Position;
        var region = position.GetRegion(pawn.Map);
        var traverseParams = TraverseParms.For(pawn);
        var chosenThings = new List<Thing>();
        var accumulatedQuantity = 0;

        RegionTraverser.BreadthFirstTraverse(region, EntryCondition, RegionProcessor, 99999);
        return accumulatedQuantity >= quantity ? chosenThings : null;

        bool RegionProcessor(Region r)
        {
            var list = r.ListerThings.ThingsMatching(ThingRequest.ForGroup(ThingRequestGroup.HaulableEver));
            foreach (var thing in list)
            {
                if (!Validator(thing))
                {
                    continue;
                }

                if (chosenThings.Contains(thing))
                {
                    continue;
                }

                if (!ReachabilityWithinRegion.ThingFromRegionListerReachable(thing, r, PathEndMode.ClosestTouch, pawn))
                {
                    continue;
                }

                chosenThings.Add(thing);
                accumulatedQuantity += thing.stackCount;
                if (accumulatedQuantity >= quantity)
                {
                    return true;
                }
            }

            return false;
        }

        bool EntryCondition(Region from, Region r)
        {
            return r.Allows(traverseParams, false);
        }

        bool Validator(Thing x)
        {
            return !x.IsForbidden(pawn) && pawn.CanReserve(x) && filter.Allows(x);
        }
    }
}