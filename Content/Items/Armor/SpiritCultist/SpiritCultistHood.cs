using AerovelenceMod.Content.Items.Accessories;
using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.SpiritCultist
{
    [AutoloadEquip(EquipType.Head)]
    public class SpiritCultistHood : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spirit Cultist Hood");
            Tooltip.SetDefault("5% increased damage");
        }
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<SpiritCultistCloak>() && legs.type == ModContent.ItemType<SpiritCultistLeggings>() && head.type == ModContent.ItemType<SpiritCultistHood>();
		}
		public override void UpdateArmorSet(Player player)
		{
            player.GetDamage(DamageClass.Generic) += 0.15f;
            player.wingTimeMax *= 2;
            player.jumpSpeedBoost += 1.5f;
            player.setBonus = "+15% increase to all damage\nIncreased flight time\nIncreased jump speed\n10% increased movement speed\nYou leave a trail of deadly spirits";
            AeroPlayer modPlayer = player.GetModPlayer<AeroPlayer>();
            modPlayer.SpiritCultistBonus = true;
        }
    
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(0, 1, 30, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 13;
        }
		public override void UpdateEquip(Player player)
        {
			player.GetDamage(DamageClass.Ranged) += 0.05f;
            player.GetCritChance(DamageClass.Ranged) += 8;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<CrystalShard>(), 8)
                .AddIngredient(ItemID.FallenStar, 6)
                .AddIngredient(ModContent.ItemType<EmeraldEmpoweredGem>(), 1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}