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
            Item.channel = true;		
            Item.crit = 4;
            Item.damage = 29;
            Item.DamageType = DamageClass.Melee;
            Item.width = 34;
            Item.height = 40;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 5;
            Item.value = Item.sellPrice(0, 3, 75, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = false;
            Item.shoot = Mod.Find<ModProjectile>("EyeOfTheGreatMothProj").Type;
            Item.shootSpeed = 16f;
        }
    }

    public class EyeOfTheGreatMothProj : ModProjectile
    {
        private int shootTimer;
        public override void SetDefaults()
        {
            Projectile.extraUpdates = 0;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 99;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 13;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 245f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 16f;
        }
        public override void AI()
        {
            if (Main.rand.Next(10) == 0)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 20, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                dust.scale = 0.50f;
            }
            float distance = 192f;
            bool npcNearby = false;
            for (int k = 0; k < 200; k++)
            {
                if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5 && Main.npc[k].type != NPCID.TargetDummy)
                {
                    Vector2 newMove = Main.npc[k].Center - Projectile.Center;
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
                        int type = Mod.Find<ModProjectile>("CryoBallProj2").Type;
                        Vector2 velocity = new Vector2(speed, speed).RotatedByRandom(MathHelper.ToRadians(360));
                        Projectile.NewProjectile(Projectile.Center, velocity, type, Projectile.damage, 5f, Projectile.owner);
                        shootTimer = 0;

                        if (Main.rand.Next(2) == 0)
                        {
                            Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 20, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                        }
                    }

                }
        }
    }
}