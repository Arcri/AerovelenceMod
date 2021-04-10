#region Using directives

using System.IO;
using System.Linq;
using System.Reflection;

using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.Graphics.Shaders;
using Terraria.Graphics.Effects;

using Microsoft.Xna.Framework.Graphics;
using AerovelenceMod.Core.Abstracts;

#endregion

namespace AerovelenceMod.Core.Loaders
{
    internal sealed class ShaderLoader : ILoadable
	{
		public float Priority => 1f;


		public bool LoadOnDedServer => false;

		public void Load(Mod mod)
		{
			MethodInfo info = typeof(Mod).GetProperty("File", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
			var file = (TmodFile)info.Invoke(mod, null);

			var shaders = file.Where(x => x.Name.StartsWith("Effects/") && x.Name.EndsWith(".xnb"));

			foreach (var entry in shaders)
			{
				var shaderPath = entry.Name.Replace(".xnb", string.Empty);
				var shaderName = Path.GetFileName(shaderPath);

				LoadShader(AerovelenceMod.AbbreviationPrefix + shaderName, shaderPath);
			}
		}

		public void Unload() { }

		internal static void LoadShader(string shaderName, string shaderPath)
		{
			var shaderRef = new Ref<Effect>(AerovelenceMod.Instance.GetEffect(shaderPath));

			if (AerovelenceMod.DEBUG)
			{
				AerovelenceMod.Instance.Logger.Debug($"Loading shader: <{shaderName}> @ <{shaderPath}>");
			}

			(Filters.Scene[shaderName] = new Filter(new ScreenShaderData(shaderRef, shaderName + "Pass"), EffectPriority.High))
				.Load();
		}
	}
}
