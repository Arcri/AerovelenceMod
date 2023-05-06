using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using AerovelenceMod.Common.Utilities;

namespace AerovelenceMod.Content.Projectiles.Other
{
    public class DistortProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public Texture2D tex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Flares/star_05");

        public float scale = 1f;

        public bool implode = false;

        int timer = 0;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Distort");
        }
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 200;
            Projectile.tileCollide = false;
            Projectile.scale = implode ? 1 : 0;
            Projectile.ignoreWater = true;

        }
        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (implode)
            {

                if (timer == 0)
                    Projectile.scale = 1;

                Projectile.scale = MathHelper.Lerp(Projectile.scale, 0, 0.2f);

                if (Projectile.scale < 0.05f)
                    Projectile.active = false;
            }
            else
            {
                Projectile.scale = MathHelper.Lerp(Projectile.scale, 1, 0.2f);

                if (Projectile.scale > 0.95f)
                {
                    Projectile.active = false;
                    Main.NewText(timer);
                }
            }

            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

    }

}
