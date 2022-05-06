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
            Item.UseSound = SoundID.Item1;
            Item.crit = 20;
            Item.damage = 26;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = false;
            Item.width = 52;
            Item.height = 52;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 8f;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ProjectileID.BallofFrost, damage, knockBack, player.whoAmI);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<FrostShard>(), 15)
                .AddRecipeGroup("IronBar", 7)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}