using PipeSystem;
using RimWorld;
using Verse;

namespace BioReactor;

[StaticConstructorOnStartup]
public static class Building_BioReactor_VNPE
{
    public static readonly bool VNPELoaded = ModsConfig.IsActive("VanillaExpanded.VNutrientE");

    public static void VNPE_Check(Building bioreactor)
    {
        if (!bioreactor.IsHashIntervalTick(250))
        {
            return;
        }

        var compResource = bioreactor.GetComp<CompResource>();
        var compRefuelable = bioreactor.GetComp<CompRefuelable>();

        if (compRefuelable == null || compResource is not { PipeNet: { } net })
        {
            return;
        }

        var stored = net.Stored;
        while (compRefuelable.GetFuelCountToFullyRefuel() > 6 && stored > 0)
        {
            net.DrawAmongStorage(1, net.storages);

            stored--;
            compRefuelable.Refuel(6);
        }
    }
}