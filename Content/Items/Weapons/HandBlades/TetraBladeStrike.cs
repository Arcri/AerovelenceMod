using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Projectiles.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.HandBlades
{
    public class TetraBladeStrike : ModProjectile
    {
        public int timer = 0;
        public Vector2 distFromPlayer = Vector2.Zero;
        public Color strikeCol = Color.Green;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tetra Strike");
            Main.projFrames[Projectile.type] = 5;
        }
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;
            Projectile.damage = 80;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            //projectile.netImportant = true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/GGS/Swing_Sword_Heavy_M_a") with { Pitch = .14f, PitchVariance = .16f, Volume = 0.25f, MaxInstances = -1 };
            SoundEngine.PlaySound(style, target.Center);
            for (int i = 0; i < 1; i++)

            {
                int roA = Projectile.NewProjectile(null, Projectile.Center, Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(3f, 4.1f), ModContent.ProjectileType<RoAHit>(), 0, 0);

                if (Main.projectile[roA].ModProjectile is RoAHit hits)
                {
                    hits.color = strikeCol;
                    hits.Projectile.frameCounter = Main.rand.Next(1, 3);
                }
            }

            for (int j = 0; j < 2 + (Main.rand.NextBool() ? 1 : 0); j++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelAlts>(), newColor: strikeCol, Scale: 0.6f + Main.rand.NextFloat(-0.1f, 0.2f));
                d.velocity = Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(5f, 6f);
                d.velocity = d.velocity.RotatedBy(Main.rand.NextFloat(-0.02f, 0.02f));
            }

            for (int i = 0; i < 4; i++)
            {

                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelAlts>(), newColor: strikeCol, Scale: 0.55f + Main.rand.NextFloat(-0.1f, 0.2f));
                d.velocity = Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(2f, 4f);
                d.velocity = d.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f));
            }

            target.immune[Projectile.owner] = 1; //20
            Projectile.damage = 0;
        }

        public override void AI()
        {
            if (timer == 0)
            {
                distFromPlayer = Projectile.Center - Main.player[Projectile.owner].Center;
            }

            Projectile.Center = Main.player[Projectile.owner].Center + distFromPlayer + (distFromPlayer.SafeNormalize(Vector2.UnitX) * (timer * 0.25f));

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 3)
            {
                if (Projectile.frame == 4)
                    Projectile.active = false;

                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
            }

            xScale = MathHelper.Clamp(MathHelper.Lerp(xScale, 1.5f, 0.4f), 0, 1);
            yScale = MathHelper.Clamp(MathHelper.Lerp(yScale, 0f, 0.1f), 0, 1);
            timer++;
        }

        float xScale = 0f;
        float yScale = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            var Tex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/HandBlades/TetraStrikeThick").Value;

            int frameHeight = Tex.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, Tex.Width, frameHeight);


            Vector2 scale = new Vector2(xScale, yScale) * 1.1f;

            Vector2 origin = sourceRectangle.Size() / 2f;


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            //Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, sourceRectangle, color * opacity, Projectile.rotation, origin, Projectile.scale * 0.5f, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, sourceRectangle, color * opacity * 0.5f, Projectile.rotation, origin, Projectile.scale * 0.75f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, sourceRectangle, strikeCol, Projectile.rotation, origin, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, sourceRectangle, strikeCol * 0.5f, Projectile.rotation, origin, scale * 1.25f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }


        //Stretch from 0 to 

        public float ph(float x)
        {
            return 0f;
        }
    }
}