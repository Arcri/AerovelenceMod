using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class PrismThrasher : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prism Thrasher");
            Tooltip.SetDefault("Slashes through the air");
        }
        public override void SetDefaults()
        {
            item.width = 66;
            item.height = 70;
            item.useAnimation = 25;
            item.useTime = 15;
            item.useStyle = 5;
            item.rare = ItemRarityID.Green;
            item.noUseGraphic = true;
            item.channel = true;
            item.noMelee = true;
            item.damage = 27;
            item.knockBack = 4f;
            item.autoReuse = false;
            item.noMelee = true;
            item.melee = true;
            item.shoot = mod.ProjectileType("PrismSlash");
            item.value = 40000;
            item.shootSpeed = 15f;
            item.UseSound = SoundID.Item109;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }


        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.width = 66;
                item.height = 70;
                item.useAnimation = 21;
                item.useTime = 21;
                item.useStyle = ItemUseStyleID.SwingThrow;
                item.rare = ItemRarityID.Green;
                item.noUseGraphic = false;
                item.channel = false;
                item.noMelee = false;
                item.damage = 40;
                item.knockBack = 6f;
                item.autoReuse = true;
                item.melee = true;
                item.shoot = ProjectileID.None;
                item.UseSound = SoundID.Item4;
            }
            else
            {
                item.width = 66;
                item.height = 70;
                item.useAnimation = 25;
                item.useTime = 15;
                item.useStyle = 5;
                item.rare = ItemRarityID.Green;
                item.noUseGraphic = true;
                item.channel = true;
                item.noMelee = true;
                item.damage = 25;
                item.knockBack = 4f;
                item.autoReuse = false;
                item.noMelee = true;
                item.melee = true;
                item.shoot = mod.ProjectileType("PrismSlash");
                item.value = 40000;
                item.shootSpeed = 15f;
                item.UseSound = SoundID.Item109;
            }
            return base.CanUseItem(player);
        }
    }



    class PrismSlash : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 10;
        }

        public override void SetDefaults()
        {
            /*projectile.width = 84;
            projectile.height = 98;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
           // projectile.hide = true;
            projectile.ownerHitCheck = true; //so you can't hit enemies through walls
            projectile.melee = true;*/

            projectile.width = 84;
            projectile.height = 98;
            projectile.aiStyle = 75;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.ownerHitCheck = true;
        }

        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //3a: target.immune[projectile.owner] = 20;
            //3b: target.immune[projectile.owner] = 5;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            //return Color.White;
            return new Color(255, 255, 255, 0) * (1f - (float)projectile.alpha / 255f);
        }
        public override void AI()
        {
            float num = (float)Math.PI / 2f;

            Player player = Main.player[projectile.owner];
            
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter);
            num = 0f;
            if (projectile.spriteDirection == -1)
            {
                num = (int)(float)Math.PI;
            }
            if (++projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
            projectile.soundDelay--;
            if (projectile.soundDelay <= 0)
            {
                Main.PlaySound(SoundID.Item1, projectile.Center);
                projectile.soundDelay = 12;
            }
            if (Main.myPlayer == projectile.owner)
            {
                if (player.channel && !player.noItems && !player.CCed)
                {
                    float num33 = 1f;
                    if (player.inventory[player.selectedItem].shoot == projectile.type)
                    {
                        num33 = player.inventory[player.selectedItem].shootSpeed * projectile.scale;
                    }
                    Vector2 vector8 = Main.MouseWorld - vector;
                    vector8.Normalize();
                    if (vector8.HasNaNs())
                    {
                        vector8 = Vector2.UnitX * player.direction;
                    }
                    vector8 *= num33;
                    if (vector8.X != projectile.velocity.X || vector8.Y != projectile.velocity.Y)
                    {
                        projectile.netUpdate = true;
                    }
                    projectile.velocity = vector8;
                }
                else
                {
                    projectile.Kill();
                }
            }
            Vector2 vector9 = projectile.Center + projectile.velocity * 3f;
            Lighting.AddLight(vector9, 0.8f, 0.8f, 0.8f);
            if (Main.rand.Next(3) == 0)
            {
                int num34 = Dust.NewDust(vector9 - projectile.Size / 2f, projectile.width, projectile.height, 63, projectile.velocity.X, projectile.velocity.Y, 100, default(Color), 2f);
                Main.dust[num34].noGravity = true;
                Main.dust[num34].position -= projectile.velocity;
            }
        }

        // Some advanced drawing because the texture image isn't centered or symetrical.
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Texture2D texture = Main.projectileTexture[projectile.type];
            int frameHeight = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int startY = frameHeight * projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            origin.X = (float)((projectile.spriteDirection == 1) ? (sourceRectangle.Width - 40) : 40);

            Color drawColor = projectile.GetAlpha(lightColor);
            Main.spriteBatch.Draw(texture,
            projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY),
            sourceRectangle, drawColor, projectile.rotation, origin, projectile.scale, spriteEffects, 0f);

            return false;
        }
    }
}