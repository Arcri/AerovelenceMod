using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles.Furniture
{
    public class CavernStatue : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.CoordinateHeights = new int[]
            {
            16,
            16,
            16
            };
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Cavern Statue");
            AddMapEntry(new Color(200, 200, 200), name);
            adjTiles = new int[] { TileID.Lamps };
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Main.PlaySound(new Terraria.Audio.LegacySoundStyle(3, 4));
            Item.NewItem(i * 16, j * 16, 48, 48, 441);
        }
    }
}