using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Frost
{
    [AutoloadEquip(EquipType.Body)]
    public class FrostMail : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Plate Mail");
            Tooltip.SetDefault("10% increased critical strike chance\nImmunity to Frostburn and Frozen");
        } 			
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 10;
            item.rare = ItemRarityID.Orange;
            item.defense = 11;
        }
        public override void UpdateEquip(Player player)
        {
			player.magicCrit += 10;
            player.rangedCrit += 10;
            player.meleeCrit += 10;
            player.buffImmune[BuffID.Frozen] = true;
            player.buffImmune[BuffID.Frostburn] = true;
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<FrostShard>(), 15);
            modRecipe.AddIngredient(ModContent.ItemType<KelvinCore>(), 1);
            modRecipe.AddIngredient(ItemID.IceBlock, 40);
            modRecipe.AddIngredient(ItemID.HellstoneBar, 12);
            modRecipe.AddTile(TileID.Anvils);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}