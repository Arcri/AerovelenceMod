using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using AerovelenceMod.Dusts;
using Microsoft.Xna.Framework.Graphics;

namespace AerovelenceMod.Projectiles
{
    public class ElementScythe : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thorn Ball");

        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 38;
            projectile.aiStyle = 18;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 999;
            projectile.timeLeft = 600;
            projectile.alpha = 100;
        }
    }
}