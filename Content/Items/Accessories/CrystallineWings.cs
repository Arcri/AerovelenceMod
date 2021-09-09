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
			var line = new TooltipLine(mod, "Verbose:RemoveMe", "Maintain Homeostasis");
			tooltips.Add(line);

			line = new TooltipLine(mod, "Crystalline Wings", "Donator Accessory")
			{
				overrideColor = new Color(036, 124, 149)
			};
			tooltips.Add(line);
			foreach (TooltipLine line2 in tooltips)
			{
				if (line2.mod == "Terraria" && line2.Name == "ItemName")
				{
					line2.overrideColor = new Color(036, 031, 149);
				}
			}
			tooltips.RemoveAll(l => l.Name.EndsWith(":RemoveMe"));
		}
		public override void SetDefaults()
		{
			item.width = 36;
			item.height = 30;
			item.value = 10000;
			item.rare = ItemRarityID.Orange;
			item.accessory = true;
		}
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.wingTimeMax = 45;
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
			ModRecipe modRecipe = new ModRecipe(mod);
			modRecipe.AddIngredient(ModContent.ItemType<CavernCrystal>(), 10);
			modRecipe.AddIngredient(ModContent.ItemType<SlateOre>(), 10);
			modRecipe.AddIngredient(ModContent.ItemType<BloodChunk>(), 10);
			modRecipe.AddIngredient(ItemID.SoulofFlight, 20);
			modRecipe.AddTile(TileID.MythrilAnvil);
			modRecipe.SetResult(this, 1);
			modRecipe.AddRecipe();
		}
	}
}