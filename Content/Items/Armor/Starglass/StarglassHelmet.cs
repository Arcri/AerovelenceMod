using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Starglass
{
    [AutoloadEquip(EquipType.Head)]
    public class StarglassHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Starglass Helmet");
            Tooltip.SetDefault("+2 max minion slots and 5% increased minion damage");
        }
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<StarglassChestplate>() && legs.type == ModContent.ItemType<StarglassGrieves>() && head.type == ModContent.ItemType<StarglassHelmet>();
		}
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "20% increased minion damage and increased minion knockback\nTaking damage will release damaging shards of crystal";
            player.GetModPlayer<AeroPlayer>().BurnshockArmorBonus = true;
            player.GetDamage(DamageClass.Summon) += 0.15f;
            player.GetKnockback(DamageClass.Summon).Base += 0.05f;
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = 10;
            Item.rare = ItemRarityID.Orange;
            Item.defense = 4;
        }
		public override void UpdateEquip(Player player)
        {
            player.maxMinions += 2;
            player.GetDamage(DamageClass.Summon) += 0.05f;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<BurnshockBar>(), 8)
                .AddIngredient(ItemID.CrystalShard, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}