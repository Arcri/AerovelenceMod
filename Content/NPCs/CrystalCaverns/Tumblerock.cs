using AerovelenceMod.Content.Biomes;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace AerovelenceMod.Content.NPCs.CrystalCaverns
{
    public class TumblerockSmall : ModNPC
    {
        private int frameVariant;
        private bool initializedFrames = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 2;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
            {
                Position = new Vector2(0f, 8f),
                PortraitPositionXOverride = 0f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 26;
            NPC.height = 26;

            NPC.lavaImmune = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;

            NPC.lifeMax = 50;
            NPC.damage = 8;
            NPC.defense = 24;
            NPC.aiStyle = 26;

            NPC.knockBackResist = 1f;

            NPC.value = Item.buyPrice(silver: 4);

            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath44;
            SpawnModBiomes = new int[] { ModContent.GetInstance<CrystalFieldsBiome>().Type, ModContent.GetInstance<CrystalCavernsBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement("These smaller tumblers come in many different shapes and sizes. They are able to grow to massive sizes, and have magnetic properties.")
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome(ModContent.GetInstance<CrystalFieldsBiome>()) || spawnInfo.Player.InModBiome(ModContent.GetInstance<CrystalCavernsBiome>()))
            {
                return SpawnCondition.OverworldNightMonster.Chance;
            }
            return 0f;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemSapphire, NPC.velocity.X, NPC.velocity.Y, 0, Color.White);
            }
        }

        public override void AI()
        {
            if (!initializedFrames)
            {
                frameVariant = Main.rand.Next(2);
                NPC.frame.Y = frameVariant * 26;
                initializedFrames = true;
            }
            NPC.rotation += NPC.velocity.X * 0.05f;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Y = frameVariant * 26;
        }
    }

    public class TumblerockMedium : ModNPC
    {
        private int frameVariant;
        private bool initializedFrames = false;

        public override void SetStaticDefaults() => Main.npcFrameCount[Type] = 2;
        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 32;

            NPC.lavaImmune = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;

            NPC.lifeMax = 75;
            NPC.damage = 10;
            NPC.defense = 24;
            NPC.aiStyle = 26;

            NPC.knockBackResist = 1f;

            NPC.value = Item.buyPrice(silver: 4);

            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath44;
            SpawnModBiomes = new int[] { ModContent.GetInstance<CrystalFieldsBiome>().Type, ModContent.GetInstance<CrystalCavernsBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement("These smaller tumblers come in many different shapes and sizes. They are able to grow to massive sizes, and have magnetic properties.")
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome(ModContent.GetInstance<CrystalFieldsBiome>()) || spawnInfo.Player.InModBiome(ModContent.GetInstance<CrystalCavernsBiome>()))
            {
                return SpawnCondition.OverworldNightMonster.Chance * 0.9f;
            }
            return 0f;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemSapphire, NPC.velocity.X, NPC.velocity.Y, 0, Color.White);
            }
        }

        public override void AI()
        {
            if (!initializedFrames)
            {
                frameVariant = Main.rand.Next(2);
                NPC.frame.Y = frameVariant * 32;
                initializedFrames = true;
            }
            NPC.rotation += NPC.velocity.X * 0.05f;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Y = frameVariant * 32;
        }
    }
}