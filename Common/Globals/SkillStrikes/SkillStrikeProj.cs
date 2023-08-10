 using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using tModPorter;
using static Terraria.ModLoader.ModContent;
using Terraria.GameContent;
using ReLogic.Graphics;

namespace AerovelenceMod.Common.Globals.SkillStrikes
{
	public class SkillStrikeProj : ModProjectile
	{
        int combatTextIndex = 0;
        public string damageNumber = "";
        int timer = 0;

        public float size = 0f;
        public bool skillCrit = false;
        public bool superCrit = false;

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 10;
            Projectile.hostile = false;
            Projectile.timeLeft = 200;
            Projectile.hide = true;
            Projectile.scale = 1.1f;
            Projectile.alpha = 255;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            //if (Projectile.timeLeft < 95)
                //return Color.White;
            //else
                //return Color.White * 0f;
            if (timer > 5)
                return Color.White;
            else 
                return lightColor * 0f; 
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            combatTextIndex = (int)Projectile.ai[1];
            CombatText anchor = Main.combatText[combatTextIndex];

            //Main.NewText("My anchor is: " + combatTextIndex);

            if (!anchor.active)
                Projectile.active = false;

            int stringLength = damageNumber.Length;

            Projectile.scale = anchor.scale; //anchor.scale * 0.7f + (0.15f * damageNumber.Length);
            Projectile.position = anchor.position - new Vector2(10 * anchor.scale,0) + new Vector2(5,0);
            Projectile.rotation = anchor.rotation;
            anchorOpacity = anchor.alpha;

            if (timer > 5)
                opacity = Math.Clamp(opacity - 0.035f, 0, 1);

            if (superCrit && timer > 75)
                mainOpacity = Math.Clamp(mainOpacity - 0.005f, 0, 1);
            else if (!superCrit && timer > 55)
                mainOpacity = Math.Clamp(mainOpacity - 0.035f, 0, 1);

            //crit = anchor.crit;

            timer++;

        }

        float opacity = 1f;
        float anchorOpacity = 1f;
        float mainOpacity = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        public override void PostDraw(Color lightColor)
        {

            //Utils.DrawBorderString(Main.spriteBatch, damageNumber, Projectile.Center - Main.screenPosition, Color.BlanchedAlmond, Projectile.scale, 0f, 0, 0);
            if (timer > 2)
            {
                Color innerColor = Color.BlanchedAlmond;
                Color outerColor;

                if (superCrit)
                    outerColor = Color.DeepPink;
                else if (skillCrit)
                    outerColor = new Color(255, 160, 0);
                else 
                    outerColor = Color.Red;

                DynamicSpriteFont myFont = FontAssets.DeathText.Value;
                Vector2 origin = (myFont.MeasureString(damageNumber) / 2f) * 0.5f;

                Vector2 posOffset = myFont.MeasureString(damageNumber) / 2f;
                Vector2 drawPos = Projectile.Center - Main.screenPosition + posOffset * 0.5f;

                Color col = superCrit ? new Color(255, 90, 170) : Color.Gold * 1f;
                for (int k = 0; k < 4; k++)
                {
                    Vector2 off = Vector2.One.RotatedBy(Main.GameUpdateCount * 0.08f + k / 4f * 6.28f) * 2.9f * Projectile.scale;
                    Main.spriteBatch.DrawString(FontAssets.DeathText.Value, damageNumber, drawPos + off, col with { A = 0 } * 0.8f * opacity, 0, origin, Projectile.scale * 0.47f, 0, 0);
                    if (skillCrit)
                        Main.spriteBatch.DrawString(FontAssets.DeathText.Value, damageNumber, drawPos + off, Color.Black * 0.15f * opacity, 0, origin, Projectile.scale * 0.47f, 0, 0);

                }
                //Main.spriteBatch.DrawString(FontAssets.DeathText.Value, damageNumber, Projectile.Center - Main.screenPosition, Color.BlanchedAlmond, 0, new Vector2(0, 0), Projectile.scale * 0.5f, 0, 0);


                Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.DeathText.Value, damageNumber, drawPos.X, drawPos.Y, innerColor * mainOpacity, outerColor * anchorOpacity * mainOpacity, origin, Projectile.scale * 0.47f);

            }
            //Utils.DrawBorderStringFourWay(Main.spriteBatch, Terraria.GameContent.FontAssets.DeathText.Value, damageNumber, Projectile.Center.X - Main.screenPosition.X, Projectile.Center.Y - Main.screenPosition.Y, Color.BlanchedAlmond, Color.Purple, new Vector2(0,0), Projectile.scale * 0.4f);

        }
    }
}