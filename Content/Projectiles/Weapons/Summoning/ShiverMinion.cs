using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Projectiles.Weapons.Summoning
{
    public class ShiverMinion : HoverShooter
    {
        public override void SetDefaults()
        {
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;

            Projectile.penetrate = -1;
            Projectile.minionSlots = 1f;
            Projectile.netImportant = Projectile.friendly = Projectile.minion = Projectile.ignoreWater = true;

            Projectile.width = 46;
            Projectile.height = 30;

            Projectile.tileCollide = false;
        }

        public override void CheckActive()
        {
            Player player = Main.player[Projectile.owner];
            AeroPlayer modPlayer = player.GetModPlayer<AeroPlayer>();
            if (player.dead)
            {
                modPlayer.ShiverMinion = false;
            }
            if (modPlayer.ShiverMinion)
            {
                Projectile.timeLeft = 2;
            }
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 idlePos = player.Center;

        }


        public override void SelectFrame()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % 3;
            }
        }
    }

    public class ShiverBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
                Projectile.localAI[0] = 1f;
            }
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 66, 0f, 0f, 100, new Color(0, 255, 0), 1.5f);
            Main.dust[dust].velocity *= 0.1f;
            if (Projectile.velocity == Vector2.Zero)
            {
                Main.dust[dust].velocity.Y -= 1f;
                Main.dust[dust].scale = 1.2f;
            }
            else
            {
                Main.dust[dust].velocity += Projectile.velocity * 0.2f;
            }
            Main.dust[dust].position.X = Projectile.Center.X + 4f + Main.rand.Next(-2, 3);
            Main.dust[dust].position.Y = Projectile.Center.Y + Main.rand.Next(-2, 3);
            Main.dust[dust].noGravity = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate = -1;
            Projectile.maxPenetrate = -1;
            Projectile.tileCollide = false;
            Projectile.position += Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = 180;
            return false;
        }
    }
}