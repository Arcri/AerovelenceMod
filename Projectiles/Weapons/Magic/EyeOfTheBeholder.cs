using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using AerovelenceMod.Dusts;
using Microsoft.Xna.Framework.Graphics;

namespace AerovelenceMod.Projectiles.Weapons.Magic
{
    public class EyeOfTheBeholder : ModProjectile
    {
        bool Active = true;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eye of the Beholder");
        }

        public override void SetDefaults()
        {
            projectile.width = 150;
            projectile.height = 100;
            projectile.aiStyle = 0;
            projectile.tileCollide = false;
            projectile.ignoreWater = false;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 999;
        }
        public override void AI()
        {
            projectile.alpha += 2;
            Vector2 projectilecenter = projectile.position - Vector2.UnitX * -50;
            int projectileCount = 5;
            if (Active)
            {
                for (int i = 0; i < projectileCount; i++)
                {
                    if (i == 4)
                    {
                        Active = false;
                    }
                    Main.PlaySound(SoundID.Item66, projectile.position);
                    float speed = 1f;
                    int type = mod.ProjectileType("BeholderOrb");
                    Vector2 velocity = new Vector2(speed, speed).RotatedByRandom(MathHelper.ToRadians(360));
                    Projectile.NewProjectile(projectilecenter, velocity, type, projectile.damage, 5f, projectile.owner);

                };
            }
        }
    }
}