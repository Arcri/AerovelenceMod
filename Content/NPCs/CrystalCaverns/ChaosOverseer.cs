using AerovelenceMod.Common.Globals.Players;
using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.CrystalCaverns
{
    public class ChaosOverseer : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chaos Overseer");
            Main.npcFrameCount[npc.type] = 8;
        }
        int i;
        public override void SetDefaults()
        {
            npc.lifeMax = 500;
            npc.damage = 70;
            npc.defense = 24;
            npc.knockBackResist = 0f;
            npc.width = 78;
            npc.height = 106;
            npc.value = Item.buyPrice(0, 3, 0, 0);
            npc.lavaImmune = true;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.NPCDeath44;
        }

        readonly int speed = 4;
        readonly int maxFrames = 7;
        int frame;

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 700;
            npc.damage = 90;
            npc.defense = 30;
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

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = ModContent.GetTexture("AerovelenceMod/Content/NPCs/CrystalCaverns/ChaosOverseer_Glow");
            spriteBatch.Draw(texture, npc.Center - Main.screenPosition, npc.frame, Color.White, npc.rotation, npc.frame.Size() / 2f, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                    Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<Sparkle>(), npc.velocity.X, npc.velocity.Y, 0, Color.White, 1);

                for (int i = 0; i < 3; i++)
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ChaosOverseerGore" + i));
            }
        }

        public override void AI()
        {
            i++;
            Player player = Main.player[npc.target];
            Vector2 distanceNorm = player.position - npc.position;
            distanceNorm.Normalize();

            npc.ai[0]++;
            if (npc.ai[0] % 100 == 0)
            {
                Main.PlaySound(SoundID.Item, (int)npc.Center.X, (int)npc.Center.Y, 94, 0.75f);
                NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<ElectricOrb>(), Main.myPlayer);
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.player.GetModPlayer<ZonePlayer>().ZoneCrystalCaverns && Main.hardMode ? .1f : 0f;
    }
}