using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Biomes
{
	public class CrystalCavernsBiome : ModBiome
	{
		public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("AerovelenceMod/CrystalCavernsBgStyle");
		public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Crimson;

		public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/CrystalCaverns");

		public override string BestiaryIcon => base.BestiaryIcon;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Crystal Caverns Surface");
		}

		public override bool IsBiomeActive(Player player)
		{
			bool b1 = ModContent.GetInstance<CrystalCavernsTileCount>().CavernTiles >= 100;

			bool b2 = Math.Abs(player.position.ToTileCoordinates().X - Main.maxTilesX / 2) < Main.maxTilesX / 6;
			bool b3 = player.ZoneSkyHeight || player.ZoneOverworldHeight;
			return b1 && b2 && b3;
		}
	

		public static int CavernTiles { get; private set; }
		public static int CitadelTiles { get; private set; }


	}
}