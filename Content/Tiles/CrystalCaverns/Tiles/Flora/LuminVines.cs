using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
//credit to GabeHasWon (again)
namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles.Flora
{
    internal class LuminVines : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileBlockLight[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileLighted[Type] = false;
            Main.tileSolid[Type] = false;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = false;
            Main.tileFrameImportant[Type] = true;
            //TileObjectData.newTile.AnchorValidTiles = new int[] { TileType<VerdantSoilGrass>(), Type };
            //TileObjectData.addTile(Type);
            AddMapEntry(new Color(089, 161, 244));
            dustType = DustID.Grass;
            soundType = SoundID.Grass;
            minPick = 20;
            Main.tileAxe[Type] = true;
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            Color colour = Color.White;

            Texture2D glow = ModContent.GetTexture("AerovelenceMod/Content/Tiles/CrystalCaverns/Tiles/Flora/LuminVines_Glow");
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            spriteBatch.Draw(glow, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(tile.frameX, tile.frameY, 16, 16), colour);
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = 59;

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tileBelow = Framing.GetTileSafely(i, j - 1);
            Tile tileAbove = Framing.GetTileSafely(i, j + 1);
            if (!tileBelow.active() && !tileBelow.active() && tileBelow.type != TileType<CavernStone>() && tileBelow.liquid > 250)
            {
                Framing.GetTileSafely(i, j).frameX = 36;
                Framing.GetTileSafely(i, j).frameY = (short)(Main.rand.Next(2) * 18);
            }
            else if (!tileAbove.active() && !tileAbove.active() && tileAbove.type != TileType<CavernStone>() && tileBelow.liquid > 250)
            {
                Framing.GetTileSafely(i, j).frameX = 18;
                Framing.GetTileSafely(i, j).frameY = (short)(Main.rand.Next(2) * 18);
            }
            else
            {
                Framing.GetTileSafely(i, j).frameX = 0;
                Framing.GetTileSafely(i, j).frameY = (short)(Main.rand.Next(2) * 18);
            }

            if (!tileAbove.active() && !tileAbove.active() && tileAbove.type != TileType<CavernStone>() && tileBelow.liquid > 250 && !tileBelow.active() && !tileBelow.active() && (tileBelow.type != TileType<CavernStone>() && tileBelow.liquid > 250))
            {
                Framing.GetTileSafely(i, j).frameX = 0;
                Framing.GetTileSafely(i, j).frameY = (short)((Main.rand.Next(2) * 18) + 36);
            }

            //if (Framing.GetTileSafely(i, j).frameX == 0)
            //    Framing.GetTileSafely(i, j).frameY = (short)(Main.rand.Next(2) * 18);
            //else
            {
                if (Framing.GetTileSafely(i, j).frameY < 36)
                    Framing.GetTileSafely(i, j).frameY = (short)(Main.rand.Next(2) * 18);
                else
                    Framing.GetTileSafely(i, j).frameY = (short)((Main.rand.Next(2) * 18) + 36);
            }
            return false;
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.0f;
            g = 0.6f;
            b = 0.9f;
        }

        public override void RandomUpdate(int i, int j)
        {
            Tile tileBelow = Framing.GetTileSafely(i, j - 1);
            Tile tileAbove = Framing.GetTileSafely(i, j + 1);

            if (!Main.tile[i, j + 1].active() && tileAbove.liquid > 250 && Main.rand.Next(7) == 0)
                WorldGen.PlaceTile(i, j + 1, Type);
            if (!Main.tile[i, j - 1].active() && tileBelow.liquid > 250 && Main.rand.Next(7) == 0)
                WorldGen.PlaceTile(i, j - 1, Type);

            if (Framing.GetTileSafely(i, j).frameX != 0 && Framing.GetTileSafely(i, j).frameY < 36 && Main.rand.Next(2) == 0)
            {
                Framing.GetTileSafely(i, j).frameY = (short)((Main.rand.Next(2) * 18) + 36);
            }
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Lighting.AddLight(new Vector2(i, j) * 16, new Vector3(0.0f, 0.20f, 0.60f));
        }
    }
}