namespace Bpendragon.GreenhouseSprinklers.Data
{
    class ModData
    {
        public bool FirstUpgrade { get; set; } = false;
        public bool SecondUpgrade { get; set; } = false;
        public bool FinalUpgrade { get; set; } = false;
        public bool HasTalkedToRobin1 { get; set; } = false;
        public bool HasTalkedToRobin2 { get; set; } = false;
        public bool HasTalkedToRobin3 { get; set; } = false;
        public bool HasRecievedLetter1 { get; set; } = false;
        public bool HasRecievedLetter2 { get; set; } = false;
        public bool HasRecievedLetter3 { get; set; } = false;
        public bool IsUpgrading { get; set; } = false;


        public int GetLevel() => (FirstUpgrade ? 1 : 0) + (SecondUpgrade ? 1 : 0) + (FinalUpgrade ? 1 : 0);
    }
}
