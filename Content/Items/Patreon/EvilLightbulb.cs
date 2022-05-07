using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
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
            Item.crit = 7;
            Item.damage = 16;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 5;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.UseSound = SoundID.Item20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 5, 30, 0);
            Item.rare = ItemRarityID.Purple;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<EvilRay>();
            Item.shootSpeed = 12f;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.useTime = 70;
                Item.useAnimation = 70;
                Item.damage = 55;
                Item.mana = 0;
                Item.shoot = ModContent.ProjectileType<EvilAura>();
                Item.shootSpeed = 0f;
            }
            else
            {
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.useTime = 12;
                Item.useAnimation = 12;
                Item.damage = 10;
                Item.mana = 5;
                Item.shoot = Item.shoot = ModContent.ProjectileType<EvilRay>();
                Item.shootSpeed = 12f;
            }
            return base.CanUseItem(player);
        }

        int i;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
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
            Projectile.width = 220;
            Projectile.height = 220;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.damage = 20;
            Projectile.timeLeft = 500;
            Projectile.extraUpdates = 5;
            Projectile.alpha = 0;
        }

		public override bool PreDraw(ref Color lightColor)
        {
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D auraTex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/YellowGlow");

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            //if (dashing)

            for (int i = 0; i <= 4; i++)
            {
                Main.EntitySpriteDraw(auraTex, Projectile.Center + new Vector2(Main.rand.Next(-3, 4), Main.rand.Next(-3, 4)) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Rectangle(0, 0, auraTex.Width, auraTex.Height), Color.Lerp(new Color(15, 15, 25), Color.Yellow, colorLerp), Projectile.rotation, auraTex.Size() / 2, ((1f + ((float)Math.Cos(cos1 / 12) * 0.1f)) * Glow) + MathHelper.Lerp(colorLerp, colorLerp / 2, Glow), effects, 0);
            }



            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);


            return true;
        }

        public override void AI()
        {
            Glow = MathHelper.Lerp(Glow, 1f, 0.1f);
            Lighting.AddLight(Projectile.Center, Color.CornflowerBlue.ToVector3() / 3);

            DrawPos = Projectile.position;

            for (int i = 0; i < 90; i++)
            {
                Vector2 position = Projectile.Center + new Vector2(0f, -100).RotatedBy(MathHelper.ToRadians(90 - 360f / 90 * i));
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

            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = true;
            Projectile.penetrate = 300;
            Projectile.damage = 50;
            
        }

        public override void AI()
        {
            Projectile.ai[0] += 1f;
            if(Projectile.ai[0] > 3f)
            {
                for (int i = 0; i < 2; i++)
                {
                    Projectile.alpha = 255;
                    Vector2 projectilePosition = Projectile.position;
                    projectilePosition -= Projectile.velocity * (i * 0.25f);
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