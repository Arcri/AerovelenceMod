using AerovelenceMod.Content.Projectiles.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class DeepFreeze : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deep Freeze");
        }
        public override void SetDefaults()
        {
            Item.damage = 35;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 5;
            Item.width = 70;
            Item.height = 44;
            Item.useTime = Item.useAnimation = 30;
            Item.UseSound = SoundID.Item13;
            Item.useStyle = 5;
            Item.shootSpeed = 17f;
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<DeepFreezeProjectile>();
            Item.value = Item.sellPrice(gold: 10);
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-12, 0);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var line = new TooltipLine(Mod, "Verbose:RemoveMe", "Maintain Homeostasis");
            tooltips.Add(line);

            line = new TooltipLine(Mod, "Book of Bees", "Artifact Weapon")
            {
                overrideColor = new Color(255, 241, 000)
            };
            tooltips.Add(line);
            foreach (TooltipLine line2 in tooltips)
            {
                if (line2.Mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.OverrideColor = new Color(255, 132, 000);
                }
            }
            tooltips.RemoveAll(l => l.Name.EndsWith(":RemoveMe"));
        }
    }
}


namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class DeepFreezeProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rainbow Blast");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            // Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width, Main.projectileTexture[projectile.type].Height);
            Texture2D texture2D = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
            Vector2 origin = new Vector2(texture2D.Width / 2, texture2D.Height / 2);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                float scale = Projectile.scale * (Projectile.oldPos.Length - k) / Projectile.oldPos.Length * 1.0f;
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + Main.projectileTexture[Projectile.type].Size() / 3f;
                Color color = Projectile.GetAlpha(fetchRainbow()) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                for (int i = 0; i < 6; i++)
                {
                    if (i == 0)
                        spriteBatch.Draw(texture2D, drawPos, null, color, Projectile.rotation, origin, scale, SpriteEffects.None, 0f);
                    spriteBatch.Draw(texture2D, drawPos + new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)), null, Color.White.MultiplyRGBA(color * 0.5f), Projectile.rotation, origin, scale * 0.6f, SpriteEffects.None, 0f);
                }
            }
            return false;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.aiStyle = -1;
            Projectile.scale = 0.8f;
            Projectile.timeLeft = 720;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
        }
        public Color fetchRainbow()
        {
            float sin1 = (float)Math.Sin(MathHelper.ToRadians(Projectile.ai[1]));
            float sin2 = (float)Math.Sin(MathHelper.ToRadians(Projectile.ai[1] + 120));
            float sin3 = (float)Math.Sin(MathHelper.ToRadians(Projectile.ai[1] + 240));
            int middle = 180;
            int length = 75;
            float r = middle + length * sin1;
            float g = middle + length * sin2;
            float b = middle + length * sin3;
            Color color = new Color((int)r, (int)g, (int)b);
            return color;
        }
        int counter = 10;
        private bool spawned;
        public override void AI()
        {
            if (!spawned)
            {
                spawned = true;
                SoundEngine.PlaySound(SoundID.Item75);
            }

            counter++;
            Projectile.velocity *= 0.955f + 0.000175f * counter;
            Projectile.ai[1] += 2f;
            Lighting.AddLight(new Vector2(Projectile.Center.X, Projectile.Center.Y), fetchRainbow().R / 255f, fetchRainbow().G / 255f, fetchRainbow().B / 255f);
            if (Main.rand.NextBool(10))
            {
                int num2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 267);
                Dust dust = Main.dust[num2];
                Color color2 = new Color(110, 110, 110, 0).MultiplyRGBA(fetchRainbow());
                dust.color = color2;
                dust.noGravity = true;
                dust.fadeIn = 0.1f;
                dust.scale *= 2f;
                dust.alpha = 255 - (int)(255 * (Projectile.timeLeft / 720f));
                dust.velocity *= 0.5f;
                dust.velocity += Projectile.velocity * 0.4f;
            }
        }
    }
}