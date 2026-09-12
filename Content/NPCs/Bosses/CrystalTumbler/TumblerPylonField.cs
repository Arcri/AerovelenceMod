using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class TumblerPylonField : ModProjectile
    {
        private const float FieldHeight = 64f;
        private int timer;
        private int Warning => Math.Max(60, (int)Projectile.ai[0]);
        private int Duration => Math.Max(20, (int)Projectile.ai[1]);
        private bool Active => timer >= Warning && timer < Warning + Duration;

        public override string Texture => "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/FencePylon";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2400;
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 360;
            Projectile.netImportant = true;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            timer++;
            if (!Main.dedServ && timer < Warning && timer % (timer > Warning - 30 ? 3 : 6) == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 position = Projectile.Center + Projectile.velocity * Main.rand.NextFloat() - new Vector2(0f, Main.rand.NextFloat(8f, FieldHeight));
                    Vector2 velocity = new(Main.rand.NextFloat(-0.7f, 0.7f), Main.rand.NextFloat(-0.7f, 0.3f));
                    TumblerVFX.SpawnSpark(position, velocity, Color.Lerp(TumblerVFX.PhaseColor(Projectile.ai[2]), Color.White, 0.85f), 0.13f + timer / (float)Warning * 0.09f);
                }
            }
            if (timer == Warning)
            {
                SoundEngine.PlaySound(SoundID.Item122 with { Volume = 0.65f, Pitch = -0.15f, MaxInstances = 2 }, Projectile.Center + Projectile.velocity * 0.5f);
                if (!Main.dedServ)
                {
                    for (int i = 0; i < 12; i++)
                        TumblerVFX.SpawnSpark(Projectile.Center + Projectile.velocity * ((i + 0.5f) / 12f) - new Vector2(0f, FieldHeight * 0.5f), Main.rand.NextVector2Circular(3f, 2f), Color.White, 0.3f);
                }
            }
            if (timer >= Warning + Duration + 20)
                Projectile.Kill();
            if (Active && !Main.dedServ)
            {
                Color color = TumblerVFX.PhaseColor(Projectile.ai[2]);
                Lighting.AddLight(Projectile.Center - new Vector2(0f, FieldHeight * 0.5f), color.ToVector3() * 0.45f);
                Lighting.AddLight(Projectile.Center + Projectile.velocity - new Vector2(0f, FieldHeight * 0.5f), color.ToVector3() * 0.45f);
                if (timer % 15 == 0)
                {
                    Vector2 endpoint = timer % 30 == 0 ? Projectile.Center : Projectile.Center + Projectile.velocity;
                    TumblerVFX.SpawnSpark(endpoint - new Vector2(0f, FieldHeight - 5f), new Vector2(Main.rand.NextFloat(-1.5f, 1.5f), -1.5f), color, 0.16f);
                }
            }
        }

        public override bool? CanDamage() => Active;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 end = Projectile.Center + Projectile.velocity;
            Rectangle field = new((int)Math.Min(Projectile.Center.X, end.X), (int)(Projectile.Center.Y - FieldHeight), (int)Math.Abs(Projectile.velocity.X), (int)FieldHeight);
            return field.Intersects(targetHitbox);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Electrified, 45);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(timer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timer = reader.ReadInt32();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Color color = TumblerVFX.PhaseColor(Projectile.ai[2]);
            float charge = MathHelper.Clamp(timer / (float)Warning, 0f, 1f);
            float opacity = MathHelper.Clamp(timer / 22f, 0f, 1f) * MathHelper.Clamp((Warning + Duration + 20f - timer) / 20f, 0f, 1f);
            opacity *= TumblerProjectileRetirement.VisualOpacity(Projectile);
            Vector2 left = Projectile.Center - Main.screenPosition;
            Vector2 right = left + Projectile.velocity;
            Vector2 top = new(0f, -FieldHeight);
            float flash = timer >= Warning ? MathF.Pow(MathHelper.Clamp(1f - (timer - Warning) / 16f, 0f, 1f), 2f) : 0f;
            Color dischargeColor = Color.Lerp(color, Color.White, flash);
            if (timer < Warning)
            {
                TumblerVFX.DrawTelegraph(spriteBatch, left + top, right + top, color, (0.3f + charge * 0.5f) * opacity);
                TumblerVFX.DrawTelegraph(spriteBatch, right + top, left + top, color, (0.3f + charge * 0.5f) * opacity);
            }
            if (timer >= Warning)
            {
                Texture2D bloom = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/SoftGlow64").Value;
                spriteBatch.Draw(bloom, (left + right + top) * 0.5f, null, TumblerVFX.Glow(dischargeColor, opacity * (0.18f + flash * 0.6f)), 0f, bloom.Size() * 0.5f, new Vector2(Projectile.velocity.Length() + 60f, FieldHeight * (1.3f + flash)) / bloom.Size(), SpriteEffects.None, 0f);
                for (int row = 1; row <= 4; row++)
                {
                    Vector2 offset = top * (row / 5f);
                    TumblerVFX.DrawElectricLine(spriteBatch, left + offset, right + offset, dischargeColor, opacity * (0.65f + flash * 0.35f), Math.Clamp((int)(Projectile.velocity.Length() / 24f), 4, 48), Projectile.identity + row * 6f, 1.5f + flash * 3f);
                }
            }
            DrawPylon(spriteBatch, left, dischargeColor, lightColor, opacity, charge, false);
            DrawPylon(spriteBatch, right, dischargeColor, lightColor, opacity, charge, true);
            return false;
        }

        private void DrawPylon(SpriteBatch spriteBatch, Vector2 position, Color color, Color lightColor, float opacity, float charge, bool facingRight)
        {
            Texture2D crystal = ModContent.Request<Texture2D>(Texture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Texture2D mask = ModContent.Request<Texture2D>(Texture + "_Glowmask", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            int variant = (Projectile.identity + (facingRight ? 1 : 0)) % 3;
            Rectangle frame = new(variant * 38, 0, 36, 62);
            Vector2 origin = new(frame.Width * 0.5f, frame.Height);
            Vector2 scale = new(FieldHeight / frame.Height);
            SpriteEffects effects = facingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(crystal, position, frame, Color.Lerp(lightColor, Color.White, 0.2f) * opacity, 0f, origin, scale, effects, 0f);
            spriteBatch.Draw(mask, position, frame, Color.White * opacity, 0f, origin, scale, effects, 0f);
            spriteBatch.Draw(mask, position, frame, TumblerVFX.Glow(color, opacity * charge * 0.65f), 0f, origin, scale, effects, 0f);
            Vector2 tip = position - new Vector2(0f, FieldHeight - 5f);
            TumblerVFX.DrawCharge(spriteBatch, tip, color, charge, 9f, timer * 0.025f, opacity);
        }
    }
}
