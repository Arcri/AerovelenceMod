using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Event.BloodMoon
{
    public class CrimsonReach : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crimson Reach");

            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.lifeMax = 250;

            npc.width = 80;
            npc.height = 40;

            npc.knockBackResist = 0f;

            npc.aiStyle = -1;
            aiType = -1;
        }

        private int frame;

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;

            if (npc.frameCounter >= 5f)
            {
                frame++;

                npc.frameCounter = 0f;
            }

            if (frame > 3)
            {
                frame = 0;
            }

            npc.frame.Y = frame * frameHeight;
        }

        private float ShootTimer
        {
            get => npc.ai[0];
            set => npc.ai[0] = value;
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

                    Projectile.NewProjectile(npc.Center, velocity, ModContent.ProjectileType<CrimsonReachBall>(), npc.damage, 2f);
                }

                ShootTimer = 0f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = Main.npcTexture[npc.type];

            SpriteEffects effects = npc.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Vector2 drawPosition = npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY);

            spriteBatch.Draw(texture, drawPosition, npc.frame, drawColor, npc.rotation, npc.frame.Size() / 2f, scale, effects, 0f);

            return false;
        }
    }
}
