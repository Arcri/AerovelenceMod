using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Common.Globals.SkillStrikes
{
    public class SkillCritImpact : ModProjectile
    {
        public int timer = 0;
        public Vector2 distFromTarget = Vector2.Zero;
        public Color strikeCol = Color.White;
        public bool sticky = false;
        public int stuckNPCIndex = 0;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Skill Strike Impact");
            Main.projFrames[Projectile.type] = 6;
        }
        public override void SetDefaults()
        {
            Projectile.width = 140;
            Projectile.height = 26;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;
            Projectile.damage = 0;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            //projectile.netImportant = true;
            Projectile.scale = 0.75f;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (sticky)
            {
                if (timer == 0)
                {
                    distFromTarget = Projectile.Center - Main.npc[stuckNPCIndex].Center;
                }

                Projectile.Center = Main.npc[stuckNPCIndex].Center + distFromTarget;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 2)
            {
                if (Projectile.frame == 5)
                    Projectile.active = false;
                
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
            }
            timer++;
        }
        public override bool PreDraw(ref Color lightColor)
        {

            var Tex = Mod.Assets.Request<Texture2D>("Common/Globals/SkillStrikes/SkillCritImpact").Value;

            int frameHeight = Tex.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, Tex.Width, frameHeight);


            Vector2 origin = sourceRectangle.Size() / 2f;

            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, sourceRectangle, strikeCol, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

    }
} 