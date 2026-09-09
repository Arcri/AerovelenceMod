using System;
using System.IO;
using AerovelenceMod.Content.Items.BossSummons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class Stalactite : ModProjectile
    {
        private int timer;
        private float targetY;

        private int Delay => 75 + Math.Max(0, (int)Projectile.ai[0]);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1400;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 48;
            Projectile.timeLeft = 360;
            Projectile.penetrate = 1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (ArenaData.Valid)
            {
                float clearY = ArenaData.FloorY - Projectile.height - 80f;
                Vector2 probe = new(Projectile.position.X, clearY);
                for (float y = clearY; y >= ArenaData.WorldBounds.Top + 32f; y -= 8f)
                {
                    probe.Y = y;
                    if (Collision.SolidCollision(probe, Projectile.width, Projectile.height))
                        break;
                    clearY = y;
                }
                Projectile.position.Y = clearY + 5f;
                targetY = ArenaData.FindGroundWorldY(Projectile.Center.X, Projectile.Bottom.Y + 8f);
            }
            else
                targetY = Projectile.Bottom.Y + 800f;
        }

        public override bool? CanDamage()
        {
            return timer >= Delay && Projectile.Opacity >= 0.85f && Projectile.timeLeft > 24;
        }

        public override void AI()
        {
            timer++;
            Projectile.Opacity = MathHelper.Clamp(timer / 22f, 0f, 1f) * MathHelper.Clamp(Projectile.timeLeft / 24f, 0f, 1f);
            if (timer < Delay)
            {
                Projectile.velocity = Vector2.Zero;
            }
            else
            {
                if (timer == Delay)
                    SoundEngine.PlaySound(SoundID.Item70 with { Volume = 0.3f, Pitch = 0.2f }, Projectile.Center);
                Projectile.tileCollide = !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height);
                Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.38f, 14f);
                if (Projectile.Bottom.Y + Projectile.velocity.Y >= targetY)
                {
                    Projectile.Bottom = new Vector2(Projectile.Center.X, targetY);
                    Projectile.Kill();
                    return;
                }
            }
            Lighting.AddLight(Projectile.Bottom, TumblerVFX.PhaseColor(Projectile.ai[1]).ToVector3() * 0.45f * Projectile.Opacity);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = true;
            return true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(timer);
            writer.Write(targetY);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timer = reader.ReadInt32();
            targetY = reader.ReadSingle();
        }

        public override void OnKill(int timeLeft)
        {
            if (ArenaData.ClearingEncounter || timer < Delay)
                return;
            SoundEngine.PlaySound(SoundID.Shatter with { Volume = 0.35f, Pitch = 0.2f }, Projectile.Bottom);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Bottom, Vector2.Zero, ModContent.ProjectileType<TumblerAuraPulse>(), 0, 0f, Main.myPlayer, 38f, 18f, Projectile.ai[1]);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 scale = new Vector2(Projectile.width, Projectile.height) / texture.Size();
            Vector2 tip = Projectile.Bottom - Main.screenPosition;
            Vector2 target = new(Projectile.Center.X - Main.screenPosition.X, targetY - Main.screenPosition.Y);
            Color color = TumblerVFX.PhaseColor(Projectile.ai[1]);
            Texture2D star = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Pixel/CrispStarPMA").Value;
            if (timer < Delay)
            {
                float charge = timer / (float)Delay;
                TumblerVFX.DrawTelegraph(Main.spriteBatch, tip, target, color, 0.18f + charge * 0.38f, 62f);
                TumblerVFX.DrawLine(Main.spriteBatch, target - new Vector2(17f, 0f), target + new Vector2(17f, 0f), TumblerVFX.Glow(color, 0.45f + charge * 0.3f), 2f);
                Main.EntitySpriteDraw(star, target, null, TumblerVFX.Glow(color, 0.5f + charge * 0.4f), 0f, star.Size() * 0.5f, new Vector2(30f, 12f) / star.Size(), SpriteEffects.None);
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation, texture.Size() * 0.5f, scale, SpriteEffects.None);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, TumblerVFX.Glow(color, Projectile.Opacity * 0.23f), Projectile.rotation, texture.Size() * 0.5f, scale, SpriteEffects.None);
            Main.EntitySpriteDraw(star, tip, null, TumblerVFX.Glow(Color.Lerp(color, Color.White, 0.35f), Projectile.Opacity * 0.7f), 0f, star.Size() * 0.5f, new Vector2(22f, 12f) / star.Size(), SpriteEffects.None);
            return false;
        }
    }
}
