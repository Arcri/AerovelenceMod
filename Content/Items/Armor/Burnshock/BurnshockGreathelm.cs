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
            player.GetDamage(DamageClass.Melee) += 0.11f;

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
            Item.width = 18;
            Item.height = 18;
            Item.value = 10;
            Item.rare = ItemRarityID.Orange;
            Item.defense = 27;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Melee) += 0.10f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.10f;
            player.GetCritChance(DamageClass.Melee) += 8;
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