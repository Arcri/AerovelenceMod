using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class BounceBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
            DisplayName.SetDefault("Power Cloud");
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 16;
            //Dimensions should always be of the upscaled sprite (or the framesize thereof).
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 600;
            //Ensure the projectile doesn't take up extra, unnecessary projectile slots in Main.projectile[]. Lasts for 10 sec (plenty of time).
            Projectile.penetrate = 2;
            //Number of bounces, easily modified.
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.townNPC)
            {
                return;
            }

            //Removed previous "bounce" code (which was just spawning another projectile).
            //It could have worked, but was not worth the extra effort.

            for (int i = 0; i < Main.maxNPCs; i++)
            //Since this only runs once every time one bounces and has bounces left, looping through npc array isn't awful.
            {
                NPC npc = Main.npc[i];

                if (!npc.friendly && npc.lifeMax > 5 && npc.type != NPCID.TargetDummy && !npc.immortal && npc.immune[Projectile.owner] == 0 && npc.whoAmI != target.whoAmI && !npc.townNPC && Projectile.DistanceSQ(npc.Center) < 102400f && Collision.CanHitLine(Projectile.Center, Projectile.width, Projectile.height, npc.Center, npc.width, npc.height) && npc.active && npc.life > 0)
                //Living, active, hostile NPCs that aren't critters/townies/target dummies, aren't immortal, haven't just been hit by the player, can be hit (collision line), and are within 20 tiles
                {
                    Projectile.velocity = Projectile.DirectionTo(npc.Center + (npc.velocity * 1.8f)) * Projectile.velocity.Length();
                    Projectile.timeLeft = 600;
                    //reset timeLeft
                    return;
                }
            }
            Projectile.Kill();
            //Kill the projectile then and there if there are no other NPCs.
        }

        public override void AI()
        {
            base.AI();

            //Animation
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                {
                    Projectile.frame = 0;
                }
            }
        }
    }
}