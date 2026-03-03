using StardewModdingAPI;

using StardewValley;
using StardewValley.Buildings;
using StardewValley.Extensions;
using StardewValley.GameData.Buildings;

using System;
using System.Threading;

// Modified under MIT license from https://github.com/rokugin/RobinBuildPosition
// See ~/Open Source Info/acknowledgements.md for full license

namespace Bpendragon.GreenhouseSprinklers.Patches;
internal class NPCPatch
{
    private static IMonitor Monitor;

    // call this method from your Entry class
    internal static void Initialize(IMonitor monitor)
    {
        Monitor = monitor;
    }

    public static void UpdateConstructionAnimation_Postfix(NPC __instance)
    {
        if (__instance.Name != "Robin") return;
        if (Game1.IsThereABuildingUnderConstruction())
        {
            Building b = Game1.GetBuildingUnderConstruction();
            BuildingData data = b.GetData();
            if (b.upgradeName.Value != null)
            {
                data = Game1.buildingData[b.upgradeName.Value];
            }

            if (!data.BuildingType.ContainsIgnoreCase("greenhouse")) return;

            Monitor.Log("Updating Robin's Position for when she upgrades the Greenhouse");

            int X = 1, Y = 15;

            if (b.daysUntilUpgrade.Value > 0 && b.GetIndoors() != null)
            {
                __instance.setTilePosition(X, Y);
            }
            else
            {
                (__instance).position.X += X;
                (__instance).position.Y += Y;
            }
        }
    }

}