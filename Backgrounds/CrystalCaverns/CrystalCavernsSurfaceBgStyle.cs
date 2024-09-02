using AerovelenceMod.Common.Globals.Players;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Backgrounds.CrystalCaverns
{
	public class CrystalCavernsSurfaceBgStyle : ModSurfaceBackgroundStyle
	{
		public override void ModifyFarFades(float[] fades, float transitionSpeed)
		{
			for (int i = 0; i < fades.Length; i++)
			{
				if (i == Slot)
				{
					fades[i] += transitionSpeed;
					if (fades[i] > 1f)
					{
						fades[i] = 1f;
					}
				}
				else

				{
					fades[i] -= transitionSpeed;
					if (fades[i] < 0f)
					{
						fades[i] = 0f;
					}
				}
			}
		}
		public override int ChooseFarTexture()
		{
			if (Main.hardMode)
				return BackgroundTextureLoader.GetBackgroundSlot("AerovelenceMod/Backgrounds/CrystalCaverns/CrystalCavernsBgHardmodeFar");
			return BackgroundTextureLoader.GetBackgroundSlot("AerovelenceMod/Backgrounds/CrystalCaverns/CrystalCavernsBgSurfaceFar");
		}
		public override int ChooseMiddleTexture()
		{
			if (Main.hardMode)
				return BackgroundTextureLoader.GetBackgroundSlot("AerovelenceMod/Backgrounds/CrystalCaverns/CrystalCavernsBgHardmodeMid0");
			return BackgroundTextureLoader.GetBackgroundSlot("AerovelenceMod/Backgrounds/CrystalCaverns/CrystalCavernsBgSurfaceMid0");
		}

		public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
		{
			if (Main.hardMode)
				return BackgroundTextureLoader.GetBackgroundSlot("AerovelenceMod/Backgrounds/CrystalCaverns/CrystalCavernsBgHardmodeClose");
			return BackgroundTextureLoader.GetBackgroundSlot("AerovelenceMod/Backgrounds/CrystalCaverns/CrystalCavernsBgSurfaceClose");
		}
	}
}