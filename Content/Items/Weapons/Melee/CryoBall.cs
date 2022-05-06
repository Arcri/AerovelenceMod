using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class CryoBall : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cryo Ball");
            Tooltip.SetDefault("Every nine hits releases an icy mist blast");
        }
        public override void SetDefaults()
        {
            Item.channel = true;
            Item.damage = 29;
            Item.DamageType = DamageClass.Melee;
            Item.width = 34;
            Item.height = 40;
            
            Item.useAnimation = Item.useTime = 24;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 5;
            Item.value = Item.sellPrice(0, 3, 75, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<CryoBallProjectile>();
            Item.shootSpeed = 16f;
        }
    }

    internal static class ProjectileExtentions
    {
        public static bool DrawProjectileCenteredWithTexture(this ModProjectile p, Texture2D texture, SpriteBatch spriteBatch, Color lightColor)
        {
            Rectangle frame = texture.Frame(1, Main.projFrames[p.Projectile.type], 0, p.Projectile.frame);
            Vector2 origin = frame.Size() / 2 + new Vector2(p.drawOriginOffsetX, p.drawOriginOffsetY);
            SpriteEffects effects = p.Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 drawPosition = p.Projectile.Center - Main.screenPosition + new Vector2(p.drawOffsetX, 0);

            spriteBatch.Draw(texture, drawPosition, frame, lightColor, p.Projectile.rotation, origin, p.Projectile.scale, effects, 0f);

            return (false);
        }
    }
    public class CryoBallProjectile : ModProjectile
    {

        public override void SetDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 13;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 245f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 16f;

            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;

            Projectile.width = Projectile.height = 16;

            Projectile.aiStyle = 99;

            Projectile.friendly = true;
            Projectile.penetrate = -1;
        }

        int OnHit = 0;
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            OnHit++;
            if (OnHit == 9)
            {
                SoundEngine.PlaySound(SoundID.Item67, (int)Projectile.Center.X, (int)Projectile.Center.Y);

                Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<CryoBallProj2>(), damage, 0, Projectile.owner);
                OnHit = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(Main.projectileTexture[Projectile.type].Width, Main.projectileTexture[Projectile.type].Height);
            Texture2D texture2D = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
            for(int k = 0; k < Projectile.oldPos.Length; k++)
            {
                float scale = Projectile.scale * (Projectile.oldPos.Length - k) / Projectile.oldPos.Length * .35f;
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(-9f, -11.5f);
                Color color = Projectile.GetAlpha(Color.DarkBlue) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
               
                spriteBatch.Draw(texture2D, drawPos, null, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0f);
            }

            return true;
        }
        
        public override void AI()
        {
            if (OnHit >= 3)
            {
                for (int j = 0; j < 10; j++)
                {
                    float t = (float)Main.time * 0.1f;
                    float x = Projectile.position.X - Projectile.velocity.X / 10f * (float)j;
                    float y = Projectile.position.Y - Projectile.velocity.Y / 10f * (float)j;
                    Dust dust = Dust.NewDustDirect(new Vector2(x, y), 1, 1, 20, 0, 0, 0, Color.LightBlue, 0.9f);
                    dust.position.X = x;
                    dust.position.Y = y;
                    dust.velocity *= 0f;
                    dust.noGravity = true;
                    dust.scale = 0.9f;
                    dust.position = Projectile.Center + new Vector2((float)Math.Sin(2 * t) / 2f, -(float)Math.Cos(3 * t) / 3f) * 100f;
                }
            }
            if (OnHit >= 6)
            {
                for (int j = 0; j < 10; j++)
                {
                    float t = (float)Main.time * 0.1f;
                    float x = Projectile.position.X - Projectile.velocity.X / 10f * (float)j;
                    float y = Projectile.position.Y - Projectile.velocity.Y / 10f * (float)j;
                    Dust dust2 = Dust.NewDustDirect(new Vector2(x, y), 1, 1, 20, 0, 0, 0, Color.LightBlue, 0.9f);
                    dust2.position.X = x;
                    dust2.position.Y = y;
                    dust2.velocity *= 0f;
                    dust2.noGravity = true;
                    dust2.scale = 0.9f;
                    dust2.position = Projectile.Center + new Vector2((float)Math.Sin(4 * t) / 2f, -(float)Math.Cos(2 * t) / 3f) * 100f;
                }
            }
        }
    }
    public class CryoBallProj2 : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.None;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 200;

            Projectile.aiStyle = -1;
            Projectile.friendly = Projectile.DamageType = // projectile.ignoreWater = true /* tModPorter - this is redundant, for more info see https://github.com/tModLoader/tModLoader/wiki/Update-Migration-Guide#damage-classes */ ;

            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;

            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;

            Projectile.alpha = 255;
        }
        public override void AI()
        {
            for (int i = 0; i < 16; ++i)
            {
                float randomDust = Main.rand.NextFloat(-4, 4);
                float randomDust2 = Main.rand.NextFloat(4, -4);
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 20, randomDust, randomDust2);
            }

        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 120);
        }       
    } 
}