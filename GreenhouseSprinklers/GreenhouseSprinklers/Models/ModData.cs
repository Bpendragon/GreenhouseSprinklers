using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bpendragon.GreenhouseSprinklers.Data
{
    class ModData
    {
        public bool FirstUpgrade { get; set; } = false;
        public bool SecondUpgrade { get; set; } = false;
        public bool IsBuilding { get; set; } = false;
        public int? DaysBuilding { get; set; } = null; 
    }
}
