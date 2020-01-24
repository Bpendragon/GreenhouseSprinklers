using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using System;

namespace Bpendragon.GreenhouseSprinklers
{
    partial class ModEntry
    {
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            throw new NotImplementedException();
            if (!(e.NewMenu is ShopMenu)) return;

            var shop = (ShopMenu)e.NewMenu;

            if (shop.portraitPerson == null || !(shop.portraitPerson.Name == "Clint"))
                return;

            var itemStock = shop.itemPriceAndStock;
            var obj = new StardewValley.Object(Vector2.Zero, 0);
            itemStock.Add(obj, new int[] { });
        }  
    }
}
