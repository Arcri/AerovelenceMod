using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Event.BloodMoon
{
    public class CrimsonReach : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crimson Reach");

            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 250;

            NPC.width = 80;
            NPC.height = 40;
            NPC.damage = 40;

            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath44;
            NPC.aiStyle = -1;
            AIType = -1;
        }

        private int frame;

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

            if (NPC.frameCounter >= 5f)
            {
                frame++;

                NPC.frameCounter = 0f;
            }

            if (frame > 3)
            {
                frame = 0;
            }

            NPC.frame.Y = frame * frameHeight;
        }

        private float ShootTimer
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        private Vector2 scale = Vector2.Zero;

        public override void AI()
        {
            scale = Vector2.Lerp(scale, Vector2.One, 0.1f);

            ShootTimer++;

            if (ShootTimer > 120f)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 velocity = new Vector2(Main.rand.NextFloat(-3f, 3f), -3f);

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, ModContent.ProjectileType<CrimsonReachBall>(), NPC.damage, 2f);
                }

                ShootTimer = 0f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
        Texture2D texture = (Texture2D)TextureAssets.Npc[NPC.type];

            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Vector2 drawPosition = NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY);

            Main.spriteBatch.Draw(texture, drawPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, scale, effects, 0);

            return false;
        }
    }
}
