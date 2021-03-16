using AerovelenceMod.Dusts;
using AerovelenceMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Equipment
{
	public class Flashlight : ModItem
	{
		bool isTurnedOn = false;
		public override void SetStaticDefaults()
		{
			Item.staff[item.type] = true;
			DisplayName.SetDefault("Flashlight");
			Tooltip.SetDefault("Right click to turn on/off the flashlight");
		}
		public override void SetDefaults()
		{
			item.width = 58;
			item.height = 18;
			item.useTime = 1;
			item.useAnimation = 1;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true;
			item.autoReuse = false;
			item.value = Item.sellPrice(0, 0, 55, 40);
			item.rare = ItemRarityID.Green;
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}
		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				isTurnedOn = !isTurnedOn;
				item.width = 58;
				item.height = 18;
				item.useTime = 1;
				item.useAnimation = 1;
				item.useStyle = ItemUseStyleID.HoldingOut;
				item.noMelee = true;
				item.autoReuse = false;
				item.value = Item.sellPrice(0, 0, 55, 40);
				item.rare = ItemRarityID.Green;
			}
			else
			{
				item.width = 58;
				item.height = 18;
				item.useTime = 1;
				item.useAnimation = 1;
				item.useStyle = ItemUseStyleID.HoldingOut;
				item.noMelee = true;
				item.autoReuse = false;
				item.value = Item.sellPrice(0, 0, 55, 40);
				item.rare = ItemRarityID.Green;
			}
			return base.CanUseItem(player);
		}
		public override void HoldItem(Player player)
		{
			Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);
			if (isTurnedOn)
			{
				if (Main.rand.Next(player.itemAnimation > 0 ? 40 : 80) == 0)
				{
					Dust.NewDust(new Vector2(player.itemLocation.X + 16f * player.direction, player.itemLocation.Y - 14f * player.gravDir), 4, 4, ModContent.DustType<Sparkle>());
				}
				Lighting.AddLight(position, 1f, 1f, 0f);
			}
		}
	}
}