using AerovelenceMod.Items.Armor.HM.Starglass;
using AerovelenceMod.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Armor.HM.Starglass




{
    [AutoloadEquip(EquipType.Head)]
    public class StarglassHeadgear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Starglass Headgear");
            Tooltip.SetDefault("10% increased ranged damage\n8% increased ranged critical strike chance");
        }
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<StarglassChestplate>() && legs.type == ModContent.ItemType<StarglassGrieves>() && head.type == ModContent.ItemType<StarglassHeadgear>();
		}
		public override void UpdateArmorSet(Player player)
		{
            player.setBonus = "11% increased ranged damage\nTaking damage will release damaging shards of crystal";
            player.GetModPlayer<AeroPlayer>().BurnshockArmorBonus = true;
            player.rangedDamage += 0.11f;
        } 	
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.sellPrice(0, 1, 30, 0);
            item.rare = ItemRarityID.Orange;
            item.defense = 10;
        }
		public override void UpdateEquip(Player player)
        {
			player.rangedDamage += 1.00f;
            player.rangedCrit += 8;
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