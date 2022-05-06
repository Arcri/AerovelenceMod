using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class GoldenColt : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Golden Colt");
		}
        public override void SetDefaults()
        {
			Item.UseSound = SoundID.Item41;
			Item.crit = 4;
            Item.damage = 16;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 18; 
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = false;
            Item.shoot = AmmoID.Bullet;
			Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 7f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.GoldBar, 15)
                .AddIngredient(ItemID.Diamond, 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}