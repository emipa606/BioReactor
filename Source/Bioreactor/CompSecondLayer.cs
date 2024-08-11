using RimWorld;
using UnityEngine;
using Verse;

namespace BioReactor;

internal sealed class CompSecondLayer : ThingComp
{
    private Graphic graphicInt;
    public Vector3 offset;

    public Graphic Graphic
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

    public CompProperties_SecondLayer Props => (CompProperties_SecondLayer)props;

    public override void PostDraw()
    {
        if (parent.Rotation == Rot4.South)
        {
            Graphic.Draw(
                GenThing.TrueCenter(parent.Position, parent.Rotation, parent.def.size, Props.Altitude) + offset,
                parent.Rotation, parent);
        }
    }
}