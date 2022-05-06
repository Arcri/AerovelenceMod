using System;
using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class WyvernsWake : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wyvern's Wake");
            Tooltip.SetDefault("Fires homing razorwind aside 4 arrows");
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item5;
            Item.crit = 20;
            Item.damage = 42;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 32;
            Item.height = 56;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2;
            Item.value = 10000;
            Item.rare = ItemRarityID.Cyan;
            Item.autoReuse = true;
            Item.shoot = AmmoID.Arrow;
            Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 8f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float numberProjectiles = 3;
            float rotation = MathHelper.ToRadians(45);
            position += Vector2.Normalize(velocity) * 45f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f;
                if (i == 1 || i == 3)
                {
                    int proj1 = Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, 2f, player.whoAmI);
                    Main.projectile[proj1].velocity *= 1.15f;
                }
                else if (i == 2)
                {
                    int proj2 = Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<WyvernProjectile>(), damage, 2f, player.whoAmI);
                }
                else
                {
                    int proj3 = Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, 2f, player.whoAmI);
                    Main.projectile[proj3].velocity *= 1.15f;
                }
            }
            return false;
        }
    }

    public class WyvernProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Razor");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 7;
            Projectile.timeLeft = 200;
            Projectile.alpha = 100;
        }
        public override bool PreDraw(ref Color lightColor)
        {
        Vector2 vector = new Vector2(TextureAssets.Projectile[Projectile.type].Width() * 0.5f, Projectile.height * 0.5f);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 position = Projectile.oldPos[i] - Main.screenPosition + vector + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - i) / Projectile.oldPos.Length);
                Main.spriteBatch.Draw((Texture2D)TextureAssets.Projectile[Projectile.type], position, null, color, Projectile.rotation, vector, Projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
        public override void PostDraw(Color lightColor)
        {
        Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Projectiles/Weapons/Magic/StormRazorProjectile_Glow");
            Main.EntitySpriteDraw(
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
                0
            );
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Vector2 offset = new Vector2(0, 0);
            SoundEngine.PlaySound(SoundID.Item10);
            for (float i = 0; i < 360; i += 0.5f)
            {
                float ang = (float)(i * Math.PI) / 180;
                float x = (float)(Math.Cos(ang) * 15) + Projectile.Center.X;
                float y = (float)(Math.Sin(ang) * 15) + Projectile.Center.Y;
                Vector2 vel = Vector2.Normalize(new Vector2(x - Projectile.Center.X, y - Projectile.Center.Y)) * 7;
                int dustIndex = Dust.NewDust(new Vector2(x - 3, y - 3), 6, 6, DustID.WhiteTorch, vel.X, vel.Y);
                Main.dust[dustIndex].noGravity = true;
            }
            return true;
        }
        public override void AI()
        {
            Projectile.scale *= 1.002f;
            Projectile.rotation += 100;
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.WhiteTorch, Projectile.velocity.X, Projectile.velocity.Y, 0, Color.White, 1);
            Main.dust[dust].velocity /= 1.2f;
            Main.dust[dust].noGravity = true;
            Projectile.localAI[1]++;
            if (Projectile.localAI[1] > 10f && Main.rand.NextBool(3))
            {
                Projectile.alpha -= 5;
                if (Projectile.alpha < 50)
                {
                    Projectile.alpha = 50;
                }
                Lighting.AddLight((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, 0.1f, 0.4f, 0.6f);
            }
            int num718 = -1;
            Vector2 vector52 = Projectile.Center;
            float num719 = 500f;
            if (Projectile.localAI[0] > 0f)
            {
                Projectile.localAI[0]--;
            }
            if (Projectile.ai[0] == 0f && Projectile.localAI[0] == 0f)
            {
                for (int num720 = 0; num720 < 200; num720++)
                {
                    NPC nPC6 = Main.npc[num720];
                    if (nPC6.CanBeChasedBy(this) && (Projectile.ai[0] == 0f || Projectile.ai[0] == (float)(num720 + 1)))
                    {
                        Vector2 center4 = nPC6.Center;
                        float num721 = Vector2.Distance(center4, vector52);
                        if (num721 < num719 && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, nPC6.position, nPC6.width, nPC6.height))
                        {
                            num719 = num721;
                            vector52 = center4;
                            num718 = num720;
                        }
                    }
                }
                if (num718 >= 0)
                {
                    Projectile.ai[0] = num718 + 1;
                    Projectile.netUpdate = true;
                }
                num718 = -1;
            }
            if (Projectile.localAI[0] == 0f && Projectile.ai[0] == 0f)
            {
                Projectile.localAI[0] = 30f;
            }
            bool flag32 = false;
            if (Projectile.ai[0] != 0f)
            {
                int num722 = (int)(Projectile.ai[0] - 1f);
                if (Main.npc[num722].active && !Main.npc[num722].dontTakeDamage && Main.npc[num722].immune[Projectile.owner] == 0)
                {
                    float num723 = Main.npc[num722].position.X + (float)(Main.npc[num722].width / 2);
                    float num724 = Main.npc[num722].position.Y + (float)(Main.npc[num722].height / 2);
                    float num725 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num723) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num724);
                    if (num725 < 1000f)
                    {
                        flag32 = true;
                        vector52 = Main.npc[num722].Center;
                    }
                }
                else
                {
                    Projectile.ai[0] = 0f;
                    flag32 = false;
                    Projectile.netUpdate = true;
                }
            }
            if (flag32)
            {
                Vector2 v = vector52 - Projectile.Center;
                float num726 = Projectile.velocity.ToRotation();
                float num727 = v.ToRotation();
                double num728 = num727 - num726;
                if (num728 > Math.PI)
                {
                    num728 -= Math.PI * 2.0;
                }
                if (num728 < -Math.PI)
                {
                    num728 += Math.PI * 2.0;
                }
                Projectile.velocity = Projectile.velocity.RotatedBy(num728 * 0.10000000149011612);
            }
            float num729 = Projectile.velocity.Length();
            Projectile.velocity.Normalize();
            Projectile.velocity *= num729 + 0.0025f;
        }
    }
}