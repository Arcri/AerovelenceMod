using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using AerovelenceMod.Common.Utilities;

namespace AerovelenceMod.Content.Items.Sets.Phantic
{
    public class PhanticPickaxe : ModItem
    {
        public override void SetDefaults()
        {
            Item.crit = 4;
            Item.damage = 11;
            Item.DamageType = DamageClass.Melee;
            Item.width = 34;
            Item.height = 34;
            Item.useTime = 13;
            Item.useAnimation = 13;
            Item.useTurn = true;
            Item.pick = 70;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarities.MidPHM;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<PhanticBar>(), 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}