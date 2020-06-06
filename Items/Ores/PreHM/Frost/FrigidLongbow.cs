using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Ores.PreHM.Frost
{
    public class FrigidLongbow : ModItem
    {
				public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Frigid Longbow");
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item5;
			item.crit = 20;
            item.damage = 18;
            item.ranged = true;
            item.width = 60;
            item.height = 32;
            item.useTime = 12;
			item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true; //so the item's animation doesn't do damage
            item.knockBack = 6;
            item.value = 10000;
            item.rare = ItemRarityID.Green;
            item.autoReuse = true;
            item.shoot = AmmoID.Arrow;
			item.useAmmo = AmmoID.Arrow;
            item.shootSpeed = 8f;
        }
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ProjectileID.FrostburnArrow, damage, knockBack, player.whoAmI);
			return false;
		}
		public override void AddRecipes()  //How to craft this item
        {
            ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<FrostBar>(), 12);
            recipe.AddTile(TileID.Anvils);   //at work bench
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}