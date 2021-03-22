using System;
using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            item.UseSound = SoundID.Item5;
            item.crit = 20;
            item.damage = 42;
            item.ranged = true;
            item.width = 32;
            item.height = 56;
            item.useTime = 18;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2;
            item.value = 10000;
            item.rare = ItemRarityID.Cyan;
            item.autoReuse = true;
            item.shoot = AmmoID.Arrow;
            item.useAmmo = AmmoID.Arrow;
            item.shootSpeed = 8f;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float numberProjectiles = 5;
            float rotation = MathHelper.ToRadians(5);
            position += Vector2.Normalize(new Vector2(speedX, speedY)) * 15f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 2f;
                if (i == 1 || i == 3)
                {
                    int proj1 = Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockBack, player.whoAmI);
                    Main.projectile[proj1].velocity *= 1.15f;
                }
                else if (i == 2)
                {
                    int proj2 = Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<WyvernProjectile>(), damage, knockBack, player.whoAmI);
                }
                else
                {
                    int proj3 = Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockBack, player.whoAmI);
                    Main.projectile[proj3].velocity *= 1.15f;
                }
            }
            return false;
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<FrostShard>(), 8);
            modRecipe.AddIngredient(ItemID.IceBlock, 35);
            modRecipe.AddIngredient(ItemID.HellstoneBar, 10);
            modRecipe.AddTile(TileID.Anvils);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }

    public class WyvernProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Razor");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 50;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 7;
            projectile.timeLeft = 200;
            projectile.alpha = 100;
        }
        public override bool PreDraw(SpriteBatch sb, Color lightColor)
        {
            Vector2 vector = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
            for (int i = 0; i < projectile.oldPos.Length; i++)
            {
                Vector2 position = projectile.oldPos[i] - Main.screenPosition + vector + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - i) / projectile.oldPos.Length);
                sb.Draw(Main.projectileTexture[projectile.type], position, null, color, projectile.rotation, vector, projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("AerovelenceMod/Content/Projectiles/Weapons/Magic/StormRazorProjectile_Glow");
            spriteBatch.Draw(
                texture,
                new Vector2
                (
                    projectile.Center.Y - Main.screenPosition.X,
                    projectile.Center.X - Main.screenPosition.Y
                ),
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                projectile.rotation,
                texture.Size(),
                projectile.scale,
                SpriteEffects.None,
                0f
            );
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Vector2 offset = new Vector2(0, 0);
            Main.PlaySound(SoundID.Item10);
            for (float i = 0; i < 360; i += 0.5f)
            {
                float ang = (float)(i * Math.PI) / 180;
                float x = (float)(Math.Cos(ang) * 15) + projectile.Center.X;
                float y = (float)(Math.Sin(ang) * 15) + projectile.Center.Y;
                Vector2 vel = Vector2.Normalize(new Vector2(x - projectile.Center.X, y - projectile.Center.Y)) * 7;
                int dustIndex = Dust.NewDust(new Vector2(x - 3, y - 3), 6, 6, 63, vel.X, vel.Y);
                Main.dust[dustIndex].noGravity = true;
            }
            return true;
        }
        public override void AI()
        {
            projectile.scale *= 1.002f;
            projectile.rotation += 100;
            int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 63, projectile.velocity.X, projectile.velocity.Y, 0, Color.White, 1);
            Main.dust[dust].velocity /= 1.2f;
            Main.dust[dust].noGravity = true;
            projectile.localAI[1]++;
            if (projectile.localAI[1] > 10f && Main.rand.Next(3) == 0)
            {
                projectile.alpha -= 5;
                if (projectile.alpha < 50)
                {
                    projectile.alpha = 50;
                }
                Lighting.AddLight((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, 0.1f, 0.4f, 0.6f);
            }
            int num718 = -1;
            Vector2 vector52 = projectile.Center;
            float num719 = 500f;
            if (projectile.localAI[0] > 0f)
            {
                projectile.localAI[0]--;
            }
            if (projectile.ai[0] == 0f && projectile.localAI[0] == 0f)
            {
                for (int num720 = 0; num720 < 200; num720++)
                {
                    NPC nPC6 = Main.npc[num720];
                    if (nPC6.CanBeChasedBy(this) && (projectile.ai[0] == 0f || projectile.ai[0] == (float)(num720 + 1)))
                    {
                        Vector2 center4 = nPC6.Center;
                        float num721 = Vector2.Distance(center4, vector52);
                        if (num721 < num719 && Collision.CanHit(projectile.position, projectile.width, projectile.height, nPC6.position, nPC6.width, nPC6.height))
                        {
                            num719 = num721;
                            vector52 = center4;
                            num718 = num720;
                        }
                    }
                }
                if (num718 >= 0)
                {
                    projectile.ai[0] = num718 + 1;
                    projectile.netUpdate = true;
                }
                num718 = -1;
            }
            if (projectile.localAI[0] == 0f && projectile.ai[0] == 0f)
            {
                projectile.localAI[0] = 30f;
            }
            bool flag32 = false;
            if (projectile.ai[0] != 0f)
            {
                int num722 = (int)(projectile.ai[0] - 1f);
                if (Main.npc[num722].active && !Main.npc[num722].dontTakeDamage && Main.npc[num722].immune[projectile.owner] == 0)
                {
                    float num723 = Main.npc[num722].position.X + (float)(Main.npc[num722].width / 2);
                    float num724 = Main.npc[num722].position.Y + (float)(Main.npc[num722].height / 2);
                    float num725 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num723) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num724);
                    if (num725 < 1000f)
                    {
                        flag32 = true;
                        vector52 = Main.npc[num722].Center;
                    }
                }
                else
                {
                    projectile.ai[0] = 0f;
                    flag32 = false;
                    projectile.netUpdate = true;
                }
            }
            if (flag32)
            {
                Vector2 v = vector52 - projectile.Center;
                float num726 = projectile.velocity.ToRotation();
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
                projectile.velocity = projectile.velocity.RotatedBy(num728 * 0.10000000149011612);
            }
            float num729 = projectile.velocity.Length();
            projectile.velocity.Normalize();
            projectile.velocity *= num729 + 0.0025f;
        }
    }
}