using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace AerovelenceMod.Effects
{
    public class ShockwaveMaker : ModProjectile
    {

        private int rippleCount = 3;
        private int rippleSize = 15;
        private int rippleSpeed = 15;
        private float distortStrength = 300f;
        public override string Texture => "AerovelenceMod/Blank";

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;

            projectile.timeLeft = 400;
            projectile.alpha = 255;
        }

        public override void AI()
        {
            projectile.ai[0] += 8;

            if (projectile.ai[1] == 0)
            {
                projectile.ai[1] = 1;
                if (!Filters.Scene["Shockwave"].IsActive())
                {
                    Filters.Scene.Activate("Shockwave", projectile.Center).GetShader().UseColor(rippleCount, rippleSize, rippleSpeed).UseTargetPosition(projectile.Center);
                }
            }
            else
            {
                projectile.ai[1]++;
                float progress = projectile.ai[1] / 60f;
                float distortStrength = 200;
                Filters.Scene["Shockwave"].GetShader().UseProgress(progress).UseOpacity(distortStrength * (1 - progress / 3f));
            }
        }
        public override void Kill(int timeLeft)
        {
            Filters.Scene["Shockwave"].Deactivate();
        }
    }
}