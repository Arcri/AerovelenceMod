using AerovelenceMod.Content.Biomes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace AerovelenceMod.Content.NPCs.CrystalCaverns
{
    public class Charger : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 10;
            NPCID.Sets.TrailCacheLength[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 3;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
            {
                Position = new Vector2(0f, 8f),
                PortraitPositionXOverride = 0f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 100;

            NPC.width = NPC.height = 42;

            NPC.noGravity = true;

            NPC.knockBackResist = 0.3f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath44;
            NPC.aiStyle = -1;
            NPC.damage = 5;
            AIType = -1;

            SpawnModBiomes = new int[] { ModContent.GetInstance<CrystalFieldsBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement("A smaller, younger version of the lightning moth variety. Really enjoys blue light, which is abundant in its environment.")
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome(ModContent.GetInstance<CrystalFieldsBiome>()) && !Main.dayTime)
            {
                return SpawnCondition.OverworldNightMonster.Chance;
            }
            return 0f;
        }


        private int frame;

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;

            NPC.frameCounter++;

            if (NPC.frameCounter >= 5f)
            {
                frame++;

                NPC.frameCounter = 0f;
            }

            int maxFrame = dashing ? 9 : 4;
            int minFrame = dashing ? 5 : 0;

            if (frame > maxFrame)
            {
                frame = minFrame;
            }

            NPC.frame.Y = frame * frameHeight;
        }

        private float SineProgress
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        private float DashCooldown
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        private bool dashing;

        public override void AI()
        {
            NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            float rotation = dashing ? NPC.velocity.ToRotation() : NPC.velocity.X * 0.1f;

            NPC.rotation = rotation;

            if (NPC.spriteDirection == -1 && dashing)
            {
                NPC.rotation += MathHelper.Pi;
            }

            if (NPC.collideX)
            {
                int yDirection = Math.Sign(player.position.Y - NPC.position.Y);

                NPC.velocity.Y += 0.01f * yDirection;
            }
            else
            {
                float maxSpeed = dashing ? 12f : 4f;

                float distance = Vector2.Distance(NPC.Center, player.Center);
                distance = MathHelper.Clamp(distance, -maxSpeed, maxSpeed);

                Vector2 direction = NPC.DirectionTo(player.Center) * distance;

                NPC.velocity = Vector2.SmoothStep(NPC.velocity, direction, 0.1f);

                bool canHit = Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);

                DashCooldown++;

                if (DashCooldown >= 5 * 60 && canHit && NPC.HasValidTarget)
                {
                    dashing = true;

                    if (DashCooldown >= 6 * 60 || !canHit || !NPC.HasValidTarget)
                    {
                        dashing = false;

                        DashCooldown = 0f;
                    }
                }

                SineProgress++;

                float sine = (float)Math.Sin(SineProgress / 20f) * 0.05f;

                NPC.velocity.Y += sine;
            }

            if (Main.rand.NextBool(20))
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.GemSapphire);
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.scale = Main.rand.NextFloat(0.6f, 1f);

                NPC.netUpdate = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/NPCs/CrystalCaverns/Charger_Glow").Value;

            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            Vector2 drawPosition = NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY);
            Texture2D glowTex = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
            Texture2D glowTex2 = Mod.Assets.Request<Texture2D>("Assets/Glorb").Value;
            Color glowColor = Color.DodgerBlue * ((255 - NPC.alpha) / 255f);
            Main.spriteBatch.Draw(glowTex, drawPosition, null, glowColor, 0f, glowTex.Size() / 2, 2f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(glowTex2, drawPosition, null, glowColor, 0f, glowTex2.Size() / 2, 0.5f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                float opacity = 0.8f - 0.2f * i;

                Vector2 trailPosition = NPC.oldPos[i] + NPC.Hitbox.Size() / 2f - Main.screenPosition + new Vector2(0f, NPC.gfxOffY);

                spriteBatch.Draw(texture, trailPosition, NPC.frame, drawColor * opacity, NPC.oldRot[i], NPC.frame.Size() / 2f, NPC.scale, effects, 0f);
            }

            

            spriteBatch.Draw(texture, drawPosition, NPC.frame, Color.Wheat, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Vector2 drawPosition = NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.Draw(texture, drawPosition, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0f);
            
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0f);
        }
    }
}