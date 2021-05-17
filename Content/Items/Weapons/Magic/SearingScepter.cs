using AerovelenceMod.Content.Items.Placeables.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class SearingScepter : ModItem
    {
        public override void SetStaticDefaults()
        {
			Item.staff[item.type] = true;
            DisplayName.SetDefault("Searing Scepter");
        }
        public override void SetDefaults()
        {
            item.crit = 4;
            item.damage = 17;
            item.magic = true;
            item.mana = 5;
            item.width = 50;
            item.height = 50;
            item.useTime = 16;
            item.useAnimation = 16;
            item.UseSound = SoundID.Item21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3;
            item.value = Item.sellPrice(0, 0, 40, 0);
            item.rare = ItemRarityID.Blue;
            item.autoReuse = true;
            item.shoot = ProjectileID.RubyBolt;
            item.shootSpeed = 12f;
		}
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SlateOre>(), 35);
            recipe.AddRecipeGroup("Wood", 15);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

