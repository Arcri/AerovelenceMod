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
            Main.npcFrameCount[npc.type] = 8;
        }
        bool IsElectricityActive = false;

        public override void SetDefaults()
        {
            npc.aiStyle = 16;
            npc.lifeMax = 4;
            npc.damage = 20;
            npc.defense = 24;
            npc.knockBackResist = 0f;
            npc.width = 34;
            npc.height = 30;
            npc.value = Item.buyPrice(0, 0, 1, 0);
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit2;
            npc.DeathSound = SoundID.NPCDeath1;
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
            npc.TargetClosest(true);
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
                        Dust dust2 = Dust.NewDustDirect(npc.Center - new Vector2(5) + circular, 0, 0, DustID.Electric, 0, 0, npc.alpha);
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
                npc.velocity.X *= 0.9f;
                if (Math.Abs(npc.velocity.X) < 0.001)
                {
                    npc.velocity.X = 0.001f * npc.direction;
                }
                if (Math.Abs(npc.velocity.Y) > 1f)
                {
                    ai += 10f;
                }
                if (ai >= 0f)
                {
                    npc.netUpdate = true;
                    npc.velocity.X += npc.direction * 0.3f;
                }
                return false;
            }
            if (ai < untilImmune)
            {
                if (npc.justHit)
                {
                    ai += 15f; //increase immune timer by 15 when hit
                }
                ai += 1f; //increases immune timer rapidly, 60 / second
            }
            else if (Math.Abs(npc.velocity.Y) <= 0.1f)
            {
                ai = -immuneTimeLength;
                npc.netUpdate = true;
            }
            return true;
        }







        public override void FindFrame(int frameHeight)
        {
            if (npc.velocity.Y == 0f)
            {
                if (npc.direction == 1)
                {
                    npc.spriteDirection = 1;
                }
                if (npc.direction == -1)
                {
                    npc.spriteDirection = -1;
                }
                if (ai < 0f)
                {
                    npc.frameCounter += 1.0;
                    if (npc.frameCounter > 3.0)
                    {
                        npc.frame.Y += frameHeight;
                        npc.frameCounter = 0.0;
                    }
                    if (npc.frame.Y >= Main.npcFrameCount[npc.type] * frameHeight)
                    {
                        npc.frame.Y = frameHeight * 5;
                    }
                    else if (npc.frame.Y < frameHeight * 5)
                    {
                        npc.frame.Y = frameHeight * 5;
                    }
                }
                else if (npc.velocity.X == 0f)
                {
                    npc.frameCounter += 1.0;
                    npc.frame.Y = 0;
                }
                else
                {
                    npc.frameCounter += 0.2f + Math.Abs(npc.velocity.X);
                    if (npc.frameCounter > 2.0)
                    {
                        npc.frame.Y += frameHeight;
                        npc.frameCounter = 0.0;
                    }
                    if (npc.frame.Y / frameHeight >= Main.npcFrameCount[npc.type] - 4)
                    {
                        npc.frame.Y = frameHeight * 2;
                    }
                    else if (npc.frame.Y / frameHeight < 2)
                    {
                        npc.frame.Y = frameHeight * 2;
                    }
                }
            }
            else
            {
                npc.frameCounter = 0.0;
                npc.frame.Y = frameHeight;
            }
            base.FindFrame(frameHeight);
        }





        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<Sparkle>(), npc.velocity.X, npc.velocity.Y, 0, Color.White, 1);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/DarkseaAnglerHead"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/DarkseaAnglerTail"), 1f);
            }
        }


        public override float SpawnChance(NPCSpawnInfo spawnInfo) =>
            spawnInfo.player.GetModPlayer<ZonePlayer>().zoneCrystalCaverns && spawnInfo.water ? 1f : 0f;
    }
}