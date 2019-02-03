namespace Bpendragon.GreenhouseSprinklers.Data
{
    public enum SprinklerType
    {
        Basic,
        Quality,
        Iridium
    }

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    class ModConfig
    {
        public Difficulty Difficulty { get; set; } = Difficulty.Medium;

        public UpgradeCost Easy { get; set; } = new UpgradeCost(
            new SingleUpgradeCost(SprinklerType.Basic, 5, 10000, 0),
            new SingleUpgradeCost(SprinklerType.Quality, 5, 15000, 1)
        );

        public UpgradeCost Medium { get; set; } = new UpgradeCost(
           new SingleUpgradeCost(SprinklerType.Quality, 5, 20000, 1),
           new SingleUpgradeCost(SprinklerType.Iridium, 5, 30000, 5)
       );

        public UpgradeCost Hard { get; set; } = new UpgradeCost(
           new SingleUpgradeCost(SprinklerType.Iridium, 5, 30000, 5),
           new SingleUpgradeCost(SprinklerType.Iridium, 10, 35000, 20)
       );
    }

    class SingleUpgradeCost
    {
        public SprinklerType Sprinkler { get; set; }
        public int SprinklerCount { get; set; }
        public int Gold { get; set; }
        public int Batteries { get; set; }

        public SingleUpgradeCost(SprinklerType SprinklerType, int NumSprinklers, int GoldAmount, int NumBatteries)
        {
            this.Sprinkler = SprinklerType;
            this.SprinklerCount = NumSprinklers;
            this.Gold = GoldAmount;
            this.Batteries = NumBatteries;
        }
    }

    class UpgradeCost
    {
        public SingleUpgradeCost FirstUpgrade { get; set; }
        public SingleUpgradeCost SecondUpgrade { get; set; }

        public UpgradeCost (SingleUpgradeCost FirstUpgrade, SingleUpgradeCost SecondUpgrade)
        {
            this.FirstUpgrade = FirstUpgrade;
            this.SecondUpgrade = SecondUpgrade;
        }
    }
}
