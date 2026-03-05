using Bpendragon.GreenhouseSprinklers.Data;

using GreenhouseSprinklers.APIs;

using Microsoft.CodeAnalysis.FlowAnalysis;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Buildings;

using System;
using System.Linq;

namespace Bpendragon.GreenhouseSprinklers
{
    partial class ModEntry
    {
        Difficulty Easy = Difficulty.Easy;
        Difficulty Medium = Difficulty.Medium;
        Difficulty Hard = Difficulty.Hard;
        Difficulty Custom = Difficulty.Custom;


        public void InitializeApis(IContentPatcherAPI contentPatcherApi, IGenericModConfigMenuApi configMenu)
        {
            if (contentPatcherApi != null)
            {
                contentPatcherApi?.RegisterToken(ModManifest, "GreenHouseLevel", () =>
                {

                    if (Context.IsWorldReady)
                    {
                        var gh = Game1.getFarm().buildings.OfType<GreenhouseBuilding>().FirstOrDefault();
                        return new[] { GetUpgradeLevel(gh).ToString() };
                    }
                    else return new[] { "0" };
                });
            }

            if (configMenu != null)
            {
                SetupModMenu(configMenu);
            }
        }

        private void SetupModMenu(IGenericModConfigMenuApi configMenu)
        {
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () =>
                {
                    Helper.WriteConfig(Config);
                    SetBuildMaterials();
                    Helper.GameContent.InvalidateCache("Data/Buildings");
                }
                );

            //General Settings
            configMenu.AddSectionTitle(ModManifest, () => "General Settings");

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("menu.ShowVisualUpgrades-Option"),
                tooltip: () => Helper.Translation.Get("menu.ShowVisualUpgrades-Tooltip"),
                getValue: () => Config.ShowVisualUpgrades,
                setValue: value => Config.ShowVisualUpgrades = value
                );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Water Sand on Beach Farm",
                tooltip: () => "Sandy areas of the Beach Farm can't normally be watered by sprinklers. Enable this to allow the Level 3 upgrade to override this behavior.",
                getValue: () => Config.WaterSandOnBeachFarm,
                setValue: value => Config.WaterSandOnBeachFarm = value
                );

            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("menu.diff"),
                getValue: () => Enum.GetName(Config.SelectedDifficulty),
                setValue: value =>
                {
                    Config.SelectedDifficulty = Enum.Parse<Difficulty>(value);
                },
                allowedValues: new string[] { "Easy", "Medium", "Hard", "Custom" },
                formatAllowedValue: inVal =>
                {
                    return inVal switch
                    {
                        "Easy" => (string)Helper.Translation.Get("menu.diff-Easy"),
                        "Medium" => (string)Helper.Translation.Get("menu.diff-Medium"),
                        "Hard" => (string)Helper.Translation.Get("menu.diff-Hard"),
                        _ => (string)Helper.Translation.Get("menu.diff-Custom"),
                    };
                }
                );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Max Number of Upgrades",
                getValue: () => Config.MaxNumberOfUpgrades,
                setValue: value => Config.MaxNumberOfUpgrades = value,
                min: 0,
                max: 3
                );

            //First Upgrade Settings
            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => "First Upgrade Settings"
                );

            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Sprinkler Type",
                getValue: () => Enum.GetName(Config.DifficultySettings[Config.SelectedDifficulty].FirstUpgrade.Sprinkler),
                setValue: value =>
                {
                    if (Config.SelectedDifficulty != Custom)
                    {
                        CopyValuesToDefault(Config.SelectedDifficulty);
                        Config.SelectedDifficulty = Custom;
                    }
                    Config.DifficultySettings[Custom].FirstUpgrade.Sprinkler = Enum.Parse<SprinklerType>(value);
                },
                allowedValues: new string[] {"Basic", "Quality", "Iridium"}
                );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Sprinkler Count",
                getValue: () => Config.DifficultySettings[Config.SelectedDifficulty].FirstUpgrade.SprinklerCount,
                setValue: value =>
                {
                    if (Config.SelectedDifficulty != Custom)
                    {
                        CopyValuesToDefault(Config.SelectedDifficulty);
                        Config.SelectedDifficulty = Custom;
                    }
                    Config.DifficultySettings[Custom].FirstUpgrade.SprinklerCount = value;
                },
                min: 1
                );
        }

        private void CopyValuesToDefault(Difficulty oldDiff)
        {
            Config.DifficultySettings[Custom].FirstUpgrade = Config.DifficultySettings[oldDiff].FirstUpgrade;
            Config.DifficultySettings[Custom].SecondUpgrade = Config.DifficultySettings[oldDiff].SecondUpgrade;
            Config.DifficultySettings[Custom].FinalUpgrade = Config.DifficultySettings[oldDiff].FinalUpgrade;
        }
    }
}
