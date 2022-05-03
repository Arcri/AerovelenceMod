using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Patreon
{
    public class EvilLightbulb : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Evil Lightbulb");
            Tooltip.SetDefault("Left-click to shoot a small laser\nRight-click to conjure a protective aura");
        }
        public override void SetDefaults()
        {
            item.crit = 7;
            item.damage = 16;
            item.magic = true;
            item.mana = 5;
            item.width = 40;
            item.height = 40;
            item.useTime = 8;
            item.useAnimation = 8;
            item.UseSound = SoundID.Item20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 5, 30, 0);
            item.rare = ItemRarityID.Purple;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<EvilRay>();
            item.shootSpeed = 12f;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.useStyle = ItemUseStyleID.HoldingOut;
                item.useTime = 70;
                item.useAnimation = 70;
                item.damage = 55;
                item.mana = 0;
                item.shoot = ModContent.ProjectileType<EvilAura>();
                item.shootSpeed = 0f;
            }
            else
            {
                item.useStyle = ItemUseStyleID.HoldingOut;
                item.useTime = 12;
                item.useAnimation = 12;
                item.damage = 10;
                item.mana = 5;
                item.shoot = item.shoot = ModContent.ProjectileType<EvilRay>();
                item.shootSpeed = 12f;
            }
            return base.CanUseItem(player);
        }

        int i;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            i++;

            if (player.altFunctionUse == 2)
            {
                for (double i = 0; i < 6.28; i += 0.1)
                {
                    Dust dust = Dust.NewDustPerfect(player.position, 159, new Vector2((float)Math.Sin(i) * 2.6f, (float)Math.Cos(i)) * 4.8f);
                    dust.noGravity = true;
                }
            }

            return true;
        }
    }

    public class EvilAura : ModProjectile
    {


        float colorLerp = 0f;
        float Glow = 0f;
        public Vector2 DrawPos;
        float cos1 = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Evil Aura");
        }

        public override void SetDefaults()
        {
            projectile.width = 220;
            projectile.height = 220;
            projectile.aiStyle = -1;
            projectile.magic = true;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.damage = 20;
            projectile.timeLeft = 500;
            projectile.extraUpdates = 5;
            projectile.alpha = 0;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var effects = projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D auraTex = ModContent.GetTexture("AerovelenceMod/Assets/YellowGlow");

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            //if (dashing)

            for (int i = 0; i <= 4; i++)
            {
                spriteBatch.Draw(auraTex, projectile.Center + new Vector2(Main.rand.Next(-3, 4), Main.rand.Next(-3, 4)) - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Rectangle(0, 0, auraTex.Width, auraTex.Height), Color.Lerp(new Color(15, 15, 25), Color.Yellow, colorLerp), projectile.rotation, auraTex.Size() / 2, ((1f + ((float)Math.Cos(cos1 / 12) * 0.1f)) * Glow) + MathHelper.Lerp(colorLerp, colorLerp / 2, Glow), effects, 0);
            }



            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);


            return true;
        }

        public override void AI()
        {
            Glow = MathHelper.Lerp(Glow, 1f, 0.1f);
            Lighting.AddLight(projectile.Center, Color.CornflowerBlue.ToVector3() / 3);

            DrawPos = projectile.position;

            for (int i = 0; i < 90; i++)
            {
                Vector2 position = projectile.Center + new Vector2(0f, -100).RotatedBy(MathHelper.ToRadians(90 - 360f / 90 * i));
                if (!Collision.SolidCollision(position, 1, 1))
                {
                    Dust dust = Dust.NewDustDirect(position, 1, 1, 159, 0, 0, 128, default, 0.5f);
                    dust.velocity.X *= 0.5f;
                    dust.scale *= 0.99f;
                    dust.velocity.Y *= 0.1f;
                    dust.noGravity = true;
                }
            }
        }
    }

    public class EvilRay : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Evil Ray");
        }
        public override void SetDefaults()
        {

            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.extraUpdates = 100;
            projectile.timeLeft = 300;
            projectile.tileCollide = true;
            projectile.penetrate = 300;
            projectile.damage = 50;
            
        }

        public override void AI()
        {
            projectile.ai[0] += 1f;
            if(projectile.ai[0] > 3f)
            {
                for (int i = 0; i < 2; i++)
                {
                    projectile.alpha = 255;
                    Vector2 projectilePosition = projectile.position;
                    projectilePosition -= projectile.velocity * (i * 0.25f);
                    int dust = Dust.NewDust(projectilePosition, 1, 1, 159, 0f, 0f, 0, default, 1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].position = projectilePosition;
                    Main.dust[dust].scale = Main.rand.Next(70, 110) * 0.013f;
                    Main.dust[dust].velocity *= 0.2f;
                }
            }
        }
    }
}