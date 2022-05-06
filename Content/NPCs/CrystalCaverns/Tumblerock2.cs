using AerovelenceMod.Common.Globals.Players;
using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.CrystalCaverns
{
    public class Tumblerock2 : ModNPC
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Tumblerock");

        public override void SetDefaults()
        {
            NPC.width = 38;
            NPC.height = 18;

            NPC.lavaImmune = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;

            NPC.lifeMax = 50;
            NPC.damage = 20;
            NPC.defense = 24;
            NPC.aiStyle = 26;

            NPC.knockBackResist = 1f;

            NPC.value = Item.buyPrice(silver: 4);

            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath44;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<Sparkle>(), NPC.velocity.X, NPC.velocity.Y, 0, Color.White, 1);

                for (int i = 0; i < 2; i++)
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/TumbleRockV2Gore" + i));
            }
        }

        public override void AI() => NPC.rotation += NPC.velocity.X * 0.05f;

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.GetModPlayer<ZonePlayer>().ZoneCrystalCaverns ? .3f : 0f;
    }
}