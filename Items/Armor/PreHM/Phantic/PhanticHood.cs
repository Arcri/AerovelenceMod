using AerovelenceMod.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Armor.PreHM.Phantic
{
    [AutoloadEquip(EquipType.Head)]
    public class PhanticHood : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantic Hood");
            Tooltip.SetDefault("7% increased ranged damage\n5% increased ranged critical strike chance");
        }
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<PhanticCuirass>() && legs.type == ModContent.ItemType<PhanticCuisses>() && head.type == ModContent.ItemType<PhanticHood>();
		}
		public override void UpdateArmorSet(Player player)
		{
            player.setBonus = "Taking over 10 damage will spawn a homing soul to chase your foes";
            player.GetModPlayer<AeroPlayer>().PhanticBonus = true;
        } 	
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.sellPrice(0, 1, 46, 34);
            item.rare = ItemRarityID.Orange;
            item.defense = 3;
        }
		public override void UpdateEquip(Player player)
        {
			player.rangedDamage *= 1.17f;
            player.rangedCrit += 5;
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<PhanticBar>(), 9);
            modRecipe.AddIngredient(ItemID.ShadowScale, 10);
            modRecipe.AddTile(TileID.Anvils);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}