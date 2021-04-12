using AerovelenceMod.Content.Items.Others.Crafting;
using AerovelenceMod.Content.Projectiles.Other.ArmorSetBonus;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Burnshock
{
    [AutoloadEquip(EquipType.Head)]
    public class BurnshockVisor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Burnshock Visor");
            Tooltip.SetDefault("+3 max minion slots and 15% increased minion damage");
        }
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<BurnshockBodyArmor>() && legs.type == ModContent.ItemType<BurnshockChausses>() && head.type == ModContent.ItemType<BurnshockVisor>();
		}
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "20% increased minion damage and increased minion knockback\nTaking damage will release damaging shards of crystal\nCommand a projectile blocking shield";
            player.minionDamage += 0.20f;
            player.minionKB += 0.05f;

            AeroPlayer ap = player.GetModPlayer<AeroPlayer>();


            ap.BurnshockArmorBonus = true;

            if (ap.burnshockSetBonusCooldown > 0)
            {
                ap.burnshockSetBonusCooldown--;
            }
            else
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<BurnshockArmorProjectile>()] < 1)
                {
                    Projectile.NewProjectile(player.Center, default, ModContent.ProjectileType<BurnshockArmorProjectile>(), 0, 0, player.whoAmI);
                }
                player.AddBuff(ModContent.BuffType<Buffs.BurnshockShield>(), 2);
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
            player.maxMinions += 3;
            player.minionDamage += 0.15f;
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<BurnshockBar>(), 8);
            modRecipe.AddIngredient(ItemID.CrystalShard, 5);
            modRecipe.AddTile(TileID.MythrilAnvil);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}