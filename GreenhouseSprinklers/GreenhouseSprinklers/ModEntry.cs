using Bpendragon.GreenhouseSprinklers.Data;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System.Linq;

namespace Bpendragon.GreenhouseSprinklers
{
    partial class ModEntry : Mod
    {
        private ModConfig Config;
        private ModData Data;

        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            Data = this.Helper.Data.ReadJsonFile<ModData>($"data/{Constants.SaveFolderName}.json") ?? new ModData();

            helper.Events.GameLoop.DayStarted   += OnDayStart;
            helper.Events.GameLoop.DayEnding    += OnDayEnding;
            helper.Events.GameLoop.Saving       += OnSave;
            //helper.Events.GameLoop.SaveLoaded   += OnLoad;
            //helper.Events.Display.MenuChanged   += OnMenuChanged;
        }



        private void WaterGreenHouse()
        {
            var gh = Game1.getLocationFromName("Greenhouse");
            if (gh != null)
            {
                this.Monitor.Log("Greenhouse Located");
            }
            else
            {
                this.Monitor.Log("No Greenhouse found", StardewModdingAPI.LogLevel.Warn);
            }
            int i = 0;
            var terrainfeatures = gh.terrainFeatures.Values;
            this.Monitor.Log($"Found {terrainfeatures.Count()} terrainfeatures in Greenhouse");

            foreach (var tf in terrainfeatures)
            {
                if (tf is HoeDirt dirt)
                {
                    dirt.state.Value = HoeDirt.watered;
                    i++;
                }
            }
            this.Monitor.Log($"{i} tiles watered.");

            int j = 0;
            this.Monitor.Log("Watering pots in greenhouse");
            foreach (IndoorPot pot in gh.objects.Values.OfType<IndoorPot>())
            {
                if (pot.hoeDirt.Value is HoeDirt dirt)
                {
                    dirt.state.Value = HoeDirt.watered;
                    pot.showNextIndex.Value = true;
                    j++;
                }
            }

            this.Monitor.Log($"{j} Pots Watered");
        }
    }
}
