using System;
using Bpendragon.GreenhouseSprinklers.Data;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Enums;

using System.Linq;
using StardewValley;
using StardewValley.Buildings;
using System.Collections.Generic;

namespace Bpendragon.GreenhouseSprinklers
{
    partial class ModEntry
    {
        internal void OnLoad(object sender, SaveLoadedEventArgs e)
        {
            if (Config.ShowVisualUpgrades)
            {
                Monitor.Log("Invalidating Texture Cache at first Load");
                Helper.GameContent.InvalidateCache("Buildings/Greenhouse");
            }//invalidate the cache on load, forcing load of new sprite if applicable.
        }

        internal void OnSave(object sender, SavingEventArgs e)
        {
            var ghl = Game1.getFarm().buildings.OfType<GreenhouseBuilding>().ToList();

            foreach(var gh in ghl)
            {
                if (gh.buildingType.Value.StartsWith("GreenhouseSprinklers"))
                {
                    gh.buildingType.Set("Greenhouse");
                }
            }
        }

        internal void OnSaveCompleted(object sender, SavedEventArgs e)
        {
            if (Config.ShowVisualUpgrades)
            {
                Monitor.Log("Invalidating Texture Cache after save");
                Helper.GameContent.InvalidateCache("Buildings/Greenhouse");
            }//invalidate the cache each night, forcing load of new sprite if applicable.
        }
    }
}
