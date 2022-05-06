using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.General
{
    public class FrostEye : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Eye");
            Main.npcFrameCount[NPC.type] = 4;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 70;
            NPC.aiStyle = 5;
            NPC.damage = 15;
            NPC.defense = 24;
            NPC.knockBackResist = 0f;
            NPC.width = 70;
            NPC.height = 42;
            NPC.value = Item.buyPrice(0, 0, 7, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath44;
        }
        int speed = 3;
        int maxFrames = 3;
        int frame;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= speed)
            {
                frame++;
                NPC.frameCounter = 0;
            }

            if (frame > maxFrames)
                frame = 0;

            NPC.frame.Y = frame * frameHeight;
        }
		public override void AI()
		{
            Player player = Main.player[NPC.target];
            NPC.rotation = (NPC.Center - player.Center).ToRotation();
            if (!NPC.noTileCollide)
            {
                if (NPC.collideX)
                {
                    NPC.velocity.X = NPC.oldVelocity.X * -0.5f;
                    if (NPC.direction == -1 && NPC.velocity.X > 0f && NPC.velocity.X < 2f)
                    {
                        NPC.velocity.X = 2f;
                    }
                    if (NPC.direction == 1 && NPC.velocity.X < 0f && NPC.velocity.X > -2f)
                    {
                        NPC.velocity.X = -2f;
                    }
                }
                if (NPC.collideY)
                {
                    NPC.velocity.Y = NPC.oldVelocity.Y * -0.5f;
                    if (NPC.velocity.Y > 0f && NPC.velocity.Y < 1f)
                    {
                        NPC.velocity.Y = 1f;
                    }
                    if (NPC.velocity.Y < 0f && NPC.velocity.Y > -1f)
                    {
                        NPC.velocity.Y = -1f;
                    }
                }
            }
            if (Main.dayTime && NPC.position.Y <= Main.worldSurface * 16.0)
            {
                if (NPC.timeLeft > 10)
                {
                    NPC.timeLeft = 10;
                }
                NPC.directionY = -1;
                NPC.velocity.Y += -0.5f;
            }
            int num4 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y + NPC.height * 0.25f), NPC.width, (int)(NPC.height * 0.5f), 20, NPC.velocity.X, 2f);
			Main.dust[num4].velocity.X *= 0.5f;
			Main.dust[num4].velocity.Y *= 0.1f;
		}
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.ZoneSnow && !Main.dayTime ? .1f : 0f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0 || NPC.life >= 0)
            {
                int d = 193;
                for (int k = 0; k < 12; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hitDirection, -2.5f, 0, Color.LightBlue, 0.7f);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hitDirection, -2.5f, 0, Color.LightBlue, 0.7f);
                }
            }
        }
    }
}