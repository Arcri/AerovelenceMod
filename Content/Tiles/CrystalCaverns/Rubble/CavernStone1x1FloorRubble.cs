using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Rubble
{
    public abstract class CavernStone1x1FloorRubbleBase : ModTile
    {
        public override string Texture => "AerovelenceMod/Content/Tiles/CrystalCaverns/Rubble/CavernStone1x1FloorRubble";

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileObsidianKill[Type] = true;

            DustType = DustID.BlueTorch;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);

            AddMapEntry(new Microsoft.Xna.Framework.Color(70, 70, 85));
        }
    }

    // Rubblemaker version
    public class CavernStone1x1FloorRubbleFake : CavernStone1x1FloorRubbleBase
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // Rubblemaker placement using cavern stone
            FlexibleTileWand.RubblePlacementLarge.AddVariations(ModContent.ItemType<CavernStoneItem>(), Type, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);

            RegisterItemDrop(ModContent.ItemType<CavernStoneItem>());
        }
    }

    // Generated version
    public class CavernStone1x1FloorRubbleNatural : CavernStone1x1FloorRubbleBase
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // Tile breaks when placed over
            TileID.Sets.BreakableWhenPlacing[Type] = true;
            TileID.Sets.ReplaceTileBreakUp[Type] = true;

            // Override Style1x1's lava death for natural rubble only
            TileObjectData.GetTileData(Type, 0).LavaDeath = false;
        }
    }
}
