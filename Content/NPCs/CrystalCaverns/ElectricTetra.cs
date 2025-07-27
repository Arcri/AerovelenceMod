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
    public class ElectricTetra : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            Main.npcCatchable[NPC.type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
            {
                Position = new Vector2(0f, 8f),
                PortraitPositionXOverride = 0f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 18;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 5;
            //NPC.catchItem = (short)ModContent.ItemType<ElectricTetraItem>();
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = .35f;
            NPC.aiStyle = 16;
            NPC.noGravity = true;
            NPC.npcSlots = 0;
            AIType = NPCID.Goldfish;
            NPC.dontCountMe = true;

            SpawnModBiomes = new int[] { ModContent.GetInstance<CrystalCavernsSurfaceBiome>().Type, ModContent.GetInstance<CrystalCavernsBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement("The electric tetra!")
            });
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY), NPC.frame,
                             drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
        public override bool PreAI()
        {
            NPC.spriteDirection = NPC.direction;

            return true;
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            {
                Player target = Main.player[NPC.target];
                int distance = (int)Math.Sqrt((NPC.Center.X - target.Center.X) * (NPC.Center.X - target.Center.X) + (NPC.Center.Y - target.Center.Y) * (NPC.Center.Y - target.Center.Y));
                if (distance < 65 && target.wet && NPC.wet)
                {
                    Vector2 vel = NPC.DirectionFrom(target.Center);
                    vel.Normalize();
                    vel *= 4.5f;
                    NPC.velocity = vel;
                    NPC.rotation = NPC.velocity.X * .06f;
                    if (target.position.X > NPC.position.X)
                    {
                        NPC.spriteDirection = -1;
                        NPC.direction = -1;
                        NPC.netUpdate = true;
                    }
                    else if (target.position.X < NPC.position.X)
                    {
                        NPC.spriteDirection = 1;
                        NPC.direction = 1;
                        NPC.netUpdate = true;
                    }
                }

            }
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if ((spawnInfo.Player.InModBiome(ModContent.GetInstance<CrystalCavernsSurfaceBiome>()) || spawnInfo.Player.InModBiome(ModContent.GetInstance<CrystalCavernsBiome>())) && spawnInfo.Water)
            {
                return SpawnCondition.OverworldWaterCritter.Chance * 1.2f;
            }
            return 0;
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