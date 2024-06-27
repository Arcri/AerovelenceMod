#region Using directives

using Terraria.Graphics.Shaders;

#endregion

//Credits to Eldrazi for crystal shaders and screen shader extentions.

namespace AerovelenceMod.Common.Utilities
{
	public static class ScreenShaderDataExtensions
	{
		public static ScreenShaderData ApplyTime(this ScreenShaderData shader, float time)
		{
			//shader.Shader.Parameters["uTime"].SetValue(time);
			return (shader);
		}

		public static ScreenShaderData ApplyOpacity(this ScreenShaderData shader, float opacity)
		{
			//shader.Shader.Parameters["uOpacity"].SetValue(opacity);
			return (shader);
		}
	}
}
