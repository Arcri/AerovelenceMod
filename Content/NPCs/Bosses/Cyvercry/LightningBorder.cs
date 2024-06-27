using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Common.Utilities;
using System;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.GameContent;
using Terraria.Audio;
using AerovelenceMod.Content.Projectiles.Other;
using AerovelenceMod.Common.Globals.SkillStrikes;
using System.Collections.Generic;

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry
{   
    public class LightningBorder : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public int timer = 0;

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.scale = 1f;
            Projectile.timeLeft = 5000;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }

        public override void AI()
        {
            rot1 += 0.018f;
            rot2 -= 0.018f;
            rot3 += 0.04f;
            rot4 -= 0.04f;

            timer++;
            Projectile.velocity = Vector2.Zero;

            if (fade)
            {
                alpha = Math.Clamp(MathHelper.Lerp(alpha, -0.6f, 0.04f), 0, 1);
                scale = MathHelper.Lerp(scale, 5, 0.05f);

                if (alpha == 0)
                    Projectile.active = false;
            }
            else
            {
                scale = Math.Clamp(MathHelper.Lerp(scale, 0, 0.06f), 1, 10);
            }
        }
        float rot1 = 0;
        float rot2 = 0;
        float rot3 = 0;
        float rot4 = 0;

        float scale = 3f;
        float alpha = 1f;
        public bool fade = false;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D barrierTex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/Textures/circle_02");
            Texture2D barrierTex2 = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/Textures/bigCircle3");

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(barrierTex, Projectile.Center - Main.screenPosition, null, Color.HotPink * 0.8f * alpha, rot1, barrierTex.Size() / 2, 3.33f * scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(barrierTex2, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(3, 3) , null, Color.HotPink * 1.25f * alpha, rot1, barrierTex2.Size() / 2, 1f * scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(barrierTex2, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(3, 3), null, Color.HotPink * alpha, rot1 * 2, barrierTex2.Size() / 2, 1.015f * scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(barrierTex2, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(3, 3) , null, Color.HotPink * alpha, rot2, barrierTex2.Size() / 2, 1f * scale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);


            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }
    }


}