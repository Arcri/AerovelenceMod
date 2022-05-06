using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
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
            Item.crit = 20;
            Item.damage = 132;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 50;
            Item.height = 24;
            Item.useTime = 21;
            Item.useAnimation = 21;
            Item.UseSound = SoundID.Item36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 8;
            Item.value = 10000;
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;
            Item.shoot = AmmoID.Bullet;
            Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 24f;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<EmberFragment>(), 6)
                .AddIngredient(ItemID.LunarBar, 25)
                .AddIngredient(ItemID.FragmentVortex, 30)
                .AddIngredient(ItemID.VortexBeater, 1)
                .AddIngredient(ItemID.VenusMagnum, 1)
                .AddIngredient(ItemID.PhoenixBlaster, 1)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}