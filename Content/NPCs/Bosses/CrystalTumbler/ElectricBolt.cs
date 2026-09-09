using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class ElectricBolt : ModProjectile
    {
        private readonly TumblerLightningVisual lightning = new();
        private int timer;

        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.timeLeft = 180;
            Projectile.penetrate = 1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            timer++;
            bool charged = Projectile.ai[0] >= 1f;
            if (charged && timer == 1 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.velocity *= 1.5f;
                Projectile.damage = (int)(Projectile.damage * 1.5f);
                Projectile.netUpdate = true;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Main.dedServ)
                return;
            Color color = TumblerVFX.PhaseColor(Projectile.ai[0]);
            Lighting.AddLight(Projectile.Center, color.ToVector3() * 0.65f);
            if (timer % 8 == 0)
            {
                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);
                Vector2 sparkVelocity = -direction.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f)) * Main.rand.NextFloat(1.2f, 2.8f);
                TumblerVFX.SpawnSpark(Projectile.Center - direction * 12f, sparkVelocity, color, Main.rand.NextFloat(0.16f, 0.24f));
            }
            Vector2 tail = Projectile.Center - Projectile.velocity * Math.Min(timer - 1, charged ? 13 : 15);
            lightning.Update(Projectile, tail, Projectile.Center, charged ? 0.65f : 0.45f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D star = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Pixel/CrispStarPMA").Value;
            Color color = TumblerVFX.PhaseColor(Projectile.ai[0]);
            lightning.Draw(Main.spriteBatch, color, 0.9f, 1.4f);
            Vector2 position = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(star, position, null, TumblerVFX.Glow(color), Projectile.rotation, star.Size() * 0.5f, new Vector2(0.58f, 0.29f), SpriteEffects.None);
            Main.EntitySpriteDraw(star, position, null, TumblerVFX.Glow(Color.White), Projectile.rotation, star.Size() * 0.5f, new Vector2(0.27f, 0.14f), SpriteEffects.None);
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Electrified, Projectile.ai[0] >= 1f ? 90 : 45);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item93 with { Pitch = 0.35f, Volume = 0.2f, MaxInstances = 8 }, Projectile.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient && !global::AerovelenceMod.Content.Items.BossSummons.ArenaData.ClearingEncounter)
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TumblerAuraPulse>(), 0, 0f, Main.myPlayer, Projectile.ai[0] >= 1f ? 34f : 24f, 16f, Projectile.ai[0]);
        }
    }
}
