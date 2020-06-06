using AerovelenceMod.Systems;
using Terraria.ModLoader;

namespace AerovelenceMod
{
	// Todo: Convert mod.XType to the new ModContent system
	public class AeroMod : Mod
	{
		public static TextureManager Textures { get; private set; } = null;

		public override void Load()
		{
			Textures = new TextureManager(this);
			Textures.Load();
		}

		public override void Unload()
		{
			Textures?.Unload();
			Textures = null;
		}
	}
}