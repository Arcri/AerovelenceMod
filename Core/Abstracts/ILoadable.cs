#region Using directives

using Terraria.ModLoader;

#endregion

namespace AerovelenceMod.Core.Abstracts
{
	/// <summary>
	/// Inspired from other mod structures, this is to be able to centralize content loading.
	/// </summary>
	public interface ILoadable
	{
		/// <summary>
		/// Load priority. Important for loadables that are load-order-dependent.
		/// </summary>
		float Priority { get; }

		/// <summary>
		/// Whether or not this loadable can load if the current running instance is dedicated server.
		/// </summary>
		bool LoadOnDedServer { get; }
		
		/// <summary>
		/// Stages loading of objects specific to the deriving class.
		/// </summary>
		void Load(Mod mod);

		/// <summary>
		/// Stages unloading of objects specific to the deriving class.
		/// </summary>
		void Unload();
	}
}
