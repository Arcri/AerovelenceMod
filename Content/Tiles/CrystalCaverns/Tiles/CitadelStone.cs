using AerovelenceMod.Content.Items.Placeables.CrystalCaverns;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles
{
    public class CitadelStone : ModTile
    {
        public override void SetDefaults()
        {
            mineResist = 2.5f;
            minPick = 40;
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][mod.TileType("CrystalDirt")] = true;
            Main.tileMerge[Type][mod.TileType("CrystalGrass")] = true;
            Main.tileMerge[Type][mod.TileType("FieldStone")] = true;
            Main.tileMerge[Type][mod.TileType("CavernStone")] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = true;
            Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(046, 045, 072));
            dustType = 59;
            soundType = SoundID.Tink;
            drop = ModContent.ItemType<Items.Placeables.Blocks.CitadelStone>();

        }
        public static Vector2 TileOffset => Lighting.lightMode > 1 ? Vector2.Zero : Vector2.One * 12;

        public static Vector2 TileCustomPosition(int i, int j, Vector2? off = null)
        {
            return ((new Vector2(i, j) + TileOffset) * 16) - Main.screenPosition - (off ?? new Vector2(0));
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }
            int height = tile.frameY == 36 ? 18 : 16;
            Main.spriteBatch.Draw(mod.GetTexture("Blocks/CrystalCaverns/Tiles/CitadelStone_Glowmask"), new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.frameX, tile.frameY, 16, height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}