using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Bpendragon.GreenhouseSprinklers.Data;
using System.Linq;
using StardewValley.Buildings;

namespace Bpendragon.GreenhouseSprinklers
{
    partial class ModEntry
    {
        private void OnBuildingListChanged(object sender, BuildingListChangedEventArgs e)
        {
            Monitor.Log("Building list changed");
        }
    }
}
