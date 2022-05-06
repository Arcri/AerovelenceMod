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
            Item.width = 22;
            Item.height = 22;
            Item.value = 10;
            Item.rare = ItemRarityID.Green;
            Item.defense = 3;
        }
		public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic) += 0.05f;
            player.statManaMax2 += 30;
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