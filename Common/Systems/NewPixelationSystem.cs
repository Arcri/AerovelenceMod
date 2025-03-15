using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Terraria.ModLoader;
using Terraria;
using System.Linq;
using System.Threading;
using AerovelenceMod.Common.Interfaces;

namespace AerovelenceMod.Common.Systems
{
    //This is VERY VERY heavily based on Starlight River's Pixelation system
    //https://github.com/ProjectStarlight/StarlightRiver/blob/master/Core/Systems/PixelationSystem/PixelationSystem.cs

    //Their code is under the GPL v3 License which basically means that if you use their code then you have to 
    // open source yours too.

    public enum RenderLayer : int
    {
        UnderTiles = 1,
        UnderNPCs = 2,
        UnderProjectiles = 3,
        OverPlayers = 4,
        Dusts = 5,
    }

    public class NewPixelationSystem : ModSystem
    {
        public List<PixelationTarget> pixelationTargets = new();

        public override void Load()
        {
            if (Main.dedServ)
                return;

            On_Main.DrawCachedProjs += DrawTargets;
            On_Main.DrawDust += DrawDustTargets;
        }

        public override void PostSetupContent()
        {
            RegisterScreenTarget(RenderLayer.UnderTiles);

            RegisterScreenTarget(RenderLayer.UnderNPCs);

            RegisterScreenTarget(RenderLayer.UnderProjectiles);

            RegisterScreenTarget(RenderLayer.OverPlayers);

            RegisterScreenTarget(RenderLayer.Dusts);
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            On_Main.DrawCachedProjs -= DrawTargets;
            On_Main.DrawDust -= DrawDustTargets;
        }


        //Calls DrawTarget() on all everything in pixelationTargets, according to what layer they are on
        private void DrawTargets(On_Main.orig_DrawCachedProjs orig, Main self, List<int> projCache, bool startSpriteBatch)
        {
            SpriteBatch sb = Main.spriteBatch;

            orig(self, projCache, startSpriteBatch);

            if (projCache.Equals(Main.instance.DrawCacheProjsBehindNPCsAndTiles))
            {
                foreach (PixelationTarget target in pixelationTargets.Where(t => t.Active && t.renderType == RenderLayer.UnderTiles))
                {
                    DrawTarget(target, Main.spriteBatch, !startSpriteBatch);
                }
            }

            if (projCache.Equals(Main.instance.DrawCacheProjsBehindNPCs))
            {
                foreach (PixelationTarget target in pixelationTargets.Where(t => t.Active && t.renderType == RenderLayer.UnderNPCs))
                {
                    DrawTarget(target, Main.spriteBatch, !startSpriteBatch);
                }
            }

            if (projCache.Equals(Main.instance.DrawCacheProjsBehindProjectiles))
            {
                foreach (PixelationTarget target in pixelationTargets.Where(t => t.Active && t.renderType == RenderLayer.UnderProjectiles))
                {
                    DrawTarget(target, Main.spriteBatch, !startSpriteBatch);
                }
            }

            if (projCache.Equals(Main.instance.DrawCacheProjsOverPlayers))
            {
                foreach (PixelationTarget target in pixelationTargets.Where(t => t.Active && t.renderType == RenderLayer.OverPlayers))
                {
                    DrawTarget(target, Main.spriteBatch, !startSpriteBatch);
                }
            }
        }

        private void DrawDustTargets(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);

            foreach (PixelationTarget target in pixelationTargets.Where(t => t.Active && t.renderType == RenderLayer.Dusts))
            {
                DrawTarget(target, Main.spriteBatch, false);
            }
        }

