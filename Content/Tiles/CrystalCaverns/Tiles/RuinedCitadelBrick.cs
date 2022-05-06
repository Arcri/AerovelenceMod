using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles
{
    public class RuinedCitadelBrick : ModTile
    {
        public override void SetStaticDefaults()
        {
            mineResist = 2.5f;
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][Mod.Find<ModTile>("CrystalGrass").Type] = true;
            Main.tileMerge[Type][Mod.Find<ModTile>("CavernCrystal").Type] = true;
            Main.tileMerge[Type][Mod.Find<ModTile>("CavernStone").Type] = true;
            Main.tileMerge[Type][Mod.Find<ModTile>("FieldStone").Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            AddMapEntry(new Color(102, 108, 117));
            dustType = 116;
            soundType = SoundID.Tink;
            drop = Mod.Find<ModItem>("CrystalDirtItem").Type;
        }
        public override bool CanExplode(int i, int j)
        {
            return true;
        }
    }
}