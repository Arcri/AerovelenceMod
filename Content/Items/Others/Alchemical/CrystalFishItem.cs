using AerovelenceMod.Content.NPCs.CrystalCaverns;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Others.Alchemical
{
	public class CrystalFishItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Electric Tetra");
		}

		public override void SetDefaults()
		{
			item.width = item.height = 20;
			item.rare = ItemRarityID.White;
			item.maxStack = 999;
			item.value = Item.sellPrice(0, 0, 3, 0);
			item.noUseGraphic = true;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.useTime = item.useAnimation = 20;

			item.noMelee = true;
			item.consumable = true;
			item.autoReuse = true;

		}

		public override bool UseItem(Player player)
		{
			NPC.NewNPC((int)player.Center.X, (int)player.Center.Y, ModContent.NPCType<CrystalFish>());
			return true;
		}

	}
}