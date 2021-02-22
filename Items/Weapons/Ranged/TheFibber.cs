using AerovelenceMod.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class TheFibber : ModItem
    {
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-6, 0);
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Fibber");
            Tooltip.SetDefault("Permanently increases maximum life by 50\n" +
                               "1500% increased damage on hit\n" +
                               "200% increased critical strike chance on hit\n" +
                               "50% love\n" +
                               "'Has no match, will always give maximum DPS'");
        }
        public override void SetDefaults()
        {
            item.crit = 20;
            item.damage = 132;
            item.ranged = true;
            item.width = 50;
            item.height = 24;
            item.useTime = 21;
            item.useAnimation = 21;
            item.UseSound = SoundID.Item36;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 8;
            item.value = 10000;
            item.rare = ItemRarityID.Yellow;
            item.autoReuse = true;
            item.shoot = AmmoID.Bullet;
            item.useAmmo = AmmoID.Bullet;
            item.shootSpeed = 24f;
        }

        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<EmberFragment>(), 6);
            modRecipe.AddIngredient(ItemID.LunarBar, 25);
            modRecipe.AddIngredient(ItemID.FragmentVortex, 30);
            modRecipe.AddIngredient(ItemID.VortexBeater, 1);
            modRecipe.AddIngredient(ItemID.VenusMagnum, 1);
            modRecipe.AddIngredient(ItemID.PhoenixBlaster, 1);
            modRecipe.AddTile(TileID.Anvils);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}