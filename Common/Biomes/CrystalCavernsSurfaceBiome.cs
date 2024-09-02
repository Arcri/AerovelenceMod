using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Biomes
{
	public class CrystalCavernsSurfaceBiome : ModBiome
	{
		public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("AerovelenceMod/CrystalCavernsSurfaceBgStyle");
		public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Crimson;

        public override int Music => Main.LocalPlayer.townNPCs >= 2 ? -1 : (Main.dayTime ? MusicLoader.GetMusicSlot(Mod, "Sounds/Music/CrystalFields") : MusicLoader.GetMusicSlot(Mod, "Sounds/Music/CrystalFieldsNight"));

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh; //default behavior is BiomeLow.

        public override string BestiaryIcon => base.BestiaryIcon;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;
        public override string MapBackground => "AerovelenceMod/Backgrounds/CrystalCaverns/CrystalCavernsMapBg";

        public override void SetStaticDefaults()
		{
			//DisplayName.SetDefault("Crystal Caverns Surface");
		}

		public override bool IsBiomeActive(Player player)
		{
			bool b1 = ModContent.GetInstance<CrystalCavernsTileCount>().CavernTiles >= 100;
			bool b2 = player.ZoneSkyHeight || player.ZoneOverworldHeight;

			return b1 && b2;
		}
	
		public static int CavernTiles { get; private set; }
		public static int CitadelTiles { get; private set; }
	}
}