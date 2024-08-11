using System.Collections.Generic;
using Verse;

namespace BioReactor;

public class CompMapRefuelable(Map map) : MapComponent(map)
{
    public readonly List<CompBioRefuelable> comps = [];
}