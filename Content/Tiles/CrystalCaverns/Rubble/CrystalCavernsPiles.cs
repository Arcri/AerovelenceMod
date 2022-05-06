using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Rubble
{
	public class CrystalCavernsPiles : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileMergeDirt[Type] = true;
			DustType = 116;
			ItemDrop = Mod.Find<ModItem>("CavernCrystal").Type;
			SoundType = SoundID.Shatter;
			AddMapEntry(new Color(100, 125, 255));
		}
		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = 59;
		}
		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
		{
			offsetY = 2;
		}
        public override void NearbyEffects(int i, int j, bool closer)
		{
			Lighting.AddLight(new Vector2(i, j) * 16, new Vector3(0.0f, 0.20f, 0.60f));
		}
	}
}