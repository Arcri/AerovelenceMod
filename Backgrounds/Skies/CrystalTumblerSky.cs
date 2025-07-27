using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using ReLogic.Content;
using System;
using Terraria.ModLoader;
using Terraria;
using AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler;
using Microsoft.Xna.Framework;

namespace AerovelenceMod.Backgrounds.Skies
{
    public class OverlaySystem : ModSystem
    {
        private Texture2D crystalTumblerTexture;
        private delegate void DrawOverlayDelegate();
        private bool hasInjected = false;

        public override void Load()
        {
            try
            {
                crystalTumblerTexture = ModContent.Request<Texture2D>(
                    "AerovelenceMod/Backgrounds/Skies/CrystalTumblerSky",
                    AssetRequestMode.ImmediateLoad
                ).Value;

                if (crystalTumblerTexture == null)
                {
                    ModContent.GetInstance<AerovelenceMod>()?.Logger.Error("Failed to load crystal tumbler texture!");
                    return;
                }

                ModContent.GetInstance<AerovelenceMod>()?.Logger.Warn($"Loaded texture with dimensions: {crystalTumblerTexture.Width}x{crystalTumblerTexture.Height}");

                IL_Main.DrawBackground += Main_DrawBackground_IL;
                ModContent.GetInstance<AerovelenceMod>()?.Logger.Warn("OverlaySystem loaded successfully");
            }
            catch (Exception e)
            {
                ModContent.GetInstance<AerovelenceMod>()?.Logger.Error("Error loading OverlaySystem: " + e.Message + "\n" + e.StackTrace);
            }
        }

        private void Main_DrawBackground_IL(ILContext il)
        {
            var c = new ILCursor(il);
            if (!c.TryGotoNext(i => i.MatchRet()))
            {
                ModContent.GetInstance<AerovelenceMod>()?.Logger.Warn("Couldn't find return in DrawBackground");
                return;
            }
            c.EmitDelegate(() =>
            {
                bool bossIsActive = NPC.AnyNPCs(ModContent.NPCType<CrystalTumbler2>());
                if (!bossIsActive || crystalTumblerTexture == null)
                    return;

                try
                {
                    ModContent.GetInstance<AerovelenceMod>()?.Logger.Warn("Tumbler active");

                    var currentState = Main.spriteBatch.GraphicsDevice.BlendState;

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(
                        SpriteSortMode.Deferred,
                        BlendState.Additive,
                        SamplerState.LinearClamp,
                        DepthStencilState.DepthRead,
                        RasterizerState.CullNone,
                        null,
                        Main.GameViewMatrix.TransformationMatrix
                    );

                    Main.spriteBatch.Draw(
                        crystalTumblerTexture,
                        new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),
                        Color.White * 0.8f
                    );

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(
                        SpriteSortMode.Immediate,
                        currentState,
                        SamplerState.LinearClamp,
                        DepthStencilState.Default,
                        RasterizerState.CullNone,
                        null,
                        Main.GameViewMatrix.TransformationMatrix
                    );
                }
                catch (Exception ex)
                {
                    ModContent.GetInstance<AerovelenceMod>()?.Logger.Warn("Error drawing overlay: " + ex.Message);
                }
            });
        }

        public override void Unload()
        {
            crystalTumblerTexture = null;
            IL_Main.DrawBackground -= Main_DrawBackground_IL;
            ModContent.GetInstance<AerovelenceMod>()?.Logger.Warn("OverlaySystem unloaded");
        }
    }
}