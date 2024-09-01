using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Localization;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture.Items;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture
{
    public class CrystallineFabricator : ModTile
    {
        private Asset<Texture2D> glowTexture;

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(123, 123, 123), Language.GetText("MapObject.CrystallineFabricator"));

            DustType = DustID.BlueCrystalShard;
            AnimationFrameHeight = 54;

            glowTexture = ModContent.Request<Texture2D>(ModContent.GetInstance<CrystallineFabricator>().Texture + "_Glow");
            Main.tileFrame[Type] = 6;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter >= 6)
            {
                frameCounter = 0;
                frame++;
                if (frame >= Main.tileFrame[Type])
                {
                    frame = 0;
                }
            }
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, ModContent.ItemType<CrystallineFabricatorItem>());
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0f;
            g = 0.75f;
            b = 1f;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            Vector2 position = new Vector2(i * 16, j * 16) - Main.screenPosition + zero;

            Color color = new(255, 255, 255, 0);
            Rectangle frame = new(tile.TileFrameX, tile.TileFrameY, 16, 16);
            spriteBatch.Draw(glowTexture.Value, position, frame, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}