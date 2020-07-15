using AerovelenceMod.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Armor.PreHM.Adobe
{
    [AutoloadEquip(EquipType.Head)]
    public class AdobeHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Adobe Helmet");
            Tooltip.SetDefault("2% increased damage");
        }
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<AdobeChestplate>() && legs.type == ModContent.ItemType<AdobeLeggings>() && head.type == ModContent.ItemType<AdobeHelmet>();
		}
		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = "Potato";
			player.AddBuff(BuffID.Ravens, -1);
		} 	
        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 22;
            item.value = 10;
            item.rare = ItemRarityID.Green;
            item.defense = 7;
        }
        public override void UpdateEquip(Player player)
        {
            player.meleeDamage += 0.02f;
			player.rangedDamage += 0.02f;
			player.magicDamage += 0.02f;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AdobeBrick>(), 25);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}