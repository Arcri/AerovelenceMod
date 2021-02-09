using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Melee
{
    public class CryoBall : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cryo Ball");
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
            item.shoot = mod.ProjectileType("CryoBallProjectile");
            item.shootSpeed = 16f;
        }
    }
}

namespace AerovelenceMod.Items.Weapons.Melee
{
    public class CryoBallProjectile : ModProjectile
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
    public class CryoBallProj2 : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 10;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.melee = true;
            projectile.penetrate = 3;
            projectile.timeLeft = 200;
            projectile.light = 0.5f;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;


        }
        public override void AI()
        {
            projectile.velocity *= 0.90f;
            projectile.alpha += 2;
            projectile.scale *= 0.99f;
            if (projectile.alpha <= 0.4)
            {
                projectile.active = false;
            }
            if (Main.rand.NextFloat() < 0.5f)
            {
                int dustIndex = Dust.NewDust(projectile.position, projectile.width, projectile.height, 20, 0f, 0f, 0, default, 1.118421f);
                Dust dust = Main.dust[dustIndex];
                dust.noGravity = true;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }
            projectile.rotation = projectile.velocity.ToRotation();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(Color.LightPink) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
                spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
    }
}