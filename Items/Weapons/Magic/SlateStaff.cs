using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Items.Placeable.Ores;

namespace AerovelenceMod.Items.Weapons.Magic
{
    public class SlateStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
			Item.staff[item.type] = true;
            DisplayName.SetDefault("Slate Staff");
        }
        public override void SetDefaults()
        {
            item.crit = 4;
            item.damage = 25;
            item.magic = true;
            item.mana = 4;
            item.width = 50;
            item.height = 50;
            item.useTime = 20;
            item.useAnimation = 20;
            item.UseSound = SoundID.Item21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3;
            item.value = Item.sellPrice(0, 0, 40, 0);
            item.rare = ItemRarityID.Blue;
            item.autoReuse = true;
            item.shoot = ProjectileID.RubyBolt;
            item.shootSpeed = 10f;
		}
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SlateOreItem>(), 35);
            recipe.AddRecipeGroup("Wood", 15);
            recipe.AddIngredient(ItemID.Ruby, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

