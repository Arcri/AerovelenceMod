using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.CrystalCaverns.Flora
{
	public class PrismaticSeeds : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Prismatic Seeds");
		}

		public override void SetDefaults()
		{
			item.autoReuse = true;
			item.useTurn = true;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.useAnimation = 15;
			item.rare = ItemRarityID.Pink;
			item.useTime = 10;
			item.maxStack = 99;
			item.consumable = true;
			item.placeStyle = 0;
			item.width = 12;
			item.height = 14;
			item.value = Item.buyPrice(0, 0, 5, 0);
			item.createTile = mod.TileType("PrismaticAster");
		}
	}
}