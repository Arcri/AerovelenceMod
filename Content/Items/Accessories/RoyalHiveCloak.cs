using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories
{
    public class RoyalHiveCloak : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Royal Hive Cloak");
			Tooltip.SetDefault("Causes stars to fall, releases bees and douses the user in honey when damaged\nHas a chance to spawn a bee when you deal damage to an enemy\n10% increased damage\nReleases bees, douses the user in honey and increased movement speed when damaged");
		}
        public override void SetDefaults()
        {
			item.accessory = true;
            item.width = 34;
            item.height = 34;
            item.value = 60000;
            item.rare = ItemRarityID.Green;
        }
		public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.allDamage += 0.10f;
            player.starCloak = true;
            player.panic = true;
            player.bee = true;
            AeroPlayer modPlayer = (AeroPlayer)player.GetModPlayer(mod, "AeroPlayer");
            modPlayer.QueensStinger = true;
		}
        public override void UpdateEquip(Player player)
        {
            player.npcTypeNoAggro[NPCID.Bee] = true;
            player.npcTypeNoAggro[NPCID.BeeSmall] = true;
            player.npcTypeNoAggro[NPCID.Hornet] = true;
            player.npcTypeNoAggro[NPCID.HornetFatty] = true;
            player.npcTypeNoAggro[NPCID.HornetHoney] = true;
            player.npcTypeNoAggro[NPCID.HornetLeafy] = true;
            player.npcTypeNoAggro[NPCID.HornetStingy] = true;
            player.npcTypeNoAggro[NPCID.HornetSpikey] = true;
            player.npcTypeNoAggro[NPCID.HornetFatty] = true;
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ItemID.BeeCloak, 1);
            modRecipe.AddIngredient(ItemID.SweetheartNecklace, 1);
            modRecipe.AddIngredient(ModContent.ItemType<DiamondEmpoweredGem>(), 1);
            modRecipe.AddIngredient(ModContent.ItemType<QueensStinger>(), 1);
            modRecipe.AddTile(TileID.TinkerersWorkbench);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}