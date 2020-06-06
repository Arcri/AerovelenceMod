using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Ores.PreHM.Adobe
{
    public class AdobeAxe : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Adobe Axe");
		}
        public override void SetDefaults()
        {
			item.crit = 2;
            item.damage = 11;
            item.melee = true;
            item.width = 38;
            item.height = 32;
            item.useTime = 24;
            item.useAnimation = 24;
			item.axe = 13;
			item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 0, 30, 0);
            item.rare = ItemRarityID.Blue;
            item.autoReuse = true;
        }
		public override void AddRecipes()  //How to craft this gun
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AdobeBrick>(), 15);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}