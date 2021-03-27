using AerovelenceMod.Content.Projectiles.NPCs.Bosses.Decurion;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.Decurion
{
    public class DecurionBodyGun1 : ModNPC
    {
        int i;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            npc.lifeMax = 1000;
            npc.damage = 120;
            npc.defense = 23;
            npc.knockBackResist = 0f;
            npc.width = 16;
            npc.height = 14;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.behindTiles = true;
            Main.npcFrameCount[npc.type] = 1;
            npc.value = Item.buyPrice(0, 0, 50, 0);
            npc.npcSlots = 1f;
            npc.netAlways = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath44;
        }
        public override void AI()
        {
            i++;
            Player player = Main.player[npc.target];
            Vector2 distanceNorm = player.position - npc.position;
            distanceNorm.Normalize();
            if (i % Main.rand.Next(100, 250) == 0)
            {
                Main.PlaySound(SoundID.Item, (int)npc.Center.X, (int)npc.Center.Y, 94, 0.75f);
                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, distanceNorm.X * 10, distanceNorm.Y * 10, ModContent.ProjectileType<DecurionGunBullet>(), 30, 0f, Main.myPlayer, 0f, 0f);
            }

            NPC parent = Main.npc[(int)npc.ai[0]];

            if (!parent.active)
            {
                npc.active = false;
            }
            Vector2 offset = new Vector2(-24, -20);
            npc.Center = parent.Center + offset.RotatedBy(parent.rotation);
            npc.rotation = parent.rotation;

        }
    }
}