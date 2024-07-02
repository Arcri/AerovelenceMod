using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using System;
using AerovelenceMod.Common.Utilities;
using Terraria.Graphics.Renderers;
using Terraria.Graphics.Capture;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace AerovelenceMod.Common.Systems
{
    //TODO Remake this 
    public class FlashSystem : ModSystem
    {
        //Practically stolen from Infernum

        private static RenderTarget2D flashRenderTarget;

        private static float FlashIntensity;

        private static int FlashLifeTime;

        private static int FlashTime;

        private static bool FlashActive;

        //Chromatic Abberation Flash
        private static float WhiteIntensity;
        
        private static float DistanceMultiplier;

        private static bool MoveColorIn;


        public static Behavior BehaviorToUse = Behavior.BasicFlash;
        public enum Behavior
        {
            BasicFlash = 0,
            ChromaticAbberationFlash = 1,
            PlaceHolder2 = 2,
            PlaceHolder3 = 3,
        }


        private static float flashProgress => (float)FlashTime / FlashLifeTime;

        
        public static void SetFlashEffect(float intensity, int lifetime)
        {
            FlashIntensity = intensity;
            FlashLifeTime = lifetime;
            FlashTime = 0;
            FlashActive = true;

            BehaviorToUse = Behavior.BasicFlash;
        }

        /// <param name="intensity">How bright to make the flash.</param>
        /// <param name="lifetime">How long the effect should last.</param>
        /// <param name="whiteIntensity">How intense what white part of the flash should be.</param>
        /// <param name="distanceMult">Multipler for the distance of the rgb offset.</param>
        /// <param name="under">Whether to draw over or under.</param>
        /// <param name="moveIn">Whether the color distance should shrink as the time left shrinks.</param>

        /// float -> distance mult
        public static void SetCAFlashEffect(float intensity, int lifetime, float whiteIntensity, float distanceMult, bool under, bool moveIn)
        {
            FlashIntensity = intensity;
            FlashLifeTime = lifetime;
            FlashTime = 0;
            FlashActive = true;

            WhiteIntensity = whiteIntensity;
            DistanceMultiplier = distanceMult;
            MoveColorIn = moveIn;

            BehaviorToUse = Behavior.ChromaticAbberationFlash;
        }

        public override void Load()
        {
            Main.OnResolutionChanged += ResizeRenderTarget;
        }

        public override void Unload()
        {
            Main.OnResolutionChanged -= ResizeRenderTarget;
        }

        public override void PostUpdateEverything()
        {
            if (FlashActive)
            {
                if (FlashTime >= FlashLifeTime)
                {
                    FlashActive = false;
                    FlashTime = 0;
                }
                else
                    FlashTime++;

            }
        }

        internal static RenderTarget2D DrawScreenFlash(RenderTarget2D screenTarget1)
        {
            if (FlashActive)
            {
                if (flashRenderTarget is null)
                    ResizeRenderTarget(Vector2.Zero);

                Vector2 FlashPosition = new Vector2(0f, 0f);

                // Draw the screen contents to the flash render target.
                flashRenderTarget.SwapToRenderTarget();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                Main.spriteBatch.Draw(screenTarget1, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, 0, 0f);
                Main.spriteBatch.End();

                // Reset the render target.
                screenTarget1.SwapToRenderTarget();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

                Color drawColor = new(1f, 1f, 1f, MathHelper.Clamp(MathHelper.Lerp(0.5f, 1f, (1f - flashProgress) * FlashIntensity), 0f, 1f));

                Color drawColor1 = new(1f, 0f, 0f, MathHelper.Clamp(MathHelper.Lerp(0.5f, 1f, (1f - flashProgress) * FlashIntensity), 0f, 1f));
                Color drawColor2 = new(0f, 1f, 0f, MathHelper.Clamp(MathHelper.Lerp(0.5f, 1f, (1f - flashProgress) * FlashIntensity), 0f, 1f));
                Color drawColor3 = new(0f, 0f, 1f, MathHelper.Clamp(MathHelper.Lerp(0.5f, 1f, (1f - flashProgress) * FlashIntensity), 0f, 1f));

                // Not doing this causes it to not properly fit on the screen. This extends it to be 100 extra in either direction.
                Rectangle frameOffset = new(-100, -100, Main.screenWidth + 200, Main.screenHeight + 200);
                // Use that and the position to set the origin to the draw position.
                Vector2 origin = FlashPosition + new Vector2(100) - Main.screenPosition;
                for (int i = 0; i < 2; i++)
                    Main.spriteBatch.Draw(flashRenderTarget, FlashPosition - Main.screenPosition, frameOffset, drawColor, 0f, origin, 1f, SpriteEffects.None, 0f);

                Vector2 off1 = new Vector2(5, 10).RotatedBy(MathHelper.ToRadians(FlashTime * 3)) * (1f - flashProgress);
                Vector2 off2 = new Vector2(5, 10).RotatedBy(MathHelper.ToRadians(-120 + (FlashTime * 3))) * (1f - flashProgress);
                Vector2 off3 = new Vector2(5, 10).RotatedBy(MathHelper.ToRadians(120 + (FlashTime * 3))) * (1f - flashProgress);

                Main.spriteBatch.Draw(flashRenderTarget, FlashPosition - Main.screenPosition + off1, frameOffset, drawColor1, 0f, origin, 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(flashRenderTarget, FlashPosition - Main.screenPosition + off2, frameOffset, drawColor2, 0f, origin, 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(flashRenderTarget, FlashPosition - Main.screenPosition + off3, frameOffset, drawColor3, 0f, origin, 1f, SpriteEffects.None, 0f);

                Main.spriteBatch.End();
            }

            return screenTarget1;
        }

        internal static RenderTarget2D DrawCAFlash(RenderTarget2D screenTarget1)
        {
            if (FlashActive)
            {
                //Make sure RT isn't null
                if (flashRenderTarget is null)
                    ResizeRenderTarget(Vector2.Zero);

                //Copy current screen to the render target.
                flashRenderTarget.SwapToRenderTarget();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                Main.spriteBatch.Draw(screenTarget1, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, 0, 0f);
                Main.spriteBatch.End();

                //Reset
                screenTarget1.SwapToRenderTarget();

                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

                //White Base color
                Color drawColor = new(1f, 1f, 1f, MathHelper.Clamp(MathHelper.Lerp(0.5f, 1f, (1f - flashProgress) * FlashIntensity), 0f, 1f));

                //R G B colors
                Color drawColorR = new(1f, 0f, 0f, MathHelper.Clamp(MathHelper.Lerp(0.5f, 1f, (1f - flashProgress) * FlashIntensity), 0f, 1f));
                Color drawColorG = new(0f, 1f, 0f, MathHelper.Clamp(MathHelper.Lerp(0.5f, 1f, (1f - flashProgress) * FlashIntensity), 0f, 1f));
                Color drawColorB = new(0f, 0f, 1f, MathHelper.Clamp(MathHelper.Lerp(0.5f, 1f, (1f - flashProgress) * FlashIntensity), 0f, 1f));


                // Set frame and origin
                // If we just have the frame be (0,0,width,height) then the edges of the screen will be visible and ugly when we move the position
                // so we adjust the bounds to give us a little wiggle room
                Rectangle screenFrame = new(-100, -100, Main.screenWidth + 200, Main.screenHeight + 200);
                Vector2 origin = new Vector2(100, 100) - Main.screenPosition;

                //Draw White Base
                Main.spriteBatch.Draw(flashRenderTarget, -Main.screenPosition, screenFrame, drawColor * WhiteIntensity, 0f, origin, 1f, SpriteEffects.None, 0f);

                //Offset for the RGB
                Vector2 offR = new Vector2(5, 10).RotatedBy(MathHelper.ToRadians(FlashTime * 3)) * (MoveColorIn ? (1f - flashProgress) : 1f) * DistanceMultiplier;
                Vector2 offG = new Vector2(5, 10).RotatedBy(MathHelper.ToRadians(-120 + (FlashTime * 3))) * (MoveColorIn ? (1f - flashProgress) : 1f) * DistanceMultiplier;
                Vector2 offB = new Vector2(5, 10).RotatedBy(MathHelper.ToRadians(120 + (FlashTime * 3))) * (MoveColorIn ? (1f - flashProgress) : 1f) * DistanceMultiplier;

                //Draw RGB
                Main.spriteBatch.Draw(flashRenderTarget, -Main.screenPosition + offR, screenFrame, drawColorR, 0f, origin, 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(flashRenderTarget, -Main.screenPosition + offG, screenFrame, drawColorG, 0f, origin, 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(flashRenderTarget, -Main.screenPosition + offB, screenFrame, drawColorB, 0f, origin, 1f, SpriteEffects.None, 0f);

                Main.spriteBatch.End();
            }

            return screenTarget1;
        }


        private static void ResizeRenderTarget(Vector2 obj)
        {
            flashRenderTarget = new(Main.instance.GraphicsDevice, Main.screenWidth, Main.screenHeight);
        }
    }

    public class FlashDetour : ModSystem
    {
        public override void Load() { On_FilterManager.EndCapture += EndCaptureManager; }
        public override void Unload() { On_FilterManager.EndCapture -= EndCaptureManager; }

        private void EndCaptureManager(On_FilterManager.orig_EndCapture orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
        {
            switch (FlashSystem.BehaviorToUse)
            {
                case FlashSystem.Behavior.BasicFlash:
                    screenTarget1 = FlashSystem.DrawScreenFlash(screenTarget1);
                    break;
                case FlashSystem.Behavior.ChromaticAbberationFlash:
                    screenTarget1 = FlashSystem.DrawCAFlash(screenTarget1);
                    break;
            }
            
            //Draw the original screen
            orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);
        }
    }
}