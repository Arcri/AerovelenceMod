using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Starglass


{
    [AutoloadEquip(EquipType.Head)]
    public class StarglassVisage : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Starglass Visage");
            Tooltip.SetDefault("10% increased magic damage\n7% increased magic critical strike chance\n12% less mana cost\n+75 max mana");
        }
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<StarglassChestplate>() && legs.type == ModContent.ItemType<StarglassGrieves>() && head.type == ModContent.ItemType<StarglassVisage>();
		}
		public override void UpdateArmorSet(Player player)
		{
            player.setBonus = "11% increased magic damage\nTaking damage will release damaging shards of crystal";
            player.GetModPlayer<AeroPlayer>().BurnshockArmorBonus = true;
            player.GetDamage(DamageClass.Magic) += 0.11f;
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
            player.GetDamage(DamageClass.Magic) += 0.10f;
            player.GetCritChance(DamageClass.Magic) += 7;
            player.manaCost -= 0.12f;
            player.statManaMax2 += 75;
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