using AerovelenceMod.Content.Items.Placeables.Blocks;
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
            DisplayName.SetDefault("Slate Helmet");
            Tooltip.SetDefault("2% increased damage");
        }
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<SlateChestplate>() && legs.type == ModContent.ItemType<SlateLeggings>() && head.type == ModContent.ItemType<SlateHelmet>();
		}
		public override void UpdateArmorSet(Player player)
		{

            var ap = player.GetModPlayer<AeroPlayer>();

            int axeProjectileType = ModContent.ProjectileType<Projectiles.Other.ArmorSetBonus.LumberjackAxe>();
            if (player.ownedProjectileCounts[axeProjectileType] < 1)
            {
                Projectile.NewProjectile(player.Center, default, axeProjectileType, 25, 0.5f, player.whoAmI);
            }
            ap.lumberjackSetBonus = true;
            player.setBonus = "Defense and melee speed increased slightly while in the cavern layer\nIncreases all damage by 10% and summoning damage by 15%\nThe Slate sword will now shoot a rock that explodes\nA sharp axe accompanies you...";
			if(player.ZoneRockLayerHeight)
            {
                player.statDefense += 7;
                player.meleeSpeed += 0.05f;
            }
            player.allDamage += 0.10f;
            player.minionDamage += 0.15f;

        } 	
        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 22;
            item.value = 10;
            item.rare = ItemRarityID.Blue;
            item.defense = 3;
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
            recipe.AddIngredient(ModContent.ItemType<SlateOre>(), 55);
            recipe.AddRecipeGroup("Wood", 20);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}