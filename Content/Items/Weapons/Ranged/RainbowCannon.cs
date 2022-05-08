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
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.width = 50;
            Item.height = 18;
            Item.shoot = ModContent.ProjectileType<RainbowCannonProj1>();
            Item.UseSound = SoundID.Item67;
            Item.damage = 45;
            Item.knockBack = 2.5f;
            Item.shootSpeed = 16f;
            Item.noMelee = true;
            Item.value = 350000;
            Item.rare = ItemRarityID.Yellow;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
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
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.scale = 1.25f;
        }
        public override void AI()
        {
            int num433 = 1200;
            if (Projectile.type == ModContent.ProjectileType<RainbowCannonProj1>())
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.localAI[0] += 1f;
                    if (Projectile.localAI[0] > 4f)
                    {
                        Projectile.localAI[0] = 3f;
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X * 0.001f, Projectile.velocity.Y * 0.001f, ModContent.ProjectileType<RainbowCannonProj2>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                    if (Projectile.timeLeft > num433)
                    {
                        Projectile.timeLeft = num433;
                    }
                }
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) - 1.57f;
                return;
            }
            if (Projectile.localAI[0] == 0f)
            {
                if (Projectile.velocity.X > 0f)
                {
                    Projectile.spriteDirection = -1;
                    Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) - 1.57f;
                }
                else
                {
                    Projectile.spriteDirection = 1;
                    Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) - 1.57f;
                }
                Projectile.localAI[0] = 1f;
            }
            Projectile.velocity.X *= 0.98f;
            Projectile.velocity.Y *= 0.98f;
            if (Projectile.rotation == 0f)
            {
                Projectile.alpha = 255;
            }
            else if (Projectile.timeLeft < 10)
            {
                Projectile.alpha = 255 - (int)(255f * Projectile.timeLeft / 10f);
            }
            else if (Projectile.timeLeft > num433 - 10)
            {
                int num436 = num433 - Projectile.timeLeft;
                Projectile.alpha = 255 - (int)(255f * num436 / 10f);
            }
            else
            {
                Projectile.alpha = 0;
            }
        }
    }

    public class RainbowCannonProj2 : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.alpha = 255;
            Projectile.light = 0.3f;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1.25f;
            Projectile.timeLeft = 50;
        }
        public override void AI()
        {
            int num433 = 50;
            if (Projectile.type == ModContent.ProjectileType<RainbowCannonProj1>())
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.localAI[0] += 1f;
                    if (Projectile.localAI[0] > 4f)
                    {
                        Projectile.localAI[0] = 3f;
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X * 0.001f, Projectile.velocity.Y * 0.001f, ModContent.ProjectileType<RainbowCannonProj2>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) - 1.57f;
                return;
            }
            if (Projectile.localAI[0] == 0f)
            {
                if (Projectile.velocity.X > 0f)
                {
                    Projectile.spriteDirection = -1;
                    Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) - 1.57f;
                }
                else
                {
                    Projectile.spriteDirection = 1;
                    Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) - 1.57f;
                }
                Projectile.localAI[0] = 1f;
            }
            if (Projectile.rotation == 0f)
            {
                Projectile.alpha = 255;
            }
            else if (Projectile.timeLeft < 10)
            {
                Projectile.alpha = 255 - (int)(255f * Projectile.timeLeft / 10f);
            }
            else if (Projectile.timeLeft > num433 - 10)
            {
                int num436 = num433 - Projectile.timeLeft;
                Projectile.alpha = 255 - (int)(255f * num436 / 10f);
            }
            else
            {
                Projectile.alpha = 0;
            }
        }
    }
}