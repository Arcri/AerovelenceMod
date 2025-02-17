using AerovelenceMod.Common.Systems;
using AerovelenceMod.Content.Biomes;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.EmoteBubbles;
using AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns;
using AerovelenceMod.Content.Items.Weapons.Misc.Ranged;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Personalities;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.NPCs.Bosses.Cyvercry;

namespace AerovelenceMod.Content.NPCs.TownNPC.RockCollector
{
    [AutoloadHead]
    public class RockCollector : ModNPC
    {
        public const string ShopName = "Shop";
        public int NumberOfTimesTalkedTo = 0;

        private static int ShimmerHeadIndex;
        private static Profiles.StackedNPCProfile NPCProfile;

        public override void Load()
        {
            ShimmerHeadIndex = Mod.AddNPCHeadTexture(Type, Texture + "_Shimmer_Head");
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 25;

            NPCID.Sets.ExtraFramesCount[Type] = 9;
            NPCID.Sets.AttackFrameCount[Type] = 4;
            NPCID.Sets.DangerDetectRange[Type] = 700;
            NPCID.Sets.AttackType[Type] = 0;
            NPCID.Sets.AttackTime[Type] = 90;
            NPCID.Sets.AttackAverageChance[Type] = 30;
            NPCID.Sets.HatOffsetY[Type] = 4;
            NPCID.Sets.ShimmerTownTransform[NPC.type] = true;
            NPCID.Sets.ShimmerTownTransform[Type] = true;
            NPCID.Sets.FaceEmote[Type] = ModContent.EmoteBubbleType<RockCollectorEmote>();

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new()
            {
                Velocity = 1f,
                Direction = 1
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
            NPC.Happiness
                .SetBiomeAffection<SnowBiome>(AffectionLevel.Like)
                .SetBiomeAffection<ForestBiome>(AffectionLevel.Dislike)
                .SetBiomeAffection<CrystalCavernsBiome>(AffectionLevel.Love)
                .SetNPCAffection(NPCID.Dryad, AffectionLevel.Love)
                .SetNPCAffection(NPCID.Truffle, AffectionLevel.Like)
                .SetNPCAffection(NPCID.Angler, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Hate)
            ;
            NPCProfile = new Profiles.StackedNPCProfile(
                new Profiles.DefaultNPCProfile(Texture, NPCHeadLoader.GetHeadSlot(HeadTexture), Texture + "_Party"),
                new Profiles.DefaultNPCProfile(Texture + "_Shimmer", ShimmerHeadIndex, Texture + "_Shimmer_Party")
            );
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = 7;
            NPC.damage = 10;
            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            AnimationType = NPCID.Guide;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange([
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
				new FlavorTextBestiaryInfoElement("The Rock Collector lives among the violent nature of the Crystal Caverns, but is always cheerful. Has a huge collection of rare gemstones!"),
				new FlavorTextBestiaryInfoElement("Mods.AerovelenceMod.Bestiary.RockCollector")
            ]);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPCID.Sets.NPCBestiaryDrawOffset.TryGetValue(Type, out NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers))
            {
                drawModifiers.Rotation += 0.001f;
                NPCID.Sets.NPCBestiaryDrawOffset.Remove(Type);
                NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
            }
            return true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            int num = NPC.life > 0 ? 1 : 5;

            for (int k = 0; k < num; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<StillDust>());
            }

