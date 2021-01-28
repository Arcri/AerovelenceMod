using AerovelenceMod.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Thrown
{
    public class StormEdge : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Storm Edge");
		}
        public override void SetDefaults()
        {	
			item.crit = 4;
            item.damage = 52;
            item.melee = true;
            item.width = 38;
            item.height = 38;
            item.useTime = 24;
            item.useAnimation = 24;
			item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
			item.noUseGraphic = true;
            item.knockBack = 3;
            item.value = Item.sellPrice(0, 2, 45, 0);
            item.rare = ItemRarityID.Yellow;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("StormEdgeProjectile");
            item.UseSound = SoundID.Item1;
            item.shootSpeed = 13f;
        }


        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BurnshockBar>(), 6);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

namespace AerovelenceMod.Items.Weapons.Thrown
{
	public class StormEdgeProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Storm Edge");
		}

		int i;

		public override void SetDefaults()
		{
			projectile.width = 38;
			projectile.height = 38;
			projectile.aiStyle = 3;
			projectile.friendly = true;
			projectile.ranged = true;
			projectile.magic = false;
			projectile.penetrate = 3;
			projectile.timeLeft = 600;
			projectile.extraUpdates = 1;
		}

		public override void AI()
		{
			i++;
			if (i % 2 == 0)
			{
				int dust = Dust.NewDust(projectile.position, 36, 30, 15, -0.2631578f, -2.631579f, 0, new Color(255, 255, 255));
			}
			projectile.rotation += 0.1f;
			Vector2 origin = new Vector2(projectile.Center.X, projectile.Center.Y);
			float radius = 48;
			int numLocations = 30;
			if (i % 2 == 0)
			{
				Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
				Dust dust = Dust.NewDustPerfect(position, 36);
				dust.velocity.X = dust.velocity.X + Main.rand.Next(-50, 51) * 0.01f;
				dust.velocity.Y = dust.velocity.Y + Main.rand.Next(-50, 51) * 0.01f;
				dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
			}
		}

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			Texture2D texture = mod.GetTexture("Items/Weapons/Thrown/StormEdgeProjectile_GlowMask");
			spriteBatch.Draw(
				texture,
				new Vector2
				(
					projectile.Center.Y - Main.screenPosition.X,
					projectile.Center.X - Main.screenPosition.Y
				),
				new Rectangle(0, 0, texture.Width, texture.Height),
				Color.White,
				projectile.rotation,
				texture.Size(),
				projectile.scale,
				SpriteEffects.None,
				0f
			);
        }
    }
}