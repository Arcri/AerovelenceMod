using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class RainbowCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rainbow Cannon");
            Tooltip.SetDefault("Shoots a rainbow that stays and fires rainbow blasts alongside it");
        }
        public override void SetDefaults()
        {
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useAnimation = 40;
            item.useTime = 40;
            item.width = 50;
            item.height = 18;
            item.shoot = ModContent.ProjectileType<RainbowCannonProj1>();
            item.UseSound = SoundID.Item67;
            item.damage = 45;
            item.knockBack = 2.5f;
            item.shootSpeed = 16f;
            item.noMelee = true;
            item.value = 350000;
            item.rare = ItemRarityID.Yellow;
            item.magic = true;
            item.mana = 20;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }
    }

    public class RainbowCannonProj1 : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.penetrate = -1;
            projectile.magic = true;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.scale = 1.25f;
        }
        public override void AI()
        {
            int num433 = 1200;
            if (projectile.type == ModContent.ProjectileType<RainbowCannonProj1>())
            {
                if (projectile.owner == Main.myPlayer)
                {
                    projectile.localAI[0] += 1f;
                    if (projectile.localAI[0] > 4f)
                    {
                        projectile.localAI[0] = 3f;
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 0.001f, projectile.velocity.Y * 0.001f, ModContent.ProjectileType<RainbowCannonProj2>(), projectile.damage, projectile.knockBack, projectile.owner);
                    }
                    if (projectile.timeLeft > num433)
                    {
                        projectile.timeLeft = num433;
                    }
                }
                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) - 1.57f;
                return;
            }
            if (projectile.localAI[0] == 0f)
            {
                if (projectile.velocity.X > 0f)
                {
                    projectile.spriteDirection = -1;
                    projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) - 1.57f;
                }
                else
                {
                    projectile.spriteDirection = 1;
                    projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) - 1.57f;
                }
                projectile.localAI[0] = 1f;
            }
            projectile.velocity.X *= 0.98f;
            projectile.velocity.Y *= 0.98f;
            if (projectile.rotation == 0f)
            {
                projectile.alpha = 255;
            }
            else if (projectile.timeLeft < 10)
            {
                projectile.alpha = 255 - (int)(255f * projectile.timeLeft / 10f);
            }
            else if (projectile.timeLeft > num433 - 10)
            {
                int num436 = num433 - projectile.timeLeft;
                projectile.alpha = 255 - (int)(255f * num436 / 10f);
            }
            else
            {
                projectile.alpha = 0;
            }
        }
    }

    public class RainbowCannonProj2 : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.magic = true;
            projectile.alpha = 255;
            projectile.light = 0.3f;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.scale = 1.25f;
            projectile.timeLeft = 50;
        }
        public override void AI()
        {
            int num433 = 50;
            if (projectile.type == ModContent.ProjectileType<RainbowCannonProj1>())
            {
                if (projectile.owner == Main.myPlayer)
                {
                    projectile.localAI[0] += 1f;
                    if (projectile.localAI[0] > 4f)
                    {
                        projectile.localAI[0] = 3f;
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 0.001f, projectile.velocity.Y * 0.001f, ModContent.ProjectileType<RainbowCannonProj2>(), projectile.damage, projectile.knockBack, projectile.owner);
                    }
                }
                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) - 1.57f;
                return;
            }
            if (projectile.localAI[0] == 0f)
            {
                if (projectile.velocity.X > 0f)
                {
                    projectile.spriteDirection = -1;
                    projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) - 1.57f;
                }
                else
                {
                    projectile.spriteDirection = 1;
                    projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) - 1.57f;
                }
                projectile.localAI[0] = 1f;
            }
            if (projectile.rotation == 0f)
            {
                projectile.alpha = 255;
            }
            else if (projectile.timeLeft < 10)
            {
                projectile.alpha = 255 - (int)(255f * projectile.timeLeft / 10f);
            }
            else if (projectile.timeLeft > num433 - 10)
            {
                int num436 = num433 - projectile.timeLeft;
                projectile.alpha = 255 - (int)(255f * num436 / 10f);
            }
            else
            {
                projectile.alpha = 0;
            }
        }
    }
}