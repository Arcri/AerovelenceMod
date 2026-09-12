using AerovelenceMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using static AerovelenceMod.Content.Projectiles.LightningUtils;

namespace AerovelenceMod.Common.Systems
{
    public class TileLightningSystem : ModSystem
    {
        public static Dictionary<LightningData, int> LightningLifetimes = new();

        public override void PostUpdateEverything()
        {
            List<LightningData> expiredLightning = [];

            foreach (var data in new List<LightningData>(LightningLifetimes.Keys)) // TODO: eee
            {
                LightningLifetimes[data]--;

                if (LightningLifetimes[data] <= 0)
                {
                    expiredLightning.Add(data);
                    continue;
                }
                
                // Must run these to render the lightning with DrawLightning()
                LightningUtils.InitializeBetweenPoints(data, data.WorldOrigin, data.WorldTarget);
                LightningUtils.UpdateSegments(data);
                LightningUtils.UpdateBranches(data);
                // Not required but makes the lightning feel much more layered
                LightningUtils.SpawnDust(data);
            }

            foreach (var data in expiredLightning)
            {
                LightningLifetimes.Remove(data);
            }
        }

        public override void PostDrawTiles()
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            foreach (var (data, lifetime) in LightningLifetimes)
            {
                if (data != null && data.Initialized)
                {
                    LightningUtils.DrawLightning(data, Main.spriteBatch);
                }
            }

            Main.spriteBatch.End();
        }
    }
}
