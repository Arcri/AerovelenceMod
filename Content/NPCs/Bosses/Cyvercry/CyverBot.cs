using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry
{
    public class CyverBot : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cyver Bot");
            Main.npcFrameCount[NPC.type] = 4;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 540;
            NPC.damage = 40;
            NPC.defense = 12;
            NPC.width = 66;
            NPC.height = 40;
            NPC.boss = false;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.value = Item.buyPrice(0, 0, 24, 48);
            NPC.alpha = 255;
            NPC.scale = 1;
            NPC.knockBackResist = 0f;
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = 720;
            NPC.damage = 20;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return !NPC.dontTakeDamage;
        }
        float dynamicCounter = 0;
        public override void AI()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.netUpdate = true;
            dynamicCounter++;
            NPC.spriteDirection = -1;
            if(NPC.alpha > 0)
            {
                NPC.dontTakeDamage = true;
                NPC.alpha -= 2;
            }
            else
            {
                NPC.dontTakeDamage = false;
                NPC.alpha = 0;
            }
            int type = DustID.Electric;
            if (NPC.ai[2] == -1)
                type = 235;
            Vector2 from = NPC.Center + new Vector2(18, 0).RotatedBy(NPC.rotation);
            int dust = Dust.NewDust(from + new Vector2(-4, -4), 0, 0, type, 0, 0, NPC.alpha, default, 1.25f);
            Vector2 dustVelo = new Vector2(3, 0).RotatedBy(NPC.rotation);
            Main.dust[dust].velocity += dustVelo;
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity *= 0.2f;
            Main.dust[dust].scale *= 0.5f * (0.5f + 0.75f * (255 - NPC.alpha) / 255f);
            if (type == 235)
                Main.dust[dust].scale *= 2;
            NPC.ai[1]++;
            if(NPC.ai[0] < 0)
            {
                NPC.active = false;
            }
            Player player = Main.player[(int)NPC.ai[0]];
            if(!player.active || player.dead)
            {
                NPC.active = false;
            }
            bool found = false;
            int total = 0;
            int ofTotal = 0;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC checkNpc = Main.npc[i];
                if (NPC.type == checkNpc.type && checkNpc.active && NPC.active && checkNpc.ai[0] == NPC.ai[0])
                {
                    if(total == 0)
                    {
                        NPC.ai[1] = checkNpc.ai[1];
                    }
                    if (checkNpc == NPC)
                    {
                        found = true;
                    }
                    if (!found)
                        ofTotal++;
                    total++;
                }
            }
            if ((int)NPC.ai[1] % 300 == (int)(ofTotal * 300f / total % 300) && !NPC.dontTakeDamage)
            {
                FireLaser(ProjectileID.RayGunnerLaser, 1.5f, 8);
            }
            Vector2 dynamicAddition = new Vector2(48, 0).RotatedBy(MathHelper.ToRadians(dynamicCounter * 3));
            float rotationSpeed = .75f;
            Vector2 properLoop = new Vector2(540 + dynamicAddition.X, 0).RotatedBy(MathHelper.ToRadians(NPC.ai[1] * rotationSpeed + (ofTotal * 360f / total)));
            properLoop.Y *= 0.75f;
            Vector2 toPos = properLoop + player.Center;
            toPos -= NPC.Center;
            float dist = toPos.Length();
            toPos = toPos.SafeNormalize(Vector2.Zero);
            float speed = 3 + 15f * (255 - NPC.alpha) / 255f;
            if (speed > dist)
                speed = dist;
            NPC.velocity *= 0.7f;
            NPC.velocity += 0.35f * toPos * speed;
            NPC.rotation = (player.Center - NPC.Center).ToRotation() + MathHelper.ToRadians(180);
        }
        public void FireLaser(int type, float speed = 6f, float recoilMult = 2f, float ai1 = 0, float ai2 = 0)
        {

            var entitySource = NPC.GetSource_FromAI();


            Player player = Main.player[NPC.target];
            Vector2 toPlayer = player.Center - NPC.Center;
            toPlayer = toPlayer.SafeNormalize(new Vector2(1, 0));
            toPlayer *= speed;
            Vector2 from = NPC.Center - new Vector2(96, 0).RotatedBy(NPC.rotation);
            int damage = 50;
            /*if (Main.expertMode)
            {
                damage = (int)(damage / Main.expertDamage);
            }*/
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(entitySource, from, toPlayer, type, damage, 3, Main.myPlayer, ai1, ai2);
            }
            NPC.velocity -= toPlayer * recoilMult;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
        Texture2D primaryTexture = (Texture2D)TextureAssets.Npc[NPC.type];
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/CyverBotRed");
            Vector2 drawOrigin = new Vector2(NPC.width * 0.5f, NPC.height * 0.5f);
            Color color2 = new Color(100, 100, 100, 0);
            if (NPC.ai[2] == -1)
                for (int i = 0; i < 360; i += 30)
                {
                    Vector2 rotationalPos = new Vector2(Main.rand.NextFloat(2, 3), 0).RotatedBy(MathHelper.ToRadians(i));
                    Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition + rotationalPos, NPC.frame, color2 * ((255f - NPC.alpha) / 255f) * ((255f - NPC.alpha) / 255f), NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
                }
            Main.EntitySpriteDraw(primaryTexture, NPC.Center - Main.screenPosition, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
        Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/GlowmaskBot");
            Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 10)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 5;
                if (NPC.frame.Y > 3 * frameHeight)
                {
                    NPC.frame.Y = 0;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y == 2 * frameHeight) //booster frame
                {
                    int type = DustID.Electric;
                    if(NPC.ai[2] == -1)
                        type = 235;
                    Vector2 from = NPC.Center + new Vector2(24 + (type == DustID.Electric ? 12 : 0), 0).RotatedBy(NPC.rotation);
                    if (NPC.alpha == 0)
                        for (int i = 0; i < 360; i += 40)
                        {
                            Vector2 circular = new Vector2(20, 0).RotatedBy(MathHelper.ToRadians(i));
                            circular.Y *= 0.6f;
                            circular = circular.RotatedBy(NPC.rotation);
                            Vector2 dustVelo = new Vector2(4, 0).RotatedBy(NPC.rotation);
                            Dust dust = Dust.NewDustDirect(from - new Vector2(5) + circular, 0, 0, type, 0, 0, NPC.alpha);
                            dust.velocity *= 0.1f;
                            dust.velocity += dustVelo;
                            dust.noGravity = true;
                            if (type == 235)
                                dust.scale *= 2.25f;
                            dust.alpha = NPC.alpha;
                        }
                }
            }
        }
    }
}