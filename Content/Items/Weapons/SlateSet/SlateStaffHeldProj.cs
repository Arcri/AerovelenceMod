 using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using AerovelenceMod.Common.Utilities;


namespace AerovelenceMod.Content.Items.Weapons.SlateSet
{
    public class SlateStaffHeldProj : ModProjectile
    {

		private int rockTimer = 0;
		private int timer;
		public float lerpage = 0.32f;
		private Vector2 mousePos;

		bool hasSpawnedRock = false;

		int rockCounter = 0;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Slate Staff");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

		}

		public override void SetDefaults()
		{
			Projectile.DamageType = DamageClass.Magic;
			Projectile.width = 42;
			Projectile.height = 42;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;

		}

        public override bool? CanDamage()
        {
			return false; //FALSE
        }

        public override void AI()
        {
			
			Vector2 storedCenter = Projectile.Center;

			Player player = Main.player[Projectile.owner];
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);

			if (player.dead || !player.active || !player.channel || player.frozen)
				Projectile.Kill();  

			Vector2 center = player.MountedCenter + (Projectile.direction == 1 ? new Vector2(-10,0) : new Vector2(5,0)); //-10, 5

			Projectile.Center = center;
			Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.direction == 1 ? MathHelper.ToRadians(45) : MathHelper.ToRadians(-45));
			float extrarotate = ((Projectile.direction * player.gravDir) < 0) ? MathHelper.Pi : 0;
			float itemrotate = Projectile.direction < 0 ? MathHelper.Pi : 0;
			player.itemRotation = Projectile.velocity.ToRotation() + itemrotate;
			player.itemRotation = MathHelper.WrapAngle(player.itemRotation);
			player.ChangeDir(Projectile.direction);
			player.heldProj = Projectile.whoAmI;
			player.itemTime = 10;
			player.itemAnimation = 10;
			Vector2 HoldOffset = new Vector2(15, 0).RotatedBy(MathHelper.WrapAngle(Projectile.velocity.ToRotation()));

			Projectile.Center += HoldOffset;
			Projectile.spriteDirection = Projectile.direction * (int)player.gravDir;
			Projectile.rotation -= extrarotate;

			Projectile.velocity = Vector2.Lerp(Vector2.Normalize(Projectile.velocity), Vector2.Normalize(mousePos - player.MountedCenter), lerpage); //slowly move towards direction of cursor
			Projectile.velocity.Normalize();



			if (Projectile.owner == Main.myPlayer)
			{
				mousePos = Main.MouseWorld;
			}
			else
			{
				
				Projectile.Center += Projectile.velocity * 20;
				return;
			}


			if (rockTimer < 85 && rockCounter < 1)
            {
				if (rockTimer % 3 == 0)
                {
					//Dust dust1 = Dust.NewDustDirect(player.Center + new Vector2(125, 0).RotatedBy((mousePos - player.Center).ToRotation() + Math.PI), 1, 1, ModContent.DustType<GlowCircleDust>(), 0, 0, 0, Color.DeepPink, 0.25f);
					//dust1.shader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/TestShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic").UseColor(Color.DeepPink * 1.5f);
					//dust1.velocity *= 0.5f;
				}
			}
			else if (rockTimer == 85 && rockCounter < 1)
            {
				for (int i = 0; i < 10; i++)
				{
					//Dust mydust = Dust.NewDustDirect(player.Center + new Vector2(125, 0).RotatedBy((mousePos - player.Center).ToRotation() + Math.PI), 4, 4, ModContent.DustType<GlowCircleDust>(), 
						//new Vector2(Main.rand.NextFloat(8, 13), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(0, 360))).X, 
						//new Vector2(Main.rand.NextFloat(8, 13), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(0, 360))).Y, 0, Color.DeepPink, 0.4f);
					//mydust.velocity *= 0.3f;
					//mydust.shader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/TestShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic").UseColor(Color.DeepPink * 1.5f);
				}

				SoundEngine.PlaySound(SoundID.Item74, Projectile.Center);

				float adjustment = Main.rand.NextBool() ? 1 : -1;
				int rock = Projectile.NewProjectile(null, player.Center + new Vector2(125, 0).RotatedBy((mousePos - player.Center).ToRotation() + Math.PI), new Vector2(1f, 0).RotatedBy((mousePos - player.Center).ToRotation() + Math.PI + MathHelper.ToRadians(adjustment)), ModContent.ProjectileType<DeerclopsRock>(), 20, 0, player.whoAmI);
				Projectile rocky = Main.projectile[rock];

				if (rocky.ModProjectile is DeerclopsRock stone)
                {
					stone.setHome(rockCounter);
                }

				rockTimer = 0;
				rockCounter++;
			
			}

			

			//Dust p = Dust.NewDustPerfect(Projectile.Center , ModContent.DustType<ShaderDustTest>(), Velocity: Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-10,10))) * 2, Scale: 0.1f);
			//Dust dust1 = Dust.NewDustDirect(Projectile.Center + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * 20), 1, 1, ModContent.DustType<GlowCircleDust>(), 0, 0, 0, Color.DeepPink, 0.2f);
			//dust1.shader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/TestShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic").UseColor(Color.Pink * 1f);
			//dust1.velocity *= 0.1f;

			if (player.channel)
			{
				Projectile.timeLeft++;
				Projectile.Center += Projectile.velocity * 20;

			}

			if (timer % 45 == 0 || timer % 50 == 0)
			{
				for (int i = 0; i < 2 + Main.rand.NextFloat(-1, 2); i++)
                {
					//Dust dust1 = Dust.NewDustDirect(player.Center, 1, 1, ModContent.DustType<GlowCircleDust>(), 0, 0, 0, Color.DeepPink, 0.2f);
					//dust1.shader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/TestShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic").UseColor(Color.DeepPink * 1.75f);
					//dust1.velocity *= 1.25f; //1.25f
					//dust1.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-45, 46))) * 4f;
				}

			}

			rockTimer++;
			timer++;
		}

		public Vector2 endPoint;
		public float LaserRotation;
		public override bool PreDraw(ref Color lightColor)
		{
			if (rockCounter < 1)
            {
				Player player = Main.player[Projectile.owner];
				Texture2D Tex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/SlateSet/DeerclopsRockPink").Value;

				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
				//Main.spriteBatch.Draw(Tex, player.Center + new Vector2(125, 0).RotatedBy((mousePos - player.Center).ToRotation() + Math.PI) - Main.screenPosition + new Vector2(5, 5), Tex.Frame(1, 1, 0, 0), Color.Pink, Projectile.rotation, Tex.Size() / 2, 0.1f, SpriteEffects.None, 0f);
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
				
				if (timer % 5 == 0)
                {
					//Dust dusty = Dust.NewDustPerfect(player.Center + new Vector2(125, 0).RotatedBy((mousePos - player.Center).ToRotation() + Math.PI), ModContent.DustType<GlowCircleDust>(), Scale: 0.1f);
					//dusty.shader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/TestShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic").UseColor(Color.DeepPink * 1.75f);
					//dusty.velocity *= 0.1f;
				}

			}

			/*
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

			Player Owner = Main.player[Projectile.owner];
			//LaserRotation = (Main.MouseWorld - Owner.Center).ToRotation();
			
			endPoint = Projectile.rotation.ToRotationVector2() * 200f;
			var texBeam = Mod.Assets.Request<Texture2D>("Items/Weapons/SlateSet/GlowTrail").Value;

			Vector2 origin = new Vector2(0, texBeam.Height / 2);

			float height = 15f + ((float)Math.Sin(timer * 0.08f) * 4f);
			int width = (int)(Projectile.Center - endPoint).Length() - 24;

			var pos = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 24;
			var target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1.2f));

			Main.spriteBatch.Draw(texBeam, target, null, Color.Red * 2f, LaserRotation, origin, 0, 0);

			for (int i = 0; i < width; i += 10)
				Lighting.AddLight(pos + Vector2.UnitX.RotatedBy(LaserRotation) * i + Main.screenPosition, Color.Red.ToVector3() * height * 0.030f);

			var spotTex = Mod.Assets.Request<Texture2D>("NPCs/Boss/SkeletronPrime/GlowEye").Value;
			Main.spriteBatch.Draw(spotTex, pos, spotTex.Frame(1, 1, 0, 0), Color.Red * 2, Projectile.rotation, spotTex.Size() / 2, 0.55f, SpriteEffects.None, 0);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			*/
			return true;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			/*
			Player player = Main.player[Projectile.owner];
			Vector2 unit = LaserRotation.ToRotationVector2();
			float point = 0f;
			// Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
			// It will look for collisions on the given line using AABB
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.Center,
				player.Center + unit * 1000, 22, ref point);
			*/
			return false;
		}
	}
}
