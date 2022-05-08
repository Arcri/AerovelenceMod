using System;
using System.IO;
using AerovelenceMod.Common.Globals.Players;
using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.CrystalCaverns
{
    public class DarkseaAngler : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Darksea Angler");
            Main.npcFrameCount[NPC.type] = 8;
        }
        bool IsElectricityActive = false;

        public override void SetDefaults()
        {
            NPC.aiStyle = 16;
            NPC.lifeMax = 4;
            NPC.damage = 20;
            NPC.defense = 24;
            NPC.knockBackResist = 0f;
            NPC.width = 34;
            NPC.height = 30;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(ai);
            writer.Write(IsElectricityActive);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            ai = reader.ReadSingle();
            IsElectricityActive = reader.ReadBoolean();
        }

        private bool IselectricityActive = false;
        float ai = 0;
        float delayBetween = 0;
        public override bool PreAI()
        {
            NPC.TargetClosest(true);
            int untilImmune = 300;
            int immuneTimeLength = 120;
            IselectricityActive = false;
            if (delayBetween > 0)
                delayBetween--;
            if (ai < 0f)
            {
                if (ai == -immuneTimeLength)
                {
                    for (int i = 0; i < 360; i += 12)
                    {
                        Vector2 circular = new Vector2(96, 0).RotatedBy(MathHelper.ToRadians(i));
                        Dust dust2 = Dust.NewDustDirect(NPC.Center - new Vector2(5) + circular, 0, 0, DustID.Electric, 0, 0, NPC.alpha);
                        dust2.velocity *= 0.15f;
                        dust2.velocity += -circular * 0.08f;
                        dust2.scale = 2.25f;
                        dust2.noGravity = true;
                    }
                }
                if (ai >= -immuneTimeLength + 20)
                {
                    IselectricityActive = true;
                }
                ai += 1f;
                NPC.velocity.X *= 0.9f;
                if (Math.Abs(NPC.velocity.X) < 0.001)
                {
                    NPC.velocity.X = 0.001f * NPC.direction;
                }
                if (Math.Abs(NPC.velocity.Y) > 1f)
                {
                    ai += 10f;
                }
                if (ai >= 0f)
                {
                    NPC.netUpdate = true;
                    NPC.velocity.X += NPC.direction * 0.3f;
                }
                return false;
            }
            if (ai < untilImmune)
            {
                if (NPC.justHit)
                {
                    ai += 15f; //increase immune timer by 15 when hit
                }
                ai += 1f; //increases immune timer rapidly, 60 / second
            }
            else if (Math.Abs(NPC.velocity.Y) <= 0.1f)
            {
                ai = -immuneTimeLength;
                NPC.netUpdate = true;
            }
            return true;
        }







        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.Y == 0f)
            {
                if (NPC.direction == 1)
                {
                    NPC.spriteDirection = 1;
                }
                if (NPC.direction == -1)
                {
                    NPC.spriteDirection = -1;
                }
                if (ai < 0f)
                {
                    NPC.frameCounter += 1.0;
                    if (NPC.frameCounter > 3.0)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0.0;
                    }
                    if (NPC.frame.Y >= Main.npcFrameCount[NPC.type] * frameHeight)
                    {
                        NPC.frame.Y = frameHeight * 5;
                    }
                    else if (NPC.frame.Y < frameHeight * 5)
                    {
                        NPC.frame.Y = frameHeight * 5;
                    }
                }
                else if (NPC.velocity.X == 0f)
                {
                    NPC.frameCounter += 1.0;
                    NPC.frame.Y = 0;
                }
                else
                {
                    NPC.frameCounter += 0.2f + Math.Abs(NPC.velocity.X);
                    if (NPC.frameCounter > 2.0)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0.0;
                    }
                    if (NPC.frame.Y / frameHeight >= Main.npcFrameCount[NPC.type] - 4)
                    {
                        NPC.frame.Y = frameHeight * 2;
                    }
                    else if (NPC.frame.Y / frameHeight < 2)
                    {
                        NPC.frame.Y = frameHeight * 2;
                    }
                }
            }
            else
            {
                NPC.frameCounter = 0.0;
                NPC.frame.Y = frameHeight;
            }
            base.FindFrame(frameHeight);
        }





        public override void HitEffect(int hitDirection, double damage)
        {
            var s = NPC.GetSource_OnHit(NPC);
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<Sparkle>(), NPC.velocity.X, NPC.velocity.Y, 0, Color.White, 1);
                }
                Gore.NewGore(s, NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/DarkseaAnglerHead").Type, 1f);
                Gore.NewGore(s, NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/DarkseaAnglerTail").Type, 1f);
            }
        }


        public override float SpawnChance(NPCSpawnInfo spawnInfo) =>
            spawnInfo.Player.GetModPlayer<ZonePlayer>().ZoneCrystalCaverns && spawnInfo.Water ? 1f : 0f;
    }
}