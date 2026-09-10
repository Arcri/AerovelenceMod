using System;
using System.IO;
using AerovelenceMod.Content.Items.BossSummons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class TumblerMagneticRock : ModProjectile
    {
        private int timer;
        private int bounces;
        private readonly TumblerLightningVisual tether = new();
        private Color ChargeColor => Projectile.ai[1] >= 1f ? TumblerVFX.PhaseColor(1f) : new Color(98, 209, 255);
        public override string Texture => "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/RockProjectile";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1800;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 48;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 330;
        }

        private bool TryBoss(out NPC boss)
        {
            int index = (int)Projectile.ai[0];
            boss = index >= 0 && index < Main.maxNPCs ? Main.npc[index] : null;
            return boss != null && boss.active && boss.ModNPC is CrystalTumbler;
        }

        public override void AI()
        {
            if (!TryBoss(out NPC boss))
            {
                Projectile.Kill();
                return;
            }
            timer++;
            Projectile.frame = Projectile.identity % 3;
            Projectile.rotation += Projectile.velocity.X * 0.022f;
            if (timer <= 90)
            {
                Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.32f, 12f);
                float floor = ArenaData.FloorY;
                foreach (ArenaPlatform platform in ArenaData.Platforms)
                {
                    if (Projectile.Right.X + Projectile.velocity.X > platform.Left.X && Projectile.Left.X + Projectile.velocity.X < platform.Right.X && Projectile.Bottom.Y <= platform.Left.Y + 4f && Projectile.Bottom.Y + Projectile.velocity.Y >= platform.Left.Y)
                        floor = Math.Min(floor, platform.Left.Y);
                }
                foreach (Projectile candidate in Main.ActiveProjectiles)
                {
                    if (!TumblerMagneticPlatform.IsArenaPlatform(candidate) || candidate.ModProjectile is not TumblerMagneticPlatform platform || !platform.CanStand)
                        continue;
                    if (Projectile.Right.X + Projectile.velocity.X <= platform.SurfaceStart.X || Projectile.Left.X + Projectile.velocity.X >= platform.SurfaceEnd.X)
                        continue;
                    if (Projectile.Bottom.Y <= platform.SurfaceY + 4f && Projectile.Bottom.Y + Projectile.velocity.Y >= platform.SurfaceY)
                        floor = Math.Min(floor, platform.SurfaceY);
                }
                if (Projectile.velocity.Y > 0f && Projectile.Bottom.Y + Projectile.velocity.Y >= floor)
                {
                    Projectile.Bottom = new Vector2(Projectile.Center.X, floor);
                    Projectile.velocity.Y = -(8.5f + (++bounces + Projectile.identity) % 3);
                    SoundEngine.PlaySound(SoundID.Item50 with { Volume = 0.2f, MaxInstances = 3 }, Projectile.Center);
                }
                if (Projectile.Left.X + Projectile.velocity.X < ArenaData.OuterArenaBoundaryLeft.X || Projectile.Right.X + Projectile.velocity.X > ArenaData.OuterArenaBoundaryRight.X)
                    Projectile.velocity.X *= -0.9f;
            }
            else
            {
                Vector2 direction = (boss.Center - Projectile.Center).SafeNormalize(Vector2.UnitY);
                float speed = MathHelper.Lerp(6f, 15f, MathHelper.Clamp((timer - 90f) / 75f, 0f, 1f));
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * speed, 0.1f);
                tether.Update(Projectile, Projectile.Center, boss.Center, 0.6f);
                if (timer == 91)
                    SoundEngine.PlaySound(SoundID.Item93 with { Volume = 0.4f, Pitch = 0.2f }, Projectile.Center);
                if (Projectile.Hitbox.Intersects(boss.Hitbox) && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TumblerAuraPulse>(), 0, 0f, Main.myPlayer, 65f, 22f, Projectile.ai[1]);
                    Projectile.Kill();
                }
            }
            Lighting.AddLight(Projectile.Center, ChargeColor.ToVector3() * (timer > 90 ? 0.6f : 0.2f));
            if (Main.netMode == NetmodeID.Server && timer % 30 == 0)
                Projectile.netUpdate = true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
                return true;
            if (timer < 90)
                return false;
            foreach (Projectile other in Main.ActiveProjectiles)
            {
                if (other.type != Type || other.identity <= Projectile.identity || other.ai[0] != Projectile.ai[0] || Vector2.DistanceSquared(other.Center, Projectile.Center) > 200f * 200f)
                    continue;
                if (((TumblerMagneticRock)other.ModProjectile).timer < 110 || timer < 110)
                    continue;
                float collision = 0f;
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, other.Center, 8f, ref collision))
                    return true;
            }
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Electrified, 60);

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(timer);
            writer.Write(bounces);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timer = reader.ReadInt32();
            bounces = reader.ReadInt32();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 position = Projectile.Center - Main.screenPosition;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame(1, 3, 0, Projectile.frame);
            float charge = MathHelper.Clamp((timer - 65f) / 45f, 0f, 1f);
            Texture2D glow = ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/RockProjectileGlow").Value;
            Rectangle glowFrame = glow.Frame(1, 3, 0, Projectile.frame);
            Main.EntitySpriteDraw(glow, position, glowFrame, TumblerVFX.Glow(ChargeColor, charge), Projectile.rotation, glowFrame.Size() * 0.5f, 0.56f + charge * 0.08f, SpriteEffects.None);
            Main.EntitySpriteDraw(texture, position, frame, Color.Lerp(lightColor, ChargeColor, charge * 0.25f), Projectile.rotation, frame.Size() * 0.5f, 1f, SpriteEffects.None);
            if (TryBoss(out NPC boss) && timer > 45)
            {
                if (timer <= 90)
                    TumblerVFX.DrawTelegraph(Main.spriteBatch, position, boss.Center - Main.screenPosition, ChargeColor, charge * 0.7f);
                else
                    tether.Draw(Main.spriteBatch, ChargeColor, charge * 0.8f, 2f);
            }
            foreach (Projectile other in Main.ActiveProjectiles)
            {
                if (other.type != Type || other.identity <= Projectile.identity || other.ai[0] != Projectile.ai[0] || Vector2.DistanceSquared(other.Center, Projectile.Center) > 200f * 200f || timer < 90)
                    continue;
                int otherTimer = ((TumblerMagneticRock)other.ModProjectile).timer;
                if (otherTimer < 90)
                    continue;
                if (timer < 110 || otherTimer < 110)
                    TumblerVFX.DrawTelegraph(Main.spriteBatch, position, other.Center - Main.screenPosition, ChargeColor, 0.55f);
                else
                    TumblerVFX.DrawElectricLine(Main.spriteBatch, position, other.Center - Main.screenPosition, ChargeColor, 0.8f, 12, Projectile.identity, 2f);
            }
            return false;
        }
    }
}
