using UnityEngine;
using Verse;

namespace BioReactor;

internal class CompProperties_SecondLayer : CompProperties
{
    private readonly AltitudeLayer altitudeLayer = AltitudeLayer.MoteOverhead;
    public readonly GraphicData graphicData = null;
    public Vector3 offset = new();

    public CompProperties_SecondLayer()
    {
        compClass = typeof(CompSecondLayer);
    }

    public float Altitude => altitudeLayer.AltitudeFor();
}