using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace AerovelenceMod.Content.Dusts
{
    public class StringSnap : ModDust
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void OnSpawn(Dust dust)
        {
            if (Main.rand.NextBool(2))
            {
                dust.noLight = true;
            }
            dust.noGravity = true;
        }

        public override bool PreDraw(Dust dust)
        {
            if (dust.alpha < 255)
            {
                dust.alpha++;
                dust.fadeIn += 0.001f;
            }

            if (dust.noLight)
            {
                dust.rotation += 0.005f;
            }
            else
            {
                dust.rotation -= 0.005f;
            }

            Rectangle source = new Rectangle(0, 0, 2, 30);
            Main.EntitySpriteDraw(TextureAssets.MagicPixel.Value, dust.position - Main.screenPosition, source, Color.Lerp(Color.Lerp(Color.Lerp(Color.Red, Color.White, 0.5f), Color.White, (float)dust.alpha / 15), Color.Transparent, (float)dust.alpha / 30), dust.rotation, source.Size() / 2, 1f, SpriteEffects.None);

            return false;
        }
    }
}
