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
			Item.accessory = true;
            Item.width = 34;
            Item.height = 34;
            Item.value = 60000;
            Item.rare = ItemRarityID.Green;
        }
		public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.allDamage += 0.10f;
            player.starCloak = true;
            player.panic = true;
            player.bee = true;
            AeroPlayer modPlayer = (AeroPlayer)player.GetModPlayer(Mod, "AeroPlayer");
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
            CreateRecipe(1)
                .AddIngredient(ItemID.BeeCloak, 1)
                .AddIngredient(ItemID.SweetheartNecklace, 1)
                .AddIngredient(ModContent.ItemType<DiamondEmpoweredGem>(), 1)
                .AddIngredient(ModContent.ItemType<QueensStinger>(), 1)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}