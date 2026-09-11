using System;
using System.IO;
using AerovelenceMod.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class TumblerProjectileRetirement : GlobalProjectile
    {
        private int remaining;
        private Vector2 extent;
        private float phase;
        private bool exitPlayed;
        internal static float VisualOpacity(Projectile projectile) => projectile.TryGetGlobalProjectile(out TumblerProjectileRetirement fade) && fade.remaining > 0 ? MathHelper.SmoothStep(0f, 1f, fade.remaining / 36f) : 1f;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => ProjectileLoader.GetProjectile(entity.type)?.GetType().Namespace == typeof(CrystalTumbler).Namespace;
        public static void Begin(Projectile projectile)
        {
            TumblerProjectileRetirement fade = projectile.GetGlobalProjectile<TumblerProjectileRetirement>();
            if (fade.remaining > 0)
                return;
            fade.PlayExit(projectile);
            if (projectile.ModProjectile is TumblerFloorRipple ripple)
            {
                ripple.Retire();
                return;
            }
            if (projectile.ModProjectile is TumblerPulseShield pulse)
            {
                pulse.Retire();
                return;
            }
            if (projectile.ModProjectile is TumblerConvergenceOrb orb)
            {
                orb.Retire();
                return;
            }
            fade.remaining = 36;
            fade.extent = projectile.velocity;
            fade.phase = PhaseFor(projectile);
            projectile.hostile = projectile.friendly = false;
            projectile.damage = 0;
            projectile.tileCollide = false;
            projectile.timeLeft = Math.Max(80, projectile.timeLeft);
            projectile.netUpdate = true;
        }
        public override bool? CanDamage(Projectile projectile) => remaining > 0 ? false : null;
        public override bool ShouldUpdatePosition(Projectile projectile) => remaining <= 0;
        public override bool PreAI(Projectile projectile)
        {
            if (remaining <= 0)
                return true;
            if (remaining == 1)
                projectile.Kill();
            else
                remaining--;
            return false;
        }
        public override bool PreKill(Projectile projectile, int timeLeft)
        {
            PlayExit(projectile);
            return remaining <= 0;
        }

        private void PlayExit(Projectile projectile)
        {
            if (exitPlayed || Main.dedServ || projectile.friendly || projectile.TryGetGlobalProjectile(out TumblerSharedProjectile shared) && !shared.FromEncounter)
                return;
            exitPlayed = true;
            TumblerLightningSystem.Release(projectile);
            if (projectile.ModProjectile is TumblerFloorRipple or TumblerSpark or TumblerAuraPulse or TumblerAimLine)
                return;
            float phase = PhaseFor(projectile);
            Color color = TumblerVFX.PhaseColor(phase);
            for (int i = 0; i < 6; i++)
            {
                Vector2 direction = (i * MathHelper.TwoPi / 6f).ToRotationVector2();
                TumblerVFX.SpawnSpark(projectile.Center + direction * Math.Min(28f, projectile.width * 0.5f), direction * Main.rand.NextFloat(1.5f, 3f), Color.Lerp(color, Color.White, 0.65f), 0.21f);
            }
        }
        private static float PhaseFor(Projectile projectile)
        {
            if (projectile.ModProjectile is TumblerPylonField or TumblerConductiveField or TumblerAuraPulse)
                return projectile.ai[2];
            if (projectile.ModProjectile is ElectricBolt or TumblerStar)
                return projectile.ai[0];
            if (projectile.ModProjectile is TumblerConvergenceOrb or TumblerRazeBeam or TumblerShieldStorm or TumblerChargedKnifeBall)
                return 1f;
            if (projectile.ModProjectile is TumblerBossAura or TumblerPulseShield or TumblerArenaGate or TumblerMagneticPlatform or TumblerFilamentRamp or TumblerLoopRail or TumblerCascadeRail)
            {
                int owner = (int)projectile.ai[0];
                return owner >= 0 && owner < Main.maxNPCs && Main.npc[owner].ModNPC is CrystalTumbler ? Main.npc[owner].ai[2] : 0f;
            }
            return projectile.ai[1];
        }
        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter writer)
        {
            writer.Write(remaining);
            writer.WriteVector2(extent);
            writer.Write(phase);
        }
        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader reader)
        {
            remaining = reader.ReadInt32();
            extent = reader.ReadVector2();
            phase = reader.ReadSingle();
            if (remaining > 0)
            {
                PlayExit(projectile);
                projectile.damage = 0;
                projectile.hostile = projectile.friendly = false;
                projectile.tileCollide = false;
            }
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            TumblerLightningSystem.BeginCapture(projectile);
            if (remaining <= 0)
                return true;
            float opacity = MathHelper.SmoothStep(0f, 1f, remaining / 36f);
            Color color = TumblerVFX.PhaseColor(phase >= 1f ? 1f : 0f);
            Vector2 center = projectile.Center - Main.screenPosition;
            if (projectile.ModProjectile is TumblerPylonField or TumblerResidualField or TumblerRazeBeam or TumblerBossAura or TumblerMagneticPlatform or TumblerArenaGate)
                return true;
            if (projectile.ModProjectile is TumblerAimLine)
            {
                Vector2 end = extent.SafeNormalize(Vector2.UnitY) * 1100f;
                TumblerVFX.DrawTelegraph(Main.spriteBatch, center, center + end, color, opacity * 0.65f);
            }
            else if (projectile.ModProjectile is TumblerLoopRail or TumblerFilamentRamp or TumblerCascadeRail)
                return true;
            else if (projectile.ModProjectile.Texture != "Terraria/Images/Projectile_0")
            {
                Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
                Rectangle frame = texture.Frame(1, Math.Max(1, Main.projFrames[projectile.type]), 0, projectile.frame);
                Main.EntitySpriteDraw(texture, center, frame, lightColor * opacity, projectile.rotation, frame.Size() * 0.5f, projectile.scale, SpriteEffects.None);
            }
            return false;
        }

        public override void PostDraw(Projectile projectile, Color lightColor) => TumblerLightningSystem.EndCapture();
    }
}
