using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.UI;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria;

namespace AerovelenceMod.Content.UI
{
    public class VerletPlayer : ModPlayer
    {
        public Vector2 verletPos1 = Vector2.Zero;
        public Vector2 verletPos2 = Vector2.Zero;

        int justRightClicked;
        int justLeftClicked;
        public override void PostUpdate()
        {
            if (Main.mouseRight)
            {
                justRightClicked = 2;
                if (verletPos1 == Vector2.Zero)
                {
                    verletPos1 = Main.MouseWorld;
                    Main.NewText("pos 1 set", Color.Green);
                }
                else
                {
                    verletPos2 = Main.MouseWorld;
                }
            }

            if (Main.mouseLeft)
                justLeftClicked = 2;

            int bothClick = 0;
            if (--justRightClicked >= 0 && Main.mouseRightRelease)
            {
                bothClick++;
            }

            if (--justLeftClicked >= 0 && Main.mouseLeftRelease)
            {
                bothClick++;
            }

            if (bothClick == 2)
            {
                verletPos1 = Vector2.Zero;
                verletPos2 = Vector2.Zero;
                Main.NewText("resetted", Color.Red);
            }
        }
    }

    public class VerletUI : UIState
    {

        public override void Draw(SpriteBatch spriteBatch)
        {
            Player p = Main.LocalPlayer;
            var verletPos1 = p.GetModPlayer<VerletPlayer>().verletPos1;
            var verletPos2 = p.GetModPlayer<VerletPlayer>().verletPos2;

            if (verletPos1 != Vector2.Zero && verletPos2 != Vector2.Zero)
            {
                Rectangle pixelSource = new Rectangle(0, 0, 2, 2);

                float calcDist = 1f;
                Vector2 dir = (verletPos2 - verletPos1).SafeNormalize(Vector2.Zero);
                for (int i = 0; i < 5000; i++)
                {
                    Vector2 stepCalcPos = (verletPos1 - Main.screenPosition) + (dir * calcDist);
                    if (Vector2.Distance(stepCalcPos, verletPos2 - Main.screenPosition) < 1)
                    {
                        break;
                    }
                    calcDist += 1f;
                }

                List<Vector2> points = new List<Vector2>();
                points.Add(verletPos1 - Main.screenPosition);

                Vector2 middlePoint = (verletPos1 - Main.screenPosition) + (dir * (calcDist * 0.15f));
                middlePoint.Y += 16;

                middlePoint = (verletPos1 - Main.screenPosition) + (dir * (calcDist * 0.25f));
                middlePoint.Y += 24;
                points.Add(middlePoint);

                middlePoint = (verletPos1 - Main.screenPosition) + (dir * (calcDist * 0.35f));
                middlePoint.Y += 30;
                points.Add(middlePoint);

                middlePoint = (verletPos1 - Main.screenPosition) + (dir * (calcDist * 0.45f));
                middlePoint.Y += 35;
                points.Add(middlePoint);

                middlePoint = (verletPos1 - Main.screenPosition) + (dir * (calcDist * 0.5f));
                middlePoint.Y += 35;
                points.Add(middlePoint);

                middlePoint = (verletPos1 - Main.screenPosition) + (dir * (calcDist * 0.65f));
                middlePoint.Y += 35;
                points.Add(middlePoint);

                middlePoint = (verletPos1 - Main.screenPosition) + (dir * (calcDist * 0.75f));
                middlePoint.Y += 32;
                points.Add(middlePoint);

                middlePoint = (verletPos1 - Main.screenPosition) + (dir * (calcDist * 0.85f));
                middlePoint.Y += 24;
                points.Add(middlePoint);

                middlePoint = (verletPos1 - Main.screenPosition) + (dir * (calcDist * 0.95f));
                middlePoint.Y += 8;
                points.Add(middlePoint);

                points.Add(verletPos2 - Main.screenPosition);

                for (int s = 0; s < points.Count - 1; s++)
                {
                    float progress = 0f;
                    for (int i = 0; i < 5000; i++)
                    {
                        Vector2 startPos = points[s];
                        Vector2 endPos = points[(s + 1)];
                        Vector2 direction = (endPos - startPos).SafeNormalize(Vector2.Zero);
                        Vector2 progPoint = startPos + (direction * progress);
                        Main.EntitySpriteDraw(TextureAssets.MagicPixel.Value, progPoint, pixelSource, Color.White, 0f, pixelSource.Size() / 2, 1f, SpriteEffects.None);
                        if (Vector2.Distance(progPoint, endPos) < 1)
                        {
                            break;
                        }
                        progress += 1f;
                    }

                }

                for (int point = 0; point < points.Count; point++)
                {
                    pixelSource = new Rectangle(0, 0, 5, 5);
                    //Main.EntitySpriteDraw(TextureAssets.MagicPixel.Value, points[point], pixelSource, Color.Red, 0f, pixelSource.Size() / 2, 1f, SpriteEffects.None);
                }

                /*for (int split = 10; split > 0; split -= 2)
                {
                    if (split != 0)
                    {
                        Vector2 middlePoint = (verletPos1 - Main.screenPosition) + (dir * calcDist / split);
                        middlePoint.Y += 8;
                        //Main.EntitySpriteDraw(TextureAssets.MagicPixel.Value, middlePoint, pixelSource, Color.Yellow, 0f, pixelSource.Size() / 2, 1f, SpriteEffects.None);
                    }
                }*/
            }
        }
    }

    public class ShowVerlets : ModSystem
    {
        public UserInterface ui;
        internal VerletUI theUI;


        public override void Load()
        {
            if (!Main.dedServ)
            {
                theUI = new();
                ui = new();
                ui.SetState(theUI);

            }
        }
        public override void UpdateUI(GameTime gameTime)
        {
            ui?.Update(gameTime);
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Dresser Window")); //Vanilla: Interface Logic 3
            if (index != -1)
            {
                var p = Main.LocalPlayer;
                if (p.GetModPlayer<VerletPlayer>().verletPos2 != Vector2.Zero)
                {
                    layers.Insert(index, new LegacyGameInterfaceLayer(
                        "Aerovelence: VerletTEst",
                        delegate
                        {
                            ui.Draw(Main.spriteBatch, new GameTime());
                            return true;
                        },
                        InterfaceScaleType.UI)
                    );
                }
            }
        }
    }
}
