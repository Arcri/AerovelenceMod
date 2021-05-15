using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class ForestsFrenzy : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("ForestsFrenzy");
            Tooltip.SetDefault("Converts wooden arrows into reinforced slate arrows that travel slow but deal a lot of damage");
        }
        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item5;
            item.crit = 4;
            item.damage = 40;
            item.ranged = true;
            item.width = 30;
            item.height = 54;
            item.useTime = 40;
            item.useAnimation = 40;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Green;
            item.autoReuse = true;
			item.shoot = AmmoID.Arrow;
            item.useAmmo = AmmoID.Arrow;
            item.shootSpeed = 1.5f;
        }
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			if (type == ProjectileID.WoodenArrowFriendly)
			{
				type = ModContent.ProjectileType<ForestsFrenzyProj>();
			}
			return true;
		}
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<Placeables.Blocks.SlateOre>(), 40);
			recipe.AddRecipeGroup("Wood", 10);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
	public class ForestsFrenzyProj : ModProjectile
	{
		public override void SetDefaults()
		{
			projectile.width = 22;
			projectile.height = 22;
			projectile.friendly = true;
			projectile.tileCollide = true;
			projectile.ignoreWater = true;
			projectile.ranged = true;
			projectile.extraUpdates = 2;
		}
		public override void AI()
		{
			projectile.velocity.Y += 0.03f;
			projectile.rotation = projectile.velocity.ToRotation();
			int dust = Dust.NewDust(projectile.Center - new Vector2(5), 0, 0, ModContent.DustType<Leaves>());
			Main.dust[dust].velocity *= 1f;
		}
		public override void Kill(int timeLeft)
		{
			Main.PlaySound(SoundID.Item10);
			for (int i = 0; i < 20; i++)
			{
				Dust dust = Dust.NewDustDirect(projectile.Center - new Vector2(5), 0, 0, ModContent.DustType<Wood>(), 0, 0, projectile.alpha);
				dust.velocity *= 0.55f;
				dust.velocity += projectile.velocity * 0.5f;
				dust.scale *= 1.75f;
				dust.noGravity = true;
			}
		}
	}
}