using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace AerovelenceMod.Content.NPCs.Event.BloodMoon
{
    public class BloodMoth : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Moth");

            Main.npcFrameCount[NPC.type] = 10;

            NPCID.Sets.TrailCacheLength[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 3;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 250;

            NPC.width = NPC.height = 42;

            NPC.noGravity = true;

            NPC.knockBackResist = 1f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath44;
            NPC.aiStyle = -1;
            NPC.damage = 20;
            AIType = -1;
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
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Blood);
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.scale = Main.rand.NextFloat(0.6f, 1f);

                NPC.netUpdate = true;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Main.instance.LoadNPC(NPC.type);
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;

            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                float opacity = 0.8f - 0.2f * i;

                Vector2 trailPosition = NPC.oldPos[i] + NPC.Hitbox.Size() / 2f - Main.screenPosition + new Vector2(0f, NPC.gfxOffY);

                Main.EntitySpriteDraw(texture, trailPosition, NPC.frame, drawColor * opacity, NPC.oldRot[i], NPC.frame.Size() / 2f, NPC.scale, effects, 0);
            }

            Vector2 drawPosition = NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY);

            Main.EntitySpriteDraw(texture, drawPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture + "_Glow");

            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Vector2 drawPosition = NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY);

            Main.EntitySpriteDraw(texture, drawPosition, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => Main.bloodMoon ? SpawnCondition.OverworldNightMonster.Chance * 0.5f : 0;
    }
}
