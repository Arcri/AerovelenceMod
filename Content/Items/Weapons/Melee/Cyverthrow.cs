using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class Cyverthrow : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 48;
            Item.damage = 62;// change this later
            Item.knockBack = 2f;
            Item.DamageType = DamageClass.Melee;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.UseSound = SoundID.Item1;
            Item.shootSpeed = 16f;
            Item.shoot = Mod.Find<ModProjectile>("CyverthrowProj").Type;
        }
    }
    public class CyverthrowProj : ModProjectile
    {
        private int shootTimer;
        private int explosionTimer;
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = 99;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.light = 0.5f;

            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 380f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 16.5f;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cyverthrow");
        }
        public override void AI()
        {

            if (Main.rand.NextFloat() < 0.8289474f)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 164, 0f, 0f, 0, new Color(155, 0, 124), 1.118421f);
                Dust dust = Main.dust[dustIndex];
                dust.shader = GameShaders.Armor.GetSecondaryShader(0, Main.LocalPlayer);
                dust.noGravity = true;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
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
            explosionTimer++;


            if (shootTimer >= Main.rand.Next(7, 19))
                if (npcNearby)
                {

                    {
                        float speed = 5f;
                        int type = Mod.Find<ModProjectile>("CyverthrowBolt").Type;
                        Vector2 velocity = new Vector2(speed, speed).RotatedByRandom(MathHelper.ToRadians(360));
                        Projectile.NewProjectile(Projectile.Center, velocity, type, Projectile.damage, 5f, Projectile.owner);
                        shootTimer = 0;
                    }
                }
            if (explosionTimer >= 200)
            {
                if (npcNearby)
                {

                    for (int i = 0; i < 12; i++)
                    {
                        int dustType = 170;
                        int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType);
                        Dust dust = Main.dust[dustIndex];
                        dust.velocity.X = dust.velocity.X + Main.rand.Next(-10, 10) * 0.01f;
                        dust.velocity.Y = dust.velocity.Y + Main.rand.Next(-65, -48) * 0.01f;
                        dust.scale *= 2f + Main.rand.Next(-30, 31) * 0.01f;
                        dust.noGravity = true;
                        dust.shader = GameShaders.Armor.GetSecondaryShader(29, Main.LocalPlayer);
                    }
                    for (int a = 0; a < 9; a++)
                    {
                        Projectile proj = Main.projectile[Projectile.NewProjectile(Projectile.Center, new Vector2(7, 7).RotatedBy(MathHelper.ToRadians(360 / 8 * a)), ProjectileID.MartianTurretBolt, Projectile.damage, 0f, Projectile.owner)];
                        proj.hostile = false;
                        proj.friendly = true;
                    }
                    explosionTimer = 0;
                }
            }
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Melee/CyverthrowProj_Glow");
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
    public class CyverthrowBolt : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 400;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;


        }
        public override void AI()
        {
            if (Main.rand.NextFloat() < 0.5f)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 164, 0f, 0f, 0, default, 1.118421f);
                Dust dust = Main.dust[dustIndex];
                dust.shader = GameShaders.Armor.GetSecondaryShader(41, Main.LocalPlayer);
                dust.noGravity = true;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(Main.projectileTexture[Projectile.type].Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.LightPink) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                spriteBatch.Draw(Main.projectileTexture[Projectile.type], drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
    }
}


