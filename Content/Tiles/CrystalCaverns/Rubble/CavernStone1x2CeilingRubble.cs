using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.RGB;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Rubble
{
    public abstract class CavernStone1x2CeilingRubbleBase : ModTile
    {
        public override string Texture => "AerovelenceMod/Content/Tiles/CrystalCaverns/Rubble/CavernStone1x2CeilingRubble";
        private Asset<Texture2D> glowTexture;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileObsidianKill[Type] = true;

            DustType = DustID.BlueTorch;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.addTile(Type);

            AddMapEntry(new Microsoft.Xna.Framework.Color(70, 70, 85));

            glowTexture = ModContent.Request<Texture2D>(Texture + "_Glowmask");
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            // Draw original texture
            spriteBatch.Draw(
                TextureAssets.Tile[Type].Value,
                new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
                new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16),
                Lighting.GetColor(i, j), 0f, default, 1f, SpriteEffects.None, 0f);

            // Pulsating color for glowmask
            Color maskColor = Color.White
                * MathHelper.Lerp(0.0f, 1f, ((float)Math.Pow(Math.Sin(NoiseHelper.GetDynamicNoise(new Vector2(i * 0.05f, j * 0.05f), Main.GlobalTimeWrappedHourly * 0.1f)), 4)));

            // Draw glowmask
            spriteBatch.Draw(
                glowTexture.Value,
                new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
                new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16),
                maskColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            // Return false to stop vanilla draw
            return false;
        }
    }

    // Rubblemaker version
    public class CavernStone1x2CeilingRubbleFake : CavernStone1x2CeilingRubbleBase
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // Rubblemaker placement using cavern stone
            FlexibleTileWand.RubblePlacementLarge.AddVariations(ModContent.ItemType<CavernStoneItem>(), Type, 0, 1, 2, 3, 4, 5);

            RegisterItemDrop(ModContent.ItemType<CavernStoneItem>());
        }
    }

    // Generated version
    public class CavernStone1x2CeilingRubbleNatural : CavernStone1x2CeilingRubbleBase
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // Tile breaks when placed over
            TileID.Sets.BreakableWhenPlacing[Type] = true;
            TileID.Sets.ReplaceTileBreakUp[Type] = true;

            // Override Style1x2Top's lava death for natural rubble only
            TileObjectData.GetTileData(Type, 0).LavaDeath = false;
        }
    }
}