        private void DrawTarget(PixelationTarget target, SpriteBatch sb, bool endSpriteBatch = true)
        {
            if (endSpriteBatch)
            {
                sb.End();
            }

            BlendState blendState = BlendState.AlphaBlend;

            //TODO: see if this works without immediate
            sb.Begin(SpriteSortMode.Immediate, blendState, Main.DefaultSamplerState,
                DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            sb.Draw(target.pixelationTarget2.RenderTarget, Vector2.Zero, null, Color.White, 0, new Vector2(0, 0), 2f, SpriteEffects.None, 0);

            sb.End();


            if (endSpriteBatch)
                sb.Begin(default, default, Main.DefaultSamplerState, default, RasterizerState.CullNone, default, Main.GameViewMatrix.TransformationMatrix);
        }

        public void RegisterScreenTarget(RenderLayer renderType = RenderLayer.UnderProjectiles)
        {
            Main.QueueMainThreadAction(() => pixelationTargets.Add(new PixelationTarget(renderType)));
        }

        public void QueueRenderAction(RenderLayer renderType, Action renderAction, int order = 0)
        {
            PixelationTarget target = pixelationTargets.Find(t => t.renderType == renderType);

            target.pixelationDrawActions.Add(new Tuple<Action, int>(renderAction, order));
            target.renderTimer = 2;
        }
    }

    public class PixelationTarget
    {
        public int renderTimer;

        // list of actions, and their draw order. Default order is zero, but actions with an order of 1 are drawn over 0, etc.
        public List<Tuple<Action, int>> pixelationDrawActions;

        public ScreenTarget pixelationTarget;

        public ScreenTarget pixelationTarget2;

        public RenderLayer renderType;

        public bool Active => renderTimer > 0;

        public PixelationTarget(RenderLayer renderType)
        {
            pixelationDrawActions = new List<Tuple<Action, int>>();

            pixelationTarget = new(DrawPixelTarget, () => Active, 1f);
            pixelationTarget2 = new(DrawPixelTarget2, () => Active, 1.1f);

            this.renderType = renderType;
        }

