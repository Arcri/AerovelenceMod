using AerovelenceMod.Content.Items.Others.Crafting;
using AerovelenceMod.Content.Projectiles.Weapons.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class HeadOfTheBloodGod : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Head Of The Blood God");
			Tooltip.SetDefault("Fires a guy on a chain");
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item1;
			item.crit = 8;
            item.damage = 24;
            item.melee = true;
            item.width = 54;
            item.height = 54; 
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 5;
			item.value = Item.sellPrice(0, 0, 40, 20);
            item.value = 10000;
            item.autoReuse = true;
            item.rare = ItemRarityID.Orange;
            item.shoot = item.shoot = mod.ProjectileType("ElementScythe");
            item.shootSpeed = 5f;
            item.autoReuse = false;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<PhanticBar>(), 15);
            recipe.AddIngredient(ItemID.HellstoneBar, 15);
            recipe.AddIngredient(ModContent.ItemType<FrostShard>(), 5);
            recipe.AddIngredient(ItemID.Fireblossom, 10);
            recipe.AddIngredient(ItemID.Daybloom, 10);
            recipe.AddIngredient(ItemID.Waterleaf, 10);
            recipe.AddIngredient(ItemID.Moonglow, 10);
            recipe.AddIngredient(ItemID.Shiverthorn, 10);
            recipe.AddIngredient(ItemID.Deathweed, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float numberProjectiles = 2 + Main.rand.Next(1);
            float rotation = MathHelper.ToRadians(20);
            position += Vector2.Normalize(new Vector2(speedX, speedY)) * 45f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(15));
                float scale = 1f - (Main.rand.NextFloat() * .3f);
                if (i == 1)
                {
                    Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<ElementScythe>(), damage, knockBack, player.whoAmI);
                    Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X * 1, perturbedSpeed.Y * 1, type, damage, knockBack, player.whoAmI);
                }
                else
                {
                    Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X * 1, perturbedSpeed.Y * 1, type, damage, knockBack, player.whoAmI);
                }
            }
            return false;
        }
    }
}