using AerovelenceMod.Content.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Items.Placeables.Walls
{
	public class VoidArenaWallItem : ModItem
	{
		public override void SetStaticDefaults() => DisplayName.SetDefault("Void Arena Wall");

		public override void SetDefaults()
		{
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;

			Item.maxStack = 999;
            Item.useTime = 7;
            Item.useAnimation = 15;

            Item.createWall = WallType<VoidArenaWall>();

			Item.useStyle = ItemUseStyleID.Swing;
        }
	}
}