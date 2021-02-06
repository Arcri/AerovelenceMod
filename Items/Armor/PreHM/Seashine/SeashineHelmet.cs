using AerovelenceMod.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Armor.PreHM.Seashine
{
    [AutoloadEquip(EquipType.Head)]
    public class SeashineHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seashine Helmet");
            Tooltip.SetDefault("6% increased summoning damage\n8% less mana cost\n+1 summon slot");
        }
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<SeashineBodyArmor>() && legs.type == ModContent.ItemType<SeashineLeggings>() && head.type == ModContent.ItemType<SeashineHelmet>();
		}
		public override void UpdateArmorSet(Player player)
		{
            player.setBonus = "Summons a flying crab to protect you\nMovement speed in water heavily increased";
            if (player.wet)
            {
                player.moveSpeed += 0.15f;
            }
        } 	
        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 22;
            item.value = 10;
            item.rare = ItemRarityID.Green;
            item.defense = 1;
        }
		public override void UpdateEquip(Player player)
        {
            player.minionDamage += 0.06f;
            player.manaCost -= 0.08f;
            player.maxMinions += 1;
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ItemID.SandBlock, 20);
            modRecipe.AddIngredient(ItemID.Seashell, 5);
            modRecipe.AddIngredient(ItemID.Starfish, 3);
            modRecipe.AddTile(TileID.Anvils);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}