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
            if (!Context.IsWorldReady) return; //World Hasn't Loaded yet, it's definitely not the menu we want
            if (e.NewMenu == null) return; //Menu was closed
            if (!Game1.getFarm().greenhouseUnlocked.Value) return; //Greenhouse has not been unlocked. You aren't gonna be able to add sprinklers to it. 
            if (!(e.NewMenu is CarpenterMenu)) return; //We aren't in Robin's Carpenter menu
            //Figure out which level of the Upgrade we already have to allow us to select the appropriate upgrade
            int bluePrintLevel = Data.GetLevel() + 1;
            if (Data.FinalUpgrade)
            {
                Monitor.Log("We've got the final upgrade, skipping");
                return; //we've built the final upgrade, 
            }
            if (Data.IsUpgrading) return; //already upgrading don't display it again
            Monitor.Log("In the Carpenter Menu, here's hoping");
            CheckLetterStatus();
            if (bluePrintLevel > Config.MaxNumberOfUpgrades) return; //User decided they didn't want all the upgrades. 
            //Don't add blueprint if we haven't recieved the letter from the wizard yet
            if (bluePrintLevel == 1 && !Data.HasRecievedLetter1) return;
            if (bluePrintLevel == 2 && !Data.HasRecievedLetter2) return;
            if (bluePrintLevel == 3 && !Data.HasRecievedLetter3) return;

            IList<BluePrint> blueprints = Helper.Reflection
                .GetField<List<BluePrint>>(e.NewMenu, "blueprints")
                .GetValue();

            blueprints.Add(GetBluePrint(bluePrintLevel));
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
            UpgradeCost cost =  Config.DifficultySettings.Find(x => x.Difficulty == difficulty);
            int days;
            if (level == 1)
            {
                desc = "Automated Sprinklers on the ceiling of your greenhouse, runs every morning";
                money = cost.FirstUpgrade.Gold;
                buildMats = BuildMaterials1;
                days = cost.FirstUpgrade.DaysToConstruct;

            }
            else if (level == 2)
            {
                desc = "Automated Sprinklers on the ceiling of your greenhouse, Runs every morning and night";
                money = cost.SecondUpgrade.Gold;
                buildMats = BuildMaterials2;
                days = cost.SecondUpgrade.DaysToConstruct;
            }
            else
            {
                desc = "Hidden underground sprinklers all over the farm, runs morning and night";
                money = cost.FinalUpgrade.Gold;
                buildMats = BuildMaterials3;
                days = cost.FinalUpgrade.DaysToConstruct;
            }
            return new BluePrint("Greenhouse")
            {
                displayName = "Sprinkler System Upgrade",
                description = desc,
                moneyRequired = money,
                nameOfBuildingToUpgrade = "Greenhouse",
                itemsRequired = buildMats,
                daysToConstruct = days,
                maxOccupants = MaxOccupantsID,
                blueprintType = "Upgrades"
            };
        }
    }
}
