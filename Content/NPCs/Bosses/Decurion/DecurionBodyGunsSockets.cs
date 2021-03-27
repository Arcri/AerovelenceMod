using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.Decurion
{
    [AutoloadBossHead]
    public class DecurionBodyGunsSockets : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Decurion"); //DONT Change me
            Main.npcFrameCount[npc.type] = 3;
        }
        public override void BossHeadRotation(ref float rotation) => rotation = npc.rotation;
        public override void SetDefaults()
        {
            npc.lifeMax = 100;        //this is the npc health
            npc.damage = 90;    //this is the npc damage
            npc.defense = 35;         //this is the npc defense
            npc.knockBackResist = 0f;
            npc.width = 66; //this is where you put the npc sprite width.     important
            npc.height = 80; //this is where you put the npc sprite height.   important
            npc.lavaImmune = true;       //this make the npc immune to lava
            npc.noGravity = true;           //this make the npc float
            npc.noTileCollide = true;        //this make the npc go tru walls
            npc.behindTiles = true;
            Main.npcFrameCount[npc.type] = 1;
            npc.value = Item.buyPrice(0, 2, 16, 90);
            npc.npcSlots = 1f;
            npc.netAlways = true;
            npc.HitSound = SoundID.NPCHit1; //Change me if you want (Rock hit sound)
            npc.DeathSound = SoundID.NPCDeath1; //Change me if you want (Heavy grunt sound)
        }
        public override bool CheckActive()
        {
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = ModContent.GetTexture("AerovelenceMod/Content/NPCs/Bosses/Decurion/DecurionBodyGunsSockets_Glow");
            spriteBatch.Draw(texture, npc.Center - Main.screenPosition, npc.frame, Color.White, npc.rotation, npc.frame.Size() / 2f, npc.scale, SpriteEffects.None, 0);
        }
        public override bool PreAI()
        {
            npc.chaseable = !NPC.AnyNPCs(mod.NPCType("DecurionBodyGun1"));
            npc.chaseable = !NPC.AnyNPCs(mod.NPCType("DecurionBodyGun2"));
            if (npc.ai[1] == 0)
            {
                npc.frame.Y = Main.rand.Next(3);
                npc.netUpdate = true;
            }
            npc.alpha = Main.npc[(int)npc.ai[1]].alpha;
            npc.damage = Main.npc[(int)npc.ai[1]].damage == 0 ? 0 : 90;
            npc.localAI[0] += new Random().Next(4);
            if (npc.localAI[0] >= Main.rand.Next(3300, 12000))
            {
                int ball = Projectile.NewProjectile(npc.Center.X,
                    npc.Center.Y,
                    new Random().Next(-1, 2) * 8,
                    new Random().Next(-1, 2) * 8,
                    ProjectileID.LaserMachinegunLaser,
                    40, 16, Main.myPlayer);
                Main.projectile[ball].friendly = false;
                Main.projectile[ball].hostile = true;
                npc.localAI[0] = 0;
            }
            if (!Main.npc[(int)npc.ai[1]].active)
            {
                npc.life = 0;
                npc.HitEffect(0, 10.0);
                npc.active = false;
            }
            Vector2 vector18 = new Vector2(npc.position.X, npc.position.Y);
            float num191 = Main.player[npc.target].Center.X;
            float num192 = Main.player[npc.target].Center.Y;
            num191 = ((int)(num191 / 16f) * 16);
            num192 = ((int)(num192 / 16f) * 16);
            vector18.X = ((int)(vector18.X / 16f) * 16);
            vector18.Y = ((int)(vector18.Y / 16f) * 16);
            num191 -= vector18.X;
            num192 -= vector18.Y;
            float num193 = (float)Math.Sqrt((num191 * num191 + num192 * num192));
            if (npc.ai[1] > 0f && npc.ai[1] < Main.npc.Length)
            {
                try
                {
                    vector18 = new Vector2(npc.Center.X, npc.Center.Y);
                    num191 = Main.npc[(int)npc.ai[1]].Center.X - vector18.X;
                    num192 = Main.npc[(int)npc.ai[1]].Center.Y - vector18.Y;
                }
                catch
                {
                }
                npc.rotation = (float)Math.Atan2(num192, num191) + 1.57f;
                num193 = (float)Math.Sqrt((num191 * num191 + num192 * num192));
                int num194 = npc.width / 2;
                num193 = (num193 - num194) / num193;
                num191 *= num193;
                num192 *= num193;
                npc.velocity = Vector2.Zero;
                npc.position.X = npc.position.X + num191;
                npc.position.Y = npc.position.Y + num192;
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

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            SpriteEffects spriteEffects = 0;
            Color alpha = npc.GetAlpha(drawColor);
            Color color = Lighting.GetColor((int)(npc.position.X + npc.width * 0.5) / 16, (int)((npc.position.Y + npc.height * 0.5) / 16.0));
            Texture2D texture2D = Main.npcTexture[npc.type];
            int num = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type];
            int num2 = num * (int)npc.frameCounter;
            Rectangle rectangle = new Rectangle(0, num2, texture2D.Width, num);
            Vector2 vector = rectangle.Size() / 2f;
            int num3 = 8;
            int num4 = 1;
            int num5 = 1;
            float num6 = 0f;
            int num7 = num5;
            while (((num4 > 0 && num7 < num3) || (num4 < 0 && num7 > num3)) && Lighting.NotRetro)
            {
                Color color2 = npc.GetAlpha(color);
                float num8 = num3 - num7;
                if (num4 < 0)
                {
                    num8 = num5 - num7;
                }
                color2 *= num8 / (NPCID.Sets.TrailCacheLength[npc.type] * 1.5f);
                Vector2 vector2 = npc.oldPos[num7];
                float rotation = npc.rotation;
                Main.spriteBatch.Draw(texture2D, vector2 + npc.Size / 2f - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(rectangle), color2, rotation + npc.rotation * num6 * (num7 - 1) * -spriteEffects.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), vector, npc.scale, spriteEffects, 0f);
                num7 += num4;
            }
            SpriteEffects spriteEffects2 = (npc.direction == -1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture2D, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), alpha, npc.rotation, npc.frame.Size() / 2f, npc.scale, spriteEffects2, 0f);
            return false;
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false; //this make that the npc does not have a health bar
        }
    }
}