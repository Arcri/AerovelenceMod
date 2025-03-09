using AerovelenceMod.Content.Biomes;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace AerovelenceMod.Content.NPCs.CrystalCaverns
{
    public class CaveSlime : ModNPC
    {
        private bool isRolling = false;
        private int rollingTimer = 0;
        private const int ROLLING_DURATION = 4 * 60;
        private const int ROLLING_COOLDOWN = 10 * 60;
        private int rollingCooldown = 0;
        private bool hasPerformedDash = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 3;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
            {
                Position = new Vector2(0f, 8f),
                PortraitPositionXOverride = 0f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = 1;
            NPC.lifeMax = 70;
            NPC.damage = 15;
            NPC.defense = 2;
            NPC.knockBackResist = 0.2f;
            AnimationType = NPCID.BlueSlime;
            NPC.width = 46;
            NPC.height = 44;
            NPC.value = Item.buyPrice(0, 0, 7, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath44;
            SpawnModBiomes = new int[] { ModContent.GetInstance<CrystalCavernsBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement("These slimes seem to have gotten themselves covered up in crystals. They like to roll around like the tumblers.")
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome(ModContent.GetInstance<CrystalCavernsBiome>()))
            {
                return SpawnCondition.OverworldNightMonster.Chance * 0.3f;
            }
            return 0f;
        }

        public override void AI()
        {
            if (rollingCooldown > 0)
                rollingCooldown--;

            NPC.TargetClosest(true);
            Player target = Main.player[NPC.target];
            float distanceToPlayer = Vector2.Distance(NPC.Center, target.Center);
            if (!isRolling && rollingCooldown <= 0 && distanceToPlayer < 250f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                isRolling = true;
                rollingTimer = ROLLING_DURATION;
                hasPerformedDash = false;
                NPC.aiStyle = 26;
                NPC.netUpdate = true;
            }
            if (isRolling)
            {
                NPC.rotation += NPC.velocity.X * 0.05f;
                if (!hasPerformedDash)
                {
                    Vector2 targetDirection = NPC.DirectionTo(target.Center);
                    NPC.velocity = targetDirection * 8f;
                    hasPerformedDash = true;
                    NPC.netUpdate = true;
                }
                rollingTimer--;
                if (rollingTimer <= 0)
                {
                    isRolling = false;
                    NPC.aiStyle = 1;
                    NPC.rotation = 0f;
                    rollingCooldown = ROLLING_COOLDOWN;
                    NPC.netUpdate = true;
                }
            }
            else
            {
                NPC.rotation = 0f;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (isRolling)
                NPC.frame.Y = 2 * frameHeight;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0 || NPC.life >= 0)
            {
                int d = 193;
                for (int k = 0; k < 12; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hit.HitDirection, -2.5f, 0, Color.LightBlue, 0.7f);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hit.HitDirection, -2.5f, 0, Color.LightBlue, 0.7f);
                }
            }
        }
    }
}