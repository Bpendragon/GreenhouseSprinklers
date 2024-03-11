using Microsoft.CodeAnalysis.CSharp.Syntax;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;

using System.ComponentModel;
using System.Linq;

namespace Bpendragon.GreenhouseSprinklers
{
    partial class ModEntry
    {
        private void OnBuildingListChanged(object sender, BuildingListChangedEventArgs e)
        {
            Monitor.Log("Building list changed");
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if(e.IsLocalPlayer && e.NewLocation.Name == "ScienceHouse")
            {
                foreach (var gh in Game1.getFarm().buildings.OfType<GreenhouseBuilding>())
                {
                    int lvl = GetUpgradeLevel(gh);
                    string oldType = gh.buildingType.Get();

                    gh.modData.Add($"{ModPrefix}.OldType", oldType);

                    //If a Level 0 building, even from a different mod, we need it called "Greenhouse"
                    if (lvl == 0)
                    {
                        gh.buildingType.Set("Greenhouse");
                        continue;
                    }

                    gh.buildingType.Set($"{ModPrefix}.Upgrade{lvl}");
                }
            }
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (e.OldMenu is CarpenterMenu)
            {
                foreach (var gh in Game1.getFarm().buildings.OfType<GreenhouseBuilding>())
                {
                    if (gh.modData.TryGetValue($"{ModPrefix}.OldType", out string oldType))
                    {
                        gh.buildingType.Set(oldType); 
                    } else
                    {
                        gh.buildingType.Set("Greenhouse");
                    }
                    gh.modData.Remove($"{ModPrefix}.OldType");
                }

                if(Config.ShowVisualUpgrades)
                {
                    Helper.GameContent.InvalidateCache("Buildings/Greenhouse");
                }

            }
        }
    }
}
