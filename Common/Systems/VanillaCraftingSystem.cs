using AerovelenceMod.Content.Items.Placeables.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Common.Systems
{
	public class VanillaCraftingSystem : ModSystem
	{
		public override void AddRecipes()
		{
			Recipe.Create(ItemID.MagicMirror, 1)
				.AddIngredient(ItemID.IceMirror, 1)
				.AddTile(TileID.Anvils)
				.Register();
			Recipe.Create(ItemID.WormholePotion, 1)
				.AddIngredient(ModContent.ItemType<CavernCrystal>(), 2)
				.AddIngredient(ItemID.Bottle, 1)
				.AddTile(TileID.Bottles)
				.Register();
			Recipe.Create(ItemID.LuckyHorseshoe, 1)
				.AddRecipeGroup("AerovelenceMod:GoldBars", 5)
				.AddIngredient(ItemID.Cloud, 5)
				.AddIngredient(ItemID.SunplateBlock, 3)
				.AddTile(TileID.SkyMill)
				.Register();
			Recipe.Create(ItemID.CloudinaBottle, 1)
				.AddIngredient(ItemID.Cloud, 25)
				.AddIngredient(ItemID.Bottle, 1)
				.AddTile(TileID.SkyMill)
				.Register();
			Recipe.Create(ItemID.SandstorminaBottle, 1)
				.AddIngredient(ItemID.SandBlock, 25)
				.AddIngredient(ItemID.Bottle, 1)
				.AddTile(TileID.SkyMill)
				.Register();
			Recipe.Create(ItemID.BlizzardinaBottle, 1)
				.AddIngredient(ItemID.SnowBlock, 25)
				.AddIngredient(ItemID.Bottle, 1)
				.AddTile(TileID.SkyMill)
				.Register();
			Recipe.Create(ItemID.TsunamiInABottle, 1)
				.AddIngredient(ItemID.Starfish, 12)
				.AddIngredient(ItemID.Seashell, 12)
				.AddIngredient(ItemID.Bottle, 1)
				.AddTile(TileID.SkyMill)
				.Register();
			Recipe.Create(ItemID.Aglet, 1)
				.AddRecipeGroup("IronBar", 5)
				.AddTile(TileID.Anvils)
				.Register();
			Recipe.Create(ItemID.AnkletoftheWind, 1)
				.AddRecipeGroup("IronBar", 4)
				.AddIngredient(ItemID.Stinger, 10)
				.AddTile(TileID.Anvils)
				.Register();
			Recipe.Create(ItemID.LavaCharm, 1)
				.AddIngredient(ItemID.HellstoneBar, 10)
				.AddTile(TileID.Anvils)
				.Register();

		}
	}
}