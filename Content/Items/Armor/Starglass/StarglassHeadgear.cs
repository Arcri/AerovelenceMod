using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Starglass




{
    [AutoloadEquip(EquipType.Head)]
    public class StarglassHeadgear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Starglass Headgear");
            Tooltip.SetDefault("10% increased ranged damage\n8% increased ranged critical strike chance");
        }
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<StarglassChestplate>() && legs.type == ModContent.ItemType<StarglassGrieves>() && head.type == ModContent.ItemType<StarglassHeadgear>();
		}
		public override void UpdateArmorSet(Player player)
		{
            player.setBonus = "11% increased ranged damage\nTaking damage will release damaging shards of crystal";
            player.GetModPlayer<AeroPlayer>().BurnshockArmorBonus = true;
            player.GetDamage(DamageClass.Ranged) += 0.11f;
        } 	
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(0, 1, 30, 0);
            Item.rare = ItemRarityID.Orange;
            Item.defense = 10;
        }
		public override void UpdateEquip(Player player)
        {
			player.GetDamage(DamageClass.Ranged) += 1.00f;
            player.GetCritChance(DamageClass.Ranged) += 8;
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