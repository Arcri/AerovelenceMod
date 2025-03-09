using AerovelenceMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using System.Collections.Generic;
using ReLogic.Content;
using Terraria.Graphics;
using System.Net;
using System;
using Microsoft.Xna.Framework.Input;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class HealerDrone : ModNPC
    {
        private bool hoverOnRight;

        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 30;
            NPC.damage = 0;
            NPC.defense = 10;
            NPC.lifeMax = 500;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
            NPC.netAlways = true;
        }

        public override void AI()
        {
            NPC targetBoss = Main.npc.FirstOrDefault(npc => npc.active && npc.type == ModContent.NPCType<CrystalTumbler>());

            if (targetBoss != null)
            {
                if (NPC.localAI[0] == 0f)
                {
                    hoverOnRight = Main.rand.NextBool();
                    NPC.localAI[0] = 1f;
                }
                float hoverSpeed = 0.05f;
                float hoverAmplitude = 20f;
                Vector2 hoverOffset = new(hoverOnRight ? 80f : -80f, -160f + (float)Math.Sin(Main.GameUpdateCount * hoverSpeed) * hoverAmplitude);

                if (Main.rand.NextBool(200))
                {
                    if (Main.rand.NextBool())
                    {
                        hoverOnRight = !hoverOnRight;
                        NPC.spriteDirection = hoverOnRight ? 1 : -1;
                    }
                    else
                        hoverOffset = new Vector2(0f, -160f);
                }
                Vector2 desiredPosition = targetBoss.Center + hoverOffset;
                float speed = 5f;
                Vector2 move = desiredPosition - NPC.Center;
                float magnitude = move.Length();
                if (magnitude > speed)
                    move *= speed / magnitude;
                NPC.velocity = Vector2.Lerp(NPC.velocity, move, 0.1f);
                NPC.spriteDirection = hoverOnRight ? 1 : -1;
                float swayAmount = 0.1f;
                float swayDirection = hoverOnRight ? 1f : -1f;
                NPC.rotation = swayDirection * (float)Math.Sin(Main.GameUpdateCount * 0.1f) * swayAmount * NPC.velocity.Length() / speed;
            }
        }
    }
}