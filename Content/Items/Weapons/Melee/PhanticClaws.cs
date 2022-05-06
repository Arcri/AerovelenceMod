using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class PhanticClaws : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Phantic Claws");
		}
        public override void SetDefaults()
        {
			Item.crit = 4;
            Item.damage = 9;
            Item.DamageType = DamageClass.Melee;
            Item.width = 20;
            Item.height = 20;
            Item.useTime = 5;
            Item.useAnimation = 5;
			Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<PhanticBar>(), 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}