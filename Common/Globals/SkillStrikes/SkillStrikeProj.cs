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

namespace AerovelenceMod.Common.Globals.SkillStrikes
{
	public class SkillStrikeProj : ModProjectile
	{
        int combatTextIndex = 0;
        public string damageNumber = "";
        int timer = 0;

        public float size = 0f;
        public bool crit = false;

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
            Projectile.alpha = 255;
            combatTextIndex = (int)Projectile.ai[1];
            CombatText anchor = Main.combatText[combatTextIndex];

            //Main.NewText("My anchor is: " + combatTextIndex);

            if (!anchor.active)
                Projectile.active = false;

            int stringLength = damageNumber.Length;

            Projectile.scale = anchor.scale; //anchor.scale * 0.7f + (0.15f * damageNumber.Length);
            Projectile.position = anchor.position - new Vector2(10 * anchor.scale,0) + new Vector2(5,0);
            Projectile.rotation = anchor.rotation;
            //crit = anchor.crit;
            
            timer++;

        }

        public override bool PreDraw(ref Color lightColor)
        {

            return false;
        }
        public override void PostDraw(Color lightColor)
        {

            //Utils.DrawBorderString(Main.spriteBatch, damageNumber, Projectile.Center - Main.screenPosition, Color.BlanchedAlmond, Projectile.scale, 0f, 0, 0);
            if (timer > 2)
            {
                Color innerColor;
                Color outerColor;
                if (crit)
                {
                    innerColor = Color.BlanchedAlmond;
                    outerColor = Color.DeepPink;
                    Utils.DrawBorderStringFourWay(Main.spriteBatch, Terraria.GameContent.FontAssets.DeathText.Value, damageNumber, Projectile.Center.X - Main.screenPosition.X, Projectile.Center.Y - Main.screenPosition.Y, innerColor, outerColor, new Vector2(0, 0), Projectile.scale * 0.5f);

                }
                else
                {
                    innerColor = Color.BlanchedAlmond;
                    outerColor = new Color(242, 169, 0);
                    Utils.DrawBorderStringFourWay(Main.spriteBatch, Terraria.GameContent.FontAssets.DeathText.Value, damageNumber, Projectile.Center.X - Main.screenPosition.X, Projectile.Center.Y - Main.screenPosition.Y, innerColor, outerColor, new Vector2(0, 0), Projectile.scale * 0.45f);
                    
                }
            }
            //Utils.DrawBorderStringFourWay(Main.spriteBatch, Terraria.GameContent.FontAssets.DeathText.Value, damageNumber, Projectile.Center.X - Main.screenPosition.X, Projectile.Center.Y - Main.screenPosition.Y, Color.BlanchedAlmond, Color.Purple, new Vector2(0,0), Projectile.scale * 0.4f);

        }
    }
}