using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;

namespace AerovelenceMod.Blocks.CrystalCaverns.Tiles
{
    public class CitadelBanner : ModTile
    {
        public override void SetDefaults()
        {
            mineResist = 2.5f;
            Main.tileSolid[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            AddMapEntry(new Color(102, 108, 117));
            dustType = 116;
            soundType = SoundID.Tink;
            drop = mod.ItemType("CrystalDirtItem");
        }
        public override bool CanExplode(int i, int j)
        {
            return true;
        }
    }
}