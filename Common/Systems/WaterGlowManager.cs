using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace AerovelenceMod.Common.Systems
{
    public class WaterGlowManager
    {
        private static float _intensity = 0f;
        private const float _increment = 0.02f;
        private static ModBiome[] _biomesCalling = [];

        /// <summary>
        /// Adds this class to an array of ModBiomes that are trying to make water glowy
        /// </summary>
        /// <param name="modBiomeCalling">The ModBiome calling the function. Pass the keyword this as a parameter.</param>
        public static void ActivateGlow(ModBiome modBiomeCalling)
        {
            if (!_biomesCalling.Contains(modBiomeCalling))
            {
                _biomesCalling = _biomesCalling.Concat([modBiomeCalling]).ToArray();
            }
        }

        /// <summary>
        /// Removes this class from an array of ModBiomes that are trying to make water glowy
        /// </summary>
        /// <param name="modBiomeCalling">The ModBiome calling the function. Pass the keyword this as a parameter.</param>
        public static void DeactivateGlow(ModBiome modBiomeCalling)
        {
            if (_biomesCalling.Contains(modBiomeCalling)) _biomesCalling = _biomesCalling.Except([modBiomeCalling]).ToArray();
        }

        public static void UpdateWaterGlow()
        {
            Player player = Main.LocalPlayer;
            if (_biomesCalling.Length > 0)
            {
                _intensity += _increment;
            }
            else
            {
                _intensity -= _increment;
            }
            _intensity = Math.Clamp(_intensity, 0f, 1f);

            if (_intensity > 0f)
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
                            Lighting.AddLight(new Vector2(x * 16, y * 16), 0.2f, 0.8f * _intensity, 0.8f * _intensity); // Adjust RGB for glow color
                        }
                    }
                }
            }
        }

    }
}