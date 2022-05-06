using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Frost
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
            player.setBonus = "Summons a Shiver to fight for you [Not Implemented Yet]\nImmune to Chilled and Frostburn debuffs";
            player.buffImmune[BuffID.Chilled] = true;
            player.buffImmune[BuffID.Frostburn] = true;
            /*if (Main.myPlayer == player.whoAmI && player.FindBuffIndex(mod.BuffType("ShiverMinion")) == -1)
            {
                //player.AddBuff(mod.BuffType("ShiverMinionBuff"), 100, false);
                //for (int m = 0; m < 1; m++) { Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, mod.ProjectileType("ShiverMinion"), (int)(25f * player.minionDamage), player.minionKB, player.whoAmI); }
            }*/
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = 10;
            Item.rare = ItemRarityID.Orange;
            Item.defense = 5;
        }
		public override void UpdateEquip(Player player)
        {
            player.maxMinions += 1;
            player.GetKnockback(DamageClass.Summon).Base += 0.1f;
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