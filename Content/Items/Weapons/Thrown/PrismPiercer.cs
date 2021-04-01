using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Thrown
{
    public class PrismPiercer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prism Piercer");
            Tooltip.SetDefault("Fires knives that electrify enemies");
        }

        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item1;
            item.crit = 16;
            item.damage = 24;
            item.melee = true;
            item.width = 22;
            item.height = 40;
            item.useTime = 17;
            item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Green;
            item.autoReuse = true;
            item.noUseGraphic = true;
            item.shoot = mod.ProjectileType("PrismPiercerProjectile");
            item.shootSpeed = 16f;
        }
    }

    public class PrismPiercerProjectile : ModProjectile
    {
        public bool e;
        public float rot = 0.5f;
        public int i;
        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.height = 38;
            projectile.melee = true;
            projectile.timeLeft = 120;
            projectile.damage = 16;
            projectile.penetrate = -1;
            projectile.light = 0.5f;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 132, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
            Main.PlaySound(SoundID.Item10);
            return true;
        }
        public override void AI()
        {
            i++;
            if (i % 2 == 0)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width / 2, projectile.height / 2, 132);
                Main.dust[dust].noGravity = true;
            }
            projectile.alpha += 2;
            projectile.rotation += rot;
            rot *= 0.99f;
            if (projectile.ai[0] == 0f)
            {
                projectile.ai[0] = projectile.velocity.X;
                projectile.ai[1] = projectile.velocity.Y;
            }
            if (Math.Sqrt(projectile.velocity.X * projectile.velocity.X + projectile.velocity.Y * projectile.velocity.Y) > 2.0)
            {
                projectile.velocity *= 0.99f;
            }
            int[] array = new int[20];
            int num438 = 0;
            float num439 = 300f;
            bool flag14 = false;
            float num440 = 0f;
            float num441 = 0f;
            for (int num442 = 0; num442 < 200; num442++)
            {
                if (!Main.npc[num442].CanBeChasedBy(this))
                {
                    continue;
                }
                float num443 = Main.npc[num442].position.X + Main.npc[num442].width / 2;
                float num444 = Main.npc[num442].position.Y + Main.npc[num442].height / 2;
                float num445 = Math.Abs(projectile.position.X + projectile.width / 2 - num443) + Math.Abs(projectile.position.Y + projectile.height / 2 - num444);
                if (num445 < num439 && Collision.CanHit(projectile.Center, 1, 1, Main.npc[num442].Center, 1, 1))
                {
                    if (num438 < 20)
                    {
                        array[num438] = num442;
                        num438++;
                        num440 = num443;
                        num441 = num444;
                    }
                    flag14 = true;
                }
            }
            if (projectile.timeLeft < 30)
            {
                flag14 = false;
            }
            if (flag14)
            {
                int num446 = Main.rand.Next(num438);
                num446 = array[num446];
                num440 = Main.npc[num446].position.X + Main.npc[num446].width / 2;
                num441 = Main.npc[num446].position.Y + Main.npc[num446].height / 2;
                projectile.localAI[0] += 1f;
                if (projectile.localAI[0] > 8f)
                {
                    projectile.localAI[0] = 0f;
                    float num447 = 6f;
                    Vector2 vector31 = new Vector2(projectile.position.X + projectile.width * 0.5f, projectile.position.Y + projectile.height * 0.5f);
                    vector31 += projectile.velocity * 4f;
                    float num448 = num440 - vector31.X;
                    float num449 = num441 - vector31.Y;
                    float num450 = (float)Math.Sqrt(num448 * num448 + num449 * num449);
                    float num451 = num450;
                    num450 = num447 / num450;
                    num448 *= num450;
                    num449 *= num450;
                    Projectile.NewProjectile(vector31.X, vector31.Y, num448, num449, ModContent.ProjectileType<PrismPiercerProjectile2>(), projectile.damage, projectile.knockBack, projectile.owner);
                }
            }
        }
    }

    public class PrismPiercerProjectile2 : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.damage = 2;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.extraUpdates = 100;
            projectile.timeLeft = 100;
        }
        public override void AI()
        {
            for (int num452 = 0; num452 < 4; num452++)
            {
                Vector2 position = projectile.position;
                position -= projectile.velocity * (num452 * 0.25f);
                projectile.alpha = 255;
                int num453 = Dust.NewDust(position, 1, 1, 160);
                Main.dust[num453].position = position;
                Main.dust[num453].position.X += projectile.width / 2;
                Main.dust[num453].position.Y += projectile.height / 2;
                Main.dust[num453].scale = Main.rand.Next(70, 110) * 0.013f;
                Dust dust77 = Main.dust[num453];
                Dust dust2 = dust77;
                dust2.velocity *= 0.2f;
            }
            return;
        }
    }
}