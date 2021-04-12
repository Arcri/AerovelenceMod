using AerovelenceMod.Content.Items.Others.Crafting;
using AerovelenceMod.Content.Projectiles.Other.ArmorSetBonus;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Burnshock
{
    [AutoloadEquip(EquipType.Head)]
    public class BurnshockGreathelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Burnshock Greathelm");
            Tooltip.SetDefault("10% increased melee damage and swing speed\n8% increased melee critical strike chance");
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<BurnshockBodyArmor>() && legs.type == ModContent.ItemType<BurnshockChausses>() && head.type == ModContent.ItemType<BurnshockGreathelm>();
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "11% increased melee damage\nTaking damage will release damaging shards of crystal\nCommand a projectile blocking shield";
            player.meleeDamage += 0.11f;

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
            item.defense = 27;
        }
        public override void UpdateEquip(Player player)
        {
            player.meleeDamage += 0.10f;
            player.meleeSpeed += 0.10f;
            player.meleeCrit += 8;
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