using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

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
            Item.UseSound = SoundID.Item5;
            Item.crit = 4;
            Item.damage = 40;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 30;
            Item.height = 54;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
			Item.shoot = AmmoID.Arrow;
            Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 1.5f;
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
			CreateRecipe(1)
				.AddIngredient(ModContent.ItemType<Placeables.Blocks.SlateOre>(), 40)
				.AddRecipeGroup("Wood", 10)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
	public class ForestsFrenzyProj : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 22;
			Projectile.height = 22;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.extraUpdates = 2;
		}
		public override void AI()
		{
			Projectile.velocity.Y += 0.03f;
			Projectile.rotation = Projectile.velocity.ToRotation();
			int dust = Dust.NewDust(Projectile.Center - new Vector2(5), 0, 0, ModContent.DustType<Leaves>());
			Main.dust[dust].velocity *= 1f;
		}
		public override void Kill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item10);
			for (int i = 0; i < 20; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(5), 0, 0, ModContent.DustType<Wood>(), 0, 0, Projectile.alpha);
				dust.velocity *= 0.55f;
				dust.velocity += Projectile.velocity * 0.5f;
				dust.scale *= 1.75f;
				dust.noGravity = true;
			}
		}
	}
}