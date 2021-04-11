#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

#endregion

namespace AerovelenceMod.Content.Items.Armor.Lumberjack
{
	// TODO: Eldrazi - Implement correct sprites.
	[AutoloadEquip(EquipType.Legs)]
	public sealed class LumberjackBoots : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Increases move speed by 3%");
		}
		public override void SetDefaults()
		{
			item.width = 22;
			item.height = 16;
			item.rare = ItemRarityID.Blue;
			item.value = Item.sellPrice(copper: 15);

			item.defense = 1;
		}

		public override void UpdateEquip(Player player)
			=> player.moveSpeed += 0.03f;
	}
}