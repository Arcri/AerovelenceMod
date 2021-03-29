using AerovelenceMod.Common.Globals.Players;
using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.CrystalCaverns
{
    public class OvergrownTumblerock: ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Overgrown Tumblerock");
        }
        public override void SetDefaults()
        {
            npc.aiStyle = 26;
            npc.lifeMax = 350;
            npc.damage = 27;
            npc.defense = 24;
            npc.knockBackResist = 0.3f;
            npc.width = 58;
            npc.height = 60;
            npc.value = Item.buyPrice(0, 3, 0, 0);
            npc.lavaImmune = true;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.NPCDeath44;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<Sparkle>(), npc.velocity.X, npc.velocity.Y, 0, Color.White, 1);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/OvergrownTumblerockGore1"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/OvergrownTumblerockGore2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/OvergrownTumblerockGore3"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/OvergrownTumblerockGore4"), 1f);
            }
        }
        
        public override void AI()
        {
            npc.rotation += npc.velocity.X * 0.05f;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) =>
            spawnInfo.player.GetModPlayer<ZonePlayer>().ZoneCrystalCaverns ? .1f : 0f;
    }
}