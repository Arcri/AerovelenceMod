using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Walls.CrystalCaverns.Natural
{
    public class CrystalGrassWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            this.SimpleWall(ModContent.ItemType<CrystalGrassWallItem>(), SoundID.Dig,
            DustID.Dirt, new Color(54, 87, 129), true);
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }

    public class CrystalGrassWallItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<CrystalGrassWall>());
        }
    }
}