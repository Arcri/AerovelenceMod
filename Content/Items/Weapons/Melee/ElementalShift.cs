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
			Item.UseSound = SoundID.Item1;
			Item.crit = 8;
            Item.damage = 24;
            Item.DamageType = DamageClass.Melee;
            Item.width = 54;
            Item.height = 54; 
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
			Item.value = Item.sellPrice(0, 0, 40, 20);
            Item.value = 10000;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Orange;
            Item.shoot = Item.shoot = ModContent.ProjectileType<ElementScythe>();
            Item.shootSpeed = 3;
            Item.autoReuse = false;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<PhanticBar>(), 15)
                .AddIngredient(ItemID.HellstoneBar, 15)
                .AddIngredient(ModContent.ItemType<FrostShard>(), 5)
                .AddIngredient(ItemID.Fireblossom, 10)
                .AddIngredient(ItemID.Daybloom, 10)
                .AddIngredient(ItemID.Waterleaf, 10)
                .AddIngredient(ItemID.Moonglow, 10)
                .AddIngredient(ItemID.Shiverthorn, 10)
                .AddIngredient(ItemID.Deathweed, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}