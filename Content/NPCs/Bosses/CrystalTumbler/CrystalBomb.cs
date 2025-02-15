using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using System;
using Microsoft.Xna.Framework;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class CrystalBomb : ModNPC
    {
        private float tumbleTimer = 0f;
        private float maxRotationAngle = 15f * (float)Math.PI / 180f;
        private float swayAmplitude = 16f;

        private float initialFallSpeed;
        private bool isFalling = false;

        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 24;
            NPC.damage = 0;
            NPC.defense = 10;
            NPC.lifeMax = 50;
            NPC.knockBackResist = 0.5f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
        }


        private Vector2 initialPosition;
        private bool reachedTargetHeight = false;

        private int timer;

        public override void AI()
        {
            timer++; 
            Lighting.AddLight(NPC.Center, Color.DeepSkyBlue.ToVector3() * 0.5f);
            NPC tumbler = Main.npc[(int)NPC.ai[0]];

            if (!reachedTargetHeight)
            {
                float targetHeight = NPC.ai[1];
                if (Math.Abs(NPC.position.Y - targetHeight) > 1f)
                {
                    NPC.velocity.Y = (targetHeight - NPC.position.Y) * 0.1f;
                }
                else
                {
                    NPC.position.Y = targetHeight;
                    NPC.velocity.Y = 0f;
                    reachedTargetHeight = true;
                    initialPosition = NPC.position;
                    isFalling = true;
                    initialFallSpeed = 0.03f;
                }
            }
            if (reachedTargetHeight && !isFalling)
            {
                isFalling = true;
                initialFallSpeed = 0.03f;
                NPC.velocity.Y = initialFallSpeed;
            }
            if (!reachedTargetHeight && NPC.position.Y <= NPC.ai[1])
            {
                reachedTargetHeight = true;
                initialPosition = NPC.position;
                NPC.velocity.Y = 0.03f;
            }
            if (reachedTargetHeight && isFalling)
            {
                float targetX = NPC.ai[2];
                if ((NPC.velocity.X > 0 && NPC.position.X > targetX) || (NPC.velocity.X < 0 && NPC.position.X < targetX))
                {
                    NPC.velocity.X *= 0.95f;
                }
                tumbleTimer += 0.05f;
                NPC.position.X = initialPosition.X + (float)Math.Sin(tumbleTimer) * swayAmplitude;
                if (Math.Abs(NPC.velocity.X) < 0.1f)
                {
                    NPC.velocity.X = 0.1f * (float)Math.Sign(Math.Sin(tumbleTimer));
                }
                NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y + 0.07f, -float.MaxValue, 1.5f);
                NPC.rotation = (float)Math.Sin(tumbleTimer) * maxRotationAngle;
            }
            else
            {
                NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, -5f, 0.1f);
            }
            if (NPC.position.Y > tumbler.Center.Y)
            {
                TriggerPenalty();
            }
            if (NPC.collideY)
            {
                TriggerPenalty();
            }
        }

        private void TriggerPenalty()
        {
            NPC tumbler = Main.npc[(int)NPC.ai[0]];
            if (tumbler.ModNPC is CrystalTumbler crystalTumbler)
            {
                crystalTumbler.StartLaserCountdown();
            }
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC otherBomb = Main.npc[i];
                if (otherBomb.active && otherBomb.type == ModContent.NPCType<CrystalBomb>())
                {
                    NPC.HitInfo hitInfo = new NPC.HitInfo
                    {
                        Damage = otherBomb.lifeMax,
                        Knockback = 0f,
                        HitDirection = 0,
                        Crit = false
                    };
                    otherBomb.StrikeNPC(hitInfo);
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (hit.Knockback > 0)
            {
                NPC.velocity.Y = Math.Min(NPC.velocity.Y - hit.Knockback, -4f);
                NPC.velocity.X *= 0.5f;
            }
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.BlueCrystalShard, NPC.velocity.X * 2f, -2f);
                }
            }
            else
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.BlueCrystalShard, NPC.velocity.X * 0.5f, -0.5f);
            }
        }

        public override bool CheckDead()
        {
            NPC.life = 0;
            NPC.HitEffect(0, 10.0);
            NPC.active = false;
            return false;
        }
    }
}