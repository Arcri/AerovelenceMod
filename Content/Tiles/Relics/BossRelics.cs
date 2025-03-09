using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using AerovelenceMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Humanizer;
using Terraria.ID;
using System.Collections.Generic;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ObjectData;
using System;

namespace AerovelenceMod.Content.Tiles.Relics
{
    public class BossRelics : ModTile
    {
        public const int FrameWidth = 18 * 3;
        public const int FrameHeight = 18 * 4;
        public const int HorizontalFrames = 1;
        public const int VerticalFrames = 2;
        public List<Point> Coordinates = [];

        public static Asset<Texture2D> RelicTexture;

        public virtual string RelicTextureName => Texture + "_Floating";

        public override void Load()
        {
            if (!Main.dedServ)
                RelicTexture = ModContent.Request<Texture2D>(RelicTextureName);
        }

        public override void Unload() { RelicTexture = null; }

        public override void SetStaticDefaults()
        {
            Coordinates = [];

            Main.tileShine[Type] = 400;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.InteractibleByNPCs[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.StyleHorizontal = false;
            TileObjectData.newTile.StyleWrapLimitVisualOverride = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.styleLineSkipVisualOverride = 0;

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);

            TileObjectData.addTile(Type);
            AddMapEntry(new Color(233, 207, 94), Language.GetText("MapObject.Relic"));
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Point p = new(i, j);
            Coordinates.Remove(p);
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            return false;
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            tileFrameX %= FrameWidth;
            tileFrameY %= FrameHeight * 2;
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            // Ensure only the top-left tile registers for special drawing
            if (drawData.tileFrameX % FrameWidth == 0 && drawData.tileFrameY % FrameHeight == 0)
            {
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
            }
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (RelicTexture == null || !RelicTexture.IsLoaded) return;

            // Make sure we're drawing only from the top-left tile of the multi-tile structure
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            // Get the tile at this position
            Tile tile = Main.tile[i, j];
            if (tile == null || !tile.HasTile) return;

            // Calculate the correct position
            // Multi-tile objects need special handling to get the actual top-left corner
            int left = i - (tile.TileFrameX % FrameWidth) / 18;
            int top = j - (tile.TileFrameY % FrameHeight) / 18;

            // Get the placeStyle (which relic we're drawing)
            int placeStyle = tile.TileFrameX / FrameWidth;

            // Get the correct frame for this relic
            Rectangle frame = RelicTexture.Value.Frame(HorizontalFrames, VerticalFrames, 0, placeStyle % VerticalFrames);

            // The origin is the center of the texture
            Vector2 origin = frame.Size() / 2f;

            // Calculate the position in world coordinates
            // For a 3x4 object, we want to center horizontally and place it above the pedestal
            float x = left * 16 + 24; // 24 = half of 3 tiles (3 * 16 / 2)
            float y = top * 16;       // Top of the multi-tile

            Vector2 worldPos = new Vector2(x, y);

            // Get the lighting at this position
            Color color = Lighting.GetColor(left, top);

            // Check if the object is drawn flipped
            bool direction = tile.TileFrameY / FrameHeight != 0;
            SpriteEffects effects = direction ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // Calculate the floating effect
            const float TwoPi = (float)Math.PI * 2f;
            float floatOffset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 5f);

            // The drawing position includes:
            // 1. The world position
            // 2. A vertical offset to position above the pedestal
            // 3. The floating animation offset
            // 4. Adjustment for screen position
            // Reduced height by 4 tiles (64 pixels) from previous -40f value
            Vector2 drawPos = worldPos + new Vector2(0, 24f + floatOffset * 4f) - Main.screenPosition + zero;

            // Draw the floating relic
            spriteBatch.Draw(RelicTexture.Value, drawPos, frame, color, 0f, origin, 1f, effects, 0f);

            // Draw glow effect
            float scale = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 2f) * 0.3f + 0.7f;
            Color effectColor = color * 0.1f * scale;
            effectColor.A = 0;

            for (float num5 = 0f; num5 < 1f; num5 += 355f / (678f * (float)Math.PI))
            {
                spriteBatch.Draw(
                    RelicTexture.Value,
                    drawPos + (TwoPi * num5).ToRotationVector2() * (6f + floatOffset * 2f),
                    frame,
                    effectColor,
                    0f,
                    origin,
                    1f,
                    effects,
                    0f
                );
            }
        }
   
    public class CyvercryBossRelic : BaseBossRelic
    {
        public override string RelicTextureName => "AerovelenceMod/Content/Tiles/Relics/CyvercryBossRelic";

        public override void SetStaticDefaults() => base.SetStaticDefaults();
    }

    public class CrystalTumblerRelicItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.KingSlimeMasterTrophy);
            Item.placeStyle = 0;
            Item.createTile = ModContent.TileType<BossRelics>();
        }
    }

    public class CyvercryRelicItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.KingSlimeMasterTrophy);
            Item.placeStyle = 1;
            Item.createTile = ModContent.TileType<BossRelics>();
        }
    }
}