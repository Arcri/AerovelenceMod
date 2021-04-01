using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class H2OCaliber : ModItem
    {
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("H2O Caliber");
            Tooltip.SetDefault("Fires short ranged bubbles");
        }
        public override void SetDefaults()
        {
            item.crit = 4;
            item.damage = 17;
            item.ranged = true;
            item.width = 44;
            item.height = 26;
            item.useTime = 20;
            item.useAnimation = 20;
            item.UseSound = SoundID.Item5;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 2;
            item.value = Item.sellPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Green;
            item.shoot = ProjectileID.FlaironBubble;
            item.autoReuse = true;
            item.shootSpeed = 9f;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float numberProjectiles = 2 + Main.rand.Next(1);
            position += Vector2.Normalize(new Vector2(speedX, speedY)) * 45f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(5));
                float scale = 1f - (Main.rand.NextFloat() * .3f);
                if (i == 1)
                {
                    Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X * 2, perturbedSpeed.Y * 2, type, damage, knockBack, player.whoAmI);
                    Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
                }
                else
                {
                    Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X * 2, perturbedSpeed.Y * 2, type, damage, knockBack, player.whoAmI);
                }
            }
            return false;
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ItemID.Sandstone, 15);
            modRecipe.AddRecipeGroup("AerovelenceMod:GoldBars", 12);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}