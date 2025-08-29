using RimWorld;
using UnityEngine;
using Verse;

namespace BioReactor;

internal sealed class CompSecondLayer : ThingComp
{
    private Graphic graphicInt;
    private Vector3 offset;

    private Graphic Graphic
    {
        get
        {
            if (graphicInt != null)
            {
                return graphicInt;
            }

            if (Props.graphicData == null)
            {
                Log.ErrorOnce(
                    $"{parent.def}BioReactor - has no SecondLayer graphicData but we are trying to access it.",
                    764532);
                return BaseContent.BadGraphic;
            }

            graphicInt = Props.graphicData.GraphicColoredFor(parent);
            offset = Props.offset;

            return graphicInt;
        }
    }

    private CompProperties_SecondLayer Props => (CompProperties_SecondLayer)props;

    public override void DrawAt(Vector3 drawLoc, bool flip = false)
    {
        if (((Building_BioReactor)parent).PauseDrawing)
        {
            return;
        }

        base.DrawAt(drawLoc, flip);
    }

    public override void PostDraw()
    {
        if (((Building_BioReactor)parent).PauseDrawing)
        {
            return;
        }

        if (parent.Rotation == Rot4.South)
        {
            Graphic.Draw(
                GenThing.TrueCenter(parent.Position, parent.Rotation, parent.def.size, Props.Altitude) + offset,
                parent.Rotation, parent);
        }
    }
}