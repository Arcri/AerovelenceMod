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
			Item.width = Item.height = 20;
			Item.rare = ItemRarityID.White;
			Item.maxStack = 999;
			Item.value = Item.sellPrice(0, 0, 3, 0);
			Item.noUseGraphic = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = Item.useAnimation = 20;

			Item.noMelee = true;
			Item.consumable = true;
			Item.autoReuse = true;

		}

		public override bool? UseItem(Player player)
		{
			NPC.NewNPC((int)player.Center.X, (int)player.Center.Y, ModContent.NPCType<CrystalFish>());
			return true;
		}

	}
}