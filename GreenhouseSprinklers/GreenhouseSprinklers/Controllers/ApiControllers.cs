using Bpendragon.GreenhouseSprinklers.Data;

using GreenhouseSprinklers.APIs;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Buildings;
using StardewValley.Network.ChestHit;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Bpendragon.GreenhouseSprinklers
{
    partial class ModEntry
    {
        readonly Difficulty Easy = Difficulty.Easy;
        readonly Difficulty Medium = Difficulty.Medium;
        readonly Difficulty Hard = Difficulty.Hard;
        readonly Difficulty Custom = Difficulty.Custom;
        List<string> IgnoredFieldChanges = new() { "showVisualUpgradesField", "waterSandyBeachField", "maxUpgradeField", "upgradeTimeField" };

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

        private void SetupModMenu(IGenericModConfigMenuApi menuConfig)
        {
            menuConfig.Register(
                mod: ModManifest,
                reset: () =>
                {
                    Config = new ModConfig();
                },
                save: () =>
                {
                    Helper.WriteConfig(Config);
                    SetBuildMaterials();
                    Helper.GameContent.InvalidateCache("Data/Buildings");
                    menuConfig.OpenModMenu(ModManifest);
                }
                );

            //General Settings
            menuConfig.AddSectionTitle(ModManifest, () => "General Settings", () => "Setting not effected by difficulty.");

            menuConfig.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("menu.ShowVisualUpgrades-Option"),
                tooltip: () => Helper.Translation.Get("menu.ShowVisualUpgrades-Tooltip"),
                getValue: () => Config.ShowVisualUpgrades,
                setValue: value => Config.ShowVisualUpgrades = value,
                fieldId: "showVisualUpgradesField"
                );

            menuConfig.AddBoolOption(
                mod: ModManifest,
                name: () => "Water Sand on Beach Farm",
                tooltip: () => "Sandy areas of the Beach Farm can't normally be watered by sprinklers. Enable this to allow the Level 3 upgrade to override this behavior.",
                getValue: () => Config.WaterSandOnBeachFarm,
                setValue: value => Config.WaterSandOnBeachFarm = value,
                fieldId: "waterSandyBeachField"
                );

            menuConfig.AddNumberOption(
                mod: ModManifest,
                name: () => "Max Number of Upgrades",
                getValue: () => Config.MaxNumberOfUpgrades,
                setValue: value => Config.MaxNumberOfUpgrades = value,
                min: 0,
                max: 3,
                fieldId: "maxUpgradeField"
                );

            menuConfig.AddNumberOption(
                mod: ModManifest,
                name: () => "Days To Upgrade",
                tooltip: () => "How long it takes Robin to finish the upgrade.",
                getValue: () => Config.MaxNumberOfUpgrades,
                setValue: value => Config.MaxNumberOfUpgrades = value,
                min: 0,
                max: 5,
                fieldId: "upgradeTimeField"
                );

            menuConfig.AddSectionTitle(
                mod: ModManifest,
                text: () =>
                {
                    var selected = Config.SelectedDifficulty switch
                    {
                        Difficulty.Easy => (string)Helper.Translation.Get("menu.diff-Easy"),
                        Difficulty.Medium => (string)Helper.Translation.Get("menu.diff-Medium"),
                        Difficulty.Hard => (string)Helper.Translation.Get("menu.diff-Hard"),
                        _ => (string)Helper.Translation.Get("menu.diff-Custom"),
                    };
                    return $"Current Selected Difficulty: {selected}";
                },
                tooltip: () => "The currently selected diffulty. NOTE: if you change a value and hit save, this will automatically change to 'Custom' in order to keep the defaults always the same."
                );

            menuConfig.AddPageLink(ModManifest, "DifficultySelectPage", () => "Select Difficulty Preset", () => "Click \"Save\" on next page to return and load preset");

            //First Upgrade Settings
            menuConfig.AddSectionTitle(
                mod: ModManifest,
                text: () => "First Upgrade Settings"
                );

            menuConfig.AddTextOption(
                mod: ModManifest,
                name: () => "Sprinkler Type",
                getValue: () => Enum.GetName(Config.DifficultySettings[Config.SelectedDifficulty].FirstUpgrade.Sprinkler),
                setValue: value =>
                {
                    if (Config.SelectedDifficulty != Custom
                        && Config.DifficultySettings[Config.SelectedDifficulty].FirstUpgrade.Sprinkler != Enum.Parse<SprinklerType>(value))
                    {
                        CopyValuesToDefault(Config.SelectedDifficulty);
                        Config.SelectedDifficulty = Custom;
                    }
                    Config.DifficultySettings[Custom].FirstUpgrade.Sprinkler = Enum.Parse<SprinklerType>(value);
                },
                allowedValues: new string[] { "Basic", "Quality", "Iridium" }
                );

            menuConfig.AddNumberOption(
                mod: ModManifest,
                name: () => "Sprinkler Count",
                getValue: () => Config.DifficultySettings[Config.SelectedDifficulty].FirstUpgrade.SprinklerCount,
                setValue: value =>
                {
                    if (Config.SelectedDifficulty != Custom
                        && Config.DifficultySettings[Config.SelectedDifficulty].FirstUpgrade.SprinklerCount != value)
                    {
                        CopyValuesToDefault(Config.SelectedDifficulty);
                        Config.SelectedDifficulty = Custom;
                    }
                    Config.DifficultySettings[Custom].FirstUpgrade.SprinklerCount = value;
                },
                min: 0
                );

            menuConfig.AddNumberOption(
                mod: ModManifest,
                name: () => "Gold Cost",
                getValue: () => Config.DifficultySettings[Config.SelectedDifficulty].FirstUpgrade.Gold,
                setValue: value =>
                {
                    if (Config.SelectedDifficulty != Custom
                    && Config.DifficultySettings[Config.SelectedDifficulty].FirstUpgrade.Gold != value)
                    {
                        CopyValuesToDefault(Config.SelectedDifficulty);
                        Config.SelectedDifficulty = Custom;
                    }
                    Config.DifficultySettings[Custom].FirstUpgrade.Gold = value;
                },
                min: 0
                );

            menuConfig.AddNumberOption(
                mod: ModManifest,
                name: () => "Number of Batteries",
                getValue: () => Config.DifficultySettings[Config.SelectedDifficulty].FirstUpgrade.Batteries,
                setValue: value =>
                {
                    if (Config.SelectedDifficulty != Custom
                    && Config.DifficultySettings[Config.SelectedDifficulty].FirstUpgrade.Batteries != value)
                    {
                        CopyValuesToDefault(Config.SelectedDifficulty);
                        Config.SelectedDifficulty = Custom;
                    }
                    Config.DifficultySettings[Custom].FirstUpgrade.Batteries = value;
                },
                min: 0
                );

            menuConfig.AddNumberOption(
                mod: ModManifest,
                name: () => "Hearts with the Wizard",
                getValue: () => Config.DifficultySettings[Config.SelectedDifficulty].FirstUpgrade.Hearts,
                setValue: value =>
                {
                    if (Config.SelectedDifficulty != Custom
                    && Config.DifficultySettings[Config.SelectedDifficulty].FirstUpgrade.Hearts != value)
                    {
                        CopyValuesToDefault(Config.SelectedDifficulty);
                        Config.SelectedDifficulty = Custom;
                    }
                    Config.DifficultySettings[Custom].FirstUpgrade.Hearts = value;
                },
                min: 0,
                max: 10
                );

            // Second Upgrade Costs
            menuConfig.AddSectionTitle(
                mod: ModManifest,
                text: () => "Second Upgrade Settings"
                );

            menuConfig.AddTextOption(
                mod: ModManifest,
                name: () => "Sprinkler Type",
                getValue: () => Enum.GetName(Config.DifficultySettings[Config.SelectedDifficulty].SecondUpgrade.Sprinkler),
                setValue: value =>
                {
                    if (Config.SelectedDifficulty != Custom
                        && Config.DifficultySettings[Config.SelectedDifficulty].SecondUpgrade.Sprinkler != Enum.Parse<SprinklerType>(value))
                    {
                        CopyValuesToDefault(Config.SelectedDifficulty);
                        Config.SelectedDifficulty = Custom;
                    }
                    Config.DifficultySettings[Custom].SecondUpgrade.Sprinkler = Enum.Parse<SprinklerType>(value);
                },
                allowedValues: new string[] { "Basic", "Quality", "Iridium" }
                );

            menuConfig.AddNumberOption(
                mod: ModManifest,
                name: () => "Sprinkler Count",
                getValue: () => Config.DifficultySettings[Config.SelectedDifficulty].SecondUpgrade.SprinklerCount,
                setValue: value =>
                {
                    if (Config.SelectedDifficulty != Custom
                        && Config.DifficultySettings[Config.SelectedDifficulty].SecondUpgrade.SprinklerCount != value)
                    {
                        CopyValuesToDefault(Config.SelectedDifficulty);
                        Config.SelectedDifficulty = Custom;
                    }
                    Config.DifficultySettings[Custom].SecondUpgrade.SprinklerCount = value;
                },
                min: 0
                );

            menuConfig.AddNumberOption(
                mod: ModManifest,
                name: () => "Gold Cost",
                getValue: () => Config.DifficultySettings[Config.SelectedDifficulty].SecondUpgrade.Gold,
                setValue: value =>
                {
                    if (Config.SelectedDifficulty != Custom
                    && Config.DifficultySettings[Config.SelectedDifficulty].SecondUpgrade.Gold != value)
                    {
                        CopyValuesToDefault(Config.SelectedDifficulty);
                        Config.SelectedDifficulty = Custom;
                    }
                    Config.DifficultySettings[Custom].SecondUpgrade.Gold = value;
                },
                min: 0
                );

            menuConfig.AddNumberOption(
                mod: ModManifest,
                name: () => "Number of Batteries",
                getValue: () => Config.DifficultySettings[Config.SelectedDifficulty].SecondUpgrade.Batteries,
                setValue: value =>
                {
                    if (Config.SelectedDifficulty != Custom
                    && Config.DifficultySettings[Config.SelectedDifficulty].SecondUpgrade.Batteries != value)
                    {
                        CopyValuesToDefault(Config.SelectedDifficulty);
                        Config.SelectedDifficulty = Custom;
                    }
                    Config.DifficultySettings[Custom].SecondUpgrade.Batteries = value;
                },
                min: 0
                );

            menuConfig.AddNumberOption(
                mod: ModManifest,
                name: () => "Hearts with the Wizard",
                getValue: () => Config.DifficultySettings[Config.SelectedDifficulty].SecondUpgrade.Hearts,
                setValue: value =>
                {
                    if (Config.SelectedDifficulty != Custom
                    && Config.DifficultySettings[Config.SelectedDifficulty].SecondUpgrade.Hearts != value)
                    {
                        CopyValuesToDefault(Config.SelectedDifficulty);
                        Config.SelectedDifficulty = Custom;
                    }
                    Config.DifficultySettings[Custom].SecondUpgrade.Hearts = value;
                },
                min: 0,
                max: 10
                );

            //Final Upgrade Section
            menuConfig.AddSectionTitle(
                mod: ModManifest,
                text: () => "Final Upgrade Settings"
                );

            menuConfig.AddTextOption(
                mod: ModManifest,
                name: () => "Sprinkler Type",
                getValue: () => Enum.GetName(Config.DifficultySettings[Config.SelectedDifficulty].FinalUpgrade.Sprinkler),
                setValue: value =>
                {
                    if (Config.SelectedDifficulty != Custom
                        && Config.DifficultySettings[Config.SelectedDifficulty].FinalUpgrade.Sprinkler != Enum.Parse<SprinklerType>(value))
                    {
                        CopyValuesToDefault(Config.SelectedDifficulty);
                        Config.SelectedDifficulty = Custom;
                    }
                    Config.DifficultySettings[Custom].FinalUpgrade.Sprinkler = Enum.Parse<SprinklerType>(value);
                },
                allowedValues: new string[] { "Basic", "Quality", "Iridium" }
                );

            menuConfig.AddNumberOption(
                mod: ModManifest,
                name: () => "Sprinkler Count",
                getValue: () => Config.DifficultySettings[Config.SelectedDifficulty].FinalUpgrade.SprinklerCount,
                setValue: value =>
                {
                    if (Config.SelectedDifficulty != Custom
                        && Config.DifficultySettings[Config.SelectedDifficulty].FinalUpgrade.SprinklerCount != value)
                    {
                        CopyValuesToDefault(Config.SelectedDifficulty);
                        Config.SelectedDifficulty = Custom;
                    }
                    Config.DifficultySettings[Custom].FinalUpgrade.SprinklerCount = value;
                },
                min: 0
                );

            menuConfig.AddNumberOption(
                mod: ModManifest,
                name: () => "Gold Cost",
                getValue: () => Config.DifficultySettings[Config.SelectedDifficulty].FinalUpgrade.Gold,
                setValue: value =>
                {
                    if (Config.SelectedDifficulty != Custom
                    && Config.DifficultySettings[Config.SelectedDifficulty].FinalUpgrade.Gold != value)
                    {
                        CopyValuesToDefault(Config.SelectedDifficulty);
                        Config.SelectedDifficulty = Custom;
                    }
                    Config.DifficultySettings[Custom].FinalUpgrade.Gold = value;
                },
                min: 0
                );

            menuConfig.AddNumberOption(
                mod: ModManifest,
                name: () => "Number of Batteries",
                getValue: () => Config.DifficultySettings[Config.SelectedDifficulty].FinalUpgrade.Batteries,
                setValue: value =>
                {
                    if (Config.SelectedDifficulty != Custom
                    && Config.DifficultySettings[Config.SelectedDifficulty].FinalUpgrade.Batteries != value)
                    {
                        CopyValuesToDefault(Config.SelectedDifficulty);
                        Config.SelectedDifficulty = Custom;
                    }
                    Config.DifficultySettings[Custom].FinalUpgrade.Batteries = value;
                },
                min: 0
                );

            menuConfig.AddNumberOption(
                mod: ModManifest,
                name: () => "Hearts with the Wizard",
                getValue: () => Config.DifficultySettings[Config.SelectedDifficulty].FinalUpgrade.Hearts,
                setValue: value =>
                {
                    if (Config.SelectedDifficulty != Custom
                    && Config.DifficultySettings[Config.SelectedDifficulty].FinalUpgrade.Hearts != value)
                    {
                        CopyValuesToDefault(Config.SelectedDifficulty);
                        Config.SelectedDifficulty = Custom;
                    }
                    Config.DifficultySettings[Custom].FinalUpgrade.Hearts = value;
                },
                min: 0,
                max: 10
                );



            //Difficulty Selector Page
            menuConfig.AddPage(ModManifest, "DifficultySelectPage", () => "Difficulty Selection");

            menuConfig.AddTextOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("menu.diff"),
                tooltip: () => "The selected diffulty. NOTE: if you change a value on the main page and hit save, this will automatically change to 'Custom' in order to keep the defaults always the same.",
                getValue: () => Enum.GetName(Config.SelectedDifficulty),
                setValue: value =>
                {
                    if (menuConfig.TryGetCurrentMenu(out var modInfo, out string page))
                    {
                        if (modInfo.UniqueID == ModManifest.UniqueID
                            && page == "DifficultySelectPage")
                        {
                            Config.SelectedDifficulty = Enum.Parse<Difficulty>(value);
                        }
                    }
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
                },
                fieldId: "difficultySelectorField"
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
