using AerovelenceMod.Content.Items.Others.Crafting;
using AerovelenceMod.Content.Items.Placeables.Blocks;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories
{
	[AutoloadEquip(EquipType.Wings)]
	public class CrystallineWings : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Crystalline Wings");
			Tooltip.SetDefault("Allows flight, but you fall faster and still take fall damage\nDedicated to FryoKnight");
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			var line = new TooltipLine(Mod, "Verbose:RemoveMe", "Maintain Homeostasis");
			tooltips.Add(line);

			line = new TooltipLine(Mod, "Crystalline Wings", "Donator Accessory")
			{
				OverrideColor = new Color(036, 124, 149)
			};
			tooltips.Add(line);
			foreach (TooltipLine line2 in tooltips)
			{
				if (line2.Mod == "Terraria" && line2.Name == "ItemName")
				{
					line2.OverrideColor = new Color(036, 031, 149);
				}
			}
			tooltips.RemoveAll(l => l.Name.EndsWith(":RemoveMe"));
		}
		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 30;
			Item.value = 10000;
			Item.rare = ItemRarityID.Orange;
			Item.accessory = true;
		}
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.wingTimeMax = 50;
			player.gravity *= 2.25f;
			player.noFallDmg = false;
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
			speed = 4.5f;
			acceleration *= 1.2f;
		}
		public override void AddRecipes()
		{
			CreateRecipe(1)
				.AddIngredient(ModContent.ItemType<CavernCrystal>(), 10)
				.AddIngredient(ModContent.ItemType<SlateOre>(), 10)
				.AddIngredient(ModContent.ItemType<BloodChunk>(), 10)
				.AddIngredient(ItemID.SoulofFlight, 20)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}