using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.TreasureBags
{
	public class DecurionBag : ModItem
	{
		public override int BossBagNPC => Mod.Find<ModNPC>("DecurionHead").Type;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Treasure Bag");
			Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
		}

		public override void SetDefaults()
		{
			Item.maxStack = 999;
			Item.consumable = true;
			Item.width = 36;
			Item.height = 32;
			Item.rare = ItemRarityID.Purple;
			Item.expert = true;
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void OpenBossBag(Player player)
		{
			var s = player.GetSource_OpenItem(Type);
			player.QuickSpawnItem(s, Mod.Find<ModItem>("EnergyShield").Type);
			player.QuickSpawnItem(s,ItemID.GoldCoin, 9);

			int ItemDrop = Main.rand.Next(7);

			switch (Main.rand.Next(4))
			{
				case 0:
					player.QuickSpawnItem(s,Mod.Find<ModItem>("DarknessDischarge").Type);
					break;
				case 1:
					player.QuickSpawnItem(s,Mod.Find<ModItem>("Oblivion").Type);
					break;
				case 2:
					player.QuickSpawnItem(s,Mod.Find<ModItem>("CyverCannon").Type);
					break;
				case 3:
					player.QuickSpawnItem(s,Mod.Find<ModItem>("Cyverthrow").Type);
					break;
				case 4:
					player.QuickSpawnItem(s,Mod.Find<ModItem>("AetherVision").Type);
					break;
			}
		}
	}
}