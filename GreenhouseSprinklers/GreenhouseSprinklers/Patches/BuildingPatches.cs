using System;
using StardewValley.Buildings;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using Bpendragon.GreenhouseSprinklers.Data;

namespace Bpendragon.GreenhouseSprinklers.Patches
{
    class BuildingPatches
    {
        private static IMonitor Monitor;
        private static ModData Data;
        private static ModConfig Config;
        private static IModHelper Helper;
        public static void Initialize(IMonitor monitor, IModHelper helper, ModData data, ModConfig config)
        {
            Monitor = monitor;
            Data = data;
            Config = config;
            Helper = helper;
        }

        public static bool Upgrade_Prefix(GreenhouseBuilding __instance, int dayOfMonth)
        {
            try
            {
                if (__instance.daysUntilUpgrade.Value == 1)
                {
                    Monitor.Log("Greenhouse Upgrade completed, moving to next level", LogLevel.Info);
                    __instance.daysUntilUpgrade.Value = 0;
                    if (!Data.FirstUpgrade) Data.FirstUpgrade = true;
                    else if (!Data.SecondUpgrade) Data.SecondUpgrade = true;
                    else if (!Data.FinalUpgrade) Data.FinalUpgrade = true;
                    else Monitor.Log("Tried to Upgrade sprinklers while all upgrades already completed", LogLevel.Error);
                    Data.IsUpgrading = false;
                    if (Config.ShowVisualUpgrades)
                    {
                        Helper.Content.InvalidateCache("Buildings/Greenhouse");
                    }
                }
                return true; //We only touched the "count down days" portion, we don't care about the rest of it

            } catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(Upgrade_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