            if (Main.netMode != NetmodeID.Server && NPC.life <= 0)
            {
                string variant = "";
                if (NPC.IsShimmerVariant) variant += "_Shimmer";
                if (NPC.altTexture == 1) variant += "_Party";
                int hatGore = NPC.GetPartyHatGore();
                int headGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Head").Type;
                int armGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Arm").Type;
                int legGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Leg").Type;
                if (hatGore > 0)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, hatGore);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, headGore, 1f);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 20), NPC.velocity, armGore);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 20), NPC.velocity, armGore);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, legGore);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, legGore);
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_SpawnNPC)
                TownNPCRespawnSystem.unlockedRockCollectorSpawn = true;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            if (TownNPCRespawnSystem.unlockedRockCollectorSpawn)
                return true;
            foreach (var player in Main.ActivePlayers)
                if (player.inventory.Any(item => item.type == ModContent.ItemType<CavernCrystalItem>() || item.type == ModContent.ItemType<CavernStoneItem>()))
                    return true;
            return false;
        }

        public override ITownNPCProfile TownNPCProfile() { return NPCProfile; }

        public override List<string> SetNPCNameList()
        {
            return [
                "Roxxane",
                "Mindy",
                "S'tony",
                "Dwayne",
                "Rockgomery",
                "John Dygg",
                "Stephing Stone",
                "Geolbert",
                "Brock",
                "Roark"
            ];
        }

        public override void FindFrame(int frameHeight)
        {
            /*npc.frame.Width = 40;
			if (((int)Main.time / 10) % 2 == 0)
			{
				npc.frame.X = 40;
			}
			else
			{
				npc.frame.X = 0;
			}*/
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new();
            int partyGirl = NPC.FindFirstNPC(NPCID.PartyGirl);
            if (partyGirl >= 0 && Main.rand.NextBool(4))
                chat.Add(Language.GetTextValue("Mods.AerovelenceMod.Dialogue.RockCollector.PartyGirlDialogue", Main.npc[partyGirl].GivenName));
            chat.Add(Language.GetTextValue("Mods.AerovelenceMod.Dialogue.RockCollector.StandardDialogue1"));
            chat.Add(Language.GetTextValue("Mods.AerovelenceMod.Dialogue.RockCollector.StandardDialogue2"));
            chat.Add(Language.GetTextValue("Mods.AerovelenceMod.Dialogue.RockCollector.StandardDialogue3"));
            chat.Add(Language.GetTextValue("Mods.AerovelenceMod.Dialogue.RockCollector.StandardDialogue4"));
            chat.Add(Language.GetTextValue("Mods.AerovelenceMod.Dialogue.RockCollector.CommonDialogue"), 5.0);
            chat.Add(Language.GetTextValue("Mods.AerovelenceMod.Dialogue.RockCollector.RareDialogue"), 0.1);
            NumberOfTimesTalkedTo++;
            if (NumberOfTimesTalkedTo >= 10)
                chat.Add(Language.GetTextValue("Mods.AerovelenceMod.Dialogue.RockCollector.TalkALot"));
            string chosenChat = chat;
            if (chosenChat == Language.GetTextValue("Mods.AerovelenceMod.Dialogue.RockCollector.StandardDialogue4"))
                Main.npcChatCornerItem = ItemID.HiveBackpack;
            return chosenChat;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            if (Main.LocalPlayer.HasItem(ModContent.ItemType<ShotgunAxe>()) /*||
                Main.LocalPlayer.HasItem(ModContent.ItemType<TinCluster>()) ||
                Main.LocalPlayer.HasItem(ModContent.ItemType<IronCluster>()) ||
                Main.LocalPlayer.HasItem(ModContent.ItemType<LeadCluster>()) ||
                Main.LocalPlayer.HasItem(ModContent.ItemType<SilverCluster>()) ||
                Main.LocalPlayer.HasItem(ModContent.ItemType<TungstenCluster>()) ||
                Main.LocalPlayer.HasItem(ModContent.ItemType<PlatinumCluster>()) ||
                Main.LocalPlayer.HasItem(ModContent.ItemType<GoldCluster>()) ||
                Main.LocalPlayer.HasItem(ModContent.ItemType<SlateCluster>()) ||
                Main.LocalPlayer.HasItem(ModContent.ItemType<CobaltCluster>()) ||
                Main.LocalPlayer.HasItem(ModContent.ItemType<PalladiumCluster>()) ||
                Main.LocalPlayer.HasItem(ModContent.ItemType<OrichalcumCluster>()) ||
                Main.LocalPlayer.HasItem(ModContent.ItemType<MythrilCluster>()) ||
                Main.LocalPlayer.HasItem(ModContent.ItemType<TitaniumCluster>()) ||
                Main.LocalPlayer.HasItem(ModContent.ItemType<AdamantiteCluster>()) ||
                Main.LocalPlayer.HasItem(ModContent.ItemType<AdamantiteSuperCluster>()) ||
                Main.LocalPlayer.HasItem(ModContent.ItemType<TitaniumSuperCluster>()) ||
                                Main.LocalPlayer.HasItem(ModContent.ItemType<PhanticCluster>())*/)
                button = "Turn in ore chunks";
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shop)
        {
            int[] itemsToReceive = [ModContent.ItemType<ShotgunAxe>()];//ReinforcedPlatinumGrapple>(), ModContent.ItemType<MiningSack>(), ModContent.ItemType<AmuletOfGlory>()];
            int[] itemSuperAdamantite = [ModContent.ItemType<AdamantitePulsar>()];
            int[] itemSuperTitanium = [ModContent.ItemType<TitaniumRocketLauncher>()];

            int[] searchForItems = [
                ModContent.ItemType<ShotgunAxe>()/*, ModContent.ItemType<TinCluster>(),
                ModContent.ItemType<IronCluster>(), ModContent.ItemType<LeadCluster>(),
                ModContent.ItemType<SilverCluster>(), ModContent.ItemType<TungstenCluster>(),
                ModContent.ItemType<PlatinumCluster>(), ModContent.ItemType<GoldCluster>(),
                ModContent.ItemType<PhanticCluster>(), ModContent.ItemType<SlateCluster>(),
                ModContent.ItemType<CobaltCluster>(), ModContent.ItemType<PalladiumCluster>(),
                ModContent.ItemType<OrichalcumCluster>(), ModContent.ItemType<MythrilCluster>(),
                ModContent.ItemType<TitaniumCluster>(), ModContent.ItemType<AdamantiteCluster>()*/ ];

            int[] searchForSuperClusters = [ModContent.ItemType<ShotgunAxe>()];//AdamantiteSuperCluster>(), ModContent.ItemType<TitaniumSuperCluster>()];
            if (firstButton)
            {
                int selectedIndex = -1;
                for (int i = 0; i < Main.LocalPlayer.inventory.Length; ++i)
                {
                    if (Main.LocalPlayer.inventory[i].IsAir)
                        continue;

                    if (searchForItems.Contains(Main.LocalPlayer.inventory[i].type) || searchForSuperClusters.Contains(Main.LocalPlayer.inventory[i].type))
                    {
                        selectedIndex = i;
                        break;
                    }
                }

                if (selectedIndex != -1)
                {
                    if (Main.LocalPlayer.HasItem(ModContent.ItemType<ShotgunAxe>()))//AdamantiteSuperCluster>()))
                    {
                        Main.LocalPlayer.inventory[selectedIndex].TurnToAir();
                        int itemGetSuperAdamantite = itemSuperAdamantite[Main.rand.Next(itemSuperAdamantite.Length)];
                        SoundEngine.PlaySound(SoundID.Item37);
                        Main.npcChatText = $"I took you for granite. I'm so sorry... Here. Have a {Lang.GetItemNameValue(itemGetSuperAdamantite)}";
                        Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), itemGetSuperAdamantite);
                    }

                    else if (Main.LocalPlayer.HasItem(ModContent.ItemType<ShotgunAxe>()))//TitaniumSuperCluster>()))
                    {
                        Main.LocalPlayer.inventory[selectedIndex].TurnToAir();
                        int itemGetSuperTitanium = itemSuperTitanium[Main.rand.Next(itemSuperTitanium.Length)];
                        SoundEngine.PlaySound(SoundID.Item37);
                        Main.npcChatText = $"I took you for granite. I'm so sorry... Here. Have a {Lang.GetItemNameValue(itemGetSuperTitanium)}";
                        Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), itemGetSuperTitanium);
                    }

                    else
                    {
                        Main.LocalPlayer.inventory[selectedIndex].TurnToAir();
                        int itemToReceive = itemsToReceive[Main.rand.Next(itemsToReceive.Length)];
                        SoundEngine.PlaySound(SoundID.Item37);
                        Main.npcChatText = $"I took you for granite. I'm so sorry... Here. Have a {Lang.GetItemNameValue(itemToReceive)}";
                        Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), itemToReceive);
                    }
                }
            }
        }

        public override void AddShops()
        {
            var npcShop = new NPCShop(Type, ShopName)
                .Add<ShotgunAxe>()
                .Add(new Item(ModContent.ItemType<ShotgunAxe>()) { shopCustomPrice = Item.buyPrice(copper: 15) })
                .Add<ShotgunAxe>()
                .Add<ShotgunAxe>()
                .Add<ShotgunAxe>(Condition.MoonPhasesQuarter1)
                .Add<ShotgunAxe>()
                .Add<ShotgunAxe>(Condition.IsNpcShimmered);
            npcShop.Register();
        }

        public override void ModifyActiveShop(string shopName, Item[] items)
        {
            foreach (Item item in items)
            {
                if (item == null || item.type == ItemID.None)
                    continue;
                if (NPC.IsShimmerVariant)
                {
                    int value = item.shopCustomPrice ?? item.value;
                    item.shopCustomPrice = value / 2;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShotgunAxe>()));

        public override bool CanGoToStatue(bool toKingStatue) => true;

        public override void OnGoToStatue(bool toKingStatue)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = Mod.GetPacket();
              //  packet.Write((byte)ExampleMod.MessageType.ExampleTeleportToStatue);
                packet.Write((byte)NPC.whoAmI);
                packet.Send();
            }
            else
            {
                StatueTeleport();
            }
        }

        public void StatueTeleport()
        {
            for (int i = 0; i < 30; i++)
            {
                Vector2 position = Main.rand.NextVector2Square(-20, 21);
                if (Math.Abs(position.X) > Math.Abs(position.Y))
                    position.X = Math.Sign(position.X) * 20;
                else
                    position.Y = Math.Sign(position.Y) * 20;

                Dust.NewDustPerfect(NPC.Center + position, ModContent.DustType<PixelGlowOrb>(), Vector2.Zero).noGravity = true;
            }
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 20;
            knockback = 4f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = randExtraCooldown = 30;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ModContent.ProjectileType<CyverBeam>();
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 12f;
            randomOffset = 2f;
        }

        public override void LoadData(TagCompound tag) => NumberOfTimesTalkedTo = tag.GetInt("numberOfTimesTalkedTo");

        public override void SaveData(TagCompound tag) => tag["numberOfTimesTalkedTo"] = NumberOfTimesTalkedTo;

        public override int? PickEmote(Player closestPlayer, List<int> emoteList, WorldUIAnchor otherAnchor)
        {
            int type = ModContent.EmoteBubbleType<CyvercryEmote>();
            if (otherAnchor.entity is NPC { type: NPCID.Demolitionist })
                type = EmoteID.EmotionAnger;
            for (int i = 0; i < 4; i++)
                emoteList.Add(type);
            return base.PickEmote(closestPlayer, emoteList, otherAnchor);
        }
    }
}