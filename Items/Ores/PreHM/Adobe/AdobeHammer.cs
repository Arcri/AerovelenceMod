using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Ores.PreHM.Adobe
{
    public class AdobeHammer : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Endroic Hammer");
		}
        public override void SetDefaults()
        {
			item.crit = 4;
            item.damage = 8;
            item.melee = true;
            item.width = 34;
            item.height = 34;
            item.useTime = 25;
            item.useAnimation = 25;
			item.hammer = 60;
			item.UseSound = SoundID.Item1;
            item.useStyle = 1;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 0, 35, 0);
            item.rare = 1;
            item.autoReuse = true;
        }
		public override void AddRecipes()  //How to craft this gun
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AdobeBrick>(), 12);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}