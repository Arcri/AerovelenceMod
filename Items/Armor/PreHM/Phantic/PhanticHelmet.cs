using AerovelenceMod.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Armor.PreHM.Phantic
{
    [AutoloadEquip(EquipType.Head)]
    public class PhanticHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantic Helmet");
            Tooltip.SetDefault("6% increased melee damage");
        }
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<PhanticCuirass>() && legs.type == ModContent.ItemType<PhanticCuisses>() && head.type == ModContent.ItemType<PhanticHelmet>();
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
            item.value = 10;
            item.rare = ItemRarityID.Orange;
            item.defense = 4;
        }
		public override void UpdateEquip(Player player)
        {
            player.meleeDamage *= 0.6f;
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