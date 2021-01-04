using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley.Buildings;
using StardewValley;
using System.Linq;
using Bpendragon.GreenhouseSprinklers.Data;

namespace Bpendragon.GreenhouseSprinklers
{
    partial class ModEntry
    {
        internal void OnDayStart(object sender, DayStartedEventArgs e)
        {
            if (Data.GetLevel() >= 1)
            {
                Monitor.Log("Watering the Greenhouse", LogLevel.Info);
                WaterGreenHouse();
            }
            if(Data.GetLevel() >= 3)
            {
                Monitor.Log("Watering entire farm", LogLevel.Info);
                WaterFarm();
            }
        }

        internal void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            CheckLetterStatus();
            AddLetterIfNeeded();
            Monitor.Log("Day ending");
            if (Data.GetLevel() >= 2) //run these checks before we check for upgrades
            {
                Monitor.Log("Watering the Greenhouse", LogLevel.Info);
                WaterGreenHouse();
            }
            if (Data.GetLevel() >= 3)
            {
                Monitor.Log("Watering entire farm", LogLevel.Info);
                WaterFarm();
            }
            if (!Data.FinalUpgrade)
            {
                var greenhouse = Game1.getFarm().buildings.Where(x => x.buildingType == "Greenhouse" && x.daysUntilUpgrade > 1).FirstOrDefault();
                if(greenhouse != null && !Data.IsUpgrading)
                {
                    UpgradeCost cost = Config.DifficultySettings.Find(x => x.Difficulty == difficulty);
                    greenhouse.daysUntilUpgrade.Value += (Data.GetLevel()) switch 
                    {
                        0 => cost.FirstUpgrade.DaysToConstruct - 1,
                        1 => cost.SecondUpgrade.DaysToConstruct - 1,
                        2 => cost.FinalUpgrade.DaysToConstruct - 1,
                        _ => 0
                    };

                    Data.IsUpgrading = true;
                }
                greenhouse = Game1.getFarm().buildings.Where(x => x.buildingType == "Greenhouse" && x.daysUntilUpgrade == 1).FirstOrDefault();
                if (greenhouse != null)
                {
                    Monitor.Log("Greenhouse Upgrade completed, moving to next level", LogLevel.Info);
                    greenhouse.daysUntilUpgrade.Value = 0;
                    if (!Data.FirstUpgrade) Data.FirstUpgrade = true;
                    else if (!Data.SecondUpgrade) Data.SecondUpgrade = true;
                    else if (!Data.FinalUpgrade) Data.FinalUpgrade = true;
                    else Monitor.Log("Tried to Upgrade sprinklers while all upgrades already completed", LogLevel.Error);
                    Data.IsUpgrading = false;
                }

            }
        }  

        private void CheckLetterStatus()
        {
            if (Game1.player.mailReceived.Contains("Bpendragon.GreenhouseSprinklers.Wizard1")) Data.HasRecievedLetter1 = true;
            if (Game1.player.mailReceived.Contains("Bpendragon.GreenhouseSprinklers.Wizard1b")) Data.HasRecievedLetter1 = true;
            if (Game1.player.mailReceived.Contains("Bpendragon.GreenhouseSprinklers.Wizard2")) Data.HasRecievedLetter2 = true;
            if (Game1.player.mailReceived.Contains("Bpendragon.GreenhouseSprinklers.Wizard3")) Data.HasRecievedLetter3 = true;
        }

        private void AddLetterIfNeeded()
        {
            if (Data.FinalUpgrade) return; //We've upgraded all the way, no need to go further
            bool canRecieveMail = true;
            bool jojaMember = Game1.player.hasOrWillReceiveMail("jojaMember");
            //Check if has forsaken the Junimos
            if (jojaMember)
            {
                canRecieveMail = Game1.getFarm().buildings.Any(x => x is JunimoHut);
            } 

            if(canRecieveMail)
            {
                var curLevel = Data.GetLevel();
                var requirements = Config.DifficultySettings.Find(x => x.Difficulty == difficulty);
                if(!Game1.player.friendshipData.TryGetValue("Wizard", out var wizard))
                {
                    return; //Haven't ever talked to the Wizard, thus can't send us mail
                }
                switch(curLevel)
                {
                    case 0: if(wizard.Points >= 250 * requirements.FirstUpgrade.Hearts && !Data.HasRecievedLetter1)
                        {
                            if (jojaMember) Game1.addMailForTomorrow("Bpendragon.GreenhouseSprinklers.Wizard1b");
                            else Game1.addMailForTomorrow("Bpendragon.GreenhouseSprinklers.Wizard1");
                        }
                        break;
                    case 1:
                        if (wizard.Points >= 250 * requirements.SecondUpgrade.Hearts && !Data.HasRecievedLetter2)
                        {
                            Game1.addMailForTomorrow("Bpendragon.GreenhouseSprinklers.Wizard2");
                        }
                        break;
                    case 2:
                        if (wizard.Points >= 250 * requirements.FinalUpgrade.Hearts && !Data.HasRecievedLetter3)
                        {
                            Game1.addMailForTomorrow("Bpendragon.GreenhouseSprinklers.Wizard3");
                        }
                        break;
                }
            }
        }
    }
}
