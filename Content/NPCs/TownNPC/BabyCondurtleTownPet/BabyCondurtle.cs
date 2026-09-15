using System;
using System.Collections.Generic;
using System.IO;
using AerovelenceMod.Common.Systems.Language;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AerovelenceMod.Content.NPCs.TownNPC.BabyCondurtleTownPet
{
    [AutoloadHead]
    public class BabyCondurtle : TranslatableModNPC
    {
        private static ITownNPCProfile profile;
        private bool frightened;
        private int fearTime;
        private int dangerClock;
        private int shellProgress;
        private float walkClock;

        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Baby Condurtle", "A tiny, humming bundle of crystal and shell. Gentle petting keeps its little sparks happy.")
                .AddName(Language.Spanish, "Condurtle Bebé")
                .AddFlavor(Language.Spanish, "Un pequeño montoncito de cristal y caparazón que tararea. Las caricias suaves alegran sus pequeñas chispas.");
            Main.npcFrameCount[Type] = 11;
            NPCID.Sets.ExtraFramesCount[Type] = 4;
            NPCID.Sets.AttackFrameCount[Type] = 0;
            NPCID.Sets.DangerDetectRange[Type] = 240;
            NPCID.Sets.AttackType[Type] = -1;
            NPCID.Sets.AttackTime[Type] = -1;
            NPCID.Sets.IsTownPet[Type] = true;
            NPCID.Sets.CannotSitOnFurniture[Type] = true;
            NPCID.Sets.NPCFramingGroup[Type] = 8;
            NPCID.Sets.HatOffsetY[Type] = 1;
            NPCID.Sets.ShimmerTownTransform[Type] = false;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Shimmer] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.PlayerDistanceWhilePetting[Type] = 28;
            NPCID.Sets.IsPetSmallForPetting[Type] = true;
            NPCID.Sets.TownNPCBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers { Velocity = 0.5f, Direction = 1 });
            if (!Main.dedServ)
                profile = new BabyCondurtleProfile(Texture, HeadTexture);
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 30;
            NPC.height = 22;
            NPC.aiStyle = NPCAIStyleID.Passive;
            NPC.damage = 0;
            NPC.defense = 12;
            NPC.lifeMax = 250;
            NPC.knockBackResist = 0.35f;
            NPC.HitSound = SoundID.NPCHit1 with { Volume = 0.45f, Pitch = 0.5f };
            NPC.DeathSound = SoundID.NPCDeath6 with { Volume = 0.4f };
            NPC.housingCategory = 1;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.Add(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface);
            base.SetBestiary(database, bestiaryEntry);
        }

        public override bool CanTownNPCSpawn(int numTownNPCs) => BabyCondurtleWorld.Unlocked;
        public override ITownNPCProfile TownNPCProfile() => profile;
        public override List<string> SetNPCNameList() => new() { "Pebble", "Pip", "Bubbles", "Nibbles", "Mica", "Pudding" };
        public override string GetChat()
        {
            SoundEngine.PlaySound(SoundID.Item4 with { Volume = 0.15f, Pitch = -0.45f }, NPC.Center);
            return Main.rand.Next(5) switch { 0 => "Prrr...", 1 => "Mrrp!", 2 => "Chirp, chirp!", 3 => "Brrup?", _ => "Hmmm... prrr!" };
        }
        public override void SetChatButtons(ref string button, ref string button2)
            => button = Terraria.Localization.Language.GetTextValue("UI.PetTheAnimal");

        public override bool PreAI()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (fearTime > 0)
                    fearTime--;
                if (++dangerClock >= 30)
                {
                    dangerClock = 0;
                    foreach (NPC other in Main.ActiveNPCs)
                    {
                        if (other.whoAmI == NPC.whoAmI || other.friendly || other.damage <= 0 || other.dontTakeDamage)
                            continue;
                        if (Vector2.DistanceSquared(NPC.Center, other.Center) < 240f * 240f && Collision.CanHitLine(NPC.position, NPC.width, NPC.height, other.position, other.width, other.height))
                        {
                            fearTime = Math.Max(fearTime, 90);
                            break;
                        }
                    }
                }
                bool next = fearTime > 0;
                if (next != frightened)
                {
                    frightened = next;
                    NPC.netUpdate = true;
                }
            }
            shellProgress = Math.Clamp(shellProgress + (frightened ? 1 : -1), 0, 18);
            if (shellProgress > 0)
            {
                NPC.velocity.X *= 0.7f;
                if (Math.Abs(NPC.velocity.X) < 0.1f)
                    NPC.velocity.X = 0f;
                return false;
            }
            return true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                fearTime = 150;
                frightened = true;
                NPC.netUpdate = true;
            }
        }
        public override void SendExtraAI(BinaryWriter writer) { writer.Write(frightened); writer.Write((byte)shellProgress); }
        public override void ReceiveExtraAI(BinaryReader reader) { frightened = reader.ReadBoolean(); shellProgress = Math.Clamp((int)reader.ReadByte(), 0, 18); }

        internal static int ShellFrame(int progress, bool emerging) => emerging ? 10 : 7 + Math.Clamp((progress - 1) / 6, 0, 2);
        private float ShellDrop => NPC.IsABestiaryIconDummy ? 0f : 2f * MathHelper.Clamp((shellProgress - 6f) / 6f, 0f, 1f);
        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            int frame = 0;
            if (shellProgress > 0 && !NPC.IsABestiaryIconDummy)
            {
                frame = ShellFrame(shellProgress, !frightened);
                walkClock = 0f;
            }
            else if (Math.Abs(NPC.velocity.X) > 0.1f)
            {
                walkClock += Math.Clamp(Math.Abs(NPC.velocity.X), 0.35f, 1.5f);
                frame = (int)(walkClock / 6f) % 7;
            }
            else
                walkClock = 0f;
            NPC.frame.Y = frame * frameHeight;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glowmask").Value;
            Rectangle frame = new(0, Math.Clamp(NPC.frame.Y / 30, 0, 10) * 30, 42, 30);
            Vector2 position = NPC.Bottom - screenPos + new Vector2(0f, NPC.gfxOffY + 6f + ShellDrop);
            Vector2 origin = new(21f, 30f);
            SpriteEffects flip = NPC.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture, position, frame, drawColor, NPC.rotation, origin, NPC.scale, flip, 0f);
            float pulse = 0.6f + MathF.Sin(Main.GlobalTimeWrappedHourly * 2f + NPC.whoAmI) * 0.15f;
            spriteBatch.Draw(glow, position, frame, Color.White * 0.8f, NPC.rotation, origin, NPC.scale, flip, 0f);
            spriteBatch.Draw(glow, position, frame, new Color(90, 180, 255, 0) * pulse * 0.4f, NPC.rotation, origin, NPC.scale, flip, 0f);
            return false;
        }

        public override void PartyHatPosition(ref Vector2 position, ref SpriteEffects spriteEffects)
        {
            position.X += shellProgress > 0 ? 0 : 11f * NPC.spriteDirection;
            position.Y += 5f + ShellDrop;
        }
    }

    internal sealed class BabyCondurtleProfile : ITownNPCProfile
    {
        private readonly Asset<Texture2D> texture;
        private readonly int head;
        internal BabyCondurtleProfile(string path, string headPath)
        {
            texture = ModContent.Request<Texture2D>(path);
            head = ModContent.GetModHeadSlot(headPath);
        }
        public int RollVariation() => 0;
        public string GetNameForVariant(NPC npc) => npc.getNewNPCName();
        public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc) => texture;
        public int GetHeadTextureIndex(NPC npc) => head;
    }

    public class BabyCondurtleWorld : ModSystem
    {
        public static bool Unlocked;
        public override void ClearWorld() => Unlocked = false;
        public override void SaveWorldData(TagCompound tag) => tag["BabyCondurtleUnlocked"] = Unlocked;
        public override void LoadWorldData(TagCompound tag) => Unlocked = tag.GetBool("BabyCondurtleUnlocked");
        public override void NetSend(BinaryWriter writer) => writer.Write(Unlocked);
        public override void NetReceive(BinaryReader reader) => Unlocked = reader.ReadBoolean();
        public override void OnWorldUnload() => Unlocked = false;
    }
}
