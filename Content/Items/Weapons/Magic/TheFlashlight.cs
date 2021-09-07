using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class TheFlashlight : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Flashlight");
            Tooltip.SetDefault("Fires quick beams of light");
            Item.staff[item.type] = true;
        }
        public override void SetDefaults()
        {
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useAnimation = 20;
            item.useTime = 20;
            item.shootSpeed = 20f;
            item.knockBack = 2f;
            item.width = 70;
            item.height = 38;
            item.damage = 22;
            item.shoot = mod.ProjectileType("TheFlashlightProj");
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(0, 5, 20, 0);
            item.noMelee = true;
            item.noUseGraphic = true;
            item.magic = true;
            item.channel = true;
            item.mana = 2;
        }
    }
    class FlashlightProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.extraUpdates = 100;
            projectile.timeLeft = 300;
            projectile.tileCollide = true;
            projectile.penetrate = 300;
        }
        public override string Texture { get { return "Terraria/Projectile_" + ProjectileID.ShadowBeamFriendly; } }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            projectile.damage = (int)(projectile.damage * 0.8);
        }

        public override void AI()
        {
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 3f)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 projectilePosition = projectile.position;
                    projectilePosition -= projectile.velocity * ((float)i * 0.25f);
                    projectile.alpha = 255;
                    int dust = Dust.NewDust(projectilePosition, 1, 1, 159, 0f, 0f, 0, default, 1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].position = projectilePosition;
                    Main.dust[dust].scale = (float)Main.rand.Next(70, 110) * 0.013f;
                    Main.dust[dust].velocity *= 0.2f;
                }
            }
        }
    }
}
namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class TheFlashlightProj : ModProjectile
    {
        public int Timer;
        public float shootSpeed = 0.5f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Flashlight");
            Main.projFrames[projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 30;
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
            Player player = Main.player[projectile.owner];
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter);
            {
                projectile.ai[0] += 1f;
                int num2 = 0;
                if (projectile.ai[0] >= 60f)
                {
                    num2++;
                }
                if (projectile.ai[0] >= 120f)
                {
                    num2++;
                }
                if (projectile.ai[0] >= 180f)
                {
                    num2++;
                }
                int num3 = 24;
                int num4 = 6;
                projectile.ai[1] += 1f;
                bool flag = false;
                if (projectile.ai[1] >= num3 - num4 * num2)
                {
                    projectile.ai[1] = 0f;
                    flag = true;
                }
                projectile.frameCounter += 1 + num2;
                if (projectile.frameCounter >= 4)
                {
                    projectile.frameCounter = 0;
                    projectile.frame++;
                    if (projectile.frame >= 6)
                    {
                        projectile.frame = 0;
                    }
                }
                if (projectile.soundDelay <= 0)
                {
                    projectile.soundDelay = num3 - num4 * num2;
                    if (projectile.ai[0] != 1f)
                    {
                        Main.PlaySound(SoundID.Item91, projectile.position);
                    }
                }
                if (projectile.ai[1] == 1f && projectile.ai[0] != 1f)
                {
                    Vector2 spinningpoint = Vector2.UnitX * 24f;
                    spinningpoint = spinningpoint.RotatedBy(projectile.rotation - (float)Math.PI / 2f);
                    Vector2 value = projectile.Center + spinningpoint;
                    for (int i = 0; i < 2; i++)
                    {
                        int num5 = Dust.NewDust(value - Vector2.One * 8f, 16, 16, 159, projectile.velocity.X / 2f, projectile.velocity.Y / 2f, 100);
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
                            value3.Y = (float)(Main.screenHeight - Main.mouseY) + Main.screenPosition.Y - value2.Y;
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
                        projectile.velocity = velocity;
                        float scaleFactor = 14f;
                        int num8 = 2;
                        int FlashlightProj = ModContent.ProjectileType<FlashlightProjectile>();
                        for (int j = 0; j < 1; j++)
                        {
                            value2 = projectile.Center + new Vector2(Main.rand.Next(-num8, num8 + 1), Main.rand.Next(-num8, num8 + 1));
                            Vector2 spinningpoint2 = Vector2.Normalize(projectile.velocity) * scaleFactor;
                            spinningpoint2 = spinningpoint2.RotatedBy(Main.rand.NextDouble() * 0.19634954631328583 - 0.098174773156642914);
                            if (float.IsNaN(spinningpoint2.X) || float.IsNaN(spinningpoint2.Y))
                            {
                                spinningpoint2 = -Vector2.UnitY;
                            }
                            Projectile.NewProjectile(value2.X, value2.Y, spinningpoint2.X, spinningpoint2.Y, FlashlightProj, projectile.damage, projectile.knockBack, projectile.owner);
                        }
                    }
                    else
                    {
                        projectile.Kill();
                    }
                }
            }
        }
    }
}