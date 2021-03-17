using Terraria;
using Terraria.Graphics.Shaders;

namespace AerovelenceMod.Skies
{
	public class DarkNightScreenShaderData : ScreenShaderData
	{
		public DarkNightScreenShaderData(string passName)
			: base(passName)
		{
		}

		public override void Apply()
		{
			float num = 2f - Utils.SmoothStep((float)Main.worldSurface + 50f, (float)Main.rockLayer + 100f, (Main.screenPosition.Y + Main.screenHeight / 2) / 16f);
			UseOpacity(num * 0.65f);
			base.Apply();
		}
	}
}