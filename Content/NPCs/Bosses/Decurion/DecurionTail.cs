using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.Decurion
{
    [AutoloadBossHead]
    public class DecurionTail : ModNPC
    {
        public float currentSpeed = 3;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Decurion"); //DONT Change me
            Main.npcFrameCount[NPC.type] = 1;
        }
        public override void BossHeadRotation(ref float rotation) => rotation = NPC.rotation;
        public override void SetDefaults()
        {
            NPC.lifeMax = 100;        //this is the npc health
            NPC.damage = 90;    //this is the npc damage
            NPC.defense = 35;         //this is the npc defense
            NPC.knockBackResist = 0f;
            NPC.width = 66; //this is where you put the npc sprite width.     important
            NPC.height = 90; //this is where you put the npc sprite height.   important
            NPC.lavaImmune = true;       //this make the npc immune to lava
            NPC.noGravity = true;           //this make the npc float
            NPC.noTileCollide = true;        //this make the npc go tru walls
            NPC.behindTiles = true;
            Main.npcFrameCount[NPC.type] = 1;
            NPC.value = Item.buyPrice(0, 2, 16, 90);
            NPC.npcSlots = 1f;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit1; //Change me if you want (Rock hit sound)
            NPC.DeathSound = SoundID.NPCDeath1; //Change me if you want (Heavy grunt sound)
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
        Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Decurion/DecurionTail_Glow");
            Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
        }
        public override bool CheckActive()
        {
            return false;
        }
        public override bool PreAI()
        {
            var entitySource = NPC.GetSource_FromAI();
            NPC.chaseable = !NPC.AnyNPCs(Mod.Find<ModNPC>("WormProbeCircler").Type);
            NPC.alpha = Main.npc[(int)NPC.ai[1]].alpha;
            NPC.damage = Main.npc[(int)NPC.ai[1]].damage == 0 ? 0 : 90;
            NPC.localAI[0] += new Random().Next(4);
            if (NPC.localAI[0] >= (float)Main.rand.Next(1400, 9000))
            {
                int ball = Projectile.NewProjectile(entitySource, NPC.Center.X,
                    NPC.Center.Y,
                    new Random().Next(-1, 2) * 8,
                    new Random().Next(-1, 2) * 8,
                    ProjectileID.LaserMachinegunLaser,
                    40, 16, Main.myPlayer);
                Main.projectile[ball].friendly = false;
                Main.projectile[ball].hostile = true;

                NPC.localAI[0] = 0;

            }
            if (!Main.npc[(int)NPC.ai[1]].active)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.active = false;
            }
            Vector2 vector18 = new Vector2(NPC.position.X, NPC.position.Y);
            float num191 = Main.player[NPC.target].Center.X;
            float num192 = Main.player[NPC.target].Center.Y;
            num191 = (float)((int)(num191 / 16f) * 16);
            num192 = (float)((int)(num192 / 16f) * 16);
            vector18.X = (float)((int)(vector18.X / 16f) * 16);
            vector18.Y = (float)((int)(vector18.Y / 16f) * 16);
            num191 -= vector18.X;
            num192 -= vector18.Y;
            float num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
            if (NPC.ai[1] > 0f && NPC.ai[1] < (float)Main.npc.Length)
            {
                try
                {
                    vector18 = new Vector2(NPC.Center.X, NPC.Center.Y);
                    num191 = Main.npc[(int)NPC.ai[1]].Center.X - vector18.X;
                    num192 = Main.npc[(int)NPC.ai[1]].Center.Y - vector18.Y;
                }
                catch
                {
                }
                NPC.rotation = (float)System.Math.Atan2((double)num192, (double)num191) + 1.57f;
                num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
                int num194 = NPC.width / 2;
                num193 = (num193 - (float)num194) / num193;
                num191 *= num193;
                num192 *= num193;
                NPC.velocity = Vector2.Zero;
                NPC.position.X = NPC.position.X + num191;
                NPC.position.Y = NPC.position.Y + num192;
            }
            return false;
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.minion)
            {
                return;
            }
            damage /= (int)MathHelper.Clamp(projectile.penetrate, 1, 3);
            projectile.penetrate--;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
        SpriteEffects spriteEffects = 0;
            Color alpha = NPC.GetAlpha(drawColor);
            Color color = Lighting.GetColor((int)((double)NPC.position.X + (double)NPC.width * 0.5) / 16, (int)(((double)NPC.position.Y + (double)NPC.height * 0.5) / 16.0));
            Texture2D texture2D = (Texture2D)TextureAssets.Npc[NPC.type];
            int num = TextureAssets.Npc[NPC.type].Height() / Main.npcFrameCount[NPC.type];
            int num2 = num * (int)NPC.frameCounter;
            Rectangle rectangle = new Rectangle(0, num2, texture2D.Width, num);
            Vector2 vector = rectangle.Size() / 2f;
            int num3 = 8;
            int num4 = 1;
            int num5 = 1;
            float num6 = 0f;
            int num7 = num5;
            while (((num4 > 0 && num7 < num3) || (num4 < 0 && num7 > num3)) && Lighting.NotRetro)
            {
                Color color2 = NPC.GetAlpha(color);
                float num8 = (float)(num3 - num7);
                if (num4 < 0)
                {
                    num8 = (float)(num5 - num7);
                }
                color2 *= num8 / ((float)NPCID.Sets.TrailCacheLength[NPC.type] * 1.5f);
                Vector2 vector2 = NPC.oldPos[num7];
                float rotation = NPC.rotation;
                Main.EntitySpriteDraw(texture2D, vector2 + NPC.Size / 2f - Main.screenPosition + new Vector2(0f, NPC.gfxOffY), new Rectangle?(rectangle), color2, rotation + NPC.rotation * num6 * (float)(num7 - 1) * -(float)spriteEffects.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), vector, NPC.scale, spriteEffects, 0);
                num7 += num4;
            }
            SpriteEffects spriteEffects2 = (NPC.direction == -1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture2D, NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY), new Rectangle?(NPC.frame), alpha, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects2, 0);
            return false;
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;       //this make that the npc does not have a health bar
        }
    }
}