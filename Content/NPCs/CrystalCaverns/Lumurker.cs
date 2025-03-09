using AerovelenceMod.Common.Globals.Players;
using Microsoft.Xna.Framework;
using System.IO;
using System;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using AerovelenceMod.Content.Biomes;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace AerovelenceMod.Content.NPCs.CrystalCaverns
{
    public class Lumurker : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
            {
                Position = new Vector2(0f, 8f),
                PortraitPositionXOverride = 0f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        bool IsElectricityActive = false;

        public override void SetDefaults()
        {
            NPC.aiStyle = 16;
            NPC.lifeMax = 4;
            NPC.damage = 20;
            NPC.defense = 24;
            NPC.knockBackResist = 0f;
            NPC.width = 78;
            NPC.height = 58;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath1;
            SpawnModBiomes = new int[] { ModContent.GetInstance<CrystalCavernsBiome>().Type };
        }


        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement("Lumurkers are the apex predator of the crystal waters. Like the angler fish from the ocean, they prey on creatures bigger than itself.")
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome(ModContent.GetInstance<CrystalCavernsBiome>()) && spawnInfo.Water)
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.8f;
            }
            return 0f;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(ai);
            writer.Write(IsElectricityActive);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            ai = reader.ReadSingle();
            IsElectricityActive = reader.ReadBoolean();
        }

        float ai = 0;
        float delayBetween = 0;
        public override bool PreAI()
        {
            NPC.TargetClosest(true);
            int untilImmune = 300;
            int immuneTimeLength = 120;
            IsElectricityActive = false;
            if (delayBetween > 0)
                delayBetween--;
            if (ai < 0f)
            {
                if (ai == -immuneTimeLength)
                {
                    for (int i = 0; i < 360; i += 12)
                    {
                        Vector2 circular = new Vector2(96, 0).RotatedBy(MathHelper.ToRadians(i));
                        Dust dust2 = Dust.NewDustDirect(NPC.Center - new Vector2(5) + circular, 0, 0, DustID.Electric, 0, 0, NPC.alpha);
                        dust2.velocity *= 0.15f;
                        dust2.velocity += -circular * 0.08f;
                        dust2.scale = 2.25f;
                        dust2.noGravity = true;
                    }
                }
                if (ai >= -immuneTimeLength + 20)
                {
                    IsElectricityActive = true;
                }
                ai += 1f;
                NPC.velocity.X *= 0.9f;
                if (Math.Abs(NPC.velocity.X) < 0.001)
                {
                    NPC.velocity.X = 0.001f * NPC.direction;
                }
                if (Math.Abs(NPC.velocity.Y) > 1f)
                {
                    ai += 10f;
                }
                if (ai >= 0f)
                {
                    NPC.netUpdate = true;
                    NPC.velocity.X += NPC.direction * 0.3f;
                }
                return false;
            }
            if (ai < untilImmune)
            {
                if (NPC.justHit)
                {
                    ai += 15f;
                }
                ai += 1f;
            }
            else if (Math.Abs(NPC.velocity.Y) <= 0.1f)
            {
                ai = -immuneTimeLength;
                NPC.netUpdate = true;
            }
            return true;
        }

        private bool animatingElectricity = false;
        private int frame = 0;

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            NPC.frameCounter++;
            if (IsElectricityActive)
            {
                animatingElectricity = true;
                if (NPC.frameCounter >= 5)
                {
                    frame++;
                    NPC.frameCounter = 0;
                    if (frame < 4 || frame > 7)
                        frame = 4;
                }
            }
            else
            {
                if (animatingElectricity && frame >= 4)
                {
                    frame = 0;
                    animatingElectricity = false;
                }
                if (NPC.frameCounter >= 8)
                {
                    frame++;
                    NPC.frameCounter = 0;
                    if (frame > 3)
                        frame = 0;
                }
            }
            NPC.frame.Y = frame * frameHeight;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 drawPosition = NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY);

            spriteBatch.Draw(
                texture,
                drawPosition,
                NPC.frame,
                drawColor,
                NPC.rotation,
                NPC.frame.Size() / 2f,
                NPC.scale,
                effects,
                0f
            );
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 drawPosition = NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Color glowColor = Color.White;
            if (IsElectricityActive)
            {
                float pulseIntensity = (float)Math.Sin(Main.GameUpdateCount * 0.1f) * 0.2f + 0.8f;
                glowColor = Color.Lerp(Color.DodgerBlue, Color.White, pulseIntensity);
            }

            spriteBatch.Draw(
                glowTexture,
                drawPosition,
                NPC.frame,
                glowColor,
                NPC.rotation,
                NPC.frame.Size() / 2f,
                NPC.scale,
                effects,
                0f
            );

            if (IsElectricityActive)
            {
                Texture2D sparkTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Glow").Value;
                float rotation = Main.GameUpdateCount * 0.05f;

                spriteBatch.Draw(
                    sparkTexture,
                    drawPosition,
                    null,
                    Color.Lerp(Color.Blue, Color.White, 0.5f) * 0.7f,
                    rotation,
                    sparkTexture.Size() / 2f,
                    NPC.scale * 0.8f,
                    SpriteEffects.None,
                    0f
                );
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}