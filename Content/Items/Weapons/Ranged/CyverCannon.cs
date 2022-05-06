using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class CyverCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cyver-Cannon");
            Tooltip.SetDefault("Fires lasers that split off into homing bolts");
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
            Item.damage = 30;
            Item.shoot = Mod.Find<ModProjectile>("CyverCannonProj").Type;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(0, 5, 20, 0);
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Ranged;
            Item.channel = true;
        }
    }

    public class CyverCannonProj : ModProjectile
    {
        public int Timer;
        public float shootSpeed = 0.5f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
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
            Player player = Main.player[Projectile.owner];
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter);
            {
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
                        SoundEngine.PlaySound(SoundID.Item91, base.Projectile.position);
                    }
                }
                if (Projectile.ai[1] == 1f && Projectile.ai[0] != 1f)
                {
                    Vector2 spinningpoint = Vector2.UnitX * 24f;
                    spinningpoint = spinningpoint.RotatedBy(Projectile.rotation - (float)Math.PI / 2f);
                    Vector2 value = base.Projectile.Center + spinningpoint;
                    for (int i = 0; i < 2; i++)
                    {
                        int num5 = Dust.NewDust(value - Vector2.One * 8f, 16, 16, 135, base.Projectile.velocity.X / 2f, base.Projectile.velocity.Y / 2f, 100);
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
                        if (velocity.X != base.Projectile.velocity.X || velocity.Y != base.Projectile.velocity.Y)
                        {
                            Projectile.netUpdate = true;
                        }
                        base.Projectile.velocity = velocity;
                        float scaleFactor = 14f;
                        int num8 = 7;
                        int DarkLaser = Mod.Find<ModProjectile>("DarkLaser").Type;
                        for (int j = 0; j < 2; j++)
                        {
                            value2 = base.Projectile.Center + new Vector2(Main.rand.Next(-num8, num8 + 1), Main.rand.Next(-num8, num8 + 1));
                            Vector2 spinningpoint2 = Vector2.Normalize(base.Projectile.velocity) * scaleFactor;
                            spinningpoint2 = spinningpoint2.RotatedBy(Main.rand.NextDouble() * 0.19634954631328583 - 0.098174773156642914);
                            if (float.IsNaN(spinningpoint2.X) || float.IsNaN(spinningpoint2.Y))
                            {
                                spinningpoint2 = -Vector2.UnitY;
                            }
                            Projectile.NewProjectile(value2.X, value2.Y, spinningpoint2.X, spinningpoint2.Y, DarkLaser, Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                    }
                    else
                    {
                        Projectile.Kill();
                    }
                }
            }
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ranged/CyverCannonProj_Glow");
            spriteBatch.Draw(
                texture,
                new Vector2
                (
                    Projectile.Center.Y - Main.screenPosition.X,
                    Projectile.Center.X - Main.screenPosition.Y
                ),
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                Projectile.rotation,
                texture.Size(),
                Projectile.scale,
                SpriteEffects.None,
                0f
            );
        }
    }

    public class DarkLaser : ModProjectile
    {
        public int i;
        public int counter = 0;
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 2;
            Projectile.scale = 1f;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void AI()
        {

            int num294 = Main.rand.Next(3, 7);
            for (int num295 = 0; num295 < num294; num295++)
            {
                counter++;
                if (counter >= 17)
                {
                    int num296 = Dust.NewDust(base.Projectile.Center - Projectile.velocity / 2f, 0, 0, 242, 0f, 0f, 100, default(Color), 2.1f);
                    Dust dust105 = Main.dust[num296];
                    Dust dust2 = dust105;
                    dust2.velocity *= 2f;
                    Main.dust[num296].noGravity = true;
                }
            }
            if (Projectile.ai[1] != 1f)
            {
                Projectile.ai[1] = 1f;
                base.Projectile.position += base.Projectile.velocity;
                Projectile.velocity = Projectile.velocity;
            }
            i++;
            if (i % Main.rand.Next(1, 50) == 0)
            {
                Projectile.NewProjectile(Projectile.Center.X + Projectile.velocity.X, Projectile.Center.Y + Projectile.velocity.Y, Projectile.velocity.X - 2f, Projectile.velocity.Y - 2, ModContent.ProjectileType<CannonSplit>(), Projectile.damage, Projectile.knockBack * 0.85f, Projectile.owner, 0f, 0f);
            }
        }
    }

    public class CannonSplit : ModProjectile
    {
        public int i;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.Homing[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.extraUpdates = 2;
            Projectile.scale = 1f;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void AI()
        {
            int count = 0;

            foreach (Projectile proj in Main.projectile.Where(x => x.active && x.whoAmI != Projectile.whoAmI && x.type == Projectile.type))
            {
                count++;

                if (count >= 5)
                    proj.Kill();
            }
            if (Projectile.alpha > 30)
            {
                Projectile.alpha -= 15;
                if (Projectile.alpha < 30)
                {
                    Projectile.alpha = 30;
                }
            }
			
            if (Projectile.localAI[0] == 0f)
            {
                AdjustMagnitude(ref Projectile.velocity);
                Projectile.localAI[0] = 1f;
            }
            Vector2 move = Vector2.Zero;
            float distance = 400f;
            bool target = false;
            for (int k = 0; k < 200; k++)
            {
                if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5 && Main.npc[k].type != NPCID.TargetDummy)
                {
                    Vector2 newMove = Main.npc[k].Center - Projectile.Center;
                    float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                    if (distanceTo < distance)
                    {
                        move = newMove;
                        distance = distanceTo;
                        target = true;
                    }
                }
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            if (target)
            {
                AdjustMagnitude(ref move);
                Projectile.velocity = (5 * Projectile.velocity + move) / 6f;
                AdjustMagnitude(ref Projectile.velocity);
            }
            if (Projectile.alpha <= 30)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height / 2, 242);
                Main.dust[dust].velocity *= 0.1f;
                Main.dust[dust].noGravity = true;
            }
        }

        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 3f)
            {
                vector *= 5f / magnitude;
            }
        }
    }
}