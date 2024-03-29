using System;
using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class EmberRifle : ModItem
    {
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-8, 0);
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ember Rifle");
			Tooltip.SetDefault("Shoots a beam of fire");
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item12;
			item.crit = 20;
            item.damage = 73;
            item.magic = true;
            item.width = 74;
            item.height = 26; 
            item.useTime = 28;
            item.useAnimation = 28;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 8;
            item.value = 10000;
            item.rare = ItemRarityID.LightPurple;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("EmberRifleProj");
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
    }

	public class EmberRifleProj : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
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
			return new Color(1f, 1f, 0.25f, 1f);
		}
        public override void AI()
        {
			Lighting.AddLight(projectile.Center, 1f, 1f, 0.25f);
			Dust.NewDust(projectile.position + projectile.velocity, 0, 0, 55, 0, 0);
			projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 1.57f;
        }
    }
}