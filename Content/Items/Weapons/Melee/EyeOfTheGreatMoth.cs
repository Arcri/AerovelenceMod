using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class EyeOfTheGreatMoth : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eye of the great Moth");
            Tooltip.SetDefault("Has a chance to explode into electricity when hitting an enemy");
        }
        public override void SetDefaults()
        {
            item.channel = true;		
            item.crit = 4;
            item.damage = 29;
            item.melee = true;
            item.width = 34;
            item.height = 40;
            item.useTime = 24;
            item.useAnimation = 24;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.knockBack = 5;
            item.value = Item.sellPrice(0, 3, 75, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = false;
            item.shoot = mod.ProjectileType("EyeOfTheGreatMothProj");
            item.shootSpeed = 16f;
        }
    }

    public class EyeOfTheGreatMothProj : ModProjectile
    {
        private int shootTimer;
        public override void SetDefaults()
        {
            projectile.extraUpdates = 0;
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = 99;
            projectile.friendly = true;
            projectile.penetrate = -1;
            ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = 13;
            ProjectileID.Sets.YoyosMaximumRange[projectile.type] = 245f;
            ProjectileID.Sets.YoyosTopSpeed[projectile.type] = 16f;
        }
        public override void AI()
        {
            if (Main.rand.Next(10) == 0)
            {
                Dust dust = Dust.NewDustDirect(projectile.position + projectile.velocity, projectile.width, projectile.height, 20, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
                dust.scale = 0.50f;
            }
            float distance = 192f;
            bool npcNearby = false;
            for (int k = 0; k < 200; k++)
            {
                if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5 && Main.npc[k].type != NPCID.TargetDummy)
                {
                    Vector2 newMove = Main.npc[k].Center - projectile.Center;
                    float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                    if (distanceTo < distance)
                    {
                        distance = distanceTo;
                        npcNearby = true;
                    }

                }

            }

            shootTimer++;


            if (shootTimer >= Main.rand.Next(20, 30))
                if (npcNearby)
                {

                    {
                        float speed = 5f;
                        int type = mod.ProjectileType("CryoBallProj2");
                        Vector2 velocity = new Vector2(speed, speed).RotatedByRandom(MathHelper.ToRadians(360));
                        Projectile.NewProjectile(projectile.Center, velocity, type, projectile.damage, 5f, projectile.owner);
                        shootTimer = 0;

                        if (Main.rand.Next(2) == 0)
                        {
                            Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 20, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
                        }
                    }

                }
        }
    }
}