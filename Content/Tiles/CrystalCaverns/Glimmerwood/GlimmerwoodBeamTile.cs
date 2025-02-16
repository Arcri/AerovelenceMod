using AerovelenceMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Glimmerwood
{
    public class GlimmerwoodBeamTile : ModTile
    {
        public override void SetStaticDefaults()
        {
			MineResist = 2.5f;
			MinPick = 59;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            AddMapEntry(new Color(061, 079, 110));
            TileID.Sets.IsBeam[Type] = true;
            DustType = 59;
			HitSound = SoundID.Tink;
        }

        public class GlimmerwoodBeamItem : ModItem
        {
            public override void SetStaticDefaults()
            {

            }

            public override void SetDefaults()
            {
                CommonItemHelper.SetupPlaceableItem(this, 28, 14, 150, ModContent.TileType<GlimmerwoodBeamTile>());
            }
        }
    }
}