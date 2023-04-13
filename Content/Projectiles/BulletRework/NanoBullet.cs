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
using System.Collections.Generic;
using AerovelenceMod.Content.Projectiles;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns;

namespace AerovelenceMod.Content.Projectiles.BulletRework
{
    public class NanoBullet : TrailProjBase
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        float timer = 0;
		public Color color = Color.White;
		public float overallSize = 1f;
		public int lineWidth = 3;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Bullet Test");
		}

        public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 400;
			Projectile.tileCollide = true;
			Projectile.scale = 1f;
			Projectile.extraUpdates = 2;

		}

		public float xScale = 1f;
		public float yScale = 1f;

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/spark_07_Black").Value;
            trailColor = Color.DodgerBlue; //191 255 255
            trailTime = timer * 0.02f;

            trailPointLimit = 120;
            trailWidth = 20;
            trailMaxLength = 100; //200

            trailRot = Projectile.velocity.ToRotation();
            trailPos = Projectile.Center;

            TrailLogic();

            Lighting.AddLight(Projectile.Center, Color.SkyBlue.ToVector3() * 0.3f);
			timer++;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            return true;

            // If the projectile hits the left or right side of the tile, reverse the X velocity
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }

            // If the projectile hits the top or bottom side of the tile, reverse the Y velocity
            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }


            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundStyle style = new SoundStyle("Terraria/Sounds/Item_40") with { Pitch = -.71f, PitchVariance = .28f, MaxInstances = 1, Volume = 0.5f };
            SoundEngine.PlaySound(style, Projectile.Center);

            for (int h = 0; h < 3; h++)
            {
                //Obv ineficietentntet
                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixel>(),
                    Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.Pi + Main.rand.NextFloat(-2, 2)) * Main.rand.Next(1, 3), 
                    newColor: Color.DodgerBlue, Alpha: 69);

                //int dust = Dust.NewDust(Projectile.Center, 30, 30, ModContent.DustType<GlowPixel>(), Scale: 1f, newColor: Color.DodgerBlue);
                //Main.dust[dust].velocity *= 3f;
                //Main.dust[dust].position += Main.dust[dust].velocity * 3;

            }

            /*
            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            for (int i = 0; i < 3; i++)
            {
                Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleDust>(),
                    Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.Pi + Main.rand.NextFloat(-1, 1)) * Main.rand.Next(1, 3),
                    new Color(255, 111, 20), Main.rand.NextFloat(0.2f, 0.4f), 0.6f, 0f, dustShader);
                p.alpha = 0;
                //p.rotation = Main.rand.NextFloat(6.28f);
            }
            */
        }

        public float widthIntensity = 0;
		//public List<Projectile> InkProj = new List<Projectile>();
		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D Tex = Mod.Assets.Request<Texture2D>("Assets/TrailImages/Starlight").Value;
			Vector2 scale = new Vector2(Projectile.scale * 2, Projectile.scale) * 0.5f;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);


            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * -10), Tex.Frame(1 ,1, 0, 0), Color.DeepSkyBlue, Projectile.rotation + MathHelper.PiOver2, Tex.Size() / 2, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * -10), Tex.Frame(1, 1, 0, 0), Color.White, Projectile.rotation + MathHelper.PiOver2, Tex.Size() / 2, scale * 0.5f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            TrailDrawing();

            return false;
		}

        public override float WidthFunction(float progress)
        {
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.4f, progress, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(0f, 30f, num) * 0.5f;
        }
    }

    public class NanoBulletReplacer : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return item.type == ItemID.NanoBullet;
        }

        public override void SetDefaults(Item item)
        {
            item.StatsModifiedBy.Add(Mod);
            item.shoot = ModContent.ProjectileType<NanoBullet>();
        }
    }

    public class NanoReticle : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
        }
        public override bool? CanDamage()
        {
            return false;
        }

        int timer = 0;
        public override void AI()
        {
            Projectile.scale = 0.8f;
            
            Projectile.rotation += 0.01f;

            distanceScale = (float)Math.Abs((float)Math.Sin(timer / 20f)) * 3f;

            timer++;
        }

        float distanceScale = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Core = Mod.Assets.Request<Texture2D>("Content/Projectiles/BulletRework/NanoCore").Value;
            Texture2D Outer = Mod.Assets.Request<Texture2D>("Content/Projectiles/BulletRework/NanoBorder").Value;
            Texture2D Ball = Mod.Assets.Request<Texture2D>("Assets/TrailImages/Twinkle").Value;



            Color col1 = Color.White;
            Color col2 = Color.DeepSkyBlue;
            col2.A = 0;
            Color col3 = Color.White;
            col3.A = 0;


            //Could make this better by just rotation one vector, but i want to guarentee this works
            float baseOffset = 15 * Projectile.scale;

            //Top Left
            Vector2 drawPosTL = new Vector2(-baseOffset + (distanceScale * -1), -baseOffset + (distanceScale * -1)).RotatedBy(Projectile.rotation);
            //Main.spriteBatch.Draw(OuterWhite, drawPosTL - Main.screenPosition + Projectile.Center, null, col1, Projectile.rotation, Outer.Size() / 2, Projectile.scale * 1.3f, SpriteEffects.None, 0f);

            //Top Right
            Vector2 drawPosTR = new Vector2(baseOffset + (distanceScale * 1), -baseOffset + (distanceScale * -1)).RotatedBy(Projectile.rotation);
            //Main.spriteBatch.Draw(OuterWhite, drawPosTR - Main.screenPosition + Projectile.Center, null, col1, Projectile.rotation + MathHelper.PiOver2, Outer.Size() / 2, Projectile.scale * 1.3f, SpriteEffects.None, 0f);

            //Bottom Left
            Vector2 drawPosBL = new Vector2(-baseOffset + (distanceScale * -1), baseOffset + (distanceScale * 1)).RotatedBy(Projectile.rotation);
            //Main.spriteBatch.Draw(OuterWhite, drawPosBL - Main.screenPosition + Projectile.Center, null, col1, Projectile.rotation - MathHelper.PiOver2, Outer.Size() / 2, Projectile.scale * 1.3f, SpriteEffects.None, 0f);

            //Bottom Right
            Vector2 drawPosBRL = new Vector2(baseOffset + (distanceScale * 1), baseOffset + (distanceScale * 1)).RotatedBy(Projectile.rotation);
            //Main.spriteBatch.Draw(OuterWhite, drawPosBRL - Main.screenPosition + Projectile.Center, null, col1, Projectile.rotation + MathHelper.Pi, Outer.Size() / 2, Projectile.scale * 1.3f, SpriteEffects.None, 0f);

            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);


            Main.spriteBatch.Draw(Outer, drawPosTL - Main.screenPosition + Projectile.Center, null, col1, Projectile.rotation, Outer.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Outer, drawPosTR - Main.screenPosition + Projectile.Center, null, col1, Projectile.rotation + MathHelper.PiOver2, Outer.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Outer, drawPosBL - Main.screenPosition + Projectile.Center, null, col1, Projectile.rotation - MathHelper.PiOver2, Outer.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Outer, drawPosBRL - Main.screenPosition + Projectile.Center, null, col1, Projectile.rotation + MathHelper.Pi, Outer.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, col2, Projectile.rotation, Ball.Size() / 2, Projectile.scale * 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, col2 * 0.5f, Projectile.rotation, Ball.Size() / 2, Projectile.scale * 0.75f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, col3 * 0.8f, Projectile.rotation, Ball.Size() / 2, Projectile.scale * 0.5f, SpriteEffects.None, 0f);



            return false;
        }
    }
}
