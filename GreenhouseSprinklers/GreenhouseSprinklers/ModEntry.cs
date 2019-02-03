using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Bpendragon.GreenhouseSprinklers
{
    class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.TimeChanged += this.TimeChanged;
            helper.Events.GameLoop.Saving += this.Saving;
        }

        private void Saving(object sender, SavingEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TimeChanged(object sender, TimeChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
