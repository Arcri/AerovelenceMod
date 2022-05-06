using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles.Furniture
{
	class CitadelChestKey : ModItem
	{
		public override void SetDefaults()
		{
			//item.CloneDefaults(ItemID.GoldenKey);
			Item.width = 14;
			Item.height = 20;
			Item.maxStack = 99;
		}
	}
}