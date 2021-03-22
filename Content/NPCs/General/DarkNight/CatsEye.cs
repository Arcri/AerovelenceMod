using AerovelenceMod.Content.Events.DarkNight;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.General.DarkNight
{
    public class CatsEye : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cat's Eye");
            Main.npcFrameCount[npc.type] = 4;
        }
        public override void SetDefaults()
        {
            npc.lifeMax = 70;
            npc.aiStyle = 5;
            npc.damage = 15;
            npc.defense = 24;
            npc.knockBackResist = 0f;
            npc.width = 44;
            npc.height = 24;
            npc.value = Item.buyPrice(0, 0, 7, 0);
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath44;
        }
        int speed = 3;
        int maxFrames = 3;
        int frame;
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
        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = ModContent.GetTexture("AerovelenceMod/Content/NPCs/General/DarkNight/CatsEye_Glow");
            spriteBatch.Draw(texture, npc.Center - Main.screenPosition, npc.frame, Color.White, npc.rotation, npc.frame.Size() / 2f, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
        }
        public override void AI()
		{
            Player player = Main.player[npc.target];
            npc.rotation = (npc.Center - player.Center).ToRotation();
            if (!npc.noTileCollide)
            {
                if (npc.collideX)
                {
                    npc.velocity.X = npc.oldVelocity.X * -0.5f;
                    if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                    {
                        npc.velocity.X = 2f;
                    }
                    if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                    {
                        npc.velocity.X = -2f;
                    }
                }
                if (npc.collideY)
                {
                    npc.velocity.Y = npc.oldVelocity.Y * -0.5f;
                    if (npc.velocity.Y > 0f && npc.velocity.Y < 1f)
                    {
                        npc.velocity.Y = 1f;
                    }
                    if (npc.velocity.Y < 0f && npc.velocity.Y > -1f)
                    {
                        npc.velocity.Y = -1f;
                    }
                }
            }
            if (Main.dayTime && npc.position.Y <= Main.worldSurface * 16.0)
            {
                if (npc.timeLeft > 10)
                {
                    npc.timeLeft = 10;
                }
                npc.directionY = -1;
                npc.velocity.Y += -0.5f;
            }
            int num4 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + npc.height * 0.25f), npc.width, (int)(npc.height * 0.5f), 20, npc.velocity.X, 2f);
			Main.dust[num4].velocity.X *= 0.5f;
			Main.dust[num4].velocity.Y *= 0.1f;
		}
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return DarkNightWorld.DarkNight && spawnInfo.player.ZoneOverworldHeight ? 1.5f : 0f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0 || npc.life >= 0)
            {
                int d = 193;
                for (int k = 0; k < 12; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, d, 2.5f * hitDirection, -2.5f, 0, Color.LightBlue, 0.7f);
                    Dust.NewDust(npc.position, npc.width, npc.height, d, 2.5f * hitDirection, -2.5f, 0, Color.LightBlue, 0.7f);
                }
            }
        }
    }
}