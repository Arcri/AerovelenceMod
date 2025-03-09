using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using System;
using Microsoft.Xna.Framework;
using static AerovelenceMod.Content.Items.BossSummons.LargeGeode;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class GroundSpike : ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            float riseSpeed = 2f;
            float maxHeight = 50f;

            if (Projectile.ai[0] < maxHeight)
            {
                Projectile.position.Y -= riseSpeed;
                Projectile.ai[0] += riseSpeed;
            }
            else
            {
                Projectile.velocity = Vector2.Zero;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            AerovelenceMod.fadeShader.Parameters["fadeStart"].SetValue(0.5f);
            AerovelenceMod.fadeShader.CurrentTechnique.Passes[0].Apply();

            Main.EntitySpriteDraw(
                TextureAssets.Projectile[Projectile.type].Value,
                Projectile.Center - Main.screenPosition,
                null,
                lightColor,
                Projectile.rotation,
                new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width / 2, TextureAssets.Projectile[Projectile.type].Value.Height / 2),
                Projectile.scale,
                SpriteEffects.None,
                0
            );

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}