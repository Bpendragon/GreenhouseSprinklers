namespace Bpendragon.GreenhouseSprinklers.Data
{
    class ModData
    {
        public bool FirstUpgrade { get; set; } = false;
        public bool SecondUpgrade { get; set; } = false;
        public bool FinalUpgrade { get; set; } = false;
        public bool SaveHasBeenUpgraded { get; set; } = false;


        public int GetLevel() => (FirstUpgrade ? 1 : 0) + (SecondUpgrade ? 1 : 0) + (FinalUpgrade ? 1 : 0);
    }
}
