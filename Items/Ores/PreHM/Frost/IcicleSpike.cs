using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Ores.PreHM.Frost
{
    public class IcicleSpike : ModItem
    {
				public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Icicle Spike");
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item1;
			item.crit = 20;
            item.damage = 26;
            item.ranged = true;
            item.width = 60;
            item.height = 32;
            item.useTime = 26;
			item.useAnimation = 26;
            item.useStyle = 1;
            item.knockBack = 6;
            item.value = 10000;
            item.rare = 2;
            item.autoReuse = true;
            item.shoot = 1;
            item.shootSpeed = 8f;
        }
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Projectile.NewProjectile(position.X, position.Y, speedX, speedY, 253, damage, knockBack, player.whoAmI);
			return false;
        }
		public override void AddRecipes()  //How to craft this gun
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<FrostBar>(), 15);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}