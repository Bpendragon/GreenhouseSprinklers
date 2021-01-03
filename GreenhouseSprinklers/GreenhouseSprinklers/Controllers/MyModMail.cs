using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewModdingAPI;

namespace Bpendragon.GreenhouseSprinklers
{
    class MyModMail : IAssetEditor
    {
        public MyModMail()
        {

        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data\\mail");
        }

        public void Edit<T>(IAssetData asset)
        {
            var data = asset.AsDictionary<string, string>().Data;

            data["Bpendragon.GreenhouseSprinklers.Wizard1"] = "@,^^The Junimos are pleased with your contributions to The Valley.^They had an idea to improve your greenhouse, I have translated these ideas into something Robin can use.^Talk to her if you want this upgrade.^^   -M. Rasmodius, Wizard";
            data["Bpendragon.GreenhouseSprinklers.Wizard1b"] = "@,^^Despite having removed them from their home in the Community Center the Junimos are pleased with your contributions to The Valley.^They had an idea to improve your greenhouse, I have translated these ideas into something Robin can use.^Talk to her if you want this upgrade.^^   -M. Rasmodius, Wizard";
            data["Bpendragon.GreenhouseSprinklers.Wizard2"] = "@,^^The Junimos continue to be impressed with your farm.^They had an idea to further improve your greenhouse, I have translated these ideas into something Robin can use.^Talk to her if you want this upgrade.^^   -M. Rasmodius, Wizard";
            data["Bpendragon.GreenhouseSprinklers.Wizard3"] = "@,^^The Junimos continue to be impressed with your farm.^They had one final idea to upgrade your farm's sprinkler system, I have translated these ideas into something Robin can use.^Talk to her if you want this upgrade.^^   -M. Rasmodius, Wizard";
        }
    }
}
