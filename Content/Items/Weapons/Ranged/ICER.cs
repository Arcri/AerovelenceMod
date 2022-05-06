using System;
using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class ICER : ModItem
    {
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-8, 0);
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("I.C.E.R.");
			Tooltip.SetDefault("Was an ounce too heavy");
		}
        public override void SetDefaults()
        {
			Item.UseSound = SoundID.Item12;
			Item.crit = 20;
            Item.damage = 73;
            Item.DamageType = DamageClass.Magic;
            Item.width = 44;
            Item.height = 26; 
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 8;
            Item.value = 10000;
            Item.rare = ItemRarityID.LightPurple;
            Item.autoReuse = true;
            Item.shoot = Mod.Find<ModProjectile>("ICERProj").Type;
            Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 36f;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.AshBlock, 25)
                .AddIngredient(ItemID.HellstoneBar, 10)
                .AddIngredient(ModContent.ItemType<EmberFragment>(), 3)
                .AddRecipeGroup("IronBar", 3)
                .AddTile(TileID.Hellforge)
                .Register();
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (type == ProjectileID.Bullet)
            {
                type = ModContent.ProjectileType<ICERProj>();
            }
            return true;
        }
    }




    public class ICERProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 3;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
			Projectile.light = 1f;
			Projectile.timeLeft = 60;
			Projectile.alpha = 100;
        }
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(0f, 0.50f, 1f, 1f);
		}

        public override void OnHitNPC(NPC target, int damage, float knockBack, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 500);
        }

        public override void AI()
        {
			Lighting.AddLight(Projectile.Center, 0f, 0.25f, 1f);
			Dust.NewDust(Projectile.position + Projectile.velocity, 0, 0, 92, 0, 0);
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f;
        }
    }
}