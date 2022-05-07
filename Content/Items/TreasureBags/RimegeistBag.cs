using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.TreasureBags
{
	public class RimegeistBag : ModItem
	{
		public override int BossBagNPC => Mod.Find<ModNPC>("Rimegeist").Type;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Treasure Bag");
			Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
		}

		public override void SetDefaults()
		{
			Item.maxStack = 999;
			Item.consumable = true;
			Item.width = 42;
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
			player.QuickSpawnItem(s,Mod.Find<ModItem>("FragileIceCrystal").Type);
			player.QuickSpawnItem(s,ItemID.GoldCoin, 15);
			player.QuickSpawnItem(s,Mod.Find<ModItem>("FrostShard").Type, Main.rand.Next(10) + 10);
			player.QuickSpawnItem(s,ItemID.HealingPotion, Main.rand.Next(4, 20));

			int ItemDrop = Main.rand.Next(7);

			switch (Main.rand.Next(5))
			{
				case 0:
					player.QuickSpawnItem(s,Mod.Find<ModItem>("CrystalArch").Type);
					break;
				case 1:
					player.QuickSpawnItem(s,Mod.Find<ModItem>("IcySaber").Type);
					break;
				case 2:
					player.QuickSpawnItem(s,Mod.Find<ModItem>("CryoBall").Type);
					break;
				case 3:
					player.QuickSpawnItem(s,Mod.Find<ModItem>("DeepFreeze").Type);
					break;
				case 4:
					player.QuickSpawnItem(s,Mod.Find<ModItem>("Snowball").Type);
					break;
			}
		}
	}
}