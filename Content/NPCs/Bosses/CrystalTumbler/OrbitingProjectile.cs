using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using System.Linq;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class OrbitingProjectile : ModProjectile
    {
        private NPC crystalTumbler;
        private float orbitRadius = 50f;
        private float orbitSpeed = 0.02f;
        private float maxOrbitSpeed = 0.1f;
        private bool released = false;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            crystalTumbler ??= Main.npc.FirstOrDefault(npc => npc.active && npc.type == ModContent.NPCType<CrystalTumbler>());
            if (crystalTumbler == null || !crystalTumbler.active)
                released = true;
            if (!released)
            {
                Projectile.ai[0] += orbitSpeed;
                Vector2 orbitPosition = crystalTumbler.Center + Vector2.UnitX.RotatedBy(Projectile.ai[0]) * orbitRadius;
                Projectile.Center = orbitPosition;
                orbitSpeed = MathHelper.Clamp(orbitSpeed + 0.001f, 0, maxOrbitSpeed);
                if (orbitSpeed >= maxOrbitSpeed)
                {
                    released = true;
                    Projectile.velocity = (Projectile.Center - crystalTumbler.Center).SafeNormalize(Vector2.Zero) * (orbitSpeed * 500f);
                    Projectile.tileCollide = true;
                }
            }
            else

                Projectile.rotation += 0.1f;
        }
    }
}