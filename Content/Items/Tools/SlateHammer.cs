using AerovelenceMod.Content.Items.Placeables.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Tools
{
    public class SlateHammer : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Slate Hammer");
		}
        public override void SetDefaults()
        {
			Item.crit = 4;
            Item.damage = 8;
            Item.DamageType = DamageClass.Melee;
            Item.width = 34;
            Item.height = 34;
            Item.useTurn = true;
            Item.useTime = 28;
            Item.useAnimation = 28;
			Item.hammer = 35;
			Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
        }
		public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<SlateOre>(), 35)
                .AddRecipeGroup("Wood", 15)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}