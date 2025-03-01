using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Effects.Waters
{
    public class CrystalCavernsWaterStyle : ModWaterStyle
    {
        public override int ChooseWaterfallStyle() => ModContent.Find<ModWaterfallStyle>("AerovelenceMod/CrystalCavernsWaterfallStyle").Slot;
        public override int GetSplashDust() => DustID.Water; // ModContent.DustType<>();
        public override int GetDropletGore() => GoreID.WaterDrip; // ModContent.GoreType<>();

        public override void LightColorMultiplier(ref float r, ref float g, ref float b)
        {
            r = 1.0f;
            g = 1.0f;
            b = 1.11f; // acts weird above ~1.15
        }

        public override Color BiomeHairColor() => Color.Blue;
        public override byte GetRainVariant() => (byte)Main.rand.Next(3);
    }
}