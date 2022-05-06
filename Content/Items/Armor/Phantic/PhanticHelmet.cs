using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Phantic
{
    [AutoloadEquip(EquipType.Head)]
    public class PhanticHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantic Helmet");
            Tooltip.SetDefault("5% increased melee damage");
        }
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<PhanticCuirass>() && legs.type == ModContent.ItemType<PhanticCuisses>() && head.type == ModContent.ItemType<PhanticHelmet>();
		}
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Taking over 10 damage will spawn a homing soul to chase your foes";
            player.GetModPlayer<AeroPlayer>().PhanticMeleeBonus = true;
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = 10;
            Item.rare = ItemRarityID.Green;
            Item.defense = 6;
        }
		public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Melee) += 0.5f;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<PhanticBar>(), 12)
                .AddRecipeGroup("AerovelenceMod:EvilMaterials", 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}