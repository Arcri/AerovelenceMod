using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using AerovelenceMod.Common.Interfaces;
using Terraria.ID;
using Terraria;



namespace AerovelenceMod.Common.Drawing
{
    //Based off SLRs IDrawAdditive implementation which I think is based off Spirit's IDrawAdditive
    class AdditiveDrawer : IOrderedLoadable
    {
        public float Priority => 1;

        public void Load()
        {
            //Never load shit on dedicated servers
            if (Main.dedServ)
                return;

            On_Main.DrawProjectiles += DrawAdditiveUnder;
            On_Main.DrawDust += DrawAdditive;
        }

        public void Unload()
        {
            On_Main.DrawProjectiles -= DrawAdditiveUnder;
            On_Main.DrawDust -= DrawAdditive;
        }

        private void DrawAdditive(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, default, RasterizerState.CullNone, default, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];

                if (p.active && p.ModProjectile is IDrawAdditive)
                        (p.ModProjectile as IDrawAdditive).DrawAdditive(Main.spriteBatch);
            }

            Main.spriteBatch.End();
        }

        //Draws on the projectile layer instead of dust layer, which is a lower layer
        private void DrawAdditiveUnder(On_Main.orig_DrawProjectiles orig, Main self)
        {
            orig(self);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, default, RasterizerState.CullNone, default, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];

                if (p.active && p.ModProjectile is IDrawAdditive)
                {
                    (p.ModProjectile as IDrawAdditive).DrawAdditive(Main.spriteBatch);
                }
            }

            Main.spriteBatch.End();
        }

    }

}
