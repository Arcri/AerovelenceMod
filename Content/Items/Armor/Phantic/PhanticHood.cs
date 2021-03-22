using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Phantic
{
    [AutoloadEquip(EquipType.Head)]
    public class PhanticHood : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantic Hood");
            Tooltip.SetDefault("5% increased ranged damage");
        }
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<PhanticCuirass>() && legs.type == ModContent.ItemType<PhanticCuisses>() && head.type == ModContent.ItemType<PhanticHood>();
		}
		public override void UpdateArmorSet(Player player)
		{
            player.setBonus = "Hitting an enemy with a Ranged projectile has a chance to spawn a Phantic Soul.";
            player.GetModPlayer<AeroPlayer>().PhanticRangedBonus = true;
        } 	
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.sellPrice(0, 1, 46, 34);
            item.rare = ItemRarityID.Green;
            item.defense = 3;
        }
		public override void UpdateEquip(Player player)
        {
			player.rangedDamage += 0.05f;

        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<PhanticBar>(), 12);
            modRecipe.AddRecipeGroup("AerovelenceMod:EvilMaterials", 10);
            modRecipe.AddTile(TileID.Anvils);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}