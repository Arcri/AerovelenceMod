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
            NPC.aiStyle = 26;
            NPC.lifeMax = 350;
            NPC.damage = 27;
            NPC.defense = 24;
            NPC.knockBackResist = 0.3f;
            NPC.width = 58;
            NPC.height = 60;
            NPC.value = Item.buyPrice(0, 3, 0, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath44;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<Sparkle>(), NPC.velocity.X, NPC.velocity.Y, 0, Color.White, 1);
                }
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/OvergrownTumblerockGore1"), 1f);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/OvergrownTumblerockGore2"), 1f);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/OvergrownTumblerockGore3"), 1f);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/OvergrownTumblerockGore4"), 1f);
            }
        }
        
        public override void AI()
        {
            NPC.rotation += NPC.velocity.X * 0.05f;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) =>
            spawnInfo.Player.GetModPlayer<ZonePlayer>().ZoneCrystalCaverns ? .1f : 0f;
    }
}