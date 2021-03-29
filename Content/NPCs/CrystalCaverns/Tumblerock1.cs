using AerovelenceMod.Common.Globals.Players;
using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.CrystalCaverns
{
    public class Tumblerock1 : ModNPC
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Tumblerock");

        public override void SetDefaults()
        {
            npc.width = 38;
            npc.height = 18;

            npc.lavaImmune = true;
            npc.noGravity = false;
            npc.noTileCollide = false;

            npc.lifeMax = 50;
            npc.damage = 20;
            npc.defense = 24;
            npc.aiStyle = 26;

            npc.knockBackResist = 1f;

            npc.value = Item.buyPrice(silver: 4);

            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.NPCDeath44;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                    Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<Sparkle>(), npc.velocity.X, npc.velocity.Y, 0, Color.White);

                for (int i = 0; i < 2; i++)
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/TumbleRockV1Gore" + i));
            }
        }
        
        public override void AI() => npc.rotation += npc.velocity.X * 0.05f;
        
        public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.player.GetModPlayer<ZonePlayer>().ZoneCrystalCaverns ? .3f : 0f;
    }
}