using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Event.BloodMoon
{
    public class BloodMoth : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Moth");

            Main.npcFrameCount[npc.type] = 10;
            npc.HitSound = SoundID.NPCHit44;
            npc.DeathSound = SoundID.NPCHit46;

            NPCID.Sets.TrailCacheLength[npc.type] = 8;
            NPCID.Sets.TrailingMode[npc.type] = 3;
        }

        public override void SetDefaults()
        {
            npc.lifeMax = 250;

            npc.width = npc.height = 42;

            npc.noGravity = true;

            npc.knockBackResist = 0f;
            
            npc.aiStyle = -1;
            npc.damage = 20;
            aiType = -1;
        }

        private int frame;

        public override void FindFrame(int frameHeight)
        {
            npc.spriteDirection = npc.direction;

            npc.frameCounter++;

            if (npc.frameCounter >= 5f)
            {
                frame++;

                npc.frameCounter = 0f;
            }

            int maxFrame = dashing ? 9 : 4;
            int minFrame = dashing ? 5 : 0;

            if (frame > maxFrame)
            {
                frame = minFrame;
            }

            npc.frame.Y = frame * frameHeight;
        }

        private float SineProgress 
        { 
            get => npc.ai[0]; 
            set => npc.ai[0] = value; 
        }

        private float DashCooldown 
        { 
            get => npc.ai[1]; 
            set => npc.ai[1] = value; 
        }

        private bool dashing;

        public override void AI()
        {
            npc.TargetClosest();

            Player player = Main.player[npc.target];

            float rotation = dashing ? npc.velocity.ToRotation() : npc.velocity.X * 0.1f;

            npc.rotation = rotation;

            if (npc.spriteDirection == -1 && dashing)
            {
                npc.rotation += MathHelper.Pi;
            }

            if (npc.collideX)
            {
                int yDirection = Math.Sign(player.position.Y - npc.position.Y);

                npc.velocity.Y += 0.01f * yDirection;
            }
            else
            {
                float maxSpeed = dashing ? 12f : 4f;

                float distance = Vector2.Distance(npc.Center, player.Center);
                distance = MathHelper.Clamp(distance, -maxSpeed, maxSpeed);

                Vector2 direction = npc.DirectionTo(player.Center) * distance;

                npc.velocity = Vector2.SmoothStep(npc.velocity, direction, 0.1f);

                bool canHit = Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height);

                DashCooldown++;

                if (DashCooldown >= 5 * 60 && canHit && npc.HasValidTarget)
                {
                    dashing = true;

                    if (DashCooldown >= 6 * 60 || !canHit || !npc.HasValidTarget)
                    {
                        dashing = false;

                        DashCooldown = 0f;
                    }
                }

                SineProgress++;

                float sine = (float)Math.Sin(SineProgress / 20f) * 0.05f;

                npc.velocity.Y += sine;
            }

            if (Main.rand.NextBool(20))
            {
                Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Blood);
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.scale = Main.rand.NextFloat(0.6f, 1f);

                npc.netUpdate = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = Main.npcTexture[npc.type];

            SpriteEffects effects = npc.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
            {
                float opacity = 0.8f - 0.2f * i;

                Vector2 trailPosition = npc.oldPos[i] + npc.Hitbox.Size() / 2f - Main.screenPosition + new Vector2(0f, npc.gfxOffY);

                spriteBatch.Draw(texture, trailPosition, npc.frame, drawColor * opacity, npc.oldRot[i], npc.frame.Size() / 2f, npc.scale, effects, 0f);
            }

            Vector2 drawPosition = npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY);

            spriteBatch.Draw(texture, drawPosition, npc.frame, drawColor, npc.rotation, npc.frame.Size() / 2f, npc.scale, effects, 0f);

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = ModContent.GetTexture(Texture + "_Glow");

            SpriteEffects effects = npc.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Vector2 drawPosition = npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY);

            spriteBatch.Draw(texture, drawPosition, npc.frame, Color.White, npc.rotation, npc.frame.Size() / 2f, npc.scale, effects, 0f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => Main.bloodMoon ? SpawnCondition.OverworldNightMonster.Chance * 0.3f : 0f;
    }
}
