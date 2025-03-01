using AerovelenceMod.Common.Globals.Players;
using AerovelenceMod.Content.Biomes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace AerovelenceMod.Content.NPCs.CrystalCaverns
{
    public class CrystalBat : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
            {
                Position = new Vector2(0f, 8f),
                PortraitPositionXOverride = 0f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = NPCID.CaveBat;
            NPC.height = NPCID.CaveBat;
            NPC.damage = 15;
            NPC.defense = 10;
            NPC.lifeMax = 50;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = .35f;
            NPC.aiStyle = 14;
            NPC.noGravity = true;
            NPC.npcSlots = 0;
            AIType = NPCID.CaveBat;
            AnimationType = NPCID.CaveBat;
            SpawnModBiomes = new int[] { ModContent.GetInstance<CrystalCavernsBiome>().Type };
        }


        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement("Unlike regular bats, the Crystal Bats are actually able to see just fine during the day. They are entirely night-blind, though.")
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome(ModContent.GetInstance<CrystalCavernsBiome>()))
            {
                return SpawnCondition.Cavern.Chance;
            }
            return 0f;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (NPC.width / 2.0f);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2.0f);
                NPC.width = 30;
                NPC.height = 30;
                NPC.position.X = NPC.position.X - (NPC.width / 2.0f);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2.0f);
                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Electric, 0f, 0f, 100, new Color(112, 244, 250), 2f);
                    Main.dust[dust].velocity *= 1f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[dust].scale = 0.3f;
                        Main.dust[dust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
            }
        }
    }
}