using Bpendragon.GreenhouseSprinklers.Data;
using Bpendragon.GreenhouseSprinklers.Patches;

using Force.DeepCloner;

using GreenhouseSprinklers.APIs;

using HarmonyLib;

using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Buildings;
using StardewValley.Delegates;
using StardewValley.GameData.Buildings;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Bpendragon.GreenhouseSprinklers
{
    partial class ModEntry : Mod
    {
        private ModConfig Config;
        public Dictionary<int, int> BuildMaterials1 { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, int> BuildMaterials2 { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, int> BuildMaterials3 { get; set; } = new Dictionary<int, int>();
        public Difficulty difficulty;
        const string ModDataKey = "Bpendragon.GreenhouseSprinklers.GHLevel";
        private bool MailChangesMade = false;

        public static int GetUpgradeLevel(GreenhouseBuilding greenhouse)
        {
            return greenhouse.modData.TryGetValue(ModDataKey, out string level)
               ? int.Parse(level)
               : 0;
        }

        public override void Entry(IModHelper helper)
        {
            //set up for translations
            I18n.Init(helper.Translation);
            //read settings
            try
            {
                Config = Helper.ReadConfig<ModConfig>();
            } 
            catch (Exception e)
            {
                Monitor.Log("Old Config Style Detected, attempting to upgrade", LogLevel.Alert);
                var oldConfig = Helper.ReadConfig<OldConfigModel>();
                Config = new ModConfig()
                {
                    SelectedDifficulty = oldConfig.SelectedDifficulty,
                    ShowVisualUpgrades = oldConfig.ShowVisualUpgrades,
                    WaterSandOnBeachFarm = oldConfig.WaterSandOnBeachFarm,
                    MaxNumberOfUpgrades = oldConfig.MaxNumberOfUpgrades,
                    BuildDays = oldConfig.BuildDays,
                    DifficultySettings = oldConfig.DifficultySettings.ToDictionary(x => x.Difficulty)
                };
                Monitor.Log("Old Config Succesfully upgraded to new style, saving new file", LogLevel.Alert);
                Helper.WriteConfig<ModConfig>(Config);
            }
            SetBuildMaterials();

            helper.ConsoleCommands.Add("ghs_setlevel", "Sets the level for the greenhouse.\n\nUsage: ghs_setlevel <value>\n- value: integer between 0 and 3 inclusive", SetGHLevel);
            helper.ConsoleCommands.Add("ghs_getlevel", "Returns the level for the greenhouse.\n\nUsage: ghs_setlevel", GetGHLevel);
            helper.ConsoleCommands.Add("ghs_waternow", "Debug command to force watering the greenhouse (and farm if level 3 unlocked).\n\nUsage: ghs_waternow", WaterNow);

            //Register Event Listeners
            helper.Events.GameLoop.DayStarted += OnDayStart;
            helper.Events.GameLoop.DayEnding += OnDayEnding;
            helper.Events.GameLoop.SaveLoaded += OnLoad;
            helper.Events.GameLoop.Saving += OnSave;
            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.Saved += OnSaveCompleted;
            helper.Events.Content.AssetRequested += OnAssetRequested;

            GameStateQuery.Register("GreenHouseSprinklers.BuildCondition", CheckForUpgrade);

            //Harmony Patches
            var harmony = new Harmony(ModManifest.UniqueID);
            NPCPatch.Initialize(Monitor);

            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), "updateConstructionAnimation"),
                postfix: new HarmonyMethod(typeof(NPCPatch), nameof(NPCPatch.UpdateConstructionAnimation_Postfix))
                );
        }

        private void WaterNow(string command, string[] args)
        {
            int level = GetUpgradeLevel(Game1.getFarm().buildings.OfType<GreenhouseBuilding>().FirstOrDefault());
            Monitor.Log($"Greenhouse is level {level}", LogLevel.Info);
            if (level == 0) Monitor.Log($"Watering nothing, no upgrades purchased", LogLevel.Info);
            if (level >= 1)
            {
                Monitor.Log($"Watering Greenhouse", LogLevel.Info);
                WaterGreenHouse();
            }
            if (level >= 3)
            {
                Monitor.Log($"Watering Farm", LogLevel.Info);
                WaterFarm();
            }
        }

        private void SetGHLevel(string command, string[] args)
        {
            var gh = Game1.getFarm().buildings.OfType<GreenhouseBuilding>().FirstOrDefault();
            gh.modData[ModDataKey] = args[0];

            if (Config.ShowVisualUpgrades) Helper.GameContent.InvalidateCache("Buildings/Greenhouse");

            Monitor.Log($"Set Greenhouse to level {args[0]} and refreshed texture cache if required.", LogLevel.Info);

        }

        private void GetGHLevel(string command, string[] args)
        {
            var gh = Game1.getFarm().buildings.OfType<GreenhouseBuilding>().FirstOrDefault();
            Monitor.Log($"Greenhouse is level {GetUpgradeLevel(gh)}.", LogLevel.Info);
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            //Register APIs
            var contentPatcherApi = Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            InitializeApis(contentPatcherApi, configMenu);
        }

        private void SetBuildMaterials()
        {
            var diff = Config.DifficultySettings[Config.SelectedDifficulty];
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
                bool shouldWaterTile = true;
                if (Game1.whichFarm == Farm.beach_layout && !Config.WaterSandOnBeachFarm)
                {
                    //If Property "NoSprinklers" is Set to true, don't water
                    shouldWaterTile = farm.doesTileHaveProperty((int)tf.Tile.X, (int)tf.Tile.Y, "NoSprinklers", "Back") != "T";
                }

                if (tf is HoeDirt dirt && shouldWaterTile)
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
            if (!Context.IsWorldReady) return false;
            var ghl = Game1.getFarm().buildings.OfType<GreenhouseBuilding>();
            if (asset.Name.IsEquivalentTo("Buildings/Greenhouse") && ghl.Any(gh => GetUpgradeLevel(gh) > 0) && Config.ShowVisualUpgrades)
            {
                return true;
            }

            return false;
        }

        internal bool CheckForUpgrade(string[] query, GameStateQueryContext context)
        {
            var gh = Game1.getFarm().buildings.OfType<GreenhouseBuilding>().FirstOrDefault();
            int bluePrintLevel = GetUpgradeLevel(gh) + 1;
            bool parseTest = int.TryParse(query[^1], out int requestedLevel);
            if (!parseTest)
            {
                Monitor.Log($"{query.Join(delimiter: " ")} is not a valid query, {query[^1]} was unable to be parsed to an int.", LogLevel.Error);
                return false;
            }

            //Don't add blueprint if we haven't recieved the letter from the wizard yet
            if (bluePrintLevel != requestedLevel) return false;
            if (!(Game1.player.mailReceived.Contains($"Bpendragon.GreenhouseSprinklers.Wizard{requestedLevel}") || Game1.player.mailReceived.Contains($"Bpendragon.GreenhouseSprinklers.Wizard{requestedLevel}b"))) return false;
            return true;
        }


        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (Context.IsWorldReady
                && e.Name.IsEquivalentTo("Buildings/Greenhouse")
                && Config.ShowVisualUpgrades)
            {
                var gh = Game1.getFarm().buildings.OfType<GreenhouseBuilding>().FirstOrDefault();

                switch (GetUpgradeLevel(gh))
                {
                    case int i when i <= 0: break;
                    case 1:
                    case 2:
                    case 3:
                        e.LoadFromModFile<Texture2D>($"assets/Greenhouse{GetUpgradeLevel(gh)}.png", AssetLoadPriority.Low);
                        break;
                    default:
                        e.LoadFromModFile<Texture2D>($"assets/Greenhouse3.png", AssetLoadPriority.Low);
                        break;
                }
            }

            if (!MailChangesMade && e.NameWithoutLocale.IsEquivalentTo(@"Data\mail"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;

                    data["Bpendragon.GreenhouseSprinklers.Wizard1"] = I18n.Mail_Wizard1();
                    data["Bpendragon.GreenhouseSprinklers.Wizard1b"] = I18n.Mail_Wizard1b();
                    data["Bpendragon.GreenhouseSprinklers.Wizard2"] = I18n.Mail_Wizard2();
                    data["Bpendragon.GreenhouseSprinklers.Wizard3"] = I18n.Mail_Wizard3();
                });
                MailChangesMade = true;
            }


            if (e.NameWithoutLocale.IsEquivalentTo("Data/Buildings"))
            {
                UpgradeCost cost = Config.DifficultySettings[difficulty];

                e.Edit(delegate (IAssetData data)
                {
                    var dict = data.AsDictionary<string, BuildingData>();

                    BuildingData bd1 = dict.Data["Greenhouse"].DeepClone();
                    BuildingData bd2 = dict.Data["Greenhouse"].DeepClone();
                    BuildingData bd3 = dict.Data["Greenhouse"].DeepClone();

                    bd1.Name = "Greenhouse Sprinkler Upgrade";

                    bd1.Texture = "Buildings\\Greenhouse";
                    bd1.Description = I18n.CarpenterShop_FirstUpgradeDescription();
                    bd1.Builder = "Robin";
                    bd1.BuildCondition = $"GreenHouseSprinklers.BuildCondition 1";
                    bd1.BuildingToUpgrade = "Greenhouse";
                    bd1.NameForGeneralType = "Greenhouse";
                    bd1.BuildCost = cost.FirstUpgrade.Gold;
                    bd1.BuildDays = Config.BuildDays;
                    bd1.BuildMaterials = new()
                    {
                        new() { ItemId = $"(O){(int)cost.FirstUpgrade.Sprinkler}", Amount = cost.FirstUpgrade.SprinklerCount },
                        new() { ItemId = "(O)787", Amount = cost.FirstUpgrade.Batteries }
                    };
                    bd1.ModData = new() { { "Bpendragon.GreenhouseSprinklers.GHLevel", "1" } };

                    dict.Data["GreenhouseSprinklers.Upgrade1"] =  bd1;

                    //Upgrade 2
                    bd2.Name = "Greenhouse Sprinkler Upgrade 2";
                    bd2.Texture = "Buildings\\Greenhouse";
                    bd2.Description = I18n.CarpenterShop_SecondUpgradeDescription();
                    bd2.Builder = "Robin";
                    bd2.BuildCondition = $"GreenHouseSprinklers.BuildCondition 2";
                    bd2.BuildingToUpgrade = "Greenhouse";
                    bd1.NameForGeneralType = "Greenhouse";
                    bd2.BuildCost = cost.SecondUpgrade.Gold;
                    bd2.BuildDays = Config.BuildDays;
                    bd2.BuildMaterials = new()
                    {
                        new() { ItemId = $"(O){(int)cost.SecondUpgrade.Sprinkler}", Amount = cost.SecondUpgrade.SprinklerCount },
                        new() { ItemId = "(O)787", Amount = cost.SecondUpgrade.Batteries }
                    };
                    bd2.ModData = new() { { "Bpendragon.GreenhouseSprinklers.GHLevel", "2" } };

                    dict.Data["GreenhouseSprinklers.Upgrade2"] = bd2;

                    //Upgrade 3
                    bd3.Name = "Greenhouse Sprinkler Upgrade 3";
                    bd3.Texture = "Buildings\\Greenhouse";
                    bd3.Description = I18n.CarpenterShop_FinalUpgradeDescription();
                    bd3.Builder = "Robin";
                    bd3.BuildCondition = $"GreenHouseSprinklers.BuildCondition 3";
                    bd3.BuildingToUpgrade = "Greenhouse";
                    bd1.NameForGeneralType = "Greenhouse";
                    bd3.BuildCost = cost.FinalUpgrade.Gold;
                    bd3.BuildDays = Config.BuildDays;
                    bd3.BuildMaterials = new()
                    {
                        new() { ItemId = $"(O){(int)cost.FinalUpgrade.Sprinkler}", Amount = cost.FinalUpgrade.SprinklerCount },
                        new() { ItemId = "(O)787", Amount = cost.FinalUpgrade.Batteries }
                    };
                    bd3.ModData = new() { { "Bpendragon.GreenhouseSprinklers.GHLevel", "3" } };

                    dict.Data["GreenhouseSprinklers.Upgrade3"] =  bd3;

                });
            }
        }
    }
}
