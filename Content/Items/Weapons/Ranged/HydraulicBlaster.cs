using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class HydraulicBlaster : ModItem
    {
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-6, 0);
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hydraulic Blaster");
            Tooltip.SetDefault("Shoots out Bursts of water that bounces of enemies and tiles");
        }
        public override void SetDefaults()
        {
            Item.crit = 20;
            Item.damage = 38;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 66;
            Item.height = 22;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.UseSound = SoundID.Item36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 8;
            Item.value = Item.sellPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = false;
            Item.shoot = Mod.Find<ModProjectile>("HydraulicBlasterProj").Type;
            Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 15f;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.HallowedBar, 10)
                .AddIngredient(ItemID.Starfish, 5)
                .AddIngredient(ItemID.SoulofSight, 3)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }


        public static Vector2[] randomSpread(float speedX, float speedY, int angle, int num)
        {
            var posArray = new Vector2[num];
            float spread = (float)(angle * 0.033);
            float baseSpeed = (float)Math.Sqrt(speedX * speedX + speedY * speedY);
            double baseAngle = Math.Atan2(speedX, speedY);
            double randomAngle;
            for (int i = 0; i < num; ++i)
            {
                randomAngle = baseAngle + (Main.rand.NextFloat() - 0.5f) * spread;
                posArray[i] = new Vector2(baseSpeed * (float)Math.Sin(randomAngle), baseSpeed * (float)Math.Cos(randomAngle));
            }
            return posArray;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2[] speeds = randomSpread(velocity.X, velocity.Y, 3, 3);
            for (int i = 0; i < 3; ++i)
            {
                Projectile.NewProjectile(source, position.X, position.Y, speeds[i].X, speeds[i].Y, Mod.Find<ModProjectile>("HydraulicBlasterProj").Type, damage, 2f, player.whoAmI);
            }
            return false;
        }
    }
	public class HydraulicBlasterProj : ModProjectile
    {
		int i;
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
			Projectile.timeLeft = 120;
			Projectile.alpha = 100;
        }
        public override void AI()
        {
			Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f;
			i++;
			/*if (i % 3 == 0)
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 56, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
			}*/
			Projectile.velocity.Y += 0.1f;
			Projectile.velocity *= 0.98f;
        }
		public override bool PreDraw(ref Color lightColor)
        {
            // Vector2 drawOrigin = new Vector2((Texture2D)TextureAssets.Projectile[projectile.type].Width, (Texture2D)TextureAssets.Projectile[projectile.type].Height);
            Texture2D texture2D = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                float scale = Projectile.scale * (Projectile.oldPos.Length - k) / Projectile.oldPos.Length * .45f;
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + TextureAssets.Projectile[Projectile.type].Size() / 3f;
                Color color = Projectile.GetAlpha(Color.Aqua) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);

                Main.EntitySpriteDraw(texture2D, drawPos, null, color, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Size(), scale, SpriteEffects.None, 0);
            }

            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity) 
		{
            var entitySource = Projectile.GetSource_FromAI();
			Projectile.NewProjectile(entitySource, Projectile.position.X, Projectile.position.Y, Main.rand.Next(-10, 10), Main.rand.Next(-10, 10), Mod.Find<ModProjectile>("HydraulicBlasterProjSmall").Type, 10, 2, Main.player[0].whoAmI);
			return true;
		}		
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.position.X, Projectile.position.Y, Main.rand.Next(-10, 10), Main.rand.Next(-10, 10), Mod.Find<ModProjectile>("HydraulicBlasterProjSmall").Type, damage / 2, knockback, Main.player[0].whoAmI);
		}
    }
		public class HydraulicBlasterProjSmall : ModProjectile
    {
		int i;
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
			Projectile.timeLeft = 60;
			Projectile.alpha = 100;
        }
		public override bool PreDraw(ref Color lightColor)
        {
            // Vector2 drawOrigin = new Vector2((Texture2D)TextureAssets.Projectile[projectile.type].Width, (Texture2D)TextureAssets.Projectile[projectile.type].Height);
            Texture2D texture2D = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                float scale = Projectile.scale * (Projectile.oldPos.Length - k) / Projectile.oldPos.Length * .45f;
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + TextureAssets.Projectile[Projectile.type].Size() / 3f;
                Color color = Projectile.GetAlpha(Color.Aqua) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);

                Main.EntitySpriteDraw(texture2D, drawPos, null, color, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Size(), scale, SpriteEffects.None, 0);
            }

            return true;
        }
        public override void AI()
        {
			Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f;
			i++;
		/*	if (i % 3 == 0)
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 56, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
			}*/
			Projectile.velocity.Y += 0.1f;
			Projectile.velocity *= 0.98f;
        }
    }
}