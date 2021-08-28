using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.Rimegeist //Change me
{
    public class Arbiter : ModNPC
    {
        public float currentSpeed = 3;
        int t = 0;
        float rotationValue = 1;

        int speed = 3;
        int maxFrames = 4;
        int frame;
        int i;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Circler"); //DONT Change me
            Main.npcFrameCount[npc.type] = 6;
        }
        public override void SetDefaults()
        {
            npc.lifeMax = 130000;        //this is the npc health
            rotationValue = Main.expertMode ? 22 * 16 : 25 * 16; //22 blocks in expert mode, 25 in normal
            npc.damage = 1;    //this is the npc damage
            npc.defense = 0;         //this is the npc defense
            npc.knockBackResist = 0f;
            npc.width = 44; //this is where you put the npc sprite width.     important
            npc.height = 56; //this is where you put the npc sprite height.   important
            npc.lavaImmune = true;       //this make the npc immune to lava
            npc.noGravity = true;           //this make the npc float
            npc.noTileCollide = true;        //this make the npc go tru walls
            Main.npcFrameCount[npc.type] = 4;
            npc.value = Item.buyPrice(0, 0, 0, 0);
            npc.npcSlots = 1f;
            npc.aiStyle = 1;
            aiType = -1;
            animationType = 10;
            npc.HitSound = SoundID.NPCHit4; //Change me if you want (Rock hit sound)
            npc.DeathSound = SoundID.NPCDeath1; //Change me if you want (Heavy grunt sound)
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 220000; //Change me
        }
        public override bool CheckActive()
        {
            return false;
        }
        public override void AI()
        {
            npc.TargetClosest(true);
            Player player = Main.player[npc.target];
            npc.ai[1] += 3.14159265f / 50f;
            npc.ai[2]++;
            var vec = player.position - npc.position;
            npc.rotation = (float)Math.Atan2(vec.Y, vec.X);
            npc.position = player.position + npc.ai[1].ToRotationVector2() * rotationValue;

            if (!NPC.AnyNPCs(base.mod.NPCType("Rimegeist")))
                rotationValue += 5;
            else if (npc.ai[2] % 8 == 0)
                rotationValue--;
            if (npc.ai[2] % 2 == 0)
                t++;
            if (t > 3)
                t = 0;
            if (rotationValue >= 75 * 16)
                npc.active = false;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter >= speed)
            {
                frame++;
                npc.frameCounter = 0;
            }

            if (frame > maxFrames)
                frame = 0;

            npc.frame.Y = frame * frameHeight;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;       //this make that the npc does not have a health bar
        }
        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.KillMe(PlayerDeathReason.ByCustomReason(player.name + "'s soul was evaporated."), 10000, 0);
        }
    }
}
