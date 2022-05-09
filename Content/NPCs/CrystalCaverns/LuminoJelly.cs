using System;
using AerovelenceMod.Common.Globals.Players;
using AerovelenceMod.Content.Biomes;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Projectiles.NPCs.CrystalCaverns;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.CrystalCaverns
{
    public class LuminoJelly : ModNPC
    {
        
            int t;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lumino Jelly");
            Main.npcFrameCount[NPC.type] = 4;
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = 26;
            NPC.lifeMax = 50;
            NPC.damage = 20;
            NPC.defense = 24;
            NPC.aiStyle = 18;
            NPC.knockBackResist = 0f;
            NPC.width = 38;
            NPC.height = 18;
            NPC.value = Item.buyPrice(0, 0, 7, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath44;
        }

        private const int Frame_Jelly0 = 0;
        private const int Frame_Jelly1 = 1;
        private const int Frame_Jelly2 = 2;
        private const int Frame_Jelly3 = 3;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter < 10)
            {
                NPC.frame.Y = Frame_Jelly0 * frameHeight;
            }
            else if (NPC.frameCounter < 20)
            {
                NPC.frame.Y = Frame_Jelly1 * frameHeight;
            }
            else if (NPC.frameCounter < 30)
            {
                NPC.frame.Y = Frame_Jelly2 * frameHeight;
            }
            else if (NPC.frameCounter < 40)
            {
                NPC.frame.Y = Frame_Jelly3 * frameHeight;
            }
            else
            {
                NPC.frameCounter = 0;
            }
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
                int d = 193;
                for (int k = 0; k < 12; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hitDirection, -2.5f, 0, Color.LightBlue, 0.7f);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hitDirection, -2.5f, 0, Color.LightBlue, 0.7f);
                }
                Gore.NewGore(s, NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/SpikeJellyHead").Type, 1f);
                Gore.NewGore(s, NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/SpikeJellyTentacle1").Type, 1f);
                Gore.NewGore(s, NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/SpikeJellyTentacle2").Type, 1f);
            }
            t++;
            Vector2 offset = NPC.Center + new Vector2(Main.rand.NextFloat(-16f, 16f), Main.rand.NextFloat(-16f, 16f));
            if (Main.rand.NextFloat() <= 0.50f)
            {
                Projectile.NewProjectileDirect(NPC.GetSource_OnHit(NPC), offset, new Vector2(Main.rand.NextFloat(-2f, 2f), -5f + Main.rand.NextFloat(-2f, 2f)), ModContent.ProjectileType<LuminoShard>(), 5, 1f, Main.myPlayer);
            }
        }
        public override void AI()
        {
            if (NPC.wet)
            {
                NPC.ai[0]++;
                NPC.velocity.Y = (float)Math.Sin(NPC.ai[0] / 20) * 2;
            }
            else
            {
                NPC.velocity.Y += 0.02f;
            }
            Lighting.AddLight(NPC.Center, 0f, 0f, 0.6f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) =>
            spawnInfo.Player.InModBiome(ModContent.GetInstance<CrystalCavernsBiome>()) && spawnInfo.Water ? 8f : 0f;
    }
}