using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Building
{
<<<<<<<< HEAD:Content/Tiles/CrystalCaverns/Glimmerwood/GlimmerwoodLeafTile.cs
    public class GlimmerwoodLeafTile : ModTile
========
    [LegacyName("CavernBrick")]
    public class CavernBrickTile : ModTile
>>>>>>>> Arcri-Branch-NonBiomeCC:Content/Tiles/CrystalCaverns/Building/CavernBrickTile.cs
    {
        public override void SetStaticDefaults()
        {
			MineResist = 2.5f;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<CrackedCavernBrickTile>()] = true;
            AddMapEntry(new Color(061, 079, 110));
			DustType = 59;
			HitSound = SoundID.Tink;
            AddMapEntry(new Color(069, 066, 088));
        }
    }

    public class CavernBrickItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<CavernBrickTile>());
        }
    }
}