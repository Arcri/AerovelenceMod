using AerovelenceMod.Content.Items.Placeables.Blocks;
using AerovelenceMod.Content.Items.Placeables.CrystalCaverns;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.AmbrosiaMiningSet
{
    [AutoloadEquip(EquipType.Head)]
    public class AmbrosiaMiningHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ambrosia Mining Helmet");
        }
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<AmbrosiaMiningChestplate>() && legs.type == ModContent.ItemType<AmbrosiaMiningBoots>() && head.type == ModContent.ItemType<AmbrosiaMiningHelmet>();
		}
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "20% increased digging speed\nPress your armor bonus hotkey to shoot energy that can explode tiles";
            player.GetModPlayer<AeroPlayer>().AmbrosiaBonus = true;
            player.pickSpeed -= 0.59f;
        }
        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 22;
            item.value = 10;
            item.rare = ItemRarityID.Blue;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("AerovelenceMod:GoldBars", 12);
            recipe.AddIngredient(ModContent.ItemType<CavernCrystal>(), 5);
            recipe.AddIngredient(ItemID.Ruby, 2);
            recipe.AddIngredient(ItemID.Topaz, 2);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}