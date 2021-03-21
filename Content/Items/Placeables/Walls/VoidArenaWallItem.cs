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
            item.useTurn = true;
            item.autoReuse = true;
            item.consumable = true;

			item.maxStack = 999;
            item.useTime = 7;
            item.useAnimation = 15;

            item.createWall = WallType<VoidArenaWall>();

			item.useStyle = ItemUseStyleID.SwingThrow;
        }
	}
}