
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Melee
{
    public class BruhBladeProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.extraUpdates = 0;
            projectile.width = 64;
            projectile.height = 64;
            projectile.aiStyle = 99;
            projectile.friendly = true;
            projectile.penetrate = -1;
            ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = 30;
            ProjectileID.Sets.YoyosMaximumRange[projectile.type] = 10000f;
            ProjectileID.Sets.YoyosTopSpeed[projectile.type] = 300f;
        }
        public override void AI()
        {
            if (Main.rand.Next(2) == 0)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 58, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.life = 0;
            Rectangle r = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);
            CombatText.NewText(r, new Color(89, 32, 255), $"Ech");
        }

    }
}