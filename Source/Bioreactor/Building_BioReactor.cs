using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace BioReactor;

public sealed class Building_BioReactor : Building_Casket, ISuspendableThingHolder
{
    //static Vector3 waterDrawY = new Vector3(0, 0.3f, 0);
    public enum ReactorState
    {
        Empty, //none
        StartFilling, //animating Filling
        Full, //Just Drawing
        HistolysisStating, //Start Animating and Changing Color
        HistolysisEnding,
        HistolysisDone //Just Drawing
    }

    private static readonly StorageSettings clipboard = new StorageSettings();

    public CompBioRefuelable compRefuelable;
    public float fillpct;
    public CompForbiddable forbiddable;
    public float histolysisPct;

    /// <summary>
    ///     내부 캐릭터 드로우 좌표. 리액터 실좌표 중심으로 드로우.
    /// </summary>
    public Vector3 innerDrawOffset;

    public ReactorState state = ReactorState.Empty;
    public Vector3 waterDrawCenter;
    public Vector2 waterDrawSize;

    public bool IsContainingThingPawn
    {
        get
        {
            if (!HasAnyContents)
            {
                return false;
            }

            return ContainedThing is Pawn;
        }
    }

    public Pawn InnerPawn
    {
        get
        {
            if (!HasAnyContents)
            {
                return null;
            }

            if (ContainedThing is Pawn pawn)
            {
                return pawn;
            }

            return null;
        }
    }

    public static bool HasCopiedSettings { get; private set; }

    bool ISuspendableThingHolder.IsContentsSuspended => true;

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        compRefuelable = GetComp<CompBioRefuelable>();
        forbiddable = GetComp<CompForbiddable>();
        fillpct = 0;
        histolysisPct = 0;
        if (def is not BioReactorDef reactorDef)
        {
            return;
        }

        innerDrawOffset = reactorDef.innerDrawOffset;
        waterDrawCenter = reactorDef.waterDrawCenter;
        waterDrawSize = reactorDef.waterDrawSize;
    }

    public override bool TryAcceptThing(Thing thing, bool allowSpecialEffects = true)
    {
        if (!base.TryAcceptThing(thing, allowSpecialEffects))
        {
            return false;
        }

        if (allowSpecialEffects)
        {
            SoundDefOf.CryptosleepCasket_Accept.PlayOneShot(new TargetInfo(Position, Map));
        }

        state = ReactorState.StartFilling;
        if (thing is Pawn pawn && pawn.RaceProps.Humanlike)
        {
            pawn.needs.mood.thoughts.memories.TryGainMemory(BioReactorThoughtDef.LivingBattery);
        }

        return true;
    }

    public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
    {
        foreach (var o in base.GetFloatMenuOptions(myPawn))
        {
            yield return o;
        }

        if (innerContainer.Count != 0)
        {
            yield break;
        }

        if (!myPawn.CanReach(this, PathEndMode.InteractionCell, Danger.Deadly))
        {
            var failer = new FloatMenuOption("CannotUseNoPath".Translate(), null);
            yield return failer;
        }
        else
        {
            var jobDef = Bio_JobDefOf.EnterBioReactor;
            string jobStr = "EnterBioReactor".Translate();

            void JobAction()
            {
                var job = new Job(jobDef, this);
                myPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }

            yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(jobStr, JobAction), myPawn,
                this);
        }
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var c in base.GetGizmos())
        {
            yield return c;
        }

        if (HasAnyContents && ContainedThing is Pawn pawn)
        {
            if (pawn.RaceProps.FleshType == FleshTypeDefOf.Normal ||
                pawn.RaceProps.FleshType == FleshTypeDefOf.Insectoid)
            {
                if (state == ReactorState.Full)
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "Histolysis".Translate(),
                        defaultDesc = "HistolysisDesc".Translate(),
                        icon = ContentFinder<Texture2D>.Get("UI/Commands/Histolysis"),
                        action = delegate
                        {
                            BioReactorSoundDef.Drowning.PlayOneShot(new TargetInfo(Position, Map));
                            state = ReactorState.HistolysisStating;
                        }
                    };
                }
            }
        }

        foreach (var gizmo2 in CopyPasteGizmosFor(compRefuelable.inputSettings))
        {
            yield return gizmo2;
        }
    }

    public override void EjectContents()
    {
        var filth_Slime = ThingDefOf.Filth_Slime;
        foreach (var thing in innerContainer)
        {
            if (thing is not Pawn pawn)
            {
                continue;
            }

            PawnComponentsUtility.AddComponentsForSpawn(pawn);
            pawn.filth.GainFilth(filth_Slime);
            if (pawn.RaceProps.IsFlesh)
            {
                pawn.health.AddHediff(HediffDefOf.CryptosleepSickness);
            }
        }

        if (!Destroyed)
        {
            SoundDefOf.CryptosleepCasket_Eject.PlayOneShot(SoundInfo.InMap(new TargetInfo(Position, Map)));
        }

        state = ReactorState.Empty;
        base.EjectContents();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref state, "state");
        Scribe_Values.Look(ref fillpct, "fillpct");
        Scribe_Values.Look(ref histolysisPct, "histolysisPct");
        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            if (def is BioReactorDef reactorDef)
            {
                innerDrawOffset = reactorDef.innerDrawOffset;
                waterDrawCenter = reactorDef.waterDrawCenter;
                waterDrawSize = reactorDef.waterDrawSize;
            }
        }

        compRefuelable = GetComp<CompBioRefuelable>();
        forbiddable = GetComp<CompForbiddable>();
    }

    public void Histolysis()
    {
        if (!HasAnyContents)
        {
            return;
        }

        if (ContainedThing is not Pawn pawn)
        {
            return;
        }

        pawn.Rotation = Rot4.South;
        compRefuelable.Refuel(35);
        var d = new DamageInfo
        {
            Def = DamageDefOf.Burn
        };
        d.SetAmount(1000);
        pawn.Kill(d);
        try
        {
            var compRottable = ContainedThing.TryGetComp<CompRottable>();
            if (compRottable != null)
            {
                compRottable.RotProgress += 600000f;
            }

            MakeFuel();
        }
        catch (Exception ee)
        {
            Log.Message($"Rot Error{ee}");
        }

        if (!pawn.RaceProps.Humanlike)
        {
            return;
        }

        foreach (var p in Map.mapPawns.SpawnedPawnsInFaction(Faction))
        {
            if (p.needs == null || p.needs.mood == null || p.needs.mood.thoughts == null)
            {
                continue;
            }

            p.needs.mood.thoughts.memories.TryGainMemory(BioReactorThoughtDef.KnowHistolysisHumanlike);
            p.needs.mood.thoughts.memories.TryGainMemory(BioReactorThoughtDef
                .KnowHistolysisHumanlikeCannibal);
            p.needs.mood.thoughts.memories.TryGainMemory(BioReactorThoughtDef
                .KnowHistolysisHumanlikePsychopath);
        }
    }

    public void MakeFuel()
    {
        var stuff = GenStuff.RandomStuffFor(ThingDefOf.Chemfuel);
        var thing = ThingMaker.MakeThing(ThingDefOf.Chemfuel, stuff);
        thing.stackCount = 35;
        GenPlace.TryPlaceThing(thing, Position, Find.CurrentMap, ThingPlaceMode.Near);
    }

    public static Building_BioReactor FindBioReactorFor(Pawn p, Pawn traveler, bool ignoreOtherReservations = false)
    {
        var enumerable = from def in DefDatabase<ThingDef>.AllDefs
            where typeof(Building_BioReactor).IsAssignableFrom(def.thingClass)
            select def;

        foreach (var singleDef in enumerable)
        {
            var building_BioReactor = (Building_BioReactor)GenClosest.ClosestThingReachable(p.Position, p.Map,
                ThingRequest.ForDef(singleDef), PathEndMode.InteractionCell, TraverseParms.For(traveler), 9999f,
                delegate(Thing x)
                {
                    bool result;
                    if (!((Building_BioReactor)x).HasAnyContents)
                    {
                        LocalTargetInfo target = x;
                        result = traveler.CanReserve(target, 1, -1, null, ignoreOtherReservations);
                    }
                    else
                    {
                        result = false;
                    }

                    return result;
                });
            if (building_BioReactor == null || building_BioReactor.forbiddable.Forbidden)
            {
                continue;
            }

            if (p.BodySize <= ((BioReactorDef)building_BioReactor.def).bodySizeMax &&
                p.BodySize >= ((BioReactorDef)building_BioReactor.def).bodySizeMin)
            {
                return building_BioReactor;
            }
        }

        return null;
    }

    public override void Tick()
    {
        base.Tick();
        switch (state)
        {
            case ReactorState.Empty:
                break;
            case ReactorState.StartFilling:
                fillpct += 0.01f;
                if (fillpct >= 1)
                {
                    state = ReactorState.Full;
                    fillpct = 0;
                    BioReactorSoundDef.Drowning.PlayOneShot(new TargetInfo(Position, Map));
                }

                break;
            case ReactorState.Full:
                break;
            case ReactorState.HistolysisStating:
                histolysisPct += 0.005f;
                if (histolysisPct >= 1)
                {
                    state = ReactorState.HistolysisEnding;
                    Histolysis();
                }

                break;
            case ReactorState.HistolysisEnding:
                histolysisPct -= 0.01f;
                if (histolysisPct <= 0)
                {
                    histolysisPct = 0;
                    state = ReactorState.HistolysisDone;
                }

                break;
            case ReactorState.HistolysisDone:
                break;
        }
    }

    protected override void DrawAt(Vector3 drawLoc, bool flip = false)
    {
        /*
         * 상태별 그래픽 UI 드로우
         *
         */
        switch (state)
        {
            case ReactorState.Empty:
                break;
            case ReactorState.StartFilling:
                foreach (var t in innerContainer)
                {
                    if (t is not Pawn pawn)
                    {
                        continue;
                    }

                    DrawInnerThing(pawn, DrawPos + innerDrawOffset);
                    LiquidDraw(new Color32(123, 255, 233, 75), fillpct);
                }

                break;
            case ReactorState.Full:
                foreach (var t in innerContainer)
                {
                    if (t is not Pawn pawn)
                    {
                        continue;
                    }

                    DrawInnerThing(pawn, DrawPos + innerDrawOffset);
                    LiquidDraw(new Color32(123, 255, 233, 75), 1);
                }

                break;
            case ReactorState.HistolysisStating:
                foreach (var t in innerContainer)
                {
                    if (t is not Pawn pawn)
                    {
                        continue;
                    }

                    DrawInnerThing(pawn, DrawPos + innerDrawOffset);
                    LiquidDraw(
                        new Color(0.48f + (0.2f * histolysisPct), 1 - (0.7f * histolysisPct),
                            0.9f - (0.6f * histolysisPct), 0.3f + (histolysisPct * 0.55f)), 1);
                }

                break;
            case ReactorState.HistolysisEnding:
                foreach (var t in innerContainer)
                {
                    t.DrawNowAt(DrawPos + innerDrawOffset, flip);
                    LiquidDraw(new Color(0.7f, 0.2f, 0.2f, 0.4f + (0.45f * histolysisPct)), 1);
                }

                break;
            case ReactorState.HistolysisDone:
                foreach (var t in innerContainer)
                {
                    t.DrawNowAt(DrawPos + innerDrawOffset, flip);
                    LiquidDraw(new Color(0.7f, 0.3f, 0.3f, 0.4f), 1);
                }

                break;
        }

        //Graphic.Draw(GenThing.TrueCenter(Position, Rot4.South, def.size, 11.7f), Rot4.South, this, 0f);
        Comps_PostDraw();
    }

    public override void Print(SectionLayer layer)
    {
        //this.Graphic.Print(layer, this);
        Printer_Plane.PrintPlane(layer, GenThing.TrueCenter(Position, Rot4.South, def.size, 11.7f), Graphic.drawSize,
            Graphic.MatSingle);
    }

    public void LiquidDraw(Color color, float fillPct)
    {
        var r = default(GenDraw.FillableBarRequest);
        r.center = DrawPos + waterDrawCenter;
        r.size = waterDrawSize;
        r.fillPercent = fillPct;
        r.filledMat = SolidColorMaterials.SimpleSolidColorMaterial(color);
        r.unfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0, 0, 0, 0));
        r.margin = 0f;
        var rotation = Rotation;
        rotation.Rotate(RotationDirection.Clockwise);
        r.rotation = rotation;
        GenDraw.DrawFillableBar(r);
    }

    public void DrawInnerThing(Pawn pawn, Vector3 rootLoc)
    {
        pawn.Drawer.renderer.RenderPawnAt(rootLoc, Rot4.South);
    }

    public static IEnumerable<Gizmo> CopyPasteGizmosFor(StorageSettings s)
    {
        yield return new Command_Action
        {
            icon = ContentFinder<Texture2D>.Get("UI/Commands/CopySettings"),
            defaultLabel = "CommandCopyBioReactorSettingsLabel".Translate(),
            defaultDesc = "CommandCopyBioReactorSettingsDesc".Translate(),
            action = delegate
            {
                SoundDefOf.Tick_High.PlayOneShotOnCamera();
                Copy(s);
            },
            hotKey = KeyBindingDefOf.Misc4
        };
        var command_Action = new Command_Action
        {
            icon = ContentFinder<Texture2D>.Get("UI/Commands/PasteSettings"),
            defaultLabel = "CommandPasteBioReactorSettingsLabel".Translate(),
            defaultDesc = "CommandPasteBioReactorSettingsDesc".Translate(),
            action = delegate
            {
                SoundDefOf.Tick_High.PlayOneShotOnCamera();
                PasteInto(s);
            },
            hotKey = KeyBindingDefOf.Misc5
        };
        if (!HasCopiedSettings)
        {
            command_Action.Disable();
        }

        yield return command_Action;
    }

    public static void Copy(StorageSettings s)
    {
        clipboard.CopyFrom(s);
        HasCopiedSettings = true;
    }

    public static void PasteInto(StorageSettings s)
    {
        s.CopyFrom(clipboard);
    }
}