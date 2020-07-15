using AerovelenceMod.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Armor.HM.Burnshock
{
    [AutoloadEquip(EquipType.Head)]
    public class BurnshockFacemask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Burnshock Facemask");
            Tooltip.SetDefault("10% increased magic damage\n7% increased magic critical strike chance\n12% less mana cost\n+75 max mana");
        }
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<BurnshockBodyArmor>() && legs.type == ModContent.ItemType<BurnshockChausses>() && head.type == ModContent.ItemType<BurnshockFacemask>();
		}
		public override void UpdateArmorSet(Player player)
		{
            player.setBonus = "11% increased magic damage\nTaking damage will release damaging shards of crystal";
            player.GetModPlayer<AeroPlayer>().BurnshockArmorBonus = true;
            player.magicDamage += 0.11f;
        } 	
        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 22;
            item.value = 10;
            item.rare = ItemRarityID.Orange;
            item.defense = 9;
        }
		public override void UpdateEquip(Player player)
        {
            player.magicDamage += 0.10f;
            player.magicCrit += 7;
            player.manaCost -= 0.12f;
            player.statManaMax2 += 75;
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