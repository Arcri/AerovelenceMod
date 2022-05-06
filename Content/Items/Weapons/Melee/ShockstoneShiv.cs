using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class ShockstoneShiv : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electrified Shiv");
        }
        public override void SetDefaults()
        {
            Item.useTurn = true;
            Item.crit = 20;
            Item.damage = 80;
            Item.DamageType = DamageClass.Melee;
            Item.width = 44;
            Item.height = 44;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 0, 65, 20);
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = false;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<BurnshockBar>(), 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}