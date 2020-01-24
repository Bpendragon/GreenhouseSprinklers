namespace Bpendragon.GreenhouseSprinklers.Data
{
    class ModData
    {
        public bool FirstUpgrade { get; set; } = false;
        public bool SecondUpgrade { get; set; } = false;
        public bool IsBuilding { get; set; } = false;  //Future update maybe
        public int? DaysBuilding { get; set; } = null; 
    }
}
