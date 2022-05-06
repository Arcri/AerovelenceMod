using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader.Utilities;

namespace AerovelenceMod.Content.NPCs.Event.BloodMoon
{
    public class BoomBug : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Boom Bug");

            Main.npcFrameCount[NPC.type] = 5;

            NPCID.Sets.TrailCacheLength[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 3;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 450;

            NPC.width = NPC.height = 42;

            NPC.noGravity = true;

            NPC.knockBackResist = 0.1f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath44;
            NPC.aiStyle = -1;
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

            if (frame > 4)
            {
                frame = 0;
            }

            NPC.frame.Y = frame * frameHeight;
        }

        private float SineProgress
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        private float ExplosionCooldown
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        public override void AI()
        {
            NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            NPC.rotation = NPC.velocity.X * 0.1f;

            if (NPC.collideX)
            {
                int yDirection = Math.Sign(player.position.Y - NPC.position.Y);

                NPC.velocity.Y += 0.01f * yDirection;
            }
            else
            {
                const float maxSpeed = 2.5f;

                float distance = Vector2.Distance(NPC.Center, player.Center);
                distance = MathHelper.Clamp(distance, -maxSpeed, maxSpeed);

                Vector2 direction = NPC.DirectionTo(player.Center) * distance;

                NPC.velocity = Vector2.SmoothStep(NPC.velocity, direction, 0.1f);

                bool canHit = Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);

                float explosionDistance = 4f * 16f;

                ExplosionCooldown++;

                if (ExplosionCooldown > 5 * 60)
                {
                    NPC.velocity *= 0.95f;

                    if (ExplosionCooldown > 8 * 60)
                    {
                        if (NPC.Distance(player.Center) < explosionDistance && canHit && NPC.HasValidTarget)
                        {
                            Explode();
                        }
                    }
                }

                SineProgress++;

                float sine = (float)Math.Sin(SineProgress / 20f) * 0.025f;

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

        private void Explode()
        {
            var entitySource = NPC.GetSource_FromAI();
            SoundEngine.PlaySound(SoundID.Item14, NPC.position);

            Projectile.NewProjectile(entitySource, NPC.Center, Vector2.Zero, ModContent.ProjectileType<BoomBugExplosion>(), NPC.damage, 4f);

            NPC.life = 0;

            NPC.active = false;

            NPC.checkDead();

            NPC.HitEffect(0, 0);

            for (int i = 0; i < 10; i++)
            {
                float rotation = i / (float)10f * MathHelper.TwoPi;

                Vector2 velocity = rotation.ToRotationVector2() * 2f;

                Dust dust = Dust.NewDustDirect(NPC.Center, 0, 0, DustID.Blood, velocity.X, velocity.Y);
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.scale = Main.rand.NextFloat(0.6f, 1f);
            }

            int smokeAmount = Main.rand.Next(3, 6);

            for (int i = 0; i < smokeAmount; i++)
            {
                var velocity = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));

                Gore.NewGore(entitySource, NPC.Center, velocity, Main.rand.Next(61, 64), Main.rand.NextFloat(0.6f, 1f));

                NPC.netUpdate = true;
            }

            NPC.netUpdate = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Main.instance.LoadProjectile(NPC.type);
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;

            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                float opacity = 0.8f - 0.2f * i;

                Vector2 trailPosition = NPC.oldPos[i] + NPC.Hitbox.Size() / 2f - Main.screenPosition + new Vector2(0f, NPC.gfxOffY);

                Main.EntitySpriteDraw(texture, trailPosition, NPC.frame, drawColor * opacity, NPC.oldRot[i], NPC.frame.Size() / 2f, NPC.scale, effects, (int)0f);
            }

            Vector2 drawPosition = NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY);

            Main.EntitySpriteDraw(texture, drawPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, (int)0f);

            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture + "_Glow");

            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Vector2 drawPosition = NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY);

            Main.EntitySpriteDraw(texture, drawPosition, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, (int)0f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => Main.bloodMoon ? SpawnCondition.OverworldNightMonster.Chance * 0.5f : 0f;
    }
}
