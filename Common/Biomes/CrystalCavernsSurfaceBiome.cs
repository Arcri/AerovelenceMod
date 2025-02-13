using Microsoft.Xna.Framework;
using System;
using System.Security.Policy;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;
using Terraria.ID;
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

        private bool FxActive = false;
        private float intensity = 0f;
        private const float increment = 0.02f;

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

        public override void SpecialVisuals(Player player, bool isActive)
        {
			// Code 'tactically borrowed' from the below method
			// layer.ManageSpecialBiomeVisuals("AerovelenceMod:CrystalCavernsSurface", isActive);
			string biomeName = "AerovelenceMod:CrystalCavernsSurface";
			bool inZone = isActive;

            if (SkyManager.Instance[biomeName] != null && inZone != SkyManager.Instance[biomeName].IsActive())
            {
                if (inZone)
                    SkyManager.Instance.Activate(biomeName);
                else
                    SkyManager.Instance.Deactivate(biomeName);
            }

            // Glowy water
            if (!isActive)
            {
                intensity -= increment;
            }
            else
            {
                intensity += increment;
            }
            intensity = Math.Clamp(intensity, 0f, 1f);
            if (intensity > 0f)
            {
                FxActive = true;
            }
            else
            {
                FxActive = false;
            }

            if (FxActive)
            {
                int startX = (int)(player.Center.X / 16f) - 75; // Adjust search radius as needed
                int endX = startX + 150;
                int startY = (int)(player.Center.Y / 16f) - 50;
                int endY = startY + 100;

                for (int x = startX; x < endX; x++)
                {
                    for (int y = startY; y < endY; y++)
                    {
                        if (WorldGen.InWorld(x, y) && Main.tile[x, y].LiquidAmount > 0 && Main.tile[x, y].LiquidType == LiquidID.Water) // Check if tile contains water
                        {
                            Lighting.AddLight(new Vector2(x * 16, y * 16), 0.0f, 0.4f * intensity, 0.8f * intensity); // Adjust RGB for glow color
                        }
                    }
                }
            }
        }

        public static int CavernTiles { get; private set; }
		public static int CitadelTiles { get; private set; }
	}
}