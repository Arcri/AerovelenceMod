using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Blocks.FrostDungeon.Furniture
{
	public class KelvinDoorLockedItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Kelvin Door Locked");
		}

		public override void SetDefaults()
		{
			item.width = 14;
			item.height = 28;
			item.maxStack = 99;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.consumable = true;
			item.value = 150;
			item.createTile = TileType<KelvinDoorLocked>();
		}
	}
}