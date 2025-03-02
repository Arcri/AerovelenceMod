using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using AerovelenceMod.Common.Utilities;
using Terraria.Localization;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;

namespace AerovelenceMod.Content.Items.Sets.Phantic
{
    public class PhanticBar : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<PhanticBarTile>());
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarities.MidPHM;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpectreBar, 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class PhanticBarTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 1100;
            Main.tileSolid[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(110, 074, 056), Language.GetText("Phantic Bar"));
        }
    }
}