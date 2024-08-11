using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace BioReactor;

public class JobDriver_EnterBioReactor : JobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        var localPawn = pawn;
        var targetA = job.targetA;
        var localJob = job;
        return localPawn.Reserve(targetA, localJob, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedOrNull(TargetIndex.A);
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
        var prepare = Toils_General.Wait(500);
        prepare.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
        prepare.WithProgressBarToilDelay(TargetIndex.A);
        yield return prepare;
        var enter = new Toil();
        enter.initAction = delegate
        {
            var actor = enter.actor;
            var pod = (Building_BioReactor)actor.CurJob.targetA.Thing;

            if (!pod.def.building.isPlayerEjectable)
            {
                var freeColonistsSpawnedOrInPlayerEjectablePodsCount =
                    Map.mapPawns.FreeColonistsSpawnedOrInPlayerEjectablePodsCount;
                if (freeColonistsSpawnedOrInPlayerEjectablePodsCount <= 1)
                {
                    Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(
                        "CasketWarning".Translate(actor.Named("PAWN")).AdjustedFor(actor), Action));
                }
                else
                {
                    Action();
                }
            }
            else
            {
                Action();
            }

            return;

            void Action()
            {
                actor.DeSpawn();
                pod.TryAcceptThing(actor);
            }
        };
        enter.defaultCompleteMode = ToilCompleteMode.Instant;
        yield return enter;
    }
}