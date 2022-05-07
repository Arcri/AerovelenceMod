using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
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
			Item.damage = 62;
			Item.crit = 8;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 12;
			Item.width = 36;
			Item.DamageType = DamageClass.Magic;
			Item.height = 38;
			Item.useTime = 17;
			Item.useAnimation = 25;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.noMelee = true;
			Item.knockBack = 5;
			Item.value = 10000;
			Item.rare = ItemRarityID.Lime;
			Item.UseSound = SoundID.Item20;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<DischargeBlade>();
			Item.shootSpeed = 0f;
			Item.staff[Item.type] = true;
		}

	}
	public class DischargeBlade : ModProjectile
	{
		private int projTimer;
		private int orbitTimer;
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.width = 34;
			Projectile.height = 38;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.penetrate = 5;
			Projectile.timeLeft = 400;
			Projectile.light = 0.5f;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.extraUpdates = 1;

		}
		public override void AI()
		{

			if (Main.rand.NextFloat() < 0.8289474f)
			{
				int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 164, 0f, 0f, 0, new Color(155, 0, 124), 1.118421f);
				Dust dust = Main.dust[dustIndex];
				dust.shader = GameShaders.Armor.GetSecondaryShader(0, Main.LocalPlayer);
				dust.noGravity = true;
				dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
			}

			Projectile.rotation += 0.15f;
			projTimer++;
			Player player = Main.player[Projectile.owner];
			if (projTimer <= 150)
			{
				orbitTimer += 2;
				Vector2 orbitPos = player.Center + new Vector2(70, 0).RotatedBy(MathHelper.ToRadians(orbitTimer));
				Projectile.velocity = (10 * Projectile.velocity + orbitPos - Projectile.Center) / 20f;
			}
			if (projTimer >= 150)
			{
				if (Projectile.localAI[0] == 0f)
				{
					Vector2 mousePos = Main.MouseWorld;
					Vector2 vector8 = new Vector2(Projectile.position.X + (Projectile.width * 0.5f), Projectile.position.Y + (Projectile.height * 0.5f));
					float Speed = 5f;
					float rotation = (float)Math.Atan2(vector8.Y - mousePos.Y, vector8.X - mousePos.X);
					Projectile.velocity = new Vector2((float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1));
					Projectile.localAI[0] = 1f;
				}
				Projectile.velocity *= 1.035f;
			}
		}

        public override bool PreDraw(ref Color lightColor)
        {
        Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Width() * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(Color.LightPink) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw((Texture2D)TextureAssets.Projectile[Projectile.type], drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			}
			return true;
		}

		public override void Kill(int timeLeft)
		{

			for (int i = 0; i < 10; i++)
			{
				int dustType = 164;
				int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 0, new Color(155, 0, 124));
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


