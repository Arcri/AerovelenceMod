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
    public class CursedBullet : TrailProjBase
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

        public float storedRot = 0f;

        bool shouldFade = false;
        float fadeAmount = 1f;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value;
            trailColor = new Color(162, 230, 47) * fadeAmount;// Color.GreenYellow;
            trailTime = timer * 0.02f;

            trailPointLimit = 120;
            trailWidth = 30;
            trailMaxLength = 140; //200

            trailRot = Projectile.velocity.ToRotation();
            trailPos = Projectile.Center;

            TrailLogic();

            Lighting.AddLight(Projectile.position, Color.GreenYellow.ToVector3() * 0.45f);

            if (shouldFade)
            {
                fadeAmount = MathHelper.Lerp(fadeAmount, 0f, 0.04f);
            }
            else
                storedRot = Projectile.rotation;

            timer++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.tileCollide = false;
            storedRot = Projectile.velocity.ToRotation();
            Projectile.velocity = Vector2.Zero;
            Projectile.damage = 0;
            Projectile.timeLeft = 200;
            HitDust();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            HitDust();

            Projectile.tileCollide = false;
            storedRot = Projectile.velocity.ToRotation();
            Projectile.velocity = Vector2.Zero;
            Projectile.damage = 0;
            Projectile.timeLeft = 200;

            shouldFade = true;
            return false;
        }

        public override void Kill(int timeLeft)
        {
            //HitDust();
        }

        public void HitDust()
        {
            SoundStyle style = new SoundStyle("Terraria/Sounds/Item_40") with { Pitch = -.71f, PitchVariance = .28f, MaxInstances = 1, Volume = 0.5f };
            SoundEngine.PlaySound(style, Projectile.Center);

            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            for (int i = 0; i < 3; i++)
            {


                Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleRise>(),
                    Vector2.One.RotatedByRandom(6.28f) * Main.rand.Next(1, 2),
                    Color.LawnGreen, 0.25f, 0.7f, 0f, dustShader);
                p.alpha = 0;
                p.noLight = true;
            }
        }

        public float widthIntensity = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Tex = Mod.Assets.Request<Texture2D>("Assets/TrailImages/Starlight").Value;
            Vector2 scale = new Vector2(Projectile.scale * 2, Projectile.scale) * 0.5f;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * -10), Tex.Frame(1, 1, 0, 0), Color.GreenYellow * fadeAmount, Projectile.rotation + MathHelper.PiOver2, Tex.Size() / 2, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * -10), Tex.Frame(1, 1, 0, 0), Color.White * fadeAmount, Projectile.rotation + MathHelper.PiOver2, Tex.Size() / 2, scale * 0.5f, SpriteEffects.None, 0f);

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
            return MathHelper.Lerp(0f, trailWidth, num) * 0.5f;

        }
    }

    public class CursedBulletReplacer : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return item.type == ItemID.CursedBullet;
        }

        public override void SetDefaults(Item item)
        {
            item.StatsModifiedBy.Add(Mod);
            item.shoot = ModContent.ProjectileType<CursedBullet>();
        }
    }
}
