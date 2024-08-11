using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace BioReactor;

[StaticConstructorOnStartup]
public static class BioReactorPatches
{
    private static readonly Type patchType = typeof(BioReactorPatches);


    static BioReactorPatches()
    {
        var harmonyInstance = new Harmony("com.BioReactor.rimworld.mod");
        harmonyInstance.Patch(AccessTools.Method(typeof(FloatMenuMakerMap), "AddHumanlikeOrders"),
            new HarmonyMethod(patchType, nameof(Prefix_AddHumanlikeOrders)));
    }

    public static bool Prefix_AddHumanlikeOrders(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
    {
        if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
        {
            return true;
        }

        foreach (var localTargetInfo3 in GenUI.TargetsAt(clickPos, TargetingParameters.ForRescue(pawn), true))
        {
            var localTargetInfo4 = localTargetInfo3;
            var victim = (Pawn)localTargetInfo4.Thing;
            if (!victim.Downed ||
                !pawn.CanReserveAndReach(victim, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, true) ||
                Building_BioReactor.FindBioReactorFor(victim, pawn, true) == null)
            {
                continue;
            }

            string text4 =
                "CarryToBioReactor".Translate(localTargetInfo4.Thing.LabelCap, localTargetInfo4.Thing);
            var jDef = Bio_JobDefOf.CarryToBioReactor;

            opts.Add(FloatMenuUtility.DecoratePrioritizedTask(
                new FloatMenuOption(text4, Action, MenuOptionPriority.Default, null, victim),
                pawn, victim));
            continue;

            void Action()
            {
                var building_BioReactor = Building_BioReactor.FindBioReactorFor(victim, pawn);
                if (building_BioReactor == null)
                {
                    building_BioReactor = Building_BioReactor.FindBioReactorFor(victim, pawn, true);
                }

                if (building_BioReactor == null)
                {
                    Messages.Message("CannotCarryToBioReactor".Translate() + ": " + "NoBioReactor".Translate(), victim,
                        MessageTypeDefOf.RejectInput, false);
                    return;
                }

                var job = new Job(jDef, victim, building_BioReactor)
                {
                    count = 1
                };
                pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }
        }

        return true;
    }
}