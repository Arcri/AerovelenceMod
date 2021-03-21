using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class Ensemble : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Ensemble");
        }
        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item47;
            item.crit = 12;
            item.damage = 45;
            item.magic = true;
            item.width = 46;
            item.height = 46;
            item.useTime = 2;
            item.useAnimation = 2;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 25, 0, 0);
            item.rare = ItemRarityID.Cyan;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("EnsembleMusic1");
            item.shootSpeed = 14f;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            List<int> musics = new List<int>();
            musics.Add(mod.ProjectileType("EnsembleMusic1"));
            musics.Add(mod.ProjectileType("EnsembleMusic2"));
            musics.Add(mod.ProjectileType("EnsembleMusic3"));
            float x = (float)Math.Cos(new Random().NextDouble() * 6.283185307179587f) * (float)new Random().NextDouble() * 8;
            float y = (float)Math.Sin(new Random().NextDouble() * 6.283185307179587f) * (float)new Random().NextDouble() * 8;
            int val = Projectile.NewProjectile(player.Center.X, player.Center.Y, x, y, musics[new Random().Next(3)], item.damage, 0f, Main.myPlayer, 0f, 0f);
            return false;
        }
    }

    public class EnsembleMusic1 : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.hostile = false;
            projectile.magic = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 520;
        }
        public override void AI()
        {
            projectile.scale *= (1 + 0.00098095238095238095238095238095f);
            if (projectile.timeLeft % 30 == 0)
            {
                projectile.damage += 4;
            }
            float centerX = projectile.Center.X;
            float centerY = projectile.Center.Y;
            float minDist = 720f;
            bool chasing = false;
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
                {
                    float centerX2 = Main.npc[i].position.X + (float)(Main.npc[i].width / 2);
                    float centerY2 = Main.npc[i].position.Y + (float)(Main.npc[i].height / 2);
                    float dist = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - centerX2) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - centerY2);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        centerX = centerX2;
                        centerY = centerY2;
                        chasing = true;
                    }
                }
            }
            if (chasing)
            {
                float idealVelocity = 45f;
                Vector2 vector = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                float xDist = centerX - vector.X;
                float yDist = centerY - vector.Y;
                float distNorm = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
                distNorm = idealVelocity / distNorm;
                xDist *= distNorm;
                yDist *= distNorm;
                projectile.velocity.X = (projectile.velocity.X * 20f + xDist) / 21f;
                projectile.velocity.Y = (projectile.velocity.Y * 20f + yDist) / 21f;
            }
        }
    }
    public class EnsembleMusic2 : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.hostile = false;
            projectile.magic = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 520;
        }
        public override void AI()
        {
            projectile.scale *= (1 + 0.00098095238095238095238095238095f);
            if (projectile.timeLeft % 30 == 0)
            {
                projectile.damage += 4;
            }
            float centerX = projectile.Center.X;
            float centerY = projectile.Center.Y;
            float minDist = 720f;
            bool chasing = false;
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
                {
                    float centerX2 = Main.npc[i].position.X + (float)(Main.npc[i].width / 2);
                    float centerY2 = Main.npc[i].position.Y + (float)(Main.npc[i].height / 2);
                    float dist = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - centerX2) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - centerY2);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        centerX = centerX2;
                        centerY = centerY2;
                        chasing = true;
                    }
                }
            }
            if (chasing)
            {
                float idealVelocity = 45f;
                Vector2 vector = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                float xDist = centerX - vector.X;
                float yDist = centerY - vector.Y;
                float distNorm = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
                distNorm = idealVelocity / distNorm;
                xDist *= distNorm;
                yDist *= distNorm;
                projectile.velocity.X = (projectile.velocity.X * 20f + xDist) / 21f;
                projectile.velocity.Y = (projectile.velocity.Y * 20f + yDist) / 21f;
            }
        }
    }
    public class EnsembleMusic3 : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.hostile = false;
            projectile.magic = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 520;
        }
        public override void AI()
        {
            projectile.scale *= (1 + 0.00098095238095238095238095238095f);
            if (projectile.timeLeft % 30 == 0)
            {
                projectile.damage += 4;
            }
            float centerX = projectile.Center.X;
            float centerY = projectile.Center.Y;
            float minDist = 720f;
            bool chasing = false;
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
                {
                    float centerX2 = Main.npc[i].position.X + (float)(Main.npc[i].width / 2);
                    float centerY2 = Main.npc[i].position.Y + (float)(Main.npc[i].height / 2);
                    float dist = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - centerX2) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - centerY2);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        centerX = centerX2;
                        centerY = centerY2;
                        chasing = true;
                    }
                }
            }
            if (chasing)
            {
                float idealVelocity = 45f;
                Vector2 vector = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                float xDist = centerX - vector.X;
                float yDist = centerY - vector.Y;
                float distNorm = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
                distNorm = idealVelocity / distNorm;
                xDist *= distNorm;
                yDist *= distNorm;
                projectile.velocity.X = (projectile.velocity.X * 20f + xDist) / 21f;
                projectile.velocity.Y = (projectile.velocity.Y * 20f + yDist) / 21f;
            }
        }
    }
}