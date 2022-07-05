using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class TheFlashlight : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Flashlight");
            Tooltip.SetDefault("Fires quick beams of light\n'Have you tried turning it off and on again?'");
            Item.staff[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.shootSpeed = 20f;
            Item.knockBack = 2f;
            Item.width = 70;
            Item.height = 38;
            Item.damage = 22;
            Item.shoot = Mod.Find<ModProjectile>("TheFlashlightProj").Type;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 3, 20, 0);
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Magic;
            Item.channel = true;
            Item.mana = 2;
        }
    }
    class FlashlightProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = true;
            Projectile.penetrate = 300;
        }
        public override string Texture { get { return "Terraria/Images/Projectile_" + ProjectileID.ShadowBeamFriendly; } }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Projectile.damage = (int)(Projectile.damage * 0.8);
        }

        public override void AI()
        {
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 3f)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 projectilePosition = Projectile.position;
                    projectilePosition -= Projectile.velocity * ((float)i * 0.25f);
                    Projectile.alpha = 255;
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
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 30;
            Projectile.aiStyle = 75;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter);
            {
                Projectile.ai[0] += 1f;
                int num2 = 0;
                if (Projectile.ai[0] >= 60f)
                {
                    num2++;
                }
                if (Projectile.ai[0] >= 120f)
                {
                    num2++;
                }
                if (Projectile.ai[0] >= 180f)
                {
                    num2++;
                }
                int num3 = 24;
                int num4 = 6;
                Projectile.ai[1] += 1f;
                bool flag = false;
                if (Projectile.ai[1] >= num3 - num4 * num2)
                {
                    Projectile.ai[1] = 0f;
                    flag = true;
                }
                Projectile.frameCounter += 1 + num2;
                if (Projectile.frameCounter >= 4)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= 6)
                    {
                        Projectile.frame = 0;
                    }
                }
                if (Projectile.soundDelay <= 0)
                {
                    Projectile.soundDelay = num3 - num4 * num2;
                    if (Projectile.ai[0] != 1f)
                    {
                        SoundEngine.PlaySound(SoundID.Item91, Projectile.position);
                    }
                }
                if (Projectile.ai[1] == 1f && Projectile.ai[0] != 1f)
                {
                    Vector2 spinningpoint = Vector2.UnitX * 24f;
                    spinningpoint = spinningpoint.RotatedBy(Projectile.rotation - (float)Math.PI / 2f);
                    Vector2 value = Projectile.Center + spinningpoint;
                    for (int i = 0; i < 2; i++)
                    {
                        int num5 = Dust.NewDust(value - Vector2.One * 8f, 16, 16, 159, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f, 100);
                        Main.dust[num5].velocity *= 0.66f;
                        Main.dust[num5].noGravity = true;
                        Main.dust[num5].scale = 1.4f;
                    }
                }
                if (flag && Main.myPlayer == Projectile.owner)
                {
                    if (player.channel && player.CheckMana(player.inventory[player.selectedItem], -1, pay: true) && !player.noItems && !player.CCed)
                    {
                        float num6 = player.inventory[player.selectedItem].shootSpeed * Projectile.scale;
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
                        if (velocity.X != Projectile.velocity.X || velocity.Y != Projectile.velocity.Y)
                        {
                            Projectile.netUpdate = true;
                        }
                        Projectile.velocity = velocity;
                        float scaleFactor = 14f;
                        int num8 = 2;
                        int FlashlightProj = ModContent.ProjectileType<FlashlightProjectile>();
                        for (int j = 0; j < 1; j++)
                        {
                            value2 = Projectile.Center + new Vector2(Main.rand.Next(-num8, num8 + 1), Main.rand.Next(-num8, num8 + 1));
                            Vector2 spinningpoint2 = Vector2.Normalize(Projectile.velocity) * scaleFactor;
                            spinningpoint2 = spinningpoint2.RotatedBy(Main.rand.NextDouble() * 0.19634954631328583 - 0.098174773156642914);
                            if (float.IsNaN(spinningpoint2.X) || float.IsNaN(spinningpoint2.Y))
                            {
                                spinningpoint2 = -Vector2.UnitY;
                            }
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), value2.X, value2.Y, spinningpoint2.X, spinningpoint2.Y, FlashlightProj, Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                    }
                    else
                    {
                        Projectile.Kill();
                    }
                }
            }
        }
    }
}