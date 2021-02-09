using AerovelenceMod.Dusts;
using AerovelenceMod.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.NPCs.CrystalCaverns
{
    public class LuminoJelly : ModNPC
    {
        
            int t;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lumino Jelly");
            Main.npcFrameCount[npc.type] = 4;
        }
        public override void SetDefaults()
        {
            npc.aiStyle = 26;
            npc.lifeMax = 50;
            npc.damage = 20;
            npc.defense = 24;
            npc.aiStyle = 18;
            npc.knockBackResist = 0f;
            npc.width = 38;
            npc.height = 18;
            npc.value = Item.buyPrice(0, 0, 7, 0);
            npc.lavaImmune = true;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.NPCDeath44;
        }

        private const int Frame_Jelly0 = 0;
        private const int Frame_Jelly1 = 1;
        private const int Frame_Jelly2 = 2;
        private const int Frame_Jelly3 = 3;
        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter < 10)
            {
                npc.frame.Y = Frame_Jelly0 * frameHeight;
            }
            else if (npc.frameCounter < 20)
            {
                npc.frame.Y = Frame_Jelly1 * frameHeight;
            }
            else if (npc.frameCounter < 30)
            {
                npc.frame.Y = Frame_Jelly2 * frameHeight;
            }
            else if (npc.frameCounter < 40)
            {
                npc.frame.Y = Frame_Jelly3 * frameHeight;
            }
            else
            {
                npc.frameCounter = 0;
            }
        }
        public override void HitEffect(int hitDirection, double damage)
        {

            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<Sparkle>(), npc.velocity.X, npc.velocity.Y, 0, Color.White, 1);
                }
                int d = 193;
                for (int k = 0; k < 12; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, d, 2.5f * hitDirection, -2.5f, 0, Color.LightBlue, 0.7f);
                    Dust.NewDust(npc.position, npc.width, npc.height, d, 2.5f * hitDirection, -2.5f, 0, Color.LightBlue, 0.7f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/SpikeJellyHead"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/SpikeJellyTentacle1"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/SpikeJellyTentacle2"), 1f);
            }
            t++;
            Vector2 offset = npc.Center + new Vector2(Main.rand.NextFloat(-16f, 16f), Main.rand.NextFloat(-16f, 16f));
            if (Main.rand.NextFloat() <= 0.50f)
            {
                Projectile.NewProjectileDirect(offset, new Vector2(Main.rand.NextFloat(-2f, 2f), -5f + Main.rand.NextFloat(-2f, 2f)), ModContent.ProjectileType<LuminoShard>(), 5, 1f, Main.myPlayer);
            }
        }
        public override void AI()
        {
            if (npc.wet)
            {
                npc.ai[0]++;
                npc.velocity.Y = (float)Math.Sin(npc.ai[0] / 20) * 2;
            }
            else
            {
                npc.velocity.Y += 0.02f;
            }
            Lighting.AddLight(npc.Center, 0f, 0f, 0.6f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.player.GetModPlayer<AeroPlayer>().ZoneCrystalCaverns && spawnInfo.water ? 8f : 0f;
        }
    }
}