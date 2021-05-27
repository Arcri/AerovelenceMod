<<<<<<< Updated upstream
using Microsoft.Xna.Framework;
=======
using System;
using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
>>>>>>> Stashed changes
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class Zodiac : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Zodiac");
<<<<<<< Updated upstream
            Tooltip.SetDefault("Fires two bullets at once");
        }
        public override void SetDefaults()
        {
            item.value = Item.sellPrice(1);
            item.damage = 25;
            item.ranged = true;
            item.width = 68;
            item.height = 28;
            item.useTime = 5;
            item.useAnimation = 5;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 5f;
            item.rare = ItemRarityID.Orange;
            item.noMelee = true;
            item.UseSound = SoundID.Item41;
            item.autoReuse = true;
            item.shootSpeed = 45f;
            item.shoot = AmmoID.Bullet;
            item.useAmmo = AmmoID.Bullet;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float numberProjectiles = 2;
            float rotation = MathHelper.ToRadians(6);
            position += Vector2.Normalize(new Vector2(speedX, speedY)) * 10f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f;
                Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockBack, player.whoAmI);
            }
            return false;
=======
            Tooltip.SetDefault("MAN");
        }

        public override void SetDefaults()
        {
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useAnimation = 2;
            item.useTime = 2;
            item.shootSpeed = 20f;
            item.knockBack = 2f;
            item.width = 70;
            item.height = 38;
            item.damage = 30;
            item.shoot = ModContent.ProjectileType<ZodiacHeld>();
            item.rare = ItemRarityID.Yellow;
            item.value = Item.sellPrice(0, 5, 20, 0);
            item.noMelee = true;
            item.noUseGraphic = true;
            item.ranged = true;
            item.channel = true;
        }
    }

    public class ZodiacHeld : ModProjectile
    {
        public int Timer;
        public float shootSpeed = 0.25f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 70;
            projectile.aiStyle = 75;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.hide = true;
            projectile.ranged = true;
            projectile.ignoreWater = true;
        }


        public override void AI()
        {
            // TODO: Make width work lmao wtf
            Player player = Main.player[projectile.owner];
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter);

            projectile.ai[0] += 1f;
            int num2 = 0;
            if (projectile.ai[0] >= 40f)
            {
                num2++;
            }
            if (projectile.ai[0] >= 80f)
            {
                num2++;
            }
            if (projectile.ai[0] >= 120f)
            {
                num2++;
            }
            int num3 = 24;
            int num4 = 6;
            projectile.ai[1] += 1f;
            bool flag = false;
            if (projectile.ai[1] >= (float)(num3 - num4 * num2))
            {
                projectile.ai[1] = 0f;
                flag = true;
            }
            projectile.frameCounter += 1 + num2;
            if (projectile.frameCounter >= 10)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame >= 2)
                {
                    projectile.frame = 0;
                }
            }
            if (projectile.soundDelay <= 0)
            {
                projectile.soundDelay = num3 - num4 * num2;
                if (projectile.ai[0] != 1f)
                {
                    Main.PlaySound(SoundID.Item1, projectile.position);
                }
            }
            if (projectile.ai[1] == 1f && projectile.ai[0] != 1f)
            {
                Vector2 spinningpoint = Vector2.UnitX * 24f;
                spinningpoint = spinningpoint.RotatedBy(projectile.rotation - (float)Math.PI / 2f);
                Vector2 value = projectile.Center + spinningpoint;
                for (int i = 0; i < 2; i++)
                {
                    int num5 = Dust.NewDust(value - Vector2.One * 8f, 16, 16, 135, projectile.velocity.X / 2f, projectile.velocity.Y / 2f, 100);
                    Main.dust[num5].velocity *= 0.66f;
                    Main.dust[num5].noGravity = true;
                    Main.dust[num5].scale = 1.4f;
                }
            }
            if (flag && Main.myPlayer == projectile.owner)
            {
                if (player.channel && player.CheckMana(player.inventory[player.selectedItem], -1, pay: true) && !player.noItems && !player.CCed)
                {
                    float num6 = player.inventory[player.selectedItem].shootSpeed * projectile.scale;
                    Vector2 value2 = vector;
                    Vector2 value3 = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY) - value2;
                    if (player.gravDir == -1f)
                    {
                        value3.Y = (Main.screenHeight - Main.mouseY) + Main.screenPosition.Y - value2.Y;
                    }
                    Vector2 velocity = Vector2.Normalize(value3);
                    if (float.IsNaN(velocity.X) || float.IsNaN(velocity.Y))
                    {
                        velocity = -Vector2.UnitY;
                    }
                    velocity *= num6;
                    if (velocity.X != projectile.velocity.X || velocity.Y != projectile.velocity.Y)
                    {
                        projectile.netUpdate = true;
                    }
                    base.projectile.velocity = velocity;
                    float scaleFactor = 14f;
                    int num8 = 7;
                    for (int j = 0; j < 2; j++)
                    {
                        value2 = projectile.Center + new Vector2(Main.rand.Next(-num8, num8 + 1), Main.rand.Next(-num8, num8 + 1));
                        Vector2 spinningpoint2 = Vector2.Normalize(projectile.velocity) * scaleFactor;
                        spinningpoint2 = spinningpoint2.RotatedBy(Main.rand.NextDouble() * 0.196 - 0.0984);
                        if (float.IsNaN(spinningpoint2.X) || float.IsNaN(spinningpoint2.Y))
                        {
                            spinningpoint2 = -Vector2.UnitY;
                        }
                        Projectile.NewProjectile(value2.X, value2.Y, spinningpoint2.X, spinningpoint2.Y, ProjectileID.Bullet, projectile.damage, projectile.knockBack, projectile.owner);
                    }
                }
                else
                {
                    projectile.Kill();
                }
            }
>>>>>>> Stashed changes
        }
    }
}