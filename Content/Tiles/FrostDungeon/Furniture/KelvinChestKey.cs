using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.FrostDungeon.Furniture
{
	class KelvinChestKey : ModItem
	{
		public override void SetDefaults()
		{
			//item.CloneDefaults(ItemID.GoldenKey);
			item.width = 14;
			item.height = 20;
			item.maxStack = 99;
		}
	}
}