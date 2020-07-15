using AerovelenceMod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class MarbleMusket : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Marble Musket");
			Tooltip.SetDefault("Fires a chunk of slow-travelling marble");
		}
        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item41;
            item.crit = 6;
            item.damage = 29;
            item.ranged = true;
            item.width = 60;
            item.height = 32;
            item.useTime = 26;
            item.useAnimation = 26;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 0, 55, 40);
            item.rare = ItemRarityID.Green;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<MarbleBlast>();
            item.useAmmo = AmmoID.Bullet;
            item.shootSpeed = 24f;
        }


        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ItemID.Marble, 45);
            modRecipe.AddRecipeGroup("IronBar", 5);
            modRecipe.AddTile(TileID.Anvils);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }

    }
}