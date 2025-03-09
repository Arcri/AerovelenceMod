using AerovelenceMod.Common.Systems;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Biomes
{
    public class CrystalCavernsBiome : ModBiome
    {
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("AerovelenceMod/CrystalCavernsWaterStyle");
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("AerovelenceMod/CrystalCavernsBgStyle");
        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Mushroom;

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/CrystalCaverns");

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh; //default behavior is BiomeLow.

        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override string MapBackground => "AerovelenceMod/Backgrounds/CrystalCaverns/CrystalCavernsMapBg";

        //public override int BiomeTorchItemType => ModContent.ItemType<GlimmerwoodTorch>();

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Crystal Caverns Surface");
        }

        public override bool IsBiomeActive(Player player)
        {
            bool b1 = ModContent.GetInstance<CrystalCavernsTileCount>().CavernTiles >= 100;
            bool b2 = player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight;

            return b1 && b2;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            // Code 'tactically borrowed' from the below method
            // layer.ManageSpecialBiomeVisuals("AerovelenceMod:CrystalCavernsSurface", isActive);
            string biomeName = "AerovelenceMod:CrystalCaverns";

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