using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class TumblerRollDust : ModDust
    {
        public override string Texture => "AerovelenceMod/Assets/Smoke/Smoke1Enhanced";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = true;
            dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            dust.frame = ModContent.Request<Texture2D>(Texture).Value.Bounds;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.velocity *= 0.95f;
            dust.velocity.Y -= 0.012f;
            dust.rotation += dust.velocity.X * 0.014f;
            dust.scale += 0.003f;
            dust.alpha += 7;
            if (dust.alpha >= 255)
                dust.active = false;
            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return TumblerVFX.Glow(dust.color, (1f - dust.alpha / 255f) * 0.7f);
        }

        public override bool PreDraw(Dust dust)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Main.spriteBatch.Draw(texture, dust.position - Main.screenPosition, null, TumblerVFX.Glow(dust.color, (1f - dust.alpha / 255f) * 0.7f), dust.rotation, texture.Size() * 0.5f, dust.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
