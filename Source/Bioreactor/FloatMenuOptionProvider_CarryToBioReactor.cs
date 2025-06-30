using RimWorld;
using Verse;
using Verse.AI;

namespace BioReactor;

public class FloatMenuOptionProvider_CarryToBioReactor : FloatMenuOptionProvider
{
    protected override bool Drafted => true;

    protected override bool Undrafted => true;

    protected override bool Multiselect => false;

    protected override bool RequiresManipulation => true;

    protected override bool MechanoidCanDo => true;

    protected override bool AppliesInt(FloatMenuContext context)
    {
        if (context.FirstSelectedPawn.IsMutant)
        {
            return !context.FirstSelectedPawn.mutant.Def.canCarryPawns;
        }

        return true;
    }

    public override bool TargetPawnValid(Pawn pawn, FloatMenuContext context)
    {
        if (pawn.Faction != Faction.OfPlayer && !pawn.IsPrisonerOfColony && !pawn.Downed)
        {
            return false;
        }

        if (pawn.InAggroMentalState)
        {
            return false;
        }

        return !pawn.def.thingClass.Name.EndsWith("VehiclePawn") && base.TargetPawnValid(pawn, context);
    }

    protected override FloatMenuOption GetSingleOptionFor(Pawn clickedPawn, FloatMenuContext context)
    {
        if (!context.FirstSelectedPawn.CanReach(clickedPawn, PathEndMode.ClosestTouch, Danger.Deadly))
        {
            return new FloatMenuOption(
                "CannotCarryThemToBioReactor".Translate(clickedPawn) + ": " + "NoPath".Translate().CapitalizeFirst(),
                null);
        }

        if (!context.FirstSelectedPawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
        {
            return new FloatMenuOption(
                "TheyCannotCarryToBioReactor".Translate(context.FirstSelectedPawn) + ": " +
                "Incapable".Translate().CapitalizeFirst(),
                null);
        }

        var buildingBioReactor = Building_BioReactor.FindBioReactorFor(clickedPawn, context.FirstSelectedPawn) ??
                                 Building_BioReactor.FindBioReactorFor(clickedPawn, context.FirstSelectedPawn, true);

        if (buildingBioReactor == null)
        {
            return new FloatMenuOption(
                "TheyCannotCarryToBioReactor".Translate(context.FirstSelectedPawn) + ": " +
                "NoBioReactor".Translate().CapitalizeFirst(), null);
        }

        return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(
            "CarryToBioReactorNew".Translate(clickedPawn),
            delegate
            {
                clickedPawn.SetForbidden(false, false);
                var job = JobMaker.MakeJob(Bio_JobDefOf.CarryToBioReactor, clickedPawn, buildingBioReactor);
                job.count = 1;
                context.FirstSelectedPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }), context.FirstSelectedPawn, clickedPawn);
    }
}