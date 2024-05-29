using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Projectiles.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Net;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Melee.HandBlades
{
    public class TetraBladeStrike : ModProjectile
    {
        public int timer = 0;
        public Vector2 distFromPlayer = Vector2.Zero;
        public Color strikeCol = Color.Green;

        public override void SetStaticDefaults()
        {
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

            Projectile.hide = true; //So DrawBehind works

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override bool? CanDamage() { return hitCount < 3; }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = Projectile.Center.X > Main.player[Projectile.owner].Center.X ? 1 : -1;
        }

        int hitCount = 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hitCount == 0)
            {
                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/GGS/Swing_Sword_Heavy_M_a") with { Pitch = .15f, PitchVariance = .15f, Volume = 0.2f, MaxInstances = -1 };
                SoundEngine.PlaySound(style, target.Center);
            }

            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<RoaParticle>(), Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(3f, 4.1f),
                newColor: strikeCol);

            for (int j = 0; j < 2 + (Main.rand.NextBool() ? 1 : 0); j++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelAlts>(), newColor: strikeCol, Scale: 0.55f + Main.rand.NextFloat(-0.1f, 0.2f));
                d.velocity = Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(5f, 6f);
                d.velocity = d.velocity.RotatedBy(Main.rand.NextFloat(-0.02f, 0.02f));
            }

            for (int i = 0; i < 4; i++)
            {

                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelAlts>(), newColor: strikeCol, Scale: 0.5f + Main.rand.NextFloat(-0.1f, 0.2f));
                d.velocity = Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(2f, 4f);
                d.velocity = d.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f));
            }

            //target.immune[Projectile.owner] = 1;
            hitCount++;
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

            Projectile.direction = Projectile.Center.X > Main.player[Projectile.owner].Center.X ? 1 : -1;

            xScale = MathHelper.Clamp(MathHelper.Lerp(xScale, 1.5f, 0.4f), 0, 1);
            yScale = MathHelper.Clamp(MathHelper.Lerp(yScale, 0f, 0.1f), 0, 1);
            timer++;
        }

        float xScale = 0f;
        float yScale = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            var Tex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Melee/HandBlades/TetraStrikeThick").Value;

            int frameHeight = Tex.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, Tex.Width, frameHeight);
            Vector2 scale = new Vector2(xScale, yScale) * 1.1f;

            Vector2 origin = sourceRectangle.Size() / 2f;

            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Main.player[Projectile.owner].gfxOffY);

            Main.spriteBatch.Draw(Tex, drawPos, sourceRectangle, strikeCol with { A = 0 }, Projectile.rotation, origin, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex, drawPos, sourceRectangle, strikeCol with { A = 0 } * 0.5f, Projectile.rotation, origin, scale * 1.25f, SpriteEffects.None, 0f);

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player owner = Main.player[Projectile.owner];

            float point = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), owner.Center, owner.Center + Projectile.rotation.ToRotationVector2() * 80f, 20f, ref point);
        }
    }
}