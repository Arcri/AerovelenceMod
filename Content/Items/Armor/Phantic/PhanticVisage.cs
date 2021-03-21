using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Phantic
{
    [AutoloadEquip(EquipType.Head)]
    public class PhanticVisage : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantic Visage");
            Tooltip.SetDefault("5% increased magic damage\n+30 max mana");
        }
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<PhanticCuirass>() && legs.type == ModContent.ItemType<PhanticCuisses>() && head.type == ModContent.ItemType<PhanticVisage>();
		}
		public override void UpdateArmorSet(Player player)
		{
            player.setBonus = "Firing a magic weapon has a chance to also call forth a Phantic Soul.";
            player.GetModPlayer<AeroPlayer>().PhanticMagicBonus = true;
        } 	
        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 22;
            item.value = 10;
            item.rare = ItemRarityID.Green;
            item.defense = 3;
        }
		public override void UpdateEquip(Player player)
        {
            player.magicDamage += 0.05f;
            player.statManaMax2 += 30;
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