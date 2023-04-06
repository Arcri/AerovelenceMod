using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Slate
{
    [AutoloadEquip(EquipType.Head)]
    public class SlateHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slate Helmet");
            // Tooltip.SetDefault("2% increased damage");
        }
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<SlateChestplate>() && legs.type == ModContent.ItemType<SlateLeggings>() && head.type == ModContent.ItemType<SlateHelmet>();
		}
		public override void UpdateArmorSet(Player player)
		{

            var ap = player.GetModPlayer<AeroPlayer>();

            //int axeProjectileType = ModContent.ProjectileType<Projectiles.Other.ArmorSetBonus.LumberjackAxe>();
            //if (player.ownedProjectileCounts[axeProjectileType] < 1)
            //{
                //Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, default, axeProjectileType, 25, 0.5f, player.whoAmI);
            //}
            //ap.lumberjackSetBonus = true;
            player.setBonus = "Defense and melee speed increased slightly while in the cavern layer\nIncreases all damage by 10% and summoning damage by 15%\nThe Slate sword will now shoot a rock that explodes\nA sharp axe accompanies you...";
			if(player.ZoneRockLayerHeight)
            {
                player.statDefense += 7;
                player.GetAttackSpeed(DamageClass.Melee) += 0.05f;
            }
            player.GetDamage(DamageClass.Ranged) += 0.10f;
            player.GetDamage(DamageClass.Summon) += 0.15f;

        } 	
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.value = 10;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 3;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Melee) += 0.02f;
			player.GetDamage(DamageClass.Ranged) += 0.02f;
			player.GetDamage(DamageClass.Magic) += 0.02f;
        }
        public override void AddRecipes()
        {
            /*
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<SlateOre>(), 55)
                .AddRecipeGroup("Wood", 20)
                .AddTile(TileID.Anvils)
                .Register();
            */
        }
    }
}