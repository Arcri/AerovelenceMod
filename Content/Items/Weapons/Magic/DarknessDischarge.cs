using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic

{ 
	public class DarknessDischarge : ModItem
	{
        public override void SetStaticDefaults()
        {
			DisplayName.SetDefault("Darkness Discharge");
			Tooltip.SetDefault("Casts discharge blades that fire towards your cursor");
        }
        public override void SetDefaults()
		{
			item.damage = 62;
			item.crit = 8;
			item.magic = true;
			item.mana = 12;
			item.width = 36;
			item.magic = true;
			item.height = 38;
			item.useTime = 17;
			item.useAnimation = 25;
			item.useStyle = ItemUseStyleID.HoldingUp;
			item.noMelee = true;
			item.knockBack = 5;
			item.value = 10000;
			item.rare = ItemRarityID.Lime;
			item.UseSound = SoundID.Item20;
			item.autoReuse = true;
			item.shoot = ModContent.ProjectileType<DischargeBlade>();
			item.shootSpeed = 0f;
			Item.staff[item.type] = true;
		}

	}
	public class DischargeBlade : ModProjectile
	{
		private int projTimer;
		private int orbitTimer;
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 7;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			projectile.width = 34;
			projectile.height = 38;
			projectile.aiStyle = -1;
			projectile.friendly = true;
			projectile.hostile = false;
			projectile.magic = true;
			projectile.penetrate = 5;
			projectile.timeLeft = 400;
			projectile.light = 0.5f;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.extraUpdates = 1;

		}
		public override void AI()
		{

			if (Main.rand.NextFloat() < 0.8289474f)
			{
				int dustIndex = Dust.NewDust(projectile.position, projectile.width, projectile.height, 164, 0f, 0f, 0, new Color(155, 0, 124), 1.118421f);
				Dust dust = Main.dust[dustIndex];
				dust.shader = GameShaders.Armor.GetSecondaryShader(0, Main.LocalPlayer);
				dust.noGravity = true;
				dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
			}

			projectile.rotation += 0.15f;
			projTimer++;
			Player player = Main.player[projectile.owner];
			if (projTimer <= 150)
			{
				orbitTimer += 2;
				Vector2 orbitPos = player.Center + new Vector2(70, 0).RotatedBy(MathHelper.ToRadians(orbitTimer));
				projectile.velocity = (10 * projectile.velocity + orbitPos - projectile.Center) / 20f;
			}
			if (projTimer >= 150)
			{
				if (projectile.localAI[0] == 0f)
				{
					Vector2 mousePos = Main.MouseWorld;
					Vector2 vector8 = new Vector2(projectile.position.X + (projectile.width * 0.5f), projectile.position.Y + (projectile.height * 0.5f));
					float Speed = 5f;
					float rotation = (float)Math.Atan2(vector8.Y - mousePos.Y, vector8.X - mousePos.X);
					projectile.velocity = new Vector2((float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1));
					projectile.localAI[0] = 1f;
				}
				projectile.velocity *= 1.035f;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
			for (int k = 0; k < projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
				Color color = projectile.GetAlpha(Color.LightPink) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
				spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
			}
			return true;
		}

		public override void Kill(int timeLeft)
		{

			for (int i = 0; i < 10; i++)
			{
				int dustType = 164;
				int dustIndex = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 0, new Color(155, 0, 124));
				Dust dust = Main.dust[dustIndex];
				dust.velocity.X = dust.velocity.X + Main.rand.Next(-50, 51) * 0.01f;
				dust.velocity.Y = dust.velocity.Y + Main.rand.Next(-50, 51) * 0.01f;
				dust.shader = GameShaders.Armor.GetSecondaryShader(39, Main.LocalPlayer);
				dust.noGravity = true;
				dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
			}
		}
	}
}


