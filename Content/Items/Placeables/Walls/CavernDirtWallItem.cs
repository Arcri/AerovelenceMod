using AerovelenceMod.Content.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Items.Placeables.Walls
{
	public class CavernDirtWallItem : ModItem
	{
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Cavern Dirt Wall");
		}

		public override void SetDefaults()
		{
			Item.width = 12;
			Item.height = 12;
			Item.maxStack = 999;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 7;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.createWall = WallType<CavernDirtWall>();
		}
	}
}