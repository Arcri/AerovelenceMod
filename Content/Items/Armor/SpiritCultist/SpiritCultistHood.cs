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
            player.allDamage += 0.15f;
            player.wingTimeMax *= 2;
            player.jumpSpeedBoost += 1.5f;
            player.setBonus = "+15% increase to all damage\nIncreased flight time\nIncreased jump speed\n10% increased movement speed\nYou leave a trail of deadly spirits";
            AeroPlayer modPlayer = (AeroPlayer)player.GetModPlayer(mod, "AeroPlayer");
            modPlayer.SpiritCultistBonus = true;
        }
    
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.sellPrice(0, 1, 30, 0);
            item.rare = ItemRarityID.Yellow;
            item.defense = 13;
        }
		public override void UpdateEquip(Player player)
        {
			player.rangedDamage += 0.05f;
            player.rangedCrit += 8;
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<CrystalShard>(), 8);
            modRecipe.AddIngredient(ItemID.FallenStar, 6);
            modRecipe.AddIngredient(ModContent.ItemType<EmeraldEmpoweredGem>(), 1);
            modRecipe.AddTile(TileID.MythrilAnvil);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}