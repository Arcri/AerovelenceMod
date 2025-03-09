using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using AerovelenceMod.Common.Systems;

namespace AerovelenceMod.Content.Biomes
{
    public class CrystalFieldsBiome : ModBiome
    {
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("AerovelenceMod/CrystalCavernsWaterStyle");
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
            bool b1 = ModContent.GetInstance<CrystalCavernsTileCount>().FieldsTiles >= 100;
            bool b2 = player.ZoneSkyHeight || player.ZoneOverworldHeight;

            return b1 && b2;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            // Code 'tactically borrowed' from the below method
            // layer.ManageSpecialBiomeVisuals("AerovelenceMod:CrystalCavernsSurface", isActive);
            string biomeName = "AerovelenceMod:CrystalCavernsSurface";

            if (SkyManager.Instance[biomeName] != null && isActive != SkyManager.Instance[biomeName].IsActive())
            {
                if (isActive)
                    SkyManager.Instance.Activate(biomeName);
                else
                    SkyManager.Instance.Deactivate(biomeName);
            }

            if (isActive)
            {
                WaterGlowManager.ActivateGlow(this);
            }
            else
            {
                WaterGlowManager.DeactivateGlow(this);
            }
        }
    }
}