using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Glimmerwood;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Tools
{
    public class GlimmerwoodHamaxe : TranslatableModItem
    {
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Glimmerwood Hamaxe", "")
                .AddName(Language.Spanish, "Hacha Martillo de Madera Reluciente");
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 50;
            Item.damage = 5;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 2f;
            Item.axe = 7;
            Item.hammer = 25;
            Item.useTime = 20;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(copper: 40);
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<GlimmerwoodItem>(12).AddTile(TileID.WorkBenches).Register();
    }
}
