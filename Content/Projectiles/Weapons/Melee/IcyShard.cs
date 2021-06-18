using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Melee
{
    public class IcyShard : ModProjectile
    {
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(randomModifier1);
            writer.Write(randomModifier2);
            writer.Write(rotationCounter);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            randomModifier1 = reader.ReadSingle();
            randomModifier2 = reader.ReadSingle();
            rotationCounter = reader.ReadInt32();
        }
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 22;
            projectile.alpha = 0;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.alpha = 100;
            projectile.melee = true;
            projectile.timeLeft = 480;
            projectile.alpha = 255;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 origin = new Vector2(texture.Width / 2, projectile.height / 2);
            Color color = Color.Black;
            for (int i = 0; i < 360; i += 60)
            {
                Vector2 circular = new Vector2(length + Main.rand.NextFloat(3.5f, 5), 0).RotatedBy(MathHelper.ToRadians(i + length * 2.5f));
                color = new Color(130, 130, 150, 0);
                Main.spriteBatch.Draw(texture, projectile.Center + circular - Main.screenPosition, null, color * ((255f - projectile.alpha) / 255f), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0.0f);
            }
            color = projectile.GetAlpha(Color.White);
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, color, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0.0f);
            return false;
        }
        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.Center.X, (int)projectile.Center.Y, 50, 0.75f, 0.1f);
            for (int k = 0; k < 2; k++)
            {
                float decrease = 3;
                for (int i = 12; i > 0; i--)
                {
                    Vector2 outwards = new Vector2(0, 1 * (k * 2 - 1)).RotatedBy(MathHelper.ToRadians(i * 12) + projectile.rotation);
                    for (float j = 0; j <= 1; j += 0.2f)
                    {
                        Vector2 spawnAt = projectile.Center;
                        Dust dust = Dust.NewDustDirect(spawnAt - new Vector2(5), 0, 0, ModContent.DustType<WispDust>());
                        dust.velocity = outwards * decrease;
                        dust.noGravity = true;
                        dust.scale *= 0.1f;
                        dust.scale += 1f;
                    }
                    decrease -= 0.25f;
                }
            }
            if (Main.myPlayer == projectile.owner)
                for (int k = 0; k < 3; k++)
                {
                    Vector2 circular = new Vector2(7, 0).RotatedBy(MathHelper.ToRadians(120 * k) + projectile.rotation);
                    Projectile.NewProjectile(projectile.Center, circular, ModContent.ProjectileType<IcyShardBaby>(), (int)projectile.damage, projectile.knockBack, Main.myPlayer);
                }
        }
        float rotationCounter = 0;
        float length = 34;
        bool runOnce = true;
        float randomModifier1 = 0;
        float randomModifier2 = 0;
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            Projectile parent = null;
            for (short i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (parent == null && proj.active && proj.owner == projectile.owner && proj.identity == (int)projectile.ai[0] && proj.type == ModContent.ProjectileType<FrostHydrasThrowProjectile>())
                {
                    parent = proj;
                }
            }
            if (runOnce)
            {
                Main.PlaySound(2, (int)projectile.Center.X, (int)projectile.Center.Y, 30, 0.75f, -0.4f);
                randomModifier1 = Main.rand.NextFloat(-1f, 1.75f);
                randomModifier2 = Main.rand.NextFloat(-24, 24);
                rotationCounter = Main.rand.NextFloat(360);
                runOnce = false;
                if (Main.myPlayer == player.whoAmI)
                    projectile.netUpdate = true;
            }
            if (Main.rand.NextBool(25) && rotationCounter > 10)
            {
                int num1 = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y) - new Vector2(5), 0, 0, DustID.RainbowMk2);
                Dust dust = Main.dust[num1];
                dust.velocity *= 0.7f;
                dust.noGravity = true;
                dust.color = new Color(130, 130, 130, 0);
                dust.fadeIn = 0.2f;
                dust.scale = 1.2f;
            }
            rotationCounter++;
            length -= 1.5f;
            if (length < 0)
                length = 0;
            if (projectile.timeLeft <= 255)
                projectile.alpha++;
            else if(projectile.alpha > 0)
            {
                projectile.alpha -= 15;
            }
            else if (projectile.alpha < 0)
            {

            }
                projectile.alpha = 0;
            if (parent != null)
            {
                float dynamicAddition = (float)(Math.Sin(MathHelper.ToRadians(rotationCounter * (1.5f + randomModifier1))) * 16);
                Projectile owner = parent;
                Vector2 targetProj = owner.Center;
                float greg = rotationCounter * (2 + randomModifier1);
                Vector2 orbitPos = targetProj + new Vector2(72 + dynamicAddition + randomModifier2, 0).RotatedBy(MathHelper.ToRadians(greg));
                Vector2 toOrbit = orbitPos - projectile.Center;
                float speed = 12 + toOrbit.Length() * 0.005f;
                if (speed > toOrbit.Length())
                    speed = toOrbit.Length();
                projectile.velocity = toOrbit.SafeNormalize(Vector2.Zero) * speed;
                projectile.rotation = MathHelper.ToRadians(greg - 30);
            }
            else
            {
                if(projectile.timeLeft > 6)
                    projectile.timeLeft = 6;
            }
        }
    }
}