using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class CrystalShard : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 20;
            Projectile.timeLeft = 240;
            Projectile.penetrate = 1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.22f, 12f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, new Vector3(0.05f, 0.35f, 0.55f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.Lerp(lightColor, Color.DeepSkyBlue, 0.3f), Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
            TumblerVFX.BeginAdditive(Main.spriteBatch);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.Cyan * 0.24f, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 1.1f, SpriteEffects.None);
            TumblerVFX.EndAdditive(Main.spriteBatch);
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Electrified, 30);
        }
    }
}
