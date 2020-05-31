using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod;

namespace AerovelenceMod.NPCs.Bosses.Snowrium
{
	public class SnowriumBag : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Treasure Bag");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }


        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 24;
            item.height = 24;
            item.rare = 11;
            item.expert = true;
				}

        public override int BossBagNPC => mod.NPCType("Snowrium");

        public override bool CanRightClick()
        {
            return true;
        }
		public override void OpenBossBag(Player player) {
			Random rnd = new Random();
			int drop = rnd.Next(0, 7);
			player.TryGettingDevArmor();
			if (Main.rand.NextBool(7))
			{
			player.QuickSpawnItem(mod.ItemType("FragileIceCrystal"));
			}
			if (drop == 0)
			{
				player.QuickSpawnItem(mod.ItemType("IcySaber"));
			}
			else if (drop == 1)
			{
				player.QuickSpawnItem(mod.ItemType("CryoBall"));
			}
			else if (drop == 2)
			{
				player.QuickSpawnItem(mod.ItemType("Snowball"));
			}
			else if (drop == 3)
			{
				player.QuickSpawnItem(mod.ItemType("CrystalArch"));
			}
			else if (drop == 4)
			{
				player.QuickSpawnItem(mod.ItemType("DeepFreeze"));
			}
			else if (drop == 5)
			{
				player.QuickSpawnItem(mod.ItemType("FrozenBliss"));
			}
			else
			{
				// TODO Add error reporting in case random outcome is unexpected
			}
		}
	}
}
