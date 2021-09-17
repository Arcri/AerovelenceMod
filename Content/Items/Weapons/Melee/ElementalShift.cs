using AerovelenceMod.Content.Items.Others.Crafting;
using AerovelenceMod.Content.Projectiles.Weapons.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class ElementalShift : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Elemental Shift");
			Tooltip.SetDefault("Fires elemental scythes when swung");
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item1;
			item.crit = 8;
            item.damage = 24;
            item.melee = true;
            item.width = 54;
            item.height = 54; 
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 5;
			item.value = Item.sellPrice(0, 0, 40, 20);
            item.value = 10000;
            item.autoReuse = true;
            item.rare = ItemRarityID.Orange;
            item.shoot = item.shoot = ModContent.ProjectileType<ElementScythe>();
            item.shootSpeed = 3;
            item.autoReuse = false;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<PhanticBar>(), 15);
            recipe.AddIngredient(ItemID.HellstoneBar, 15);
            recipe.AddIngredient(ModContent.ItemType<FrostShard>(), 5);
            recipe.AddIngredient(ItemID.Fireblossom, 10);
            recipe.AddIngredient(ItemID.Daybloom, 10);
            recipe.AddIngredient(ItemID.Waterleaf, 10);
            recipe.AddIngredient(ItemID.Moonglow, 10);
            recipe.AddIngredient(ItemID.Shiverthorn, 10);
            recipe.AddIngredient(ItemID.Deathweed, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}