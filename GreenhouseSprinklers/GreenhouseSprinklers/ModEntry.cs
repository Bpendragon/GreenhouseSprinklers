using Bpendragon.GreenhouseSprinklers.Data;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Bpendragon.GreenhouseSprinklers
{
    partial class ModEntry : Mod, IAssetLoader
    {
        private ModConfig Config;
        private ModData Data = new ModData(); //Pre-load the defaults, this guarantees Data.GetLeve() will always return a value 
        public Dictionary<int, int> BuildMaterials1 { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, int> BuildMaterials2 { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, int> BuildMaterials3 { get; set; } = new Dictionary<int, int>();
        public Difficulty difficulty;

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            SetBuildMaterials();

            helper.Events.GameLoop.DayStarted += OnDayStart;
            helper.Events.GameLoop.DayEnding += OnDayEnding;
            helper.Events.GameLoop.Saving += OnSave;
            helper.Events.GameLoop.SaveLoaded += OnLoad;
            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.ReturnedToTitle += OnReturnToTitle;
            helper.Events.GameLoop.Saved += OnSaveCompleted;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Helper.Content.AssetEditors.Add(new MyModMail());
            if(Config.ShowVisualUpgrades)
            {

            }
        }

        private void SetBuildMaterials()
        {
            var diff = Config.DifficultySettings.First(x => x.Difficulty == Config.SelectedDifficulty);
            difficulty = Config.SelectedDifficulty;
            if (null == diff)
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
                Monitor.Log("Greenhouse Located");
            }
            else
            {
                Monitor.Log("No Greenhouse found", LogLevel.Warn);
            }
            int i = 0;
            var terrainfeatures = gh.terrainFeatures.Values;
            Monitor.Log($"Found {terrainfeatures.Count()} terrainfeatures in Greenhouse");

            foreach (var tf in terrainfeatures)
            {
                if (tf is HoeDirt dirt)
                {
                    dirt.state.Value = HoeDirt.watered;
                    i++;
                }
            }
            Monitor.Log($"{i} tiles watered.");

            int j = 0;
            Monitor.Log("Watering pots in greenhouse");
            foreach (IndoorPot pot in gh.objects.Values.OfType<IndoorPot>())
            {
                if (pot.hoeDirt.Value is HoeDirt dirt)
                {
                    dirt.state.Value = HoeDirt.watered;
                    pot.showNextIndex.Value = true;
                    j++;
                }
            }

            Monitor.Log($"{j} Pots Watered");
        }

        private void WaterFarm()
        {
            var farm = Game1.getFarm();
            int i = 0;
            var terrainFeatures = farm.terrainFeatures.Values;
            Monitor.Log($"Found {terrainFeatures.Count()} terrainfeatures in Farm");

            foreach (var tf in terrainFeatures)
            {
                if (tf is HoeDirt dirt)
                {
                    dirt.state.Value = HoeDirt.watered;
                    i++;
                }
            }
            Monitor.Log($"{i} tiles watered.");

            int j = 0;
            Monitor.Log("Watering pots on farm");
            foreach (IndoorPot pot in farm.objects.Values.OfType<IndoorPot>())
            {
                if (pot.hoeDirt.Value is HoeDirt dirt)
                {
                    dirt.state.Value = HoeDirt.watered;
                    pot.showNextIndex.Value = true;
                    j++;
                }
            }

            Monitor.Log($"{j} Pots Watered");
        }

        /// <summary>Get whether this instance can load the initial version of the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanLoad<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Buildings/Greenhouse") && Data.GetLevel() > 0 && Config.ShowVisualUpgrades)
            {
                return true;
            }

            return false;
        }

        /// <summary>Load a matched asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public T Load<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Buildings/Greenhouse"))
            {
                return Helper.Content.Load<T>($"assets/Greenhouse{Data.GetLevel()}.png", ContentSource.ModFolder);
            }

            throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
        }
    }
}
