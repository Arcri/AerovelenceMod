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
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            AddMapEntry(new Color(061, 079, 110));
            TileID.Sets.IsBeam[Type] = true;
            DustType = 59;
			HitSound = SoundID.Dig;
        }

        public class GlimmerwoodBeamItem : ModItem
        {
            public override void SetDefaults() => CommonItemHelper.SetupPlaceableItem(this, 28, 14, 150, ModContent.TileType<GlimmerwoodBeamTile>());
        }
    }
}