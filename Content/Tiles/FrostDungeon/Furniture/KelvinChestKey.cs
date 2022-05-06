using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.FrostDungeon.Furniture
{
	class KelvinChestKey : ModItem
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