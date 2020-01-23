using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using System;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Linq;


namespace Bpendragon.GreenhouseSprinklers
{
    partial class ModEntry
    {
        internal void OnDayStart(object sender, DayStartedEventArgs e)
        {
            this.Monitor.Log("Day starting, looking for greenhouse");

            var gh = Game1.getLocationFromName("Greenhouse");
            if (gh != null)
            { 
                this.Monitor.Log("Greenhouse Located");
            } else
            {
                this.Monitor.Log("No Greenhouse found", StardewModdingAPI.LogLevel.Warn);
            }
            int i = 0;
            foreach (var hd in gh.terrainFeatures.OfType<HoeDirt>())
            {
                hd.state.Value = 1;
                i++;
            }
            this.Monitor.Log($"{i} tiles watered.");

        }

        internal void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
