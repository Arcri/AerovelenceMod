using AerovelenceMod.Content.Biomes;
using AerovelenceMod.Common.Globals.Worlds;
using AerovelenceMod.Content.Items.Others.Quest;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Rubble;
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
using AerovelenceMod.Content.Items.Tools;
using AerovelenceMod.Content.Items.Accessories.SmallAccessories;
using AerovelenceMod.Content.Items.Potions;
using AerovelenceMod.Content.Items.Tools.Drills;
using AerovelenceMod.Common.Systems;
using AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Launchers;
using AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns.AdamantitePulsar;

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
                .SetBiomeAffection<CrystalCavernsBiome>(AffectionLevel.Love).SetBiomeAffection<CrystalCavernsSurfaceBiome>(AffectionLevel.Love)
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
                int headGore = Mod.TryFind<ModGore>($"{Name}_Gore{variant}_Head", out var headVariant) ? headVariant.Type : Mod.Find<ModGore>($"{Name}_Gore_Head").Type;
                int armGore = Mod.TryFind<ModGore>($"{Name}_Gore{variant}_Arm", out var armVariant) ? armVariant.Type : Mod.Find<ModGore>($"{Name}_Gore_Arm").Type;
                int legGore = Mod.TryFind<ModGore>($"{Name}_Gore{variant}_Leg", out var legVariant) ? legVariant.Type : Mod.Find<ModGore>($"{Name}_Gore_Leg").Type;
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

        public override bool CanTownNPCSpawn(int numTownNPCs) => DownedWorld.DownedCrystalTumbler;

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
                "Geolbertina",
                "Brock",
                "Roark",
                "Opalia"
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
            //Commenting out this dialogue just in case, but probably not necessary to keep, someone more knowledgeable clean up
            //int demolitionist = NPC.FindFirstNPC(NPCID.Demolitionist); 
            //int angler = NPC.FindFirstNPC(NPCID.Angler);
            //int dryad = NPC.FindFirstNPC(NPCID.Dryad);

            //if (demolitionist >= 0 && Main.rand.NextBool(4))
            //    chat.Add(Language.GetTextValue("Mods.AerovelenceMod.Dialogue.RockCollector.DemolitionistDialogue", Main.npc[demolitionist].GivenName));
            //if (angler >= 0 && Main.rand.NextBool(6))
            //    chat.Add(Language.GetTextValue("Mods.AerovelenceMod.Dialogue.RockCollector.AnglerDialogue", Main.npc[angler].GivenName));
            //if (dryad >= 0 && Main.rand.NextBool(6))
            //    chat.Add(Language.GetTextValue("Mods.AerovelenceMod.Dialogue.RockCollector.DryadDialogue", Main.npc[dryad].GivenName));

            chat.Add(Language.GetTextValue("Mods.AerovelenceMod.NPCs.RockCollector.Dialogue.StandardDialogue1"));
            chat.Add(Language.GetTextValue("Mods.AerovelenceMod.NPCs.RockCollector.Dialogue.StandardDialogue2"));
            chat.Add(Language.GetTextValue("Mods.AerovelenceMod.NPCs.RockCollector.Dialogue.StandardDialogue3"));
            chat.Add(Language.GetTextValue("Mods.AerovelenceMod.NPCs.RockCollector.Dialogue.StandardDialogue4"));
            chat.Add(Language.GetTextValue("Mods.AerovelenceMod.NPCs.RockCollector.Dialogue.CommonDialogue"), 5.0);
            chat.Add(Language.GetTextValue("Mods.AerovelenceMod.NPCs.RockCollector.Dialogue.RareDialogue"), 0.1);

            //Utterly worthless line + it saves the timestalkedto for some reason???
            //NumberOfTimesTalkedTo++;

            //if (NumberOfTimesTalkedTo >= 10 && dryad >= 0)
            //    chat.Add(Language.GetTextValue("Mods.AerovelenceMod.Dialogue.RockCollector.TalkALot", Main.npc[dryad].GivenName));

            // Ensure chat isn't empty to prevent crashes
            if (chat.elements.Count == 0)
            {
                chat.Add(Language.GetTextValue("Mods.AerovelenceMod.NPCs.RockCollector.Dialogue.StandardDialogue1"));
            }

            string chosenChat = chat.Get();

            // Ensure chosenChat isn't null or empty
            if (string.IsNullOrEmpty(chosenChat))
                chosenChat = Language.GetTextValue("Mods.AerovelenceMod.NPCs.RockCollector.Dialogue.StandardDialogue2");

            if (chosenChat == Language.GetTextValue("Mods.AerovelenceMod.NPCs.RockCollector.Dialogue.StandardDialogue4"))
                Main.npcChatCornerItem = ModContent.ItemType<OnTheRocks>();

            return chosenChat;
        }


        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = RockCollectorTrade.Text("TurnIn");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shop)
        {
            if (firstButton)
            {
                shop = ShopName;
                return;
            }
            Player player = Main.LocalPlayer;
            int slot = RockCollectorTrade.FindSpecimen(player);
            if (slot < 0)
            {
                Main.npcChatText = RockCollectorTrade.Text("NoOre");
                return;
            }
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write(RockCollectorTrade.RequestPacket);
                packet.Write((short)NPC.whoAmI);
                packet.Write((byte)slot);
                packet.Write(player.inventory[slot].type);
                packet.Send();
            }
            else
                RockCollectorTrade.TurnIn(player.whoAmI, NPC.whoAmI, slot, player.inventory[slot].type);
        }

        public override void AddShops()
        {
            var npcShop = new NPCShop(Type, ShopName)
                .Add<PouchOfRocks>()
                .Add<SpeedstersPickaxe>()
                .Add<ResonanceDrill>()
                .Add<OpalOfCaVea>()
                .Add<AmuletOfGlory>()
                .Add<PlatinumHook>()
                .Add<OnTheRocks>()


                .Add(new Item(ItemID.Geode) { shopCustomPrice = Item.buyPrice(gold: 3) })

                .Add(ItemID.CopperOre)
                .Add(ItemID.TinOre)
                .Add(ItemID.IronOre)
                .Add(ItemID.LeadOre)

                .Add(ItemID.SilverOre, Condition.DownedEowOrBoc)
                .Add(ItemID.TungstenOre, Condition.DownedEowOrBoc)
                .Add(ItemID.GoldOre, Condition.DownedEowOrBoc)
                .Add(ItemID.PlatinumOre, Condition.DownedEowOrBoc)

                .Add(ItemID.DemoniteOre, Condition.Hardmode)
                .Add(ItemID.CrimtaneOre, Condition.Hardmode)
                .Add(ItemID.Hellstone, Condition.Hardmode)

                .Add(ItemID.ChlorophyteOre, Condition.DownedGolem)

                .Add(new Item(ModContent.ItemType<ElectricBlueSolution>()), Condition.DownedMechBossAny);

                //.Add(new Item(ModContent.ItemType<ShotgunAxe>()) { shopCustomPrice = Item.buyPrice(copper: 15) })
                //.Add<ShotgunAxe>(Condition.IsNpcShimmered);

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

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CavernStoneItem>(), 1, 8, 16));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CavernCrystalItem>(), 1, 2, 5));
        }

        public override void OnKill()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            Point feet = NPC.Bottom.ToTileCoordinates();
            int type = ModContent.TileType<CavernStone1x1FloorRubbleNatural>();
            for (int down = 0; down <= 4; down++)
                for (int offset = 0; offset < 5; offset++)
                {
                    int x = feet.X + (offset % 2 == 0 ? offset / 2 : -(offset + 1) / 2);
                    int y = feet.Y + down - 1;
                    if (!WorldGen.InWorld(x, y, 2)) continue;
                    Tile empty = Main.tile[x, y];
                    Tile support = Main.tile[x, y + 1];
                    if (empty.HasTile || empty.LiquidAmount > 0 || !support.HasUnactuatedTile || !Main.tileSolid[support.TileType] ||
                        Main.tileSolidTop[support.TileType] || support.IsHalfBlock || support.Slope != SlopeType.Solid)
                        continue;
                    Rectangle space = new(x * 16, y * 16, 16, 16);
                    bool occupied = false;
                    foreach (Player player in Main.ActivePlayers)
                        occupied |= !player.dead && player.Hitbox.Intersects(space);
                    foreach (NPC other in Main.ActiveNPCs)
                        occupied |= other.whoAmI != NPC.whoAmI && other.Hitbox.Intersects(space);
                    if (occupied) continue;
                    WorldGen.PlaceObject(x, y, type, mute: true, style: Main.rand.Next(12));
                    if (Main.tile[x, y].HasTile && Main.tile[x, y].TileType == type)
                    {
                        if (Main.netMode == NetmodeID.Server) NetMessage.SendTileSquare(-1, x, y, 1);
                        return;
                    }
                }
        }

        //COMMENTING OUT CUZ ROCK COLLECTOR IS LITERALLY AGENDER
        
        //public override bool CanGoToStatue(bool toKingStatue) => true;

        //public override void OnGoToStatue(bool toKingStatue)
        //{
        //    if (Main.netMode == NetmodeID.Server)
        //    {
        //        ModPacket packet = Mod.GetPacket();
        //      //  packet.Write((byte)ExampleMod.MessageType.ExampleTeleportToStatue);
        //        packet.Write((byte)NPC.whoAmI);
        //        packet.Send();
        //    }
        //    else
        //    {
        //        StatueTeleport();
        //    }
        //}

        //public void StatueTeleport()
        //{
        //    for (int i = 0; i < 30; i++)
        //    {
        //        Vector2 position = Main.rand.NextVector2Square(-20, 21);
        //        if (Math.Abs(position.X) > Math.Abs(position.Y))
        //            position.X = Math.Sign(position.X) * 20;
        //        else
        //            position.Y = Math.Sign(position.Y) * 20;

        //        Dust.NewDustPerfect(NPC.Center + position, ModContent.DustType<PixelGlowOrb>(), Vector2.Zero).noGravity = true;
        //    }
        //}

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
    public class RockCollectorTrade : ModPlayer
    {
        internal const byte RequestPacket = 231;
        internal const byte ResultPacket = 232;
        private int cooldown;
        public override void PostUpdate() => cooldown = Math.Max(0, cooldown - 1);
        internal static string Text(string key) => global::AerovelenceMod.Common.Systems.Language.LocalizationManager.GetTranslation("AerovelenceMod.RockCollectorTrade." + key);
        internal static int FindSpecimen(Player player)
        {
            int selected = player.selectedItem;
            if (selected >= 0 && selected < 50 && !player.inventory[selected].favorited && player.inventory[selected].ModItem is RareOreCluster && player.inventory[selected].stack > 0)
                return selected;
            int best = -1;
            for (int slot = 0; slot < 50; slot++)
            {
                Item item = player.inventory[slot];
                if (item.stack <= 0 || item.favorited || item.ModItem is not RareOreCluster ore) continue;
                if (best < 0 || ore.RewardTier > ((RareOreCluster)player.inventory[best].ModItem).RewardTier)
                    best = slot;
            }
            return best;
        }
        internal static void TurnIn(int sender, int npcIndex, int slot, int expectedType)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient || sender < 0 || sender >= Main.maxPlayers ||
                npcIndex < 0 || npcIndex >= Main.maxNPCs || slot < 0 || slot >= 50)
                return;
            Player player = Main.player[sender];
            NPC collector = Main.npc[npcIndex];
            if (!player.active || player.dead || !collector.active || collector.type != ModContent.NPCType<RockCollector>() ||
                Vector2.DistanceSquared(player.Center, collector.Center) > 240f * 240f)
                return;
            Item item = player.inventory[slot];
            var state = player.GetModPlayer<RockCollectorTrade>();
            if (state.cooldown > 0 || item.type != expectedType || item.stack <= 0 || item.favorited || item.ModItem is not RareOreCluster ore)
                return;
            int silver = ore.RewardSilver;
            int crystals = ore.RewardCrystals;
            state.cooldown = 20;
            item.stack--;
            if (item.stack == 0) item.TurnToAir();
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncEquipment, -1, -1, null, sender, slot);
            if (silver >= 100) Reward(player, collector, ItemID.GoldCoin, silver / 100);
            if (silver % 100 > 0) Reward(player, collector, ItemID.SilverCoin, silver % 100);
            Reward(player, collector, ModContent.ItemType<CavernCrystalItem>(), crystals);
            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = ModContent.GetInstance<RockCollector>().Mod.GetPacket();
                packet.Write(ResultPacket);
                packet.Write(silver);
                packet.Write(crystals);
                packet.Send(sender);
            }
            else ShowReward(silver, crystals);
        }
        private static void Reward(Player player, NPC collector, int type, int stack)
        {
            int index = Item.NewItem(collector.GetSource_GiftOrReward(), player.Hitbox, type, stack, noBroadcast: true);
            if (index < 0 || index >= Main.maxItems) return;
            Main.item[index].playerIndexTheItemIsReservedFor = player.whoAmI;
            Main.item[index].noGrabDelay = 0;
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, index);
                NetMessage.SendData(MessageID.ItemOwner, -1, -1, null, index);
            }
        }
        internal static void ShowReward(int silver, int crystals)
        {
            if (Main.dedServ) return;
            SoundEngine.PlaySound(SoundID.Item37 with { Volume = 0.5f });
            Main.npcChatText = string.Format(Text("Thanks"), silver, crystals);
        }
    }

    public class RockCollectorSupport : ModSystem
    {
        public override void Load()
        {
            Register("TurnIn", "Turn in rare ores", "Entregar minerales raros");
            Register("NoOre", "Bring me a super ore specimen from your mining trips! Hold the one you want to trade, or I will take your highest-tier unfavorited specimen. Favorites stay in your collection.",
                "¡Tráeme una muestra de supermineral de tus expediciones! Sostén la que quieras entregar, o elegiré la de mayor categoría que no sea favorita. Tus favoritas se quedan contigo.");
            Register("Thanks", "Now that is a rock worth collecting! Here are {0} silver coins and {1} cavern crystals for your specimen.",
                "¡Esta roca merece estar en mi colección! Aquí tienes {0} monedas de plata y {1} cristales de caverna por tu muestra.");
        }
        private static void Register(string key, string english, string spanish)
        {
            string fullKey = "AerovelenceMod.RockCollectorTrade." + key;
            global::AerovelenceMod.Common.Systems.Language.LocalizationManager.RegisterTranslation(fullKey, english, "default");
            global::AerovelenceMod.Common.Systems.Language.LocalizationManager.RegisterTranslation(fullKey, spanish, "es-ES");
        }
        public override void PostSetupContent()
        {
            if (ModLoader.TryGetMod("Census", out Mod census))
                census.Call("TownNPCCondition", ModContent.NPCType<RockCollector>(),
                    Language.GetText("Mods.AerovelenceMod.NPCs.RockCollector.Census.SpawnCondition"));
        }
    }
}