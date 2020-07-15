using AerovelenceMod.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Thrown
{
    public class SoulChakram : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Soul Chakram");
		}
        public override void SetDefaults()
        {	
			item.crit = 20;
            item.damage = 20;
            item.melee = true;
            item.width = 20;
            item.height = 30;
            item.useTime = 24;
            item.useAnimation = 24;
			item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
			item.noUseGraphic = true;
            item.knockBack = 3;
            item.value = Item.sellPrice(0, 2, 45, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = false;
            item.shoot = mod.ProjectileType("SoulChakramProjectile");
            item.UseSound = SoundID.Item1;
            item.shootSpeed = 10f;
        }


        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<PhanticBar>(), 6);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool CanUseItem(Player player)
        {
            for (int i = 0; i < 1000; ++i)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == item.shoot)
                {
                    return false;
                }
            }
            return true;
        }
    }
}