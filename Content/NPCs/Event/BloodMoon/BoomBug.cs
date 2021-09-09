using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Event.BloodMoon
{
    public class BoomBug : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Boom Bug");

            Main.npcFrameCount[npc.type] = 5;
            npc.HitSound = SoundID.NPCHit44;
            npc.DeathSound = SoundID.NPCHit46;

            NPCID.Sets.TrailCacheLength[npc.type] = 8;
            NPCID.Sets.TrailingMode[npc.type] = 3;
        }

        public override void SetDefaults()
        {
            npc.lifeMax = 450;
            npc.damage = 20;

            npc.width = npc.height = 42;

            npc.noGravity = true;

            npc.knockBackResist = 0f;

            npc.aiStyle = -1;
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

            if (frame > 4)
            {
                frame = 0;
            }

            npc.frame.Y = frame * frameHeight;
        }

        private float SineProgress
        {
            get => npc.ai[0];
            set => npc.ai[0] = value;
        }

        private float ExplosionCooldown
        {
            get => npc.ai[1];
            set => npc.ai[1] = value;
        }

        public override void AI()
        {
            npc.TargetClosest();

            Player player = Main.player[npc.target];

            npc.rotation = npc.velocity.X * 0.1f;

            if (npc.collideX)
            {
                int yDirection = Math.Sign(player.position.Y - npc.position.Y);

                npc.velocity.Y += 0.01f * yDirection;
            }
            else
            {
                const float maxSpeed = 2.5f;

                float distance = Vector2.Distance(npc.Center, player.Center);
                distance = MathHelper.Clamp(distance, -maxSpeed, maxSpeed);

                Vector2 direction = npc.DirectionTo(player.Center) * distance;

                npc.velocity = Vector2.SmoothStep(npc.velocity, direction, 0.1f);

                bool canHit = Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height);

                float explosionDistance = 4f * 16f;

                ExplosionCooldown++;

                if (ExplosionCooldown > 5 * 60)
                {
                    npc.velocity *= 0.95f;

                    if (ExplosionCooldown > 8 * 60)
                    {
                        if (npc.Distance(player.Center) < explosionDistance && canHit && npc.HasValidTarget)
                        {
                            Explode();
                        }
                    }
                }

                SineProgress++;

                float sine = (float)Math.Sin(SineProgress / 20f) * 0.025f;

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

        private void Explode()
        {
            Main.PlaySound(SoundID.Item14, npc.position);

            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<BoomBugExplosion>(), npc.damage, 4f);

            npc.life = 0;

            npc.active = false;

            npc.checkDead();

            npc.HitEffect(0, 0);

            for (int i = 0; i < 10; i++)
            {
                float rotation = i / (float)10f * MathHelper.TwoPi;

                Vector2 velocity = rotation.ToRotationVector2() * 2f;

                Dust dust = Dust.NewDustDirect(npc.Center, 0, 0, DustID.Blood, velocity.X, velocity.Y);
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.scale = Main.rand.NextFloat(0.6f, 1f);
            }

            int smokeAmount = Main.rand.Next(3, 6);

            for (int i = 0; i < smokeAmount; i++)
            {
                var velocity = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));

                Gore.NewGore(npc.Center, velocity, Main.rand.Next(61, 64), Main.rand.NextFloat(0.6f, 1f));

                npc.netUpdate = true;
            }

            npc.netUpdate = true;
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

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => Main.bloodMoon ? SpawnCondition.OverworldNightMonster.Chance * 0.2f : 0f;
    }
}
