using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Seashine
{
    [AutoloadEquip(EquipType.Body)]
    public class SeashineBodyArmor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seashine Body Armor");
            Tooltip.SetDefault("3% increased critical strike chance\n2% increased minion knockback");
        } 			
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = 10;
            Item.rare = ItemRarityID.Green;
            Item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {
			player.GetCritChance(DamageClass.Magic) += 3;
            player.GetCritChance(DamageClass.Ranged) += 3;
            player.GetCritChance(DamageClass.Melee) += 3;
            player.GetKnockback(DamageClass.Summon).Base += 0.02f;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.SandBlock, 25)
                .AddIngredient(ItemID.Seashell, 5)
                .AddIngredient(ItemID.Starfish, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}