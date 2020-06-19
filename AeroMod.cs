using AerovelenceMod.Systems;
using Microsoft.Xna.Framework;
using System;
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

		public override void Close()
		{
			var slots = new int[] {
				GetSoundSlot(SoundType.Music, "Sounds/Music/CrystalCaverns")
			};
			foreach (var slot in slots)
			{
				if (Main.music.IndexInRange(slot) && Main.music[slot]?.IsPlaying == true)
				{
					Main.music[slot].Stop(Microsoft.Xna.Framework.Audio.AudioStopOptions.Immediate);
				}
			}

			base.Close();
		}

		public override void UpdateMusic(ref int music, ref MusicPriority priority)
		{
			if (Main.myPlayer == -1 || Main.gameMenu || !Main.LocalPlayer.active)
			{
				return;
			}
			Player player = Main.LocalPlayer;
			if (player.GetModPlayer<AeroPlayer>().ZoneCrystalCaverns)
			{
				music = this.GetSoundSlot(SoundType.Music, "Sounds/Music/CrystalCaverns");
				priority = MusicPriority.BiomeMedium;
			}
		}
	}
}