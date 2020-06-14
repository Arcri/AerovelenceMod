using AerovelenceMod.Systems;
using Terraria;
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


		public override void UpdateMusic(ref int music, ref MusicPriority priority)
		{
			if (Main.myPlayer == -1 || Main.gameMenu || !Main.LocalPlayer.active)
			{
				return;
			}
			if (Main.LocalPlayer.GetModPlayer<AeroPlayer>().ZoneCrystalCaverns)
			{
				music = GetSoundSlot(SoundType.Music, "Sounds/Music/CrystalCaverns");
				priority = MusicPriority.BiomeLow;
			}
		}
	}
}