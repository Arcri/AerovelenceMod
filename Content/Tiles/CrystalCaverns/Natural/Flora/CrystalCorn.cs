using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.ObjectData;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Natural.Flora
{
    public enum CrystalCornStage : byte
    {
        Planted,
        Growing1,
        Growing2,
        Grown
    }

    public class CrystalCorn : ModTile
    {
        private const int FrameWidth = 18;
        private const int FrameHeight = 18;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.StyleAlch);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            TileObjectData.newTile.AnchorValidTiles =
            [
                TileID.Grass,
                TileID.HallowedGrass,
                ModContent.TileType<CrystalGrass>()
            ];
            TileObjectData.newTile.AnchorAlternateTiles =
            [
                TileID.ClayPot,
                TileID.PlanterBox
            ];
            TileObjectData.addTile(Type);
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            CrystalCornStage stage = GetStage(i, j);

            if (stage == CrystalCornStage.Grown)
            {
                yield return new Item(ModContent.ItemType<ShotgunAxe>());
                yield return new Item(ModContent.ItemType<ShotgunAxe>());
            }
            else if (stage == CrystalCornStage.Growing2)
            {
                yield return new Item(ModContent.ItemType<ShotgunAxe>());
            }
        }

        public override void RandomUpdate(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            CrystalCornStage stage = GetStage(i, j);
            if (stage != CrystalCornStage.Grown)
            {
                tile.TileFrameX += FrameWidth;

                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetMessage.SendTileSquare(-1, i, j, 3, TileChangeType.None);
                }
            }
        }

        private CrystalCornStage GetStage(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            return (CrystalCornStage)(tile.TileFrameX / FrameWidth);
        }
    }
}