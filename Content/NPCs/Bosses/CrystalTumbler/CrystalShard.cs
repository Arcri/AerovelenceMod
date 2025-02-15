using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Common.Utilities;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class CrystalShard : ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.aiStyle = 1;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.alpha = 0;
            Projectile.damage = 6;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        private int dustSpawnTimer = 0;

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.Blue.ToVector3() * 0.9f);

            dustSpawnTimer++;
            if (dustSpawnTimer >= 5)
            {
                Vector2 dustVel = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(2f, 3.25f);

                Dust gd = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(), dustVel, newColor: Color.SkyBlue, Scale: Main.rand.NextFloat(0.2f, 0.4f));
                gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, timeBeforeSlow: 5,
                    preSlowPower: 0.95f, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.9f, shouldFadeColor: false);

                dustSpawnTimer = 0; 
            }
        }
    }
}