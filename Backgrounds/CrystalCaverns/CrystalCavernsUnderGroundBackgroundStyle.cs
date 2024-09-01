using AerovelenceMod.Content.Biomes;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Backgrounds.CrystalCaverns
{
    public class CrystalCavernsUndergroundBackgroundStyle : ModUndergroundBackgroundStyle
    {
        public override void FillTextureArray(int[] textureSlots)
        {
            textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Backgrounds/CrystalCaverns/CrystalCavernsBiomeUnderground0");
            textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Backgrounds/CrystalCaverns/CrystalCavernsBiomeUnderground1");
            textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Backgrounds/CrystalCaverns/CrystalCavernsBiomeUnderground2");
            textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Backgrounds/CrystalCaverns/CrystalCavernsBiomeUnderground3");
        }
    }

	public class CrystalCavernsUndergroundBiome : ModBiome
    {
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<CrystalCavernsUndergroundBackgroundStyle>();
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/CrystalCaverns_Night");

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeLow; // We have set the SceneEffectPriority to be BiomeLow for purpose of example, however default behavior is BiomeLow.

        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;

        // Calculate when the biome is active.
        public override bool IsBiomeActive(Player player)
        {
            // Limit the biome height to be underground in either rock layer or dirt layer
            return (player.ZoneRockLayerHeight || player.ZoneDirtLayerHeight) &&
                // Check how many tiles of our biome are present, such that biome should be active
                ModContent.GetInstance<CrystalCavernsTileCount>().CavernTiles >= 40 &&
                // Limit our biome to be in only the horizontal center third of the world.
                Math.Abs(player.position.ToTileCoordinates().X - Main.maxTilesX / 2) < Main.maxTilesX / 6;
        }

    }
}