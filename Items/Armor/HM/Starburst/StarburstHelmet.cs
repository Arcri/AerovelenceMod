using AerovelenceMod.Items.Armor.HM.Starburst;
using AerovelenceMod.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Armor.HM.Starburst
{
    [AutoloadEquip(EquipType.Head)]
    public class StarburstHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Starburst Helmet");
            Tooltip.SetDefault("+2 max minion slots and 5% increased minion damage");
        }
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<StarburstChestplate>() && legs.type == ModContent.ItemType<StarburstGrieves>() && head.type == ModContent.ItemType<StarburstHelmet>();
		}
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "20% increased minion damage and increased minion knockback\nTaking damage will release damaging shards of crystal";
            player.GetModPlayer<AeroPlayer>().BurnshockArmorBonus = true;
            player.minionDamage += 0.15f;
            player.minionKB += 0.05f;
        }
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 10;
            item.rare = ItemRarityID.Orange;
            item.defense = 4;
        }
		public override void UpdateEquip(Player player)
        {
            player.maxMinions += 2;
            player.minionDamage += 0.05f;
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<BurnshockBar>(), 8);
            modRecipe.AddIngredient(ItemID.CrystalShard, 5);
            modRecipe.AddTile(TileID.MythrilAnvil);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}