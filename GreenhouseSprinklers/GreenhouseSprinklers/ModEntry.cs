using Bpendragon.GreenhouseSprinklers.Data;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Linq;

namespace Bpendragon.GreenhouseSprinklers
{
    partial class ModEntry : Mod
    {
        private ModConfig Config;
        private ModData Data;
        public Dictionary<int, int> BuildMaterials1 { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, int> BuildMaterials2 { get; set; } = new Dictionary<int, int>(); 
        public Dictionary<int, int> BuildMaterials3 { get; set; } = new Dictionary<int, int>();
        public Difficulty difficulty;

        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            SetBuildMaterials();
            Data = this.Helper.Data.ReadJsonFile<ModData>($"data/{Constants.SaveFolderName}.json") ?? new ModData();

           

            helper.Events.GameLoop.DayStarted   += OnDayStart;
            helper.Events.GameLoop.DayEnding    += OnDayEnding;
            helper.Events.GameLoop.Saving       += OnSave;
            //helper.Events.GameLoop.SaveLoaded   += OnLoad;
            helper.Events.Display.MenuChanged  += OnMenuChanged;
        }


        private void SetBuildMaterials()
        {
            var diff = Config.DifficultySettings.First(x => x.Difficulty == Config.SelectedDifficulty);
            this.difficulty = Config.SelectedDifficulty;
            if(null == diff)
            {
                Monitor.Log("Difficulty Settings not found or set, mod will not work properly", LogLevel.Error);
            }

            BuildMaterials1[(int)diff.FirstUpgrade.Sprinkler] = diff.FirstUpgrade.SprinklerCount;
            BuildMaterials1[787] = diff.FirstUpgrade.Batteries;

            BuildMaterials2[(int)diff.SecondUpgrade.Sprinkler] = diff.SecondUpgrade.SprinklerCount;
            BuildMaterials2[787] = diff.SecondUpgrade.Batteries;

            BuildMaterials3[(int)diff.FinalUpgrade.Sprinkler] = diff.FinalUpgrade.SprinklerCount;
            BuildMaterials3[787] = diff.FinalUpgrade.Batteries;

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
