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
            Item.UseSound = SoundID.Item47;
            Item.crit = 12;
            Item.damage = 45;
            Item.DamageType = DamageClass.Magic;
            Item.width = 46;
            Item.height = 46;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 25, 0, 0);
            Item.rare = ItemRarityID.Cyan;
            Item.autoReuse = true;
            Item.shoot = Mod.Find<ModProjectile>("EnsembleMusic1").Type;
            Item.shootSpeed = 14f;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            List<int> musics = new List<int>();
            musics.Add(Mod.Find<ModProjectile>("EnsembleMusic1").Type);
            musics.Add(Mod.Find<ModProjectile>("EnsembleMusic2").Type);
            musics.Add(Mod.Find<ModProjectile>("EnsembleMusic3").Type);
            float x = (float)Math.Cos(new Random().NextDouble() * 6.283185307179587f) * (float)new Random().NextDouble() * 8;
            float y = (float)Math.Sin(new Random().NextDouble() * 6.283185307179587f) * (float)new Random().NextDouble() * 8;
            int val = Projectile.NewProjectile(player.Center.X, player.Center.Y, x, y, musics[new Random().Next(3)], Item.damage, 0f, Main.myPlayer, 0f, 0f);
            return false;
        }
    }

    public class EnsembleMusic1 : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 520;
        }
        public override void AI()
        {
            Projectile.scale *= (1 + 0.00098095238095238095238095238095f);
            if (Projectile.timeLeft % 30 == 0)
            {
                Projectile.damage += 4;
            }
            float centerX = Projectile.Center.X;
            float centerY = Projectile.Center.Y;
            float minDist = 720f;
            bool chasing = false;
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
                {
                    float centerX2 = Main.npc[i].position.X + (float)(Main.npc[i].width / 2);
                    float centerY2 = Main.npc[i].position.Y + (float)(Main.npc[i].height / 2);
                    float dist = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - centerX2) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - centerY2);
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
                Vector2 vector = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                float xDist = centerX - vector.X;
                float yDist = centerY - vector.Y;
                float distNorm = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
                distNorm = idealVelocity / distNorm;
                xDist *= distNorm;
                yDist *= distNorm;
                Projectile.velocity.X = (Projectile.velocity.X * 20f + xDist) / 21f;
                Projectile.velocity.Y = (Projectile.velocity.Y * 20f + yDist) / 21f;
            }
        }
    }
    public class EnsembleMusic2 : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 520;
        }
        public override void AI()
        {
            Projectile.scale *= (1 + 0.00098095238095238095238095238095f);
            if (Projectile.timeLeft % 30 == 0)
            {
                Projectile.damage += 4;
            }
            float centerX = Projectile.Center.X;
            float centerY = Projectile.Center.Y;
            float minDist = 720f;
            bool chasing = false;
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
                {
                    float centerX2 = Main.npc[i].position.X + (float)(Main.npc[i].width / 2);
                    float centerY2 = Main.npc[i].position.Y + (float)(Main.npc[i].height / 2);
                    float dist = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - centerX2) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - centerY2);
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
                Vector2 vector = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                float xDist = centerX - vector.X;
                float yDist = centerY - vector.Y;
                float distNorm = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
                distNorm = idealVelocity / distNorm;
                xDist *= distNorm;
                yDist *= distNorm;
                Projectile.velocity.X = (Projectile.velocity.X * 20f + xDist) / 21f;
                Projectile.velocity.Y = (Projectile.velocity.Y * 20f + yDist) / 21f;
            }
        }
    }
    public class EnsembleMusic3 : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 520;
        }
        public override void AI()
        {
            Projectile.scale *= (1 + 0.00098095238095238095238095238095f);
            if (Projectile.timeLeft % 30 == 0)
            {
                Projectile.damage += 4;
            }
            float centerX = Projectile.Center.X;
            float centerY = Projectile.Center.Y;
            float minDist = 720f;
            bool chasing = false;
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
                {
                    float centerX2 = Main.npc[i].position.X + (float)(Main.npc[i].width / 2);
                    float centerY2 = Main.npc[i].position.Y + (float)(Main.npc[i].height / 2);
                    float dist = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - centerX2) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - centerY2);
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
                Vector2 vector = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                float xDist = centerX - vector.X;
                float yDist = centerY - vector.Y;
                float distNorm = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
                distNorm = idealVelocity / distNorm;
                xDist *= distNorm;
                yDist *= distNorm;
                Projectile.velocity.X = (Projectile.velocity.X * 20f + xDist) / 21f;
                Projectile.velocity.Y = (Projectile.velocity.Y * 20f + yDist) / 21f;
            }
        }
    }
}