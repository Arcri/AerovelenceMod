using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Blocks.FrostDungeon.Traps
{
	public class SnowriumArenaTrapTest : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The vibe trap");
		}

		public override void SetDefaults()
		{
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.useTurn = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.autoReuse = true;
			item.maxStack = 999;
			item.consumable = true;
			item.createTile = TileType<Blocks.FrostDungeon.Traps.SnowriumArenaTrap>();
			item.width = 12;
			item.height = 12;
			item.value = 10000;
		}
	}
}