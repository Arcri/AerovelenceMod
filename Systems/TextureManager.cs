using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace AerovelenceMod.Systems
{
	/// <summary>
	/// Object class handling all textures. Make sure to null out texture fields in Unload
	/// </summary>
	public class TextureManager
	{
		/// <summary>
		/// The mod you're getting textures from.
		/// </summary>
		private readonly Mod mod = null;

		public TextureManager(Mod mod)
		{
			this.mod = mod;
		}

		public Texture2D Shield = null;

		/// <summary>
		/// This is where you initialize any Texture2D fields
		/// </summary>
		internal void Load()
		{
			Shield = mod.GetTexture("Systems/ShieldSystem/ShieldTexture");
		}

		/// <summary>
		/// This is where you unload any Texture2D fields by nulling them.
		/// </summary>
		internal void Unload()
		{
			Shield = null;
		}
	}
}