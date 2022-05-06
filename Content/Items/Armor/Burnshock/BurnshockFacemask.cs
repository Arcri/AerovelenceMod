using AerovelenceMod.Content.Items.Others.Crafting;
using AerovelenceMod.Content.Projectiles.Other.ArmorSetBonus;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Burnshock
{
    [AutoloadEquip(EquipType.Head)]
    public class BurnshockFacemask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Burnshock Facemask");
            Tooltip.SetDefault("10% increased magic damage\n7% increased magic critical strike chance\n12% less mana cost\n+75 max mana");
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<BurnshockBodyArmor>() && legs.type == ModContent.ItemType<BurnshockChausses>() && head.type == ModContent.ItemType<BurnshockFacemask>();
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "11% increased magic damage\nTaking damage will release damaging shards of crystal\nCommand a projectile blocking shield";
            player.GetDamage(DamageClass.Magic) += 0.11f;

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
            Item.width = 22;
            Item.height = 22;
            Item.value = 10;
            Item.rare = ItemRarityID.Orange;
            Item.defense = 9;
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