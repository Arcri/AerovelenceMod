using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria;
using System.Collections.Generic;
using AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Natural.Flora
{

	public enum PlantStage : byte
    {
        Planted,
        Growing,
        Grown
    }
    public class PrismaticAster : ModTile
    {
        private const int FrameWidth = 18;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileNoFail[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleAlch);
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
            PlantStage stage = GetStage(i, j);

            if (stage == PlantStage.Grown)
            {
                yield return new Item(ModContent.ItemType<ShotgunAxe>());
                yield return new Item(ModContent.ItemType<ShotgunAxe>());
            }
            else
            {
                yield return new Item(ModContent.ItemType<ShotgunAxe>());
            }
        }

        public override void RandomUpdate(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            PlantStage stage = GetStage(i, j);
            if (stage != PlantStage.Grown)
            {
                tile.TileFrameX += FrameWidth;

                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
                }
            }
        }

        private PlantStage GetStage(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            return (PlantStage)(tile.TileFrameX / FrameWidth);
        }
    }
}