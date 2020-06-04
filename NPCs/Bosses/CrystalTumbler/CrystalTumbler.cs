using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.NPCs.Bosses.CrystalTumbler
{
    [AutoloadBossHead]
    public class CrystalTumbler : ModNPC
    {
        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.lifeMax = 4800;
            npc.damage = 32;
            npc.defense = 24;
            npc.knockBackResist = 0f;
            npc.width = 112;
            npc.height = 124;
            npc.value = Item.buyPrice(0, 40, 75, 45);
            npc.npcSlots = 1f;
            npc.boss = true;
            npc.lavaImmune = true;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit2;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.buffImmune[24] = true;
        }
        public override void AI()
        {
            npc.TargetClosest(true);
            var player = Main.player[npc.target];
            if (player.position.X > npc.position.X)
            {
                if (npc.velocity.X < 5)
                {
                    npc.velocity.X += 0.1f;
                }
            }
            if (player.position.X < npc.position.X)
            {
                if (npc.velocity.X > -5)
                {
                    npc.velocity.X -= 0.1f;
                }
            }
            npc.rotation += npc.velocity.X * 0.013f;
        }
    }
}