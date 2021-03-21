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
			item.UseSound = SoundID.Item12;
			item.crit = 20;
            item.damage = 73;
            item.magic = true;
            item.width = 44;
            item.height = 26; 
            item.useTime = 28;
            item.useAnimation = 28;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 8;
            item.value = 10000;
            item.rare = ItemRarityID.LightPurple;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("ICERProj");
            item.useAmmo = AmmoID.Bullet;
            item.shootSpeed = 36f;
        }

        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ItemID.AshBlock, 25);
            modRecipe.AddIngredient(ItemID.HellstoneBar, 10);
            modRecipe.AddIngredient(ModContent.ItemType<EmberFragment>(), 3);
            modRecipe.AddRecipeGroup("IronBar", 3);
            modRecipe.AddTile(TileID.Hellforge);
            modRecipe.SetResult(this);
            modRecipe.AddRecipe();
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
            projectile.width = 24;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = 3;
            projectile.ranged = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = false;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
			projectile.light = 1f;
			projectile.timeLeft = 60;
			projectile.alpha = 100;
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
			Lighting.AddLight(projectile.Center, 0f, 0.25f, 1f);
			Dust.NewDust(projectile.position + projectile.velocity, 0, 0, 92, 0, 0);
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
        }
    }
}