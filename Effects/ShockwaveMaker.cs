using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aerovelence.Effects
{
    public class ShockwaveMaker : ModProjectile
    {

        private int rippleCount = 3;
        private int rippleSize = 15;
        private int rippleSpeed = 15;
        private float distortStrength = 300f;

        public override string Texture => "Aerovelence/Blank";

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.timeLeft = 400;
            projectile.alpha = 255;

        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("trol");
        }
        public override void AI()
        {
            if (projectile.ai[0] == 0)
            {
                projectile.ai[0] = 1;
                if (!Filters.Scene["Shockwave"].IsActive())
                {
                    Filters.Scene.Activate("Shockwave", projectile.Center).GetShader().UseColor(rippleCount, rippleSize, rippleSpeed).UseTargetPosition(projectile.Center);
                }
            }
            else
            {
                projectile.ai[0]++;
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