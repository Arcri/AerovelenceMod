using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace AerovelenceMod.Backgrounds.CrystalCaverns.Underground
{
    public class CrystalCavernsConceptBackgroundStyle : ModUndergroundBackgroundStyle
    {
        public override void FillTextureArray(int[] textureSlots)
        {
            int border = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Backgrounds/CrystalCaverns/Underground/CrystalCavernsBlankBorder");
            int fill = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Backgrounds/CrystalCaverns/Underground/CrystalCavernsBlankFill");
            textureSlots[0] = border;
            textureSlots[1] = fill;
            textureSlots[2] = border;
            textureSlots[3] = fill;
        }
    }

    public class CrystalCavernsConceptBackgroundSystem : ModSystem
    {
        private const string BackgroundTexture = "AerovelenceMod/Backgrounds/CrystalCaverns/Underground/ConceptCC";

        private const float FadeInSpeed = 0.05f;
        private const float FadeOutSpeed = 0.025f;
        private static bool visualTargetActive;
        private static float visualOpacity;

        public override void Load()
        {
            if (Main.dedServ)
                return;
            On_Main.DrawBackgroundBlackFill += DrawCrystalCavernsBackground;
            IL_TileDrawing.DrawSingleTile += KeepZeroLightTilesVisible;
            IL_WallDrawing.DrawWalls += KeepZeroLightWallsVisible;
        }

        public override void Unload()
        {
            if (!Main.dedServ)
            {
                On_Main.DrawBackgroundBlackFill -= DrawCrystalCavernsBackground;
                IL_TileDrawing.DrawSingleTile -= KeepZeroLightTilesVisible;
                IL_WallDrawing.DrawWalls -= KeepZeroLightWallsVisible;
            }

            visualTargetActive = false;
            visualOpacity = 0f;
        }

        public override void OnWorldUnload()
        {
            visualTargetActive = false;
            visualOpacity = 0f;
        }

        public override void PostUpdateEverything()
        {
            if (Main.gameMenu || Main.dedServ || Main.LocalPlayer == null || !Main.LocalPlayer.active || !Main.BackgroundEnabled)
            {
                SetVisualTarget(false);
                UpdateVisualOpacity();
                return;
            }
            Player player = Main.LocalPlayer;
            if (!player.dead)
            {
                int cavernTiles = ModContent
                    .GetInstance<global::AerovelenceMod.Content.Biomes.CrystalCavernsTileCount>()
                    .CavernTiles;
                bool underground = player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight;
                int requiredTiles = visualTargetActive ? 500 : 1000;
                SetVisualTarget(underground && cavernTiles >= requiredTiles);
            }
            UpdateVisualOpacity();
        }

        private static void SetVisualTarget(bool value)
        {
            if (visualTargetActive == value)
                return;
            bool wasCompletelyHidden = visualOpacity <= 0f;
            visualTargetActive = value;
            if (value && wasCompletelyHidden && !Main.dedServ)
                Main.renderNow = true;
        }

        private static void UpdateVisualOpacity()
        {
            float previousOpacity = visualOpacity;
            if (visualTargetActive)
                visualOpacity = MathHelper.Clamp(visualOpacity + FadeInSpeed, 0f, 1f);
            else
                visualOpacity = MathHelper.Clamp(visualOpacity - FadeOutSpeed, 0f, 1f);

            if (previousOpacity > 0f && visualOpacity <= 0f && !Main.dedServ)
                Main.renderNow = true;
        }

        private static bool Active =>
            visualOpacity > 0f &&
            !Main.gameMenu &&
            !Main.dedServ &&
            Main.BackgroundEnabled;

        private static void KeepZeroLightTilesVisible(ILContext il)
        {
            ILCursor cursor = new(il);

            while (cursor.TryGotoNext(
                MoveType.After,
                instruction => instruction.MatchCall(typeof(Lighting), nameof(Lighting.GetColor))))
            {
                cursor.EmitDelegate<Func<Color, Color>>(ClampBlackColor);
            }
        }

        private static void KeepZeroLightWallsVisible(ILContext il)
        {
            ILCursor cursor = new(il);
            while (cursor.TryGotoNext(
                MoveType.After,
                instruction => instruction.MatchCall(typeof(Lighting), nameof(Lighting.GetColor))))
            {
                cursor.EmitDelegate<Func<Color, Color>>(ClampBlackColor);
            }
            cursor.Index = 0;
            int vertexColorsLocal = -1;
            if (cursor.TryGotoNext(
                MoveType.After,
                instruction => instruction.MatchLdloca(out vertexColorsLocal),
                instruction => instruction.MatchLdcR4(1f),
                instruction => instruction.MatchCall(typeof(Lighting), nameof(Lighting.GetCornerColors))))
            {
                cursor.EmitLdloc(vertexColorsLocal);
                cursor.EmitDelegate<Func<VertexColors, VertexColors>>(ClampBlackCorners);
                cursor.EmitStloc(vertexColorsLocal);
            }
        }

        private static Color ClampBlackColor(Color color)
        {
            if (Active && color.R == 0 && color.G == 0 && color.B == 0)
            {
                color.R = 1;
                color.G = 1;
                color.B = 1;
            }

            return color;
        }

        private static VertexColors ClampBlackCorners(VertexColors colors)
        {
            if (!Active)
                return colors;
            colors.TopLeftColor = ClampBlackColor(colors.TopLeftColor);
            colors.TopRightColor = ClampBlackColor(colors.TopRightColor);
            colors.BottomLeftColor = ClampBlackColor(colors.BottomLeftColor);
            colors.BottomRightColor = ClampBlackColor(colors.BottomRightColor);
            return colors;
        }

        private static void DrawCrystalCavernsBackground(On_Main.orig_DrawBackgroundBlackFill orig, Main self)
        {
            orig(self);
            if (!Active)
                return;
            Texture2D texture = ModContent.Request<Texture2D>(BackgroundTexture).Value;
            float parallaxX = Main.screenPosition.X * 0.08f;
            float parallaxY = MathHelper.Clamp((Main.screenPosition.Y - (float)Main.rockLayer * 16f) * 0.02f, -54f, 54f);
            float x = -(parallaxX % texture.Width);
            float y = (Main.screenHeight - texture.Height) * 0.5f - parallaxY;
            while (x > 0f)
                x -= texture.Width;
            Color drawColor = Color.White * visualOpacity;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            for (float drawX = x - texture.Width; drawX < Main.screenWidth + texture.Width; drawX += texture.Width)
                Main.spriteBatch.Draw(texture, new Vector2(drawX, y), drawColor);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}