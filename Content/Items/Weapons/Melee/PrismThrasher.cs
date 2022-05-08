using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;

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
            Item.width = 66;
            Item.height = 70;
            Item.useAnimation = 25;
            Item.useTime = 15;
            Item.useStyle = 5;
            Item.rare = ItemRarityID.Green;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.noMelee = true;
            Item.damage = 27;
            Item.knockBack = 4f;
            Item.autoReuse = false;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Melee;
            Item.shoot = Mod.Find<ModProjectile>("PrismSlash").Type;
            Item.value = 40000;
            Item.shootSpeed = 15f;
            Item.UseSound = SoundID.Item109;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }


        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.width = 66;
                Item.height = 70;
                Item.useAnimation = 21;
                Item.useTime = 21;
                Item.useStyle = ItemUseStyleID.Swing;
                Item.rare = ItemRarityID.Green;
                Item.noUseGraphic = false;
                Item.channel = false;
                Item.noMelee = false;
                Item.damage = 40;
                Item.knockBack = 6f;
                Item.autoReuse = true;
                Item.DamageType = DamageClass.Melee;
                Item.shoot = ProjectileID.None;
                Item.UseSound = SoundID.Item4;
            }
            else
            {
                Item.width = 66;
                Item.height = 70;
                Item.useAnimation = 25;
                Item.useTime = 15;
                Item.useStyle = 5;
                Item.rare = ItemRarityID.Green;
                Item.noUseGraphic = true;
                Item.channel = true;
                Item.noMelee = true;
                Item.damage = 25;
                Item.knockBack = 4f;
                Item.autoReuse = false;
                Item.noMelee = true;
                Item.DamageType = DamageClass.Melee;
                Item.shoot = Mod.Find<ModProjectile>("PrismSlash").Type;
                Item.value = 40000;
                Item.shootSpeed = 15f;
                Item.UseSound = SoundID.Item109;
            }
            return base.CanUseItem(player);
        }
    }



    class PrismSlash : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 10;
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

            Projectile.width = 84;
            Projectile.height = 98;
            Projectile.aiStyle = 75;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
        }


        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //3a: target.immune[projectile.owner] = 20;
            //3b: target.immune[projectile.owner] = 5;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            //return Color.White;
            return new Color(255, 255, 255, 0) * (1f - (float)Projectile.alpha / 255f);
        }
        public override void AI()
        {
            float num = (float)Math.PI / 2f;

            Player player = Main.player[Projectile.owner];

            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter);
            num = 0f;
            if (Projectile.spriteDirection == -1)
            {
                num = (int)(float)Math.PI;
            }
            if (++Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
            Projectile.soundDelay--;
            if (Projectile.soundDelay <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
                Projectile.soundDelay = 12;
            }
            if (Main.myPlayer == Projectile.owner)
            {
                if (player.channel && !player.noItems && !player.CCed)
                {
                    float num33 = 1f;
                    if (player.inventory[player.selectedItem].shoot == Projectile.type)
                    {
                        num33 = player.inventory[player.selectedItem].shootSpeed * Projectile.scale;
                    }
                    Vector2 vector8 = Main.MouseWorld - vector;
                    vector8.Normalize();
                    if (vector8.HasNaNs())
                    {
                        vector8 = Vector2.UnitX * player.direction;
                    }
                    vector8 *= num33;
                    if (vector8.X != Projectile.velocity.X || vector8.Y != Projectile.velocity.Y)
                    {
                        Projectile.netUpdate = true;
                    }
                    Projectile.velocity = vector8;
                }
                else
                {
                    Projectile.Kill();
                }
            }
            Vector2 vector9 = Projectile.Center + Projectile.velocity * 3f;
            Lighting.AddLight(vector9, 0.8f, 0.8f, 0.8f);
            if (Main.rand.NextBool(3))
            {
                int num34 = Dust.NewDust(vector9 - Projectile.Size / 2f, Projectile.width, Projectile.height, 63, Projectile.velocity.X, Projectile.velocity.Y, 100, default(Color), 2f);
                Main.dust[num34].noGravity = true;
                Main.dust[num34].position -= Projectile.velocity;
            }
        }


        // Some advanced drawing because the texture image isn't centered or symetrical.
        public override bool PreDraw(ref Color lightColor)
        {
            {
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (Projectile.spriteDirection == -1)
                {
                    spriteEffects = SpriteEffects.FlipHorizontally;
                }
                Texture2D texture = (Texture2D)TextureAssets.Projectile[Projectile.type];
                int frameHeight = TextureAssets.Projectile[Projectile.type].Height() / Main.projFrames[Projectile.type];
                int startY = frameHeight * Projectile.frame;
                Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
                Vector2 origin = sourceRectangle.Size() / 2f;
                origin.X = (float)((Projectile.spriteDirection == 1) ? (sourceRectangle.Width - 40) : 40);

                Color drawColor = Projectile.GetAlpha(lightColor);
                Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

                return false;
            }
        }
    }
}