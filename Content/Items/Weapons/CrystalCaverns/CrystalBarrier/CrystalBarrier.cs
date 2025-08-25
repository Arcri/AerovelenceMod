using AerovelenceMod.Content.Buffs.PlayerInflictedDebuffs;
using AerovelenceMod.Content.Items.Weapons.CrystalCaverns.CrystalBarrier;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.CrystalCaverns.CrystalBarrier
{
    internal class CrystalBarrier : ModProjectile
    {

        public override void SetStaticDefaults()
        {

            Main.projFrames[Projectile.type] = 5;
        }
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 60; //it should be a lot taller/bigger than this, i just put together a VERY quick placeholder sprite without considering size
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.sentry = true; //Sets the weapon as a sentry for sentry accessories to properly work.
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.ignoreWater = true; //If this is set to false, the projectile will be slowed in water.
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = false;
        }
        public int CooldownTimer = 0;
        public override void AI()
        {
            //This AI will function as a static sentry, and will not move. If you would like to know how to do more advanced minion AI, check out PurityWisp.cs.

            Main.player[Projectile.owner].UpdateMaxTurrets(); //This makes the sentry be able to spawn more if your sentry cap is greater than one.
            //Animate the projectile.
            Projectile.frameCounter++;
            //10 here changes frame rate (60/10, so 6 fps here), and stops updating after 60
            if (Projectile.frameCounter % 10 == 0 && Projectile.frameCounter < 50)
            {
                Projectile.frame++;
            }
            if (CooldownTimer >= 0)
            {
                CooldownTimer--;
                if (CooldownTimer >= 10 && CooldownTimer <= 25 || CooldownTimer >= 35 && CooldownTimer <= 50 || CooldownTimer >= 60 && CooldownTimer <= 75)
                {
                    Projectile.alpha = 0;
                }
                else if (CooldownTimer >= 0)
                {
                    {
                        Projectile.alpha = 80; //just a very simple transparency pass, but i'm sure there's a better way to make this cooler(think like literally mario 1 damage blinking)
                    }
                }
            }
            }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CooldownTimer = 180; //could be even longer, mostly a default testing count
        }
        public override bool? CanDamage() //make it somehow hit one enemy at a time? gimmicky and not useful mechanic, but possible addition
        {
            if (CooldownTimer >= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}

