using System;
using Bpendragon.GreenhouseSprinklers.Data;
using StardewModdingAPI;
using StardewModdingAPI.Events;

using System.Linq;
using StardewValley;

namespace Bpendragon.GreenhouseSprinklers
{
    partial class ModEntry
    {
        internal void OnLoad(object sender, SaveLoadedEventArgs e)
        {
            Data = Helper.Data.ReadJsonFile<ModData>($"data/{Constants.SaveFolderName}.json") ?? new ModData();
            var greenhouse = Game1.getFarm().buildings.Where(x => x.buildingType == "Greenhouse").FirstOrDefault();
            //Quickly default it to 0 for sanity
            if (!greenhouse.modData.ContainsKey(ModDataKey))
                greenhouse.modData[ModDataKey] = "0";
            
            if (!Data.SaveHasBeenUpgraded)
            {
                greenhouse.modData[ModDataKey] = Data.GetLevel().ToString();
                Data.SaveHasBeenUpgraded = true;
                Helper.Data.WriteJsonFile($"data/{Constants.SaveFolderName}.json", Data);
            }

            CurLevel = int.Parse(greenhouse.modData[ModDataKey]);
            if (Config.ShowVisualUpgrades) Helper.Content.InvalidateCache("Buildings/Greenhouse"); //invalidate the cache on load, forcing load of new sprite if applicable.
        }

        internal void OnSave(object sender, SavingEventArgs e)
        {
            Helper.Data.WriteJsonFile($"data/{Constants.SaveFolderName}.json", Data);
        }

        internal void OnSaveCompleted(object sender, SavedEventArgs e)
        {
            if(Config.ShowVisualUpgrades) Helper.Content.InvalidateCache("Buildings/Greenhouse"); //invalidate the cache each night, forcing load of new sprite if applicable.
        }

        internal void OnReturnToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            if (Config.ShowVisualUpgrades)
            {
                Data = new ModData(); //Force Back to Defaults.
                Helper.Content.InvalidateCache("Buildings/Greenhouse");
            }
        }
    }
}
