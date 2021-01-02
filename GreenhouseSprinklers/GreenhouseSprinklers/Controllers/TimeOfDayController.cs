using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley;
using System.Linq;

namespace Bpendragon.GreenhouseSprinklers
{
    partial class ModEntry
    {
        internal void OnDayStart(object sender, DayStartedEventArgs e)
        {
            Monitor.Log("Day starting");
            if (Data.FirstUpgrade)
            {
                Monitor.Log("first upgrade owned, watering");
                WaterGreenHouse();
            }
            if(Data.FinalUpgrade)
            {
                Monitor.Log("final ugrade owned, watering entire farm");
                WaterFarm();
            }
        }

        internal void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            Monitor.Log("Day ending");
            if (Data.SecondUpgrade) //run these checks before we check for upgrades
            {
                Monitor.Log("second upgrade owned, watering");
                WaterGreenHouse();
            }
            if (Data.FinalUpgrade)
            {
                Monitor.Log("final ugrade owned, watering entire farm");
                WaterFarm();
            }
            if (!Data.FinalUpgrade)
            {
                var silo = Game1.getFarm().buildings.Where(x => x.buildingType == "Silo" && x.daysUntilUpgrade == 1).FirstOrDefault();
                if (silo != null)
                {
                    Monitor.Log("Silo \"Upgrade\" completed, moving to next level");
                    silo.daysUntilUpgrade.Value = 0;
                    if (!Data.FirstUpgrade) Data.FirstUpgrade = true;
                    else if (!Data.SecondUpgrade) Data.SecondUpgrade = true;
                    else if (!Data.FinalUpgrade) Data.FinalUpgrade = true;
                    else Monitor.Log("Tried to Upgrade sprinklers while all upgrades already completed", LogLevel.Error);
                }
            }
        }  
    }
}
