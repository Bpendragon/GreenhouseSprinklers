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
        private readonly int MaxOccupantsID = -794738;
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu == null) return; //if menu was closed this should fix it.
            Monitor.Log($"Menu type {e.NewMenu.GetType()} opened");
            if (!Context.IsWorldReady) return;
            if (!(e.NewMenu is CarpenterMenu)) return;
            int level = 1;
            if (Data.FirstUpgrade) level = 2;
            if (Data.SecondUpgrade) level = 3;
            if (Data.FinalUpgrade)
            {
                Monitor.Log("We've got the final upgrade, skipping");
                return; //we've built the final upgrade, 
            }
            Monitor.Log("In the Carpenter Menu, here's hoping");
            IList<BluePrint> blueprints = Helper.Reflection
                .GetField<List<BluePrint>>(e.NewMenu, "blueprints")
                .GetValue();

            blueprints.Add(GetBluePrint(level));
            Monitor.Log("Blueprint should be added");
        }

        private void OnBuildingListChanged(object sender, BuildingListChangedEventArgs e)
        {
            Monitor.Log("Building list changed");
        }

        private BluePrint GetBluePrint(int level)
        {
            string desc;
            Dictionary<int, int> buildMats;
            int money;
            UpgradeCost cost;
            int days;
            if (level == 1)
            {
                cost = Config.DifficultySettings.Find(x => x.Difficulty == difficulty);
                desc = "Automated Sprinklers on the ceiling of your greenhouse, runs every morning";
                money = cost.FirstUpgrade.Gold;
                buildMats = BuildMaterials1;
                days = cost.FirstUpgrade.DaysToConstruct;

            }
            else if (level == 2)
            {
                cost = Config.DifficultySettings.Find(x => x.Difficulty == difficulty);
                desc = "Automated Sprinklers on the ceiling of your greenhouse, Runs every morning and night";
                money = cost.SecondUpgrade.Gold;
                buildMats = BuildMaterials2;
                days = cost.SecondUpgrade.DaysToConstruct;
            }
            else
            {
                cost = Config.DifficultySettings.Find(x => x.Difficulty == difficulty);
                desc = "Hidden underground sprinklers all over the farm, runs morning and night";
                money = cost.FinalUpgrade.Gold;
                buildMats = BuildMaterials3;
                days = cost.SecondUpgrade.DaysToConstruct;
            }
            return new BluePrint("Silo")
            {
                displayName = "Sprinkler System Upgrade",
                description = desc,
                moneyRequired = money,
                nameOfBuildingToUpgrade = "Silo",
                itemsRequired = buildMats,
                daysToConstruct = days,
                maxOccupants = MaxOccupantsID,
                blueprintType = "Upgrades"
            };
        }
    }
}
