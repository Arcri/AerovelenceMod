#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

#endregion

namespace AerovelenceMod.Content.Items.Armor.Lumberjack
{
	// TODO: Arcri - Implement (and make) correct sprites.
	[AutoloadEquip(EquipType.Body)]
	public sealed class LumberjackShirt : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Increases melee speed by 5%");
		}
		public override void SetDefaults()
		{
			item.width = 38;
			item.height = 20;
			item.rare = ItemRarityID.Blue;
			item.value = Item.sellPrice(copper: 30);

			item.defense = 2;
		}

		public override void UpdateEquip(Player player)
			=> player.meleeSpeed += 0.05f;
	}
}
