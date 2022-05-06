using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Tools
{
    public class PhanticHammer : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Phantic Hammer");
		}
        public override void SetDefaults()
        {
			Item.crit = 3;
            Item.damage = 12;
            Item.DamageType = DamageClass.Melee;
            Item.width = 38;
            Item.height = 38;
            Item.useTime = 25;
            Item.useAnimation = 25;
			Item.hammer = 60;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 0, 35, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
        }
		public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<PhanticBar>(), 12)
                .AddRecipeGroup("AerovelenceMod:EvilMaterials", 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}