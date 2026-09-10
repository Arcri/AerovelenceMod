using System;
using System.Collections.Generic;
using AerovelenceMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class TumblerPhaseTextures : ModSystem
    {
        private static readonly Dictionary<string, Texture2D> orangeTextures = new();

        internal static Texture2D Get(string name, bool orange)
        {
            Texture2D original = ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/" + name, AssetRequestMode.ImmediateLoad).Value;
            if (!orange)
                return original;
            if (orangeTextures.TryGetValue(name, out Texture2D texture))
                return texture;
            Color[] pixels = new Color[original.Width * original.Height];
            original.GetData(pixels);
            for (int i = 0; i < pixels.Length; i++)
            {
                byte alpha = pixels[i].A;
                pixels[i] = ColorUtils.ShiftHue(pixels[i], 187f / 360f);
                Vector3 hsv = ColorUtils.RgbToHsv(pixels[i]);
                hsv.X = Math.Clamp(hsv.X, 8f / 360f, 48f / 360f);
                pixels[i] = ColorUtils.HsvToRgb(hsv);
                pixels[i].A = alpha;
            }
            texture = new Texture2D(Main.instance.GraphicsDevice, original.Width, original.Height);
            texture.SetData(pixels);
            orangeTextures.Add(name, texture);
            return texture;
        }

        public override void Unload()
        {
            Texture2D[] textures = new Texture2D[orangeTextures.Count];
            orangeTextures.Values.CopyTo(textures, 0);
            orangeTextures.Clear();
            Main.QueueMainThreadAction(() =>
            {
                foreach (Texture2D texture in textures)
                    texture.Dispose();
            });
        }
    }
}
