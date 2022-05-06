using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories
{
	[AutoloadEquip(EquipType.Wings)]
	public class StormWings : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Storm Wings");
			Tooltip.SetDefault("Allows flight and slow fall");
		}
		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 30;
			Item.value = 10000;
			Item.rare = ItemRarityID.Yellow;
			Item.accessory = true;
		}
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.wingTimeMax = 185;
		}
		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
			ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
		{
			ascentWhenFalling = 1.75f;
			ascentWhenRising = 0.1f;
			maxCanAscendMultiplier = 1.5f;
			maxAscentMultiplier = 2f;
			constantAscend = 0.10f;
		}
		public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
		{
			speed = 8.5f;
			acceleration *= 1.2f;
		}
		public override void AddRecipes()
		{
			CreateRecipe(1)
				.AddIngredient(ModContent.ItemType<BurnshockBar>(), 6)
				.AddIngredient(ItemID.CrystalShard, 2)
				.AddIngredient(ItemID.SoulofFlight, 20)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}