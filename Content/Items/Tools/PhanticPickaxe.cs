using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Tools
{
    public class PhanticPickaxe : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Phantic Pickaxe");
		}
        public override void SetDefaults()
        {
			Item.crit = 4;
            Item.damage = 11;
            Item.DamageType = DamageClass.Melee;
            Item.width = 34;
            Item.height = 34;
            Item.useTime = 13;
            Item.useAnimation = 13;
            Item.useTurn = true;
			Item.pick = 70;
			Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<PhanticBar>(), 12)
                .AddRecipeGroup("AerovelenceMod:EvilMaterials", 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}