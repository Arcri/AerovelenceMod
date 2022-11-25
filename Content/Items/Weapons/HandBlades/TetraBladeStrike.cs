using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.HandBlades
{
    public class TetraBladeStrike : ModProjectile
    {
        public int timer = 0;
        public Vector2 distFromPlayer = Vector2.Zero;
        public Color strikeCol = Color.Green;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tetra Strike");
            Main.projFrames[Projectile.type] = 5;
        }
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;
            Projectile.damage = 80;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            //projectile.netImportant = true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override void AI()
        {
            if (timer == 0)
            {
                distFromPlayer = Projectile.Center - Main.player[Projectile.owner].Center;
            }

            Projectile.Center = Main.player[Projectile.owner].Center + distFromPlayer;

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 3)
            {
                if (Projectile.frame == 4)
                    Projectile.active = false;
                
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
            }
            timer++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var Tex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/HandBlades/TetraBladeStrike").Value;

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