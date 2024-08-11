using UnityEngine;
using Verse;

namespace BioReactor;

internal class CompProperties_SecondLayer : CompProperties
{
    public readonly AltitudeLayer altitudeLayer = AltitudeLayer.MoteOverhead;
    public readonly GraphicData graphicData = null;
    public Vector3 offset = new Vector3();

    public CompProperties_SecondLayer()
    {
        compClass = typeof(CompSecondLayer);
    }

    public float Altitude => altitudeLayer.AltitudeFor();
}