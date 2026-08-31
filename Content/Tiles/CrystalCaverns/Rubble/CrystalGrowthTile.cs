using AerovelenceMod.Content.Tiles.Citadel;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Building;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.RGB;
using Terraria.ID;
using Terraria.ModLoader;
namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Rubble
{
    public class CrystalGrowthTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileWaterDeath[Type] = false;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = false;
            Main.tileLighted[Type] = true;
            DustType = 116;
            HitSound = SoundID.Shatter;
            AddMapEntry(new Color(100, 125, 255));
        }
        public override IEnumerable<Item> GetItemDrops(int i, int j) { yield return new Item(ModContent.ItemType<CavernCrystalItem>()); }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = 59;
        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;
        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tile = Main.tile[i, j];
            bool validPlacement = false;

            // Check all adjacent tiles
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            Tile tileLeft = Framing.GetTileSafely(i - 1, j);
            Tile tileRight = Framing.GetTileSafely(i + 1, j);

            static bool IsValidSurface(Tile tile)
            {
                if (!tile.HasTile || tile.IsHalfBlock || tile.TopSlope)
                    return false;
                return tile.TileType == ModContent.TileType<CavernStoneTile>() ||
                        tile.TileType == ModContent.TileType<CitadelBrickTile>() ||
                       tile.TileType == ModContent.TileType<CrackedCavernBrickTile>() ||
                       tile.TileType == ModContent.TileType<CavernCrystalTile>() ||
                       tile.TileType == ModContent.TileType<SmoothCavernStoneTile>() ||
                       tile.TileType == ModContent.TileType<CavernBrickTile>();
            }

            // Determine anchor point and set appropriate frame
            if (IsValidSurface(tileBelow))
            {
                tile.TileFrameY = 0; // First row - bottom anchor
                validPlacement = true;
            }
            else if (IsValidSurface(tileRight))
            {
                tile.TileFrameY = 18; // Second row - right anchor
                validPlacement = true;
            }
            else if (IsValidSurface(tileAbove))
            {
                tile.TileFrameY = 36; // Third row - top anchor
                validPlacement = true;
            }
            else if (IsValidSurface(tileLeft))
            {
                tile.TileFrameY = 54; // Fourth row - left anchor
                validPlacement = true;
            }

            // Set random frame X for variation (15 frames per row)
            if (validPlacement && resetFrame)
            {
                tile.TileFrameX = (short)(WorldGen.genRand.Next(15) * 18); // 18 pixels per frame
            }

            if (!validPlacement)
            {
                WorldGen.KillTile(i, j);
            }

            return true;
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            float lightFactor = MathHelper.Lerp(0.5f, 2f, ((float)Math.Pow(Math.Sin(NoiseHelper.GetDynamicNoise(new Vector2(i * 0.05f, j * 0.05f), Main.GlobalTimeWrappedHourly * 0.1f)), 4)));
            r = 0.0f * lightFactor;
            g = 0.6f * lightFactor;
            b = 0.9f * lightFactor;
        }
    }
    public class CrystalGrowthRubbleItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<CrystalGrowthTile>();
            Item.rare = ItemRarityID.White;
            Item.value = 5;
        }
    }
}