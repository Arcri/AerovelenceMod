using AerovelenceMod.Content.Items.Placeables.Blocks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class PumpkinPiercer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pumpkin Piercer");
            Tooltip.SetDefault("'Don't get tricked!'");
        }
        public override void SetDefaults()
        {
            item.crit = 4;
            item.damage = 40;
            item.melee = true;
            item.width = 40;
            item.height = 40;
            item.useTime = 24;
            item.useAnimation = 24;
            item.useTurn = true;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 0, 20, 0);
            item.rare = ItemRarityID.Blue;
            item.autoReuse = false;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Pumpkin, 15);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}