        //Draw RenderTarget at half scale
        private void DrawPixelTarget2(SpriteBatch sb)
        {
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            sb.End();
            sb.Begin(default, default, Main.DefaultSamplerState, default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);

            sb.Draw(pixelationTarget.RenderTarget, Vector2.Zero, null, Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);

            sb.End();
            sb.Begin(default, default, Main.DefaultSamplerState, default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        private void DrawPixelTarget(SpriteBatch sb)
        {
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            sb.End();
            sb.Begin(default, default, Main.DefaultSamplerState, default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);

            foreach (Tuple<Action, int> tuple in pixelationDrawActions.OrderBy(t => t.Item2))
            {
                tuple.Item1.Invoke();
            }

            pixelationDrawActions.Clear();
            renderTimer--;

            sb.End();
            sb.Begin(default, default, Main.DefaultSamplerState, default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

    }

    //ScreenTarget and ScreenTarget Handler are especially ripped from SLR
    public class ScreenTarget
    {
        /// <summary>
        /// What gets rendered to this screen target. Spritebatch is automatically started and RT automatically set, you only need to write the code for what you are rendering.
        /// </summary>
        public Action<SpriteBatch> drawFunct;

        /// <summary>
        /// If this render target should be rendered. Make sure this it as restrictive as possible to prevent uneccisary rendering work.
        /// </summary>
        public Func<bool> activeFunct;

        /// <summary>
        /// Optional function that runs when the screen is resized. Returns the size the render target should be. Return null to prevent resizing.
        /// </summary>
        public Func<Vector2, Vector2?> onResize;

        /// <summary>
        /// Where this render target should fall in the order of rendering. Important if you want to render something to chain into another RT.
        /// </summary>
        public float order;

        public RenderTarget2D RenderTarget { get; set; }

        public ScreenTarget(Action<SpriteBatch> draw, Func<bool> active, float order, Func<Vector2, Vector2?> onResize = null)
        {
            if (Main.dedServ)
                return;

            drawFunct = draw;
            activeFunct = active;
            this.order = order;
            this.onResize = onResize;

            Vector2? initialDims = onResize is null ? new Vector2(Main.screenWidth, Main.screenHeight) : onResize(new Vector2(Main.screenWidth, Main.screenHeight));
            Main.QueueMainThreadAction(() => RenderTarget = new RenderTarget2D(Main.instance.GraphicsDevice, (int)initialDims?.X, (int)initialDims?.Y, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents));

            ScreenTargetHandler.AddTarget(this);
        }

        /// <summary>
        /// Foribly resize a target to a new size
        /// </summary>
        /// <param name="size"></param>
        public void ForceResize(Vector2 size)
        {
            if (Main.dedServ)
                return;

            RenderTarget.Dispose();
            RenderTarget = new RenderTarget2D(Main.instance.GraphicsDevice, (int)size.X, (int)size.Y, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        }
    }

    internal class ScreenTargetHandler : ModSystem, IOrderedLoadable
    {
        public static List<ScreenTarget> targets = new();
        public static Semaphore targetSem = new(1, 1);

        private static int firstResizeTime = 0;

        public float Priority => 1;

        new public void Load() //We want to use IOrderedLoadable's load here to preserve our load order
        {
            if (!Main.dedServ)
            {
                On_Main.CheckMonoliths += RenderScreens;
                Main.OnResolutionChanged += ResizeScreens;
            }
        }

        new public void Unload()
        {
            if (!Main.dedServ)
            {
                On_Main.CheckMonoliths -= RenderScreens;
                Main.OnResolutionChanged -= ResizeScreens;

                Main.QueueMainThreadAction(() =>
                {
                    if (targets != null)
                    {
                        targets.ForEach(n => n.RenderTarget?.Dispose());
                        targets.Clear();
                        targets = null;
                    }
                    else
                    {
                        Mod.Logger.Warn("Screen targets was null, all ScreenTargets may not have been released! (leaking VRAM!)");
                    }
                });
            }
        }

        /// <summary>
        /// Registers a new screen target and orders it into the list. Called automatically by the constructor of ScreenTarget!
        /// </summary>
        /// <param name="toAdd"></param>
        public static void AddTarget(ScreenTarget toAdd)
        {
            targetSem.WaitOne();

            targets.Add(toAdd);
            targets.Sort((a, b) => a.order.CompareTo(b.order));

            targetSem.Release();
        }

        /// <summary>
        /// Removes a screen target from the targets list. Should not normally need to be used.
        /// </summary>
        /// <param name="toRemove"></param>
        public static void RemoveTarget(ScreenTarget toRemove)
        {
            targetSem.WaitOne();

            targets.Remove(toRemove);
            targets.Sort((a, b) => a.order - b.order > 0 ? 1 : -1);

            targetSem.Release();
        }

        public static void ResizeScreens(Vector2 obj)
        {
            if (Main.gameMenu || Main.dedServ)
                return;

            targetSem.WaitOne();

            targets.ForEach(n =>
            {
                Vector2? size = obj;

                if (n.onResize != null)
                    size = n.onResize(obj);

                if (size != null)
                {
                    n.RenderTarget?.Dispose();
                    n.RenderTarget = new RenderTarget2D(Main.instance.GraphicsDevice, (int)size?.X, (int)size?.Y, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                }
            });

            targetSem.Release();
        }

        private void RenderScreens(On_Main.orig_CheckMonoliths orig)
        {
            orig();

            if (Main.gameMenu || Main.dedServ)
                return;

            RenderTargetBinding[] bindings = Main.graphics.GraphicsDevice.GetRenderTargets();

            targetSem.WaitOne();

            foreach (ScreenTarget target in targets)
            {
                if (target.drawFunct is null) //allows for RTs which dont draw in the default loop, like the lighting tile buffers
                    continue;

                Main.spriteBatch.Begin(default, default, Main.DefaultSamplerState, default, RasterizerState.CullNone, default);
                Main.graphics.GraphicsDevice.SetRenderTarget(target.RenderTarget);
                Main.graphics.GraphicsDevice.Clear(Color.Transparent);

                if (target.activeFunct())
                    target.drawFunct(Main.spriteBatch);

                Main.spriteBatch.End();
            }

            Main.graphics.GraphicsDevice.SetRenderTargets(bindings);

            targetSem.Release();
        }

        public override void PostUpdateEverything()
        {
            if (Main.gameMenu)
                firstResizeTime = 0;
            else
                firstResizeTime++;

            if (firstResizeTime == 20)
                ResizeScreens(new Vector2(Main.screenWidth, Main.screenHeight));
        }
    }


    //Seperate Pixelization System for when we want to additive draw
    //Is there a smarter way to have done this? Absolutely.
    //However this was the path of least resistance so I did it anyway
    //I could only get this to work when it was a separate class for some reason
    //Make sure to use Effect matrix if using a shader with this 
    public class NewAdditivePixelationSystem : ModSystem
    {
        public List<PixelationTarget> pixelationTargets = new();

        public override void Load()
        {
            if (Main.dedServ)
                return;

            On_Main.DrawCachedProjs += DrawTargets;
            On_Main.DrawDust += DrawDustTargets;
        }

        public override void PostSetupContent()
        {
            RegisterScreenTarget(RenderLayer.UnderTiles);

            RegisterScreenTarget(RenderLayer.UnderNPCs);

            RegisterScreenTarget(RenderLayer.UnderProjectiles);

            RegisterScreenTarget(RenderLayer.OverPlayers);

            RegisterScreenTarget(RenderLayer.Dusts);
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            On_Main.DrawCachedProjs -= DrawTargets;
            On_Main.DrawDust -= DrawDustTargets;
        }


        //Calls DrawTarget() on all everything in pixelationTargets, according to what layer they are on
        private void DrawTargets(On_Main.orig_DrawCachedProjs orig, Main self, List<int> projCache, bool startSpriteBatch)
        {
            SpriteBatch sb = Main.spriteBatch;

            orig(self, projCache, startSpriteBatch);

            if (projCache.Equals(Main.instance.DrawCacheProjsBehindNPCsAndTiles))
            {
                foreach (PixelationTarget target in pixelationTargets.Where(t => t.Active && t.renderType == RenderLayer.UnderTiles))
                {
                    DrawTarget(target, Main.spriteBatch, !startSpriteBatch);
                }
            }

            if (projCache.Equals(Main.instance.DrawCacheProjsBehindNPCs))
            {
                foreach (PixelationTarget target in pixelationTargets.Where(t => t.Active && t.renderType == RenderLayer.UnderNPCs))
                {
                    DrawTarget(target, Main.spriteBatch, !startSpriteBatch);
                }
            }

            if (projCache.Equals(Main.instance.DrawCacheProjsBehindProjectiles))
            {
                foreach (PixelationTarget target in pixelationTargets.Where(t => t.Active && t.renderType == RenderLayer.UnderProjectiles))
                {
                    DrawTarget(target, Main.spriteBatch, !startSpriteBatch);
                }
            }

            if (projCache.Equals(Main.instance.DrawCacheProjsOverPlayers))
            {
                foreach (PixelationTarget target in pixelationTargets.Where(t => t.Active && t.renderType == RenderLayer.OverPlayers))
                {
                    DrawTarget(target, Main.spriteBatch, !startSpriteBatch);
                }
            }
        }

        private void DrawDustTargets(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);

            foreach (PixelationTarget target in pixelationTargets.Where(t => t.Active && t.renderType == RenderLayer.Dusts))
            {
                DrawTarget(target, Main.spriteBatch, false);
            }
        }

        private void DrawTarget(PixelationTarget target, SpriteBatch sb, bool endSpriteBatch = true)
        {
            if (endSpriteBatch)
                sb.End();

            sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState,
                DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            sb.Draw(target.pixelationTarget2.RenderTarget, Vector2.Zero, null, Color.White, 0, new Vector2(0, 0), 2f, SpriteEffects.None, 0);

            sb.End();

            //Reset spritebatch again to stop bleedthrough for some reason
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState,
                DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            sb.End();


            if (endSpriteBatch)
                sb.Begin(default, default, Main.DefaultSamplerState, default, RasterizerState.CullNone, default, Main.GameViewMatrix.TransformationMatrix);
        }

        public void RegisterScreenTarget(RenderLayer renderType = RenderLayer.UnderProjectiles)
        {
            Main.QueueMainThreadAction(() => pixelationTargets.Add(new PixelationTarget(renderType)));
        }

        public void QueueRenderAction(RenderLayer renderType, Action renderAction, int order = 0)
        {
            PixelationTarget target = pixelationTargets.Find(t => t.renderType == renderType);

            target.pixelationDrawActions.Add(new Tuple<Action, int>(renderAction, order));
            target.renderTimer = 2;
        }
    }

}