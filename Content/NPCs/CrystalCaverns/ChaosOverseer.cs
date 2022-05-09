using AerovelenceMod.Common.Globals.Players;
using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using AerovelenceMod.Content.Biomes;

namespace AerovelenceMod.Content.NPCs.CrystalCaverns
{
    public class ChaosOverseer : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chaos Overseer");
            Main.npcFrameCount[NPC.type] = 8;
        }
        int i;
        public override void SetDefaults()
        {
            NPC.lifeMax = 500;
            NPC.damage = 70;
            NPC.defense = 24;
            NPC.knockBackResist = 0f;
            NPC.width = 78;
            NPC.height = 106;
            NPC.value = Item.buyPrice(0, 3, 0, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath44;
        }

        readonly int speed = 4;
        readonly int maxFrames = 7;
        int frame;

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = 700;
            NPC.damage = 90;
            NPC.defense = 30;
        }
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

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
        Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/CrystalCaverns/ChaosOverseer_Glow");
            Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            var deathSource = NPC.GetSource_Death();
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<Sparkle>(), NPC.velocity.X, NPC.velocity.Y, 0, Color.White, 1);

                for (int i = 0; i < 3; i++)
                    Gore.NewGore(deathSource, NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/ChaosOverseerGore" + i).Type);
            }
        }

        public override void AI()
        {
            i++;
            Player player = Main.player[NPC.target];
            Vector2 distanceNorm = player.position - NPC.position;
            distanceNorm.Normalize();

            NPC.ai[0]++;
            if (NPC.ai[0] % 100 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item, (int)NPC.Center.X, (int)NPC.Center.Y, 94, 0.75f);
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<ElectricOrb>(), Main.myPlayer);
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.InModBiome(ModContent.GetInstance<CrystalCavernsBiome>()) && Main.hardMode ? .1f : 0f;
    }
}