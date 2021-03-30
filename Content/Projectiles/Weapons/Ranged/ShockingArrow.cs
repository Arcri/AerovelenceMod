using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Ranged
{
    public class ShockingArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shocking Arrow");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 7;
            projectile.height = 7;
            projectile.aiStyle = 1;
            projectile.ignoreWater = false;
            projectile.friendly = true;
            projectile.ranged = true;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int Max = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && Vector2.Distance(projectile.Center, Main.npc[i].Center) < 66f && Max < 5)
                {
                    Main.npc[i].AddBuff(BuffID.Electrified, 155);
                    Max++;
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Lighting.AddLight((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, 0.3f, 0.8f, 1.1f);
            Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
                spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
    }
}