using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class IcicleSpike : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Icicle Spike");
        }
        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item1;
            item.crit = 20;
            item.damage = 26;
            item.melee = true;
            item.noMelee = false;
            item.width = 52;
            item.height = 52;
            item.useTime = 26;
            item.useAnimation = 26;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 6;
            item.value = 10000;
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
            item.shoot = ProjectileID.WoodenArrowFriendly;
            item.shootSpeed = 8f;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ProjectileID.BallofFrost, damage, knockBack, player.whoAmI);
            return false;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<FrostShard>(), 15);
            recipe.AddRecipeGroup("IronBar", 7);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}