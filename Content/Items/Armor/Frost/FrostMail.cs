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
            Item.width = 18;
            Item.height = 18;
            Item.value = 10;
            Item.rare = ItemRarityID.Orange;
            Item.defense = 11;
        }
        public override void UpdateEquip(Player player)
        {
			player.GetCritChance(DamageClass.Magic) += 10;
            player.GetCritChance(DamageClass.Ranged) += 10;
            player.GetCritChance(DamageClass.Melee) += 10;
            player.buffImmune[BuffID.Frozen] = true;
            player.buffImmune[BuffID.Frostburn] = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<FrostShard>(), 15)
                .AddIngredient(ModContent.ItemType<KelvinCore>(), 1)
                .AddIngredient(ItemID.IceBlock, 40)
                .AddIngredient(ItemID.HellstoneBar, 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}