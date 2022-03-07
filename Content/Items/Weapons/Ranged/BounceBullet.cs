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
            Main.projFrames[projectile.type] = 8;
            DisplayName.SetDefault("Power Cloud");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 16;
            //Dimensions should always be of the upscaled sprite (or the framesize thereof).
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.timeLeft = 600;
            //Ensure the projectile doesn't take up extra, unnecessary projectile slots in Main.projectile[]. Lasts for 10 sec (plenty of time).
            projectile.penetrate = 2;
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

                if (!npc.friendly && npc.lifeMax > 5 && npc.type != NPCID.TargetDummy && !npc.immortal && npc.immune[projectile.owner] == 0 && npc.whoAmI != target.whoAmI && !npc.townNPC && projectile.DistanceSQ(npc.Center) < 102400f && Collision.CanHitLine(projectile.Center, projectile.width, projectile.height, npc.Center, npc.width, npc.height) && npc.active && npc.life > 0)
                //Living, active, hostile NPCs that aren't critters/townies/target dummies, aren't immortal, haven't just been hit by the player, can be hit (collision line), and are within 20 tiles
                {
                    projectile.velocity = projectile.DirectionTo(npc.Center + (npc.velocity * 1.8f)) * projectile.velocity.Length();
                    projectile.timeLeft = 600;
                    //reset timeLeft
                    return;
                }
            }
            projectile.Kill();
            //Kill the projectile then and there if there are no other NPCs.
        }

        public override void AI()
        {
            base.AI();

            //Animation
            if (++projectile.frameCounter >= 3)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= 3)
                {
                    projectile.frame = 0;
                }
            }
        }
    }
}