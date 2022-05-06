
using Microsoft.Xna.Framework;
using System;
using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class Zodiac : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Zodiac");
            Tooltip.SetDefault("Fires two bullets at once");
        }
        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 2;
            Item.useTime = 2;
            Item.shootSpeed = 20f;
            Item.knockBack = 2f;
            Item.width = 70;
            Item.height = 38;
            Item.damage = 30;
            Item.shoot = ModContent.ProjectileType<ZodiacHeld>();
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(0, 5, 20, 0);
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Ranged;
            Item.channel = true;
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

        }
    }

    public class ZodiacHeld : ModProjectile
    {
        public int Timer;
        public float shootSpeed = 0.25f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 70;
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
            // TODO: Make width work lmao wtf
            Player player = Main.player[Projectile.owner];
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter);

            Projectile.ai[0] += 1f;
            int num2 = 0;
            if (Projectile.ai[0] >= 40f)
            {
                num2++;
            }
            if (Projectile.ai[0] >= 80f)
            {
                num2++;
            }
            if (Projectile.ai[0] >= 120f)
            {
                num2++;
            }
            int num3 = 24;
            int num4 = 6;
            Projectile.ai[1] += 1f;
            bool flag = false;
            if (Projectile.ai[1] >= (float)(num3 - num4 * num2))
            {
                Projectile.ai[1] = 0f;
                flag = true;
            }
            Projectile.frameCounter += 1 + num2;
            if (Projectile.frameCounter >= 10)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 2)
                {
                    Projectile.frame = 0;
                }
            }
            if (Projectile.soundDelay <= 0)
            {
                Projectile.soundDelay = num3 - num4 * num2;
                if (Projectile.ai[0] != 1f)
                {
                    SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                }
            }
            if (Projectile.ai[1] == 1f && Projectile.ai[0] != 1f)
            {
                Vector2 spinningpoint = Vector2.UnitX * 24f;
                spinningpoint = spinningpoint.RotatedBy(Projectile.rotation - (float)Math.PI / 2f);
                Vector2 value = Projectile.Center + spinningpoint;
                for (int i = 0; i < 2; i++)
                {
                    int num5 = Dust.NewDust(value - Vector2.One * 8f, 16, 16, 135, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f, 100);
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
                        value3.Y = (Main.screenHeight - Main.mouseY) + Main.screenPosition.Y - value2.Y;
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
                    base.Projectile.velocity = velocity;
                    float scaleFactor = 14f;
                    int num8 = 7;
                    for (int j = 0; j < 2; j++)
                    {
                        value2 = Projectile.Center + new Vector2(Main.rand.Next(-num8, num8 + 1), Main.rand.Next(-num8, num8 + 1));
                        Vector2 spinningpoint2 = Vector2.Normalize(Projectile.velocity) * scaleFactor;
                        spinningpoint2 = spinningpoint2.RotatedBy(Main.rand.NextDouble() * 0.196 - 0.0984);
                        if (float.IsNaN(spinningpoint2.X) || float.IsNaN(spinningpoint2.Y))
                        {
                            spinningpoint2 = -Vector2.UnitY;
                        }
                        Projectile.NewProjectile(value2.X, value2.Y, spinningpoint2.X, spinningpoint2.Y, ProjectileID.Bullet, Projectile.damage, Projectile.knockBack, Projectile.owner);
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