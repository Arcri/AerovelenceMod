using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories
{
	[AutoloadEquip(EquipType.Wings)]
	public class SunflareWings : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sunflare Wings");
			Tooltip.SetDefault("Allows flight and slow fall");
		}
		public override void SetDefaults()
		{
			item.width = 36;
			item.height = 30;
			item.value = 10000;
			item.rare = ItemRarityID.Yellow;
			item.accessory = true;
		}
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.wingTimeMax = 300;
		}
		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
			ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
		{
			ascentWhenFalling = 1.75f;
			ascentWhenRising = 0.15f;
			maxCanAscendMultiplier = 1.6f;
			maxAscentMultiplier = 2.3f;
			constantAscend = 0.10f;
		}
		public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
		{
			speed = 8.5f;
			acceleration *= 1.2f;
		}
		public override void AddRecipes()
		{
			ModRecipe modRecipe = new ModRecipe(mod);
			modRecipe.AddIngredient(ModContent.ItemType<BurnshockBar>(), 6);
			modRecipe.AddIngredient(ItemID.CrystalShard, 2);
			modRecipe.AddIngredient(ItemID.SoulofFlight, 20);
			modRecipe.AddTile(TileID.MythrilAnvil);
			modRecipe.SetResult(this, 1);
			modRecipe.AddRecipe();
		}
	}
}