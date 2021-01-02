using System;
using Bpendragon.GreenhouseSprinklers.Data;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Bpendragon.GreenhouseSprinklers
{
    partial class ModEntry
    {
        internal void OnLoad(object sender, SaveLoadedEventArgs e)
        {
            Data = Helper.Data.ReadJsonFile<ModData>($"data/{Constants.SaveFolderName}.json") ?? new ModData();
        }

        internal void OnSave(object sender, SavingEventArgs e)
        {
            Helper.Data.WriteJsonFile($"data/{Constants.SaveFolderName}.json", Data);
        }
    }
}
