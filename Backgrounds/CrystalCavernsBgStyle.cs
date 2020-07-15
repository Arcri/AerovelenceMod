using Terraria;
using AerovelenceMod;
using Terraria.ModLoader;

namespace ExampleMod.Backgrounds
{
	public class CrystalCavernsBgStyle : ModSurfaceBgStyle
	{
		public override bool ChooseBgStyle()
		{
			return !Main.gameMenu && Main.LocalPlayer.GetModPlayer<AeroPlayer>().ZoneCrystalCaverns;
		}

		// Use this to keep far Backgrounds like the mountains.
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
			if (Main.dayTime)
			{
				return mod.GetBackgroundSlot("Backgrounds/CrystalCavernsBgWeatherStorm");
			}
			else if (Main.hardMode)
			{
				return mod.GetBackgroundSlot("Backgrounds/CrystalCavernsBgHardmodeFar");
			}
			else
			{
				return mod.GetBackgroundSlot("Backgrounds/CrystalCavernsBgSurfaceFar");
			}
		}



		public override int ChooseMiddleTexture()
		{
			if (Main.hardMode)
				return mod.GetBackgroundSlot("Backgrounds/CrystalCavernsBgHardmodeMid0");
			return mod.GetBackgroundSlot("Backgrounds/CrystalCavernsBgSurfaceMid0");
		}

		public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
		{
			if (Main.hardMode)
				return mod.GetBackgroundSlot("Backgrounds/CrystalCavernsBgHardmodeClose");
			return mod.GetBackgroundSlot("Backgrounds/CrystalCavernsBgSurfaceClose");
		}
	}
}