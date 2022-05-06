using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Items.Placeables.CrystalCaverns;
using AerovelenceMod.Content.Items.Placeables.Furniture.Glimmering;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles.Furniture
{
    public class GlimmeringSink : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = false;
            Main.tileLavaDeath[Type] = false;
            dustType = DustType<Sparkle>();
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.addTile(Type);
            adjTiles = new int[] { TileID.Sinks };
            AddMapEntry(new Color(068, 077, 098));
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 48, 48, ModContent.ItemType<Items.Placeables.Furniture.Glimmering.GlimmeringSink>());
        }
    }
}