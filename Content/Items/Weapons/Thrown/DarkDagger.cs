using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;

namespace AerovelenceMod.Content.Items.Weapons.Thrown
{
    public class DarkDagger : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Secret Shadow Blade");
            Tooltip.SetDefault("Fires a spread of dark daggers around the player that fire towards the cursor");
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item11;
            Item.crit = 8;
            Item.damage = 120;
            Item.DamageType = DamageClass.Melee;
            Item.width = 22;
            Item.height = 38;
            Item.useTime = 80;
            Item.useAnimation = 80;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.shoot = Mod.Find<ModProjectile>("DarkDaggerProjectile").Type;
            Item.shootSpeed = 12f;
        }
        float dynamicCounter = 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 3 + (Main.expertMode ? 1 : 0); i++)
            {
                Vector2 toLocation = player.Center + new Vector2(Main.rand.NextFloat(100, 240), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(360)));
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    damage = Item.damage;
                    Projectile.NewProjectile(source, toLocation, Vector2.Zero, ModContent.ProjectileType<DarkDaggerProjectile>(), damage, 0, Main.myPlayer, player.whoAmI);
                }
                Vector2 toLocationVelo = toLocation - player.Center;
                Vector2 from = player.Center;
                for (int j = 0; j < 300; j++)
                {
                    Vector2 velo = toLocationVelo.SafeNormalize(Vector2.Zero);
                    from += velo * 12;
                    Vector2 circularLocation = new Vector2(10, 0).RotatedBy(MathHelper.ToRadians(j * 12 + dynamicCounter));

                    int dust = Dust.NewDust(from + new Vector2(-4, -4) + circularLocation, 0, 0, 164, 0, 0, 0, default, 1.25f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.1f;
                    Main.dust[dust].scale = 1.8f;

                    if ((from - toLocation).Length() < 24)
                    {
                        break;
                    }
                }
            }
            return false;
        }
    }

    public class DarkDaggerProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 44;
            Projectile.timeLeft = 560;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.damage = 56;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            //projectile.netImportant = true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override bool ShouldUpdatePosition()
        {
            return Projectile.timeLeft <= 420;
        }
        public override void AI()
        {
            Projectile.rotation = MathHelper.ToRadians(90) + Projectile.velocity.ToRotation();
            if (Projectile.timeLeft == 420)
            {
                SoundEngine.PlaySound(SoundID.Item, (int)Projectile.Center.X, (int)Projectile.Center.Y, 71, 0.75f);
                for (int i = 0; i < 360; i += 5)
                {
                    Vector2 circular = new Vector2(12, 0).RotatedBy(MathHelper.ToRadians(i));
                    Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(5) + circular, 0, 0, 164, 0, 0, Projectile.alpha);
                    dust.velocity *= 0.15f;
                    dust.velocity += -Projectile.velocity;
                    dust.scale = 2.75f;
                    dust.noGravity = true;
                }
            }
            if (Projectile.timeLeft > 420)
            {
                Player player = Main.player[(int)Projectile.ai[0]];
                if (player.active)
                {
                    Vector2 toPlayer = Projectile.Center - Main.MouseWorld;
                    toPlayer = toPlayer.SafeNormalize(Vector2.Zero) * 12;
                    Projectile.velocity = -toPlayer;
                }
            }
            else
            {
                Projectile.hostile = false;
                int dust = Dust.NewDust(Projectile.Center + new Vector2(-4, -4), 0, 0, 164, 0, 0, Projectile.alpha, default, 1.25f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.1f;
                Main.dust[dust].scale *= 0.75f;
            }
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 1.8f / 255f, (255 - Projectile.alpha) * 0.0f / 255f, (255 - Projectile.alpha) * 0.0f / 255f);
            if (Projectile.timeLeft <= 25)
                Projectile.alpha += 10;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 3;
        }
    }
}