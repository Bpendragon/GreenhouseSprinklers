using StardewModdingAPI.Events;
using System;

namespace Bpendragon.GreenhouseSprinklers
{
    partial class ModEntry
    {
        internal void OnDayStart(object sender, DayStartedEventArgs e)
        {
            WaterGreenHouse();
            this.Monitor.Log("Day starting");
            if (Data.FirstUpgrade)
            {
                this.Monitor.Log("first upgrade owned, watering");
                
            }
        }

        internal void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            this.Monitor.Log("Day ending");
            if (Data.SecondUpgrade)
            {
                this.Monitor.Log("second upgrade owned, watering");
                WaterGreenHouse();
            }
        }

        internal void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        
    }
}
