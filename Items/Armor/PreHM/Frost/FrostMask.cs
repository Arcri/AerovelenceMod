using AerovelenceMod.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Armor.PreHM.Frost
{
    [AutoloadEquip(EquipType.Head)]
    public class FrostMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Mask");
            Tooltip.SetDefault("+1 minion slot\nIncreased minion knockback");
        }
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<FrostMail>() && legs.type == ModContent.ItemType<FrostGreaves>() && head.type == ModContent.ItemType<FrostMask>();
		}
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Summons a Shiver thing to fight for you";
            if (Main.myPlayer == player.whoAmI && player.FindBuffIndex(mod.BuffType("ShiverMinion")) == -1)
            {
                player.AddBuff(mod.BuffType("ShiverMinionBuff"), 100, false);
                for (int m = 0; m < 1; m++) { Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, mod.ProjectileType("ShiverMinion"), (int)(25f * player.minionDamage), player.minionKB, player.whoAmI); }
            }
        }
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 10;
            item.rare = ItemRarityID.Orange;
            item.defense = 5;
        }
		public override void UpdateEquip(Player player)
        {
            player.maxMinions += 1;
            player.minionKB += 0.1f;
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<FrostShard>(), 8);
            modRecipe.AddIngredient(ModContent.ItemType<KelvinCore>(), 1);
            modRecipe.AddIngredient(ItemID.IceBlock, 35);
            modRecipe.AddIngredient(ItemID.HellstoneBar, 8);
            modRecipe.AddTile(TileID.Anvils);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}