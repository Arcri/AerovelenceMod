using AerovelenceMod.Items.Placeable.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Tools
{
    public class SlateAxe : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Slate Axe");
		}
        public override void SetDefaults()
        {
			item.crit = 3;
            item.damage = 6;
            item.melee = true;
            item.width = 40;
            item.useTurn = true;
            item.height = 34;
            item.useTime = 26;
            item.useAnimation = 26;
			item.axe = 13;
			item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 0, 20, 0);
            item.rare = ItemRarityID.Blue;
            item.autoReuse = true;
        }


		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);




            //Mod LegacyMod = ModLoader.GetMod("SOTS");
            //if (LegacyMod != null)
            //{
            //recipe.AddIngredient(LegacyMod.ItemType("SanguiteBar"), 40);
            //recipe.AddIngredient(ModContent.ItemType<SlateOreItem>(), 35);
            //recipe.AddRecipeGroup("Wood", 15);
            //}
            //else
            //{
            recipe.AddIngredient(ModContent.ItemType<SlateOreItem>(), 35);
            recipe.AddRecipeGroup("Wood", 15);
            //}
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}