using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Frost
{
    [AutoloadEquip(EquipType.Head)]
    public class FrostHeadgear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Headgear");
            Tooltip.SetDefault("10% increased ranged damage");
        }
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<FrostMail>() && legs.type == ModContent.ItemType<FrostGreaves>() && head.type == ModContent.ItemType<FrostHeadgear>();
		}
		public override void UpdateArmorSet(Player player)
		{
            player.setBonus = "50% chance to inflict Frostburn on enemies\nImmune to Chilled and Frostburn debuffs";
            player.buffImmune[BuffID.Chilled] = true;
            player.buffImmune[BuffID.Frostburn] = true;
            player.GetModPlayer<AeroPlayer>().FrostProjectile = true;
        } 	
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(0, 1, 46, 34);
            Item.rare = ItemRarityID.Orange;
            Item.defense = 7;
        }
		public override void UpdateEquip(Player player)
        {
			player.GetDamage(DamageClass.Ranged) += 0.1f;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<FrostShard>(), 8)
                .AddIngredient(ModContent.ItemType<KelvinCore>(), 1)
                .AddIngredient(ItemID.IceBlock, 35)
                .AddIngredient(ItemID.HellstoneBar, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}