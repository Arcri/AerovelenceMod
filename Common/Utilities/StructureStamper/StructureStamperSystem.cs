using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria.UI;
using ReLogic.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameInput;
using System;

namespace AerovelenceMod.Common.Utilities.StructureStamper
{
    public class StructureStamperSystem : ModSystem
    {
        private Vector2? Point1;
        private Vector2? Point2;
        private bool awaitingName = false;
        private string structureName = string.Empty;

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            if (Point1.HasValue && Point2.HasValue)
            {
                Rectangle selectionRectangle = new(
                    (int)Point1.Value.X * 16,
                    (int)Point1.Value.Y * 16,
                    (int)(Point2.Value.X - Point1.Value.X + 1) * 16,
                    (int)(Point2.Value.Y - Point1.Value.Y + 1) * 16
                );

                spriteBatch.Draw(TextureAssets.MagicPixel.Value, selectionRectangle, Color.White * 0.5f);
            }

            if (awaitingName)
            {
                DrawNameInputUI(spriteBatch);
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (awaitingName)
            {
                HandleTextInput();

                if (Main.keyState.IsKeyDown(Keys.Enter))
                {
                    awaitingName = false;
                    Main.drawingPlayerChat = false;
                    StructureStamper.ExtractStructure(Point1.Value, Point2.Value, structureName);
                    Point1 = null;
                    Point2 = null;
                    structureName = string.Empty;
                }

                PlayerInput.SetZoom_UI();
                Main.blockInput = true;
            }
            else
            {
                Main.blockInput = false;
            }
        }

        public void StartNamingProcess(Vector2 point1, Vector2 point2)
        {
            Point1 = point1;
            Point2 = point2;
            awaitingName = true;
            Main.drawingPlayerChat = true;
        }

        public void SetPoint1(Vector2 point)
        {
            Point1 = point;
            Main.NewText($"Point 1 set at {Point1}.");
        }

        public void SetPoint2(Vector2 point)
        {
            Point2 = point;
            Main.NewText($"Point 2 set at {Point2}.");

            if (Point1.HasValue && Point2.HasValue)
            {
                StartNamingProcess(Point1.Value, Point2.Value);
            }
        }

        public Vector2? GetPoint1()
        {
            return Point1;    
        }

        public Vector2? GetPoint2()
        {
            return Point2;
        }

        private void HandleTextInput()
        {
            if (Main.keyState.IsKeyDown(Keys.Back) && structureName.Length > 0)
            {
                structureName = structureName.Substring(0, structureName.Length - 1);
            }

            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (Main.keyState.IsKeyDown(key) && Main.oldKeyState.IsKeyUp(key))
                {
                    string keyString = key.ToString();

                    if (keyString.Length == 1)
                    {
                        if (Main.keyState.IsKeyDown(Keys.LeftShift) || Main.keyState.IsKeyDown(Keys.RightShift))
                        {
                            structureName += keyString.ToUpper();
                        }
                        else
                        {
                            structureName += keyString.ToLower();
                        }
                    }

                    if (key == Keys.Space)
                    {
                        structureName += " ";
                    }
                }
            }
        }

        private void DrawNameInputUI(SpriteBatch spriteBatch)
        {
            Vector2 uiPosition = new(Main.screenWidth / 2, Main.screenHeight / 2);
            string promptText = "Enter Structure Name:";
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(promptText);
            Vector2 textPosition = uiPosition - textSize / 2;

            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)textPosition.X - 10, (int)textPosition.Y - 10, (int)textSize.X + 20, (int)textSize.Y + 40), Color.Black * 0.8f);
            spriteBatch.DrawString(FontAssets.MouseText.Value, promptText, textPosition, Color.White);

            Vector2 inputPosition = textPosition + new Vector2(0, textSize.Y + 10);
            spriteBatch.DrawString(FontAssets.MouseText.Value, structureName + "|", inputPosition, Color.Yellow);
        }
    }
}