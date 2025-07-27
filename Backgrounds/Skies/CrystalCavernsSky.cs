
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

        private struct Bolt
        {
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
            bolts = new Bolt[500];
            for (int i = 0; i < bolts.Length; i++)
            {
                bolts[i].IsAlive = false;
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
            spriteBatch.Draw(AerovelenceMod.Instance.Assets.Request<Texture2D>("Backgrounds/Skies/CrystalCavernsSky", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                    new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * intensity);
            if (Main.raining)
                DrawLightning(spriteBatch, minDepth, maxDepth);
        }

        public override bool IsActive()
        {
            return isActive;
        }

        public override void Reset()
        {
            isActive = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (isActive && !deactivating)
            {
                intensity += increment;
                /*if (intensity > 1f)
                {
                    intensity = MathHelper.Lerp(intensity, 1, 0.2f); //1f;
                }*/
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
            if (ticksUntilNextBolt <= 0)
            {
                ticksUntilNextBolt = random.Next(3, 6);
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
            for (int i = 0; i < bolts.Length; i++)
            {
                if (!bolts[i].IsAlive || !(bolts[i].Depth > minDepth) || !(bolts[i].Depth < maxDepth))
                {
                    continue;
                }
                Texture2D value = AerovelenceMod.Instance.Assets.Request<Texture2D>("Backgrounds/Skies/CrystalCavernsSkyBolt", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                int life = bolts[i].Life;
                if (life > 26 && life % 2 == 0)
                {
                    value = AerovelenceMod.Instance.Assets.Request<Texture2D>("Backgrounds/Skies/CrystalCavernsSkyFlash", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                }
                Vector2 vector3 = Main.screenPosition + new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1);
                Vector2 position = (bolts[i].Position - vector3) * new Vector2(1f / bolts[i].Depth, 0.9f / bolts[i].Depth) + vector3 - Main.screenPosition;
                float lifeColorDecay = life / 30f;
                spriteBatch.Draw(
                    texture: value,
                    position: position,
                    sourceRectangle: null,
                    color: Color.White * lifeColorDecay,
                    rotation: bolts[i].Rotation, //
                    origin: Vector2.Zero, //
                    scale: 5f / bolts[i].Depth,
                    effects: 0,
                    layerDepth: 0f
                    );
            }
        }
    }
}