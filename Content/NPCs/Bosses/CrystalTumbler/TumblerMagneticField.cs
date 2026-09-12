using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class TumblerMagneticField : ModProjectile
    {
        private int Age => 330 - Projectile.timeLeft;
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetDefaults()
        {
            Projectile.width = 128;
            Projectile.height = 46;
            Projectile.timeLeft = 330;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => Age >= 120 && Projectile.timeLeft > 30 && Platform(out _);
        public override void SendExtraAI(BinaryWriter writer) => writer.Write(Projectile.timeLeft);
        public override void ReceiveExtraAI(BinaryReader reader) => Projectile.timeLeft = reader.ReadInt32();
        private bool Platform(out TumblerMagneticPlatform platform)
        {
            platform = null;
            int index = (int)Projectile.ai[0];
            if (index < 0 || index >= Main.maxProjectiles)
                return false;
            Projectile target = Main.projectile[index];
            if (!TumblerMagneticPlatform.IsArenaPlatform(target) || target.identity != (int)Projectile.ai[1])
                return false;
            platform = (TumblerMagneticPlatform)target.ModProjectile;
            return !platform.Collapsing;
        }
        public override void AI()
        {
            if (!Platform(out TumblerMagneticPlatform platform))
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    TumblerProjectileRetirement.Begin(Projectile);
                return;
            }
            Projectile.position = platform.SurfaceStart - new Vector2(0f, Projectile.height);
            Projectile.width = platform.Projectile.width;
            float charge = MathHelper.Clamp(Age / 120f, 0f, 1f);
            platform.FieldCharge = MathHelper.Clamp(Projectile.timeLeft / 30f, 0f, 1f) * charge;
            if (Age is 1 or 60 or 90 or 110 or 120)
                SoundEngine.PlaySound(SoundID.Item93 with { Volume = Age == 120 ? 0.7f : 0.4f, Pitch = Age / 180f }, Projectile.Center);
            if (!Main.dedServ && Age % (Age < 120 ? 8 : 4) == 0)
            {
                Vector2 position = new(Main.rand.NextFloat(Projectile.Left.X, Projectile.Right.X), Projectile.Bottom.Y);
                TumblerVFX.SpawnSpark(position, new Vector2(Main.rand.NextFloat(-1f, 1f), -2f - charge * 2f), Color.Lerp(TumblerVFX.PhaseColor(Projectile.ai[2]), Color.White, 0.6f), 0.3f);
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Electrified, 45);
        public override bool PreDraw(ref Color lightColor)
        {
            float opacity = TumblerProjectileRetirement.VisualOpacity(Projectile) * MathHelper.Clamp(Projectile.timeLeft / 30f, 0f, 1f);
            Color color = TumblerVFX.PhaseColor(Projectile.ai[2]);
            Vector2 left = Projectile.BottomLeft;
            if (Age < 120)
            {
                float warning = MathHelper.Clamp(Age / 10f, 0f, 1f);
                TumblerResidualField.DrawField(left, Projectile.Right.X, Projectile.ai[2], opacity * warning * (0.15f + MathF.Abs(MathF.Sin(Age * 0.12f)) * 0.15f));
                for (int side = -1; side <= 1; side += 2)
                {
                    Vector2 tip = Projectile.Top - Main.screenPosition + new Vector2(side * (20f + Age % 30), -8f);
                    Color arrow = TumblerVFX.Glow(Color.Lerp(color, Color.White, 0.5f), warning * opacity * 0.8f);
                    TumblerVFX.DrawLine(Main.spriteBatch, tip - new Vector2(side * 8f, 7f), tip, arrow, 2f);
                    TumblerVFX.DrawLine(Main.spriteBatch, tip - new Vector2(side * 8f, -7f), tip, arrow, 2f);
                }
            }
            else
            {
                TumblerResidualField.DrawField(left, Projectile.Right.X, Projectile.ai[2], opacity);
                float flash = MathHelper.Clamp((132f - Age) / 12f, 0f, 1f);
                if (flash > 0f)
                    TumblerVFX.DrawElectricLine(Main.spriteBatch, left - Main.screenPosition - new Vector2(0f, 20f), Projectile.BottomRight - Main.screenPosition - new Vector2(0f, 20f), Color.White, flash * opacity, 12, Projectile.identity + Age, 4f);
            }
            return false;
        }
    }
}
