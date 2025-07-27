using AerovelenceMod.Common.Globals.NPCs;
using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Content.Biomes;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace AerovelenceMod.Content.NPCs.CrystalCaverns
{
    public class TumblerockSmall : TranslatableModNPC
    {
        private int frameVariant;
        private bool initializedFrames = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 2;
            this.ModifyLocalization("Tumblerock", "These smaller tumblers come in many different shapes and sizes. They can grow to enormous sizes, and exhibit magnetic properties.")
                .AddName(Language.Spanish, "Roca Rodante")
                .AddFlavor(Language.Spanish, "Estas rocas rodantes más pequeñas vienen en muchas formas y tamaños. Pueden crecer a tamaños enormes y tienen propiedades magnéticas.")
                .AddName(Language.French, "Roche Roulante")
                .AddFlavor(Language.French, "Ces petites roches roulantes se déclinent sous de nombreuses formes et tailles. Elles peuvent atteindre des tailles énormes et possèdent des propriétés magnétiques.")
                .AddName(Language.German, "Rollstein")
                .AddFlavor(Language.German, "Diese kleineren rollenden Felsen treten in vielen verschiedenen Formen und Größen auf. Sie können zu enormer Größe heranwachsen und besitzen magnetische Eigenschaften.")
                .AddName(Language.Russian, "Камневалун")
                .AddFlavor(Language.Russian, "Эти небольшие катящиеся камни бывают самых разных форм и размеров. Они могут вырастать до огромных размеров и обладают магнитными свойствами.")
                .AddName(Language.Italian, "Rocciarotola")
                .AddFlavor(Language.Italian, "Queste piccole rocce rotolanti esistono in varie forme e dimensioni. Possono crescere fino a dimensioni enormi e possiedono proprietà magnetiche.");
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 26;
            NPC.height = 26;
            NPC.lavaImmune = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.lifeMax = 50;
            NPC.damage = 8;
            NPC.defense = 24;
            NPC.aiStyle = 26;
            NPC.knockBackResist = 1f;
            NPC.value = Item.buyPrice(silver: 4);
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath44;
            SpawnModBiomes = [ModContent.GetInstance<CrystalCavernsSurfaceBiome>().Type, ModContent.GetInstance<CrystalCavernsBiome>().Type];
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome(ModContent.GetInstance<CrystalCavernsSurfaceBiome>()) || spawnInfo.Player.InModBiome(ModContent.GetInstance<CrystalCavernsBiome>()))
                return SpawnCondition.OverworldDaySlime.Chance * 0.5f + SpawnCondition.OverworldNightMonster.Chance;
            return 0f;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemSapphire,
                        NPC.velocity.X, NPC.velocity.Y, 0, Color.White);
            }
        }

        public override void AI()
        {
            if (!initializedFrames)
            {
                frameVariant = Main.rand.Next(2);
                NPC.frame.Y = frameVariant * 26;
                initializedFrames = true;
            }
            NPC.rotation += NPC.velocity.X * 0.05f;
        }

        public override void FindFrame(int frameHeight) { NPC.frame.Y = frameVariant * 26; }
    }

    public class TumblerockMedium : TranslatableModNPC
    {
        private int frameVariant;
        private bool initializedFrames = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 2;
            this.ModifyLocalization("Tumblerock", "These smaller tumblers come in many different shapes and sizes. They can grow to enormous sizes, and exhibit magnetic properties.")
                .AddName(Language.Spanish, "Roca Rodante")
                .AddFlavor(Language.Spanish, "Estas rocas rodantes más pequeñas vienen en muchas formas y tamaños. Pueden crecer a tamaños enormes y tienen propiedades magnéticas.")
                .AddName(Language.French, "Roche Roulante")
                .AddFlavor(Language.French, "Ces petites roches roulantes se déclinent sous de nombreuses formes et tailles. Elles peuvent atteindre des tailles énormes et possèdent des propriétés magnétiques.")
                .AddName(Language.German, "Rollstein")
                .AddFlavor(Language.German, "Diese kleineren rollenden Felsen treten in vielen verschiedenen Formen und Größen auf. Sie können zu enormer Größe heranwachsen und besitzen magnetische Eigenschaften.")
                .AddName(Language.Russian, "Камневалун")
                .AddFlavor(Language.Russian, "Эти небольшие катящиеся камни бывают самых разных форм и размеров. Они могут вырастать до огромных размеров и обладают магнитными свойствами.")
                .AddName(Language.Italian, "Rocciarotola")
                .AddFlavor(Language.Italian, "Queste piccole rocce rotolanti esistono in varie forme e dimensioni. Possono crescere fino a dimensioni enormi e possiedono proprietà magnetiche.");
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 32;
            NPC.height = 32;
            NPC.lavaImmune = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.lifeMax = 75;
            NPC.damage = 10;
            NPC.defense = 24;
            NPC.aiStyle = 26;
            NPC.knockBackResist = 1f;
            NPC.value = Item.buyPrice(silver: 4); NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath44;
            SpawnModBiomes = [ModContent.GetInstance<CrystalCavernsSurfaceBiome>().Type, ModContent.GetInstance<CrystalCavernsBiome>().Type];
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome(ModContent.GetInstance<CrystalCavernsSurfaceBiome>()) || spawnInfo.Player.InModBiome(ModContent.GetInstance<CrystalCavernsBiome>()))
                return SpawnCondition.OverworldDaySlime.Chance * 0.5f + SpawnCondition.OverworldNightMonster.Chance;
            return 0f;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemSapphire, NPC.velocity.X, NPC.velocity.Y, 0, Color.White);
            }
        }

        public override void AI()
        {
            if (!initializedFrames)
            {
                frameVariant = Main.rand.Next(2);
                NPC.frame.Y = frameVariant * 32;
                initializedFrames = true;
            }
            NPC.rotation += NPC.velocity.X * 0.05f;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Y = frameVariant * 32;
        }
    }
}