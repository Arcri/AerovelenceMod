using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Frost
{
    [AutoloadEquip(EquipType.Head)]
    public class FrostHood : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Hood");
            Tooltip.SetDefault("10% increased magic damage\n10% less mana cost\n+35 max mana");
        }
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<FrostMail>() && legs.type == ModContent.ItemType<FrostGreaves>() && head.type == ModContent.ItemType<FrostHood>();
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
            Item.width = 22;
            Item.height = 22;
            Item.value = 10;
            Item.rare = ItemRarityID.Orange;
            Item.defense = 6;
        }
		public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic) += 0.1f;
            player.manaCost -= 0.1f;
            player.statManaMax2 += 35;
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