using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Terraria.ModLoader;
using Terraria;

namespace AerovelenceMod.Common.Systems
{
    public class PixellationSystem : ModSystem
    {
        // KainSett: Credits for this whole class to Naka, and thanks to petrichor: I got my hands on this gem thanks to them
        /* Credits for this whole class to:
         * Naka for developing
         * Petrichor
         * KainSett
         */

        public enum RenderType
        {
            AlphaBlend,
            //NonPremultiplied,
            Additive
        }
        private static List<Action> DrawActions { get; } = new();
        //private static List<Action> DrawActionsNonPremultiplied { get; } = new();
        private static List<Action> DrawActionsAdditive { get; } = new();
        private static List<Action> PrimitiveActions { get; } = new();
        private static RenderTarget2D AlphaBlendTarget { get; set; }
        private static RenderTarget2D AdditiveTarget { get; set; }
        private static RenderTarget2D PrimitiveTarget { get; set; }
        public static Matrix World { get => _world; }
        public static Matrix View { get => _view; }
        public static Matrix Projection { get => _projection; }
        private static Matrix _world;
        private static Matrix _view;
        private static Matrix _projection;
        //private static RenderTarget2D NonPremultipliedTarget { get; set; }
        public override void Load()
        {

            if (!Main.dedServ)
            {
                Main.OnResolutionChanged += InitializeRT;
                Main.RunOnMainThread(() => {
                    AlphaBlendTarget = new(Main.instance.GraphicsDevice, Main.screenWidth / 2, Main.screenHeight / 2);
                    AdditiveTarget = new(Main.instance.GraphicsDevice, Main.screenWidth / 2, Main.screenHeight / 2);
                    PrimitiveTarget = new RenderTarget2D(Main.instance.GraphicsDevice, Main.screenWidth / 2, Main.screenHeight / 2);
                });
            }
            On_Main.DrawProjectiles += On_Main_DrawProjectiles;
            On_Main.CheckMonoliths += DrawToRT;
        }

        private void DrawToRT(On_Main.orig_CheckMonoliths orig)
        {
            _world = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0));
            _view = Main.GameViewMatrix.TransformationMatrix;
            _projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            orig.Invoke();
            // has to go here bc order of execution
            var gd = Main.graphics.GraphicsDevice;
            var oldRTs = gd.GetRenderTargets();
            gd.SetRenderTarget(AlphaBlendTarget);
            gd.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            foreach (var action in DrawActions)
            {
                action.Invoke();
            }
            Main.spriteBatch.End();
            gd.SetRenderTarget(PrimitiveTarget);
            Main.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Matrix.Identity);
            gd.Clear(Color.Transparent);
            foreach (var action in PrimitiveActions)
            {
                action.Invoke();
            }
            Main.spriteBatch.End();
            gd.SetRenderTarget(AdditiveTarget);
            Main.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive, Main.DefaultSamplerState, default, Main.Rasterizer, null, Matrix.Identity);
            gd.Clear(Color.Transparent);
            foreach (var action in DrawActionsAdditive)
            {
                action.Invoke();
            }
            Main.spriteBatch.End();
            gd.SetRenderTargets(oldRTs);
            DrawActions.Clear();
            DrawActionsAdditive.Clear();
            PrimitiveActions.Clear();
        }
        public static void DrawPixelPrimitive(Action action)
        {
            PrimitiveActions.Add(action);
        }
        public override void Unload()
        {
            On_Main.DrawProjectiles -= On_Main_DrawProjectiles;
        }
        private void InitializeRT(Vector2 obj)
        {
            if (Main.dedServ)
            {
                return;
            }
            AlphaBlendTarget?.Dispose();
            //NonPremultipliedTarget?.Dispose();
            AdditiveTarget?.Dispose();
            PrimitiveTarget?.Dispose();
            GraphicsDevice gd = Main.instance.GraphicsDevice;
            int width = Main.screenWidth / 2;
            int height = Main.screenHeight / 2;
            AlphaBlendTarget = new(gd, width, height);
            AdditiveTarget = new(gd, width, height);
            PrimitiveTarget = new(gd, width, height);
        }
        private void On_Main_DrawProjectiles(On_Main.orig_DrawProjectiles orig, Main self)
        {
            DrawRT();
            orig.Invoke(self);
        }
        /// <summary>
        /// Queues a draw action to the pixelation system.
        /// Remember to halve the scale and draw position!
        /// </summary>
        /// <param name="action"></param>
        public static void QueuePixelationAction(Action action, RenderType type)
        {
            switch (type)
            {
                case RenderType.Additive:
                    DrawActionsAdditive.Add(action);
                    break;
                case RenderType.AlphaBlend:
                    DrawActions.Add(action);
                    break;
            }
        }
        /// <summary>
        /// Draws the RT with its pixelated content to 2x scale
        /// </summary>
        private static void DrawRT()
        {
            if (AlphaBlendTarget == null || AlphaBlendTarget.IsDisposed)
            {
                return;
            }
            Main.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Matrix.Identity);
            Main.spriteBatch.Draw(AlphaBlendTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Matrix.Identity);
            Main.spriteBatch.Draw(PrimitiveTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
            Main.spriteBatch.End();
            if (AdditiveTarget == null || AdditiveTarget.IsDisposed)
            {
                return;
            }
            Main.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(AdditiveTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
            Main.spriteBatch.End();
        }
    }
}