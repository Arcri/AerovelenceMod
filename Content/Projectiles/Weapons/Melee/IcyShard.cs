using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;

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
            Projectile.width = 10;
            Projectile.height = 22;
            Projectile.alpha = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 100;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 480;
            Projectile.alpha = 255;
        }
		public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = (Texture2D)TextureAssets.Projectile[Projectile.type];
            Vector2 origin = new Vector2(texture.Width / 2, Projectile.height / 2);
            Color color = Color.Black;
            for (int i = 0; i < 360; i += 60)
            {
                Vector2 circular = new Vector2(length + Main.rand.NextFloat(3.5f, 5), 0).RotatedBy(MathHelper.ToRadians(i + length * 2.5f));
                color = new Color(130, 130, 150, 0);
                Main.EntitySpriteDraw(texture, Projectile.Center + circular - Main.screenPosition, null, color * ((255f - Projectile.alpha) / 255f), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }
            color = Projectile.GetAlpha(Color.White);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(2, (int)Projectile.Center.X, (int)Projectile.Center.Y, 50, 0.75f, 0.1f);
            for (int k = 0; k < 2; k++)
            {
                float decrease = 3;
                for (int i = 12; i > 0; i--)
                {
                    Vector2 outwards = new Vector2(0, 1 * (k * 2 - 1)).RotatedBy(MathHelper.ToRadians(i * 12) + Projectile.rotation);
                    for (float j = 0; j <= 1; j += 0.2f)
                    {
                        Vector2 spawnAt = Projectile.Center;
                        Dust dust = Dust.NewDustDirect(spawnAt - new Vector2(5), 0, 0, ModContent.DustType<WispDust>());
                        dust.velocity = outwards * decrease;
                        dust.noGravity = true;
                        dust.scale *= 0.1f;
                        dust.scale += 1f;
                    }
                    decrease -= 0.25f;
                }
            }
            if (Main.myPlayer == Projectile.owner)
                for (int k = 0; k < 3; k++)
                {
                    Vector2 circular = new Vector2(7, 0).RotatedBy(MathHelper.ToRadians(120 * k) + Projectile.rotation);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, circular, ModContent.ProjectileType<IcyShardBaby>(), (int)Projectile.damage, Projectile.knockBack, Main.myPlayer);
                }
        }
        float rotationCounter = 0;
        float length = 34;
        bool runOnce = true;
        float randomModifier1 = 0;
        float randomModifier2 = 0;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile parent = null;
            for (short i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (parent == null && proj.active && proj.owner == Projectile.owner && proj.identity == (int)Projectile.ai[0] && proj.type == ModContent.ProjectileType<FrostHydrasThrowProjectile>())
                {
                    parent = proj;
                }
            }
            if (runOnce)
            {
                SoundEngine.PlaySound(2, (int)Projectile.Center.X, (int)Projectile.Center.Y, 30, 0.75f, -0.4f);
                randomModifier1 = Main.rand.NextFloat(-1f, 1.75f);
                randomModifier2 = Main.rand.NextFloat(-24, 24);
                rotationCounter = Main.rand.NextFloat(360);
                runOnce = false;
                if (Main.myPlayer == player.whoAmI)
                    Projectile.netUpdate = true;
            }
            if (Main.rand.NextBool(25) && rotationCounter > 10)
            {
                int num1 = Dust.NewDust(new Vector2(Projectile.Center.X, Projectile.Center.Y) - new Vector2(5), 0, 0, DustID.RainbowMk2);
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
            if (Projectile.timeLeft <= 255)
                Projectile.alpha++;
            else if(Projectile.alpha > 0)
            {
                Projectile.alpha -= 15;
            }
            else if (Projectile.alpha < 0)
            {

            }
                Projectile.alpha = 0;
            if (parent != null)
            {
                float dynamicAddition = (float)(Math.Sin(MathHelper.ToRadians(rotationCounter * (1.5f + randomModifier1))) * 16);
                Projectile owner = parent;
                Vector2 targetProj = owner.Center;
                float greg = rotationCounter * (2 + randomModifier1);
                Vector2 orbitPos = targetProj + new Vector2(72 + dynamicAddition + randomModifier2, 0).RotatedBy(MathHelper.ToRadians(greg));
                Vector2 toOrbit = orbitPos - Projectile.Center;
                float speed = 12 + toOrbit.Length() * 0.005f;
                if (speed > toOrbit.Length())
                    speed = toOrbit.Length();
                Projectile.velocity = toOrbit.SafeNormalize(Vector2.Zero) * speed;
                Projectile.rotation = MathHelper.ToRadians(greg - 30);
            }
            else
            {
                if(Projectile.timeLeft > 6)
                    Projectile.timeLeft = 6;
            }
        }
    }
}