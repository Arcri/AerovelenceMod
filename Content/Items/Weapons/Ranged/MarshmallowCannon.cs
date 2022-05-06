using AerovelenceMod.Content.Items.Weapons.ProjectileItem;
using AerovelenceMod.Content.Projectiles.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class MarshmallowCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Marshmallow Cannon");
            Tooltip.SetDefault("Requires mallow bullets to work");
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item41;
            Item.crit = 8;
            Item.damage = 28;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 46;
            Item.height = 28;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 3, 50, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<MallowBulletProj>();
            Item.useAmmo = ModContent.ItemType<MallowBullet>();
            Item.shootSpeed = 0.05f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(3));
            speedX = perturbedSpeed.X;
            speedY = perturbedSpeed.Y;
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddRecipeGroup("Wood", 15)
                .AddRecipeGroup("IronBar", 4)
                .AddIngredient(ItemID.Marshmallow, 5)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}