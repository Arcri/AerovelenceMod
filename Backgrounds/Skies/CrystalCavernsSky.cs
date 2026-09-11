
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AerovelenceMod.Backgrounds.Skies
{
    public class CrystalCavernsSky : CustomSky
    {
        private bool isActive = false;
        private float intensity = 0f;
        const float increment = 0.01f;
        private bool deactivating = false;

        private Bolt[] bolts;
        private int ticksUntilNextBolt;
        private UnifiedRandom random = new UnifiedRandom();
        private static Texture2D[] boltTextures = [
            AerovelenceMod.Instance.Assets.Request<Texture2D>("Backgrounds/Skies/CrystalCavernsSkyBolt1", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
            AerovelenceMod.Instance.Assets.Request<Texture2D>("Backgrounds/Skies/CrystalCavernsSkyBolt2", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
            AerovelenceMod.Instance.Assets.Request<Texture2D>("Backgrounds/Skies/CrystalCavernsSkyBolt3", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
            AerovelenceMod.Instance.Assets.Request<Texture2D>("Backgrounds/Skies/CrystalCavernsSkyBolt4", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
            AerovelenceMod.Instance.Assets.Request<Texture2D>("Backgrounds/Skies/CrystalCavernsSkyBolt5", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value];
        private static Texture2D[] boltFlashTextures = [
            AerovelenceMod.Instance.Assets.Request<Texture2D>("Backgrounds/Skies/CrystalCavernsSkyFlash1", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
            AerovelenceMod.Instance.Assets.Request<Texture2D>("Backgrounds/Skies/CrystalCavernsSkyFlash2", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
            AerovelenceMod.Instance.Assets.Request<Texture2D>("Backgrounds/Skies/CrystalCavernsSkyFlash3", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
            AerovelenceMod.Instance.Assets.Request<Texture2D>("Backgrounds/Skies/CrystalCavernsSkyFlash4", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
            AerovelenceMod.Instance.Assets.Request<Texture2D>("Backgrounds/Skies/CrystalCavernsSkyFlash5", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value];

        private struct Bolt
        {
            public Texture2D Texture;
            public Texture2D FlashTexture;
            public Vector2 Position;
            public float Rotation;
            public float Depth;
            public int Life;
            public bool IsAlive;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            deactivating = false;
            isActive = true;
            bolts = new Bolt[12];
            ticksUntilNextBolt = random.Next(600, 1200);
            for (int i = 0; i < bolts.Length; i++)
            {
                bolts[i].IsAlive = false;
                int textureNum = random.Next(5);
                bolts[i].Texture = boltTextures[textureNum];
                bolts[i].FlashTexture = boltFlashTextures[textureNum];
            }
        }

        public override void Deactivate(params object[] args)
        {
            deactivating = true;
            intensity -= increment;
            if (intensity <= 0f)
            {
                intensity = 0f;
                isActive = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth == float.MaxValue)
                spriteBatch.Draw(AerovelenceMod.Instance.Assets.Request<Texture2D>("Backgrounds/Skies/CrystalCavernsSky", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                    new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * intensity);
            if (CrystalRainAtmosphere.IsRainActive)
                DrawLightning(spriteBatch, minDepth, maxDepth);
        }

        public override bool IsActive()
        {
            return isActive;
        }

        public override void Reset()
        {
            isActive = false;
            intensity = 0f;
            deactivating = false;
            bolts = null;
        }

        public override void Update(GameTime gameTime)
        {
            if (isActive && !deactivating)
            {
                intensity += increment;
            }
            else
            {
                Deactivate();
            }

            intensity = Math.Clamp(intensity, 0, 0.3f);

            UpdateLightning();
        }

        public override Color OnTileColor(Color color)
        {
            float amount = intensity * 1.25f;
            float redMod = amount;
            float greenMod = amount * 1.25f;
            float blueMod = amount * 0.875f;
            return color.MultiplyRGB(new Color(1f - redMod, 1f - greenMod, 1f - blueMod));
        }
        
        private void UpdateLightning()
        {
            if (bolts == null)
                return;
            if (ticksUntilNextBolt <= 0 && !deactivating && CrystalRainAtmosphere.IsRainActive)
            {
                ticksUntilNextBolt = Main.hardMode ? random.Next(600, 1080) : random.Next(900, 1800);
                int i;
                for (i = 0; bolts[i].IsAlive && i != bolts.Length - 1; i++)
                {
                }
                bolts[i].IsAlive = true;
                bolts[i].Position.X = random.NextFloat() * ((float)Main.maxTilesX * 16f + 4000f) - 2000f;
                bolts[i].Position.Y = random.NextFloat() * 500f;
                bolts[i].Rotation = random.NextFloat(0, MathHelper.PiOver4) - MathHelper.PiOver4 / 2;
                bolts[i].Depth = random.NextFloat() * 8f + 2f;
                bolts[i].Life = 30;
            }
            if (ticksUntilNextBolt > 0)
                ticksUntilNextBolt--;
            for (int j = 0; j < bolts.Length; j++)
            {
                if (bolts[j].IsAlive)
                {
                    bolts[j].Life--;
                    if (bolts[j].Life <= 0)
                    {
                        bolts[j].IsAlive = false;
                    }
                }
            }
        }

        private void DrawLightning(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (bolts == null)
                return;
            for (int i = 0; i < bolts.Length; i++)
            {
                if (!bolts[i].IsAlive || !(bolts[i].Depth > minDepth) || !(bolts[i].Depth < maxDepth))
                {
                    continue;
                }
                Texture2D texture = bolts[i].Texture;
                int life = bolts[i].Life;
                if (life > 26 && life % 2 == 0)
                {
                }
                Vector2 vector3 = Main.screenPosition + new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1);
                Vector2 position = (bolts[i].Position - vector3) * new Vector2(1f / bolts[i].Depth, 0.6f / bolts[i].Depth) + vector3 - Main.screenPosition;
                float lifeColorDecay = life / 30f;
                spriteBatch.Draw(
                    texture: texture,
                    position: position,
                    sourceRectangle: null,
                    color: (new Color(205, 215, 255, 0) * lifeColorDecay * (intensity / 0.3f) * 0.6f),
                    rotation: bolts[i].Rotation,
                    origin: Vector2.Zero,
                    scale: 5f / bolts[i].Depth,
                    effects: 0,
                    layerDepth: 0f
                    );  
            }
        }
    }
}
