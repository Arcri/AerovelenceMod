using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Summoning
{
    public class ShiverMinion : HoverShooter
    {
        public override void SetDefaults()
        {
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;

            projectile.penetrate = -1;
            projectile.minionSlots = 1f;
            projectile.netImportant = projectile.friendly = projectile.minion = projectile.ignoreWater = true;

            projectile.width = 46;
            projectile.height = 30;

            projectile.tileCollide = false;
        }

        public override void CheckActive()
        {
            Player player = Main.player[projectile.owner];
            AeroPlayer modPlayer = player.GetModPlayer<AeroPlayer>();
            if (player.dead)
            {
                modPlayer.ShiverMinion = false;
            }
            if (modPlayer.ShiverMinion)
            {
                projectile.timeLeft = 2;
            }
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            Vector2 idlePos = player.Center;

        }


        public override void SelectFrame()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter >= 8)
            {
                projectile.frameCounter = 0;
                projectile.frame = (projectile.frame + 1) % 3;
            }
        }
    }

    public class ShiverBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.Homing[projectile.type] = true;
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.friendly = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0f)
            {
                Main.PlaySound(SoundID.Item20, projectile.position);
                projectile.localAI[0] = 1f;
            }
            int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 66, 0f, 0f, 100, new Color(0, 255, 0), 1.5f);
            Main.dust[dust].velocity *= 0.1f;
            if (projectile.velocity == Vector2.Zero)
            {
                Main.dust[dust].velocity.Y -= 1f;
                Main.dust[dust].scale = 1.2f;
            }
            else
            {
                Main.dust[dust].velocity += projectile.velocity * 0.2f;
            }
            Main.dust[dust].position.X = projectile.Center.X + 4f + Main.rand.Next(-2, 3);
            Main.dust[dust].position.Y = projectile.Center.Y + Main.rand.Next(-2, 3);
            Main.dust[dust].noGravity = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.penetrate = -1;
            projectile.maxPenetrate = -1;
            projectile.tileCollide = false;
            projectile.position += projectile.velocity;
            projectile.velocity = Vector2.Zero;
            projectile.timeLeft = 180;
            return false;
        }
    }
}