using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace BioReactor;

public class JobDriver_CarryToBioReactor : JobDriver
{
    private const TargetIndex TakeeInd = TargetIndex.A;

    private const TargetIndex DropPodInd = TargetIndex.B;

    protected Pawn Takee => (Pawn)job.GetTarget(TakeeInd).Thing;

    protected Building_BioReactor DropPod => job.GetTarget(DropPodInd).Thing as Building_BioReactor;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        var localPawn = pawn;
        LocalTargetInfo target = Takee;
        var localJob = job;
        bool result;
        if (localPawn.Reserve(target, localJob, 1, -1, null, errorOnFailed))
        {
            localPawn = pawn;
            target = DropPod;
            localJob = job;
            result = localPawn.Reserve(target, localJob, 1, -1, null, errorOnFailed);
        }
        else
        {
            result = false;
        }

        return result;
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDestroyedOrNull(TakeeInd);
        this.FailOnDestroyedOrNull(DropPodInd);
        this.FailOnAggroMentalState(TakeeInd);
        this.FailOn(() => !DropPod.Accepts(Takee));
        yield return Toils_Goto.GotoThing(
                TakeeInd, PathEndMode.OnCell).FailOnDestroyedNullOrForbidden(TakeeInd)
            .FailOnDespawnedNullOrForbidden(DropPodInd).FailOn(() =>
                DropPod.GetDirectlyHeldThings().Count > 0).FailOn(() =>
                !Takee.Downed).FailOn(() =>
                !pawn.CanReach(Takee, PathEndMode.OnCell, Danger.Deadly))
            .FailOnSomeonePhysicallyInteracting(TakeeInd);
        yield return Toils_Haul.StartCarryThing(TakeeInd);
        yield return Toils_Goto.GotoThing(DropPodInd, PathEndMode.InteractionCell);
        var prepare = Toils_General.Wait(500);
        prepare.FailOnCannotTouch(DropPodInd, PathEndMode.InteractionCell);
        prepare.WithProgressBarToilDelay(DropPodInd);
        yield return prepare;
        yield return new Toil
        {
            initAction = delegate { DropPod.TryAcceptThing(Takee); },
            defaultCompleteMode = ToilCompleteMode.Instant
        };
    }

    public override object[] TaleParameters()
    {
        return
        [
            pawn,
            Takee
        ];
    }
}