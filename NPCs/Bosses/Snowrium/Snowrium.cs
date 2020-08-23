using AerovelenceMod.Items.BossBags;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.NPCs.Bosses.Snowrium
{
    [AutoloadBossHead]
    public class Snowrium : ModNPC
    {
        public int timerPhase1;
        public bool PrePhase2;
        private bool Phase2;
        private int phase1attack1;
        private float rotation;
        private int phase1attack2;
        private int phase1attack3;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 13;    //boss frame/animation 
        }
        public override void SetDefaults()
        {
            npc.aiStyle = -1;  //5 is the flying AI
            npc.lifeMax = 9500;   //boss life
            npc.damage = 32;  //boss damage
            npc.defense = 9;    //boss defense
            npc.alpha = 0;
            npc.knockBackResist = 0f;
            npc.width = 188;
            npc.height = 182;
            npc.value = Item.buyPrice(0, 5, 75, 45);
            npc.npcSlots = 1f;
            npc.boss = true;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit5;
            npc.DeathSound = SoundID.NPCHit5;
            npc.buffImmune[24] = true;
            bossBag = ModContent.ItemType<SnowriumBag>();
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Snowrium");
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = mod.GetTexture("NPCs/Bosses/Snowrium/Glowmask");
            spriteBatch.Draw(texture, npc.Center - Main.screenPosition, npc.frame, Color.White, npc.rotation, npc.frame.Size() / 2f, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            Vector2 drawOrigin = new Vector2(Main.npcTexture[npc.type].Width * 0.5f, npc.height * 0.5f);
            for (int k = 0; k < npc.oldPos.Length; k++)
            {
                Vector2 drawPos = npc.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, npc.gfxOffY);
                Color color = npc.GetAlpha(lightColor) * ((float)(npc.oldPos.Length - k) / (float)npc.oldPos.Length);
                spriteBatch.Draw(Main.npcTexture[npc.type], drawPos, npc.frame, color, npc.rotation, drawOrigin, npc.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
        public override void NPCLoot()
        {
            if (Main.expertMode)
            {
                npc.DropBossBags();
            }
            if (!Main.expertMode)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.HealingPotion, Main.rand.Next(4, 12), false, 0, false, false);
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("FrostShard"), Main.rand.Next(10, 20), false, 0, false, false);
                switch (Main.rand.Next(5))
                {
                    case 0:
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CrystalArch"), 1, false, 0, false, false);
                        break;
                    case 1:
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DeepFreeze"), 1, false, 0, false, false);
                        break;
                    case 2:
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("IcySaber"), 1, false, 0, false, false);
                        break;
                    case 3:
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CryoBall"), 1, false, 0, false, false);
                        break;
                    case 4:
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Snowball"), 1, false, 0, false, false);
                        break;
                }
            }
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 11500;
            npc.damage = 40;
        }
        public override void AI()
        {
            var player = Main.player[npc.target];
            if (player.dead || !player.active)
            {
                npc.noTileCollide = true;
                npc.TargetClosest(false);
                npc.velocity.Y = -20f;
                if (npc.timeLeft > 10)
                {
                    npc.timeLeft = 10;
                }
            }
            if (!PrePhase2)
            {
                if (npc.life <= npc.lifeMax / 2)
                {
                    PrePhase2 = true;
                }
            }
            if (!PrePhase2 && !Phase2)
            {
                timerPhase1++;
                if (timerPhase1 >= 0 && timerPhase1 < 400)
                {
                    phase1attack1++;
                    PlayerFollow();
                    if (phase1attack1 >= 60)
                    {
                        float Speed = 10f;
                        int damage = Main.expertMode ? 15 : 10;// if u want to change this, 15 is for expert mode, 10 is for normal mod
                        int type = mod.ProjectileType("IceBlast");
                        float rotation = (float)Math.Atan2(npc.Center.Y - (player.position.Y + (player.height * 0.5f)), npc.Center.X - (player.position.X + (player.width * 0.5f)));
                        Projectile.NewProjectile(npc.Center.X, npc.Center.Y, (float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1), type, damage, 0f, 0);
                        phase1attack1 = 0;

                    }
                }
                if (timerPhase1 >= 400 && timerPhase1 < 700)
                {
                    phase1attack1++;
                    if (phase1attack1 >= 0)
                    {
                        float Speed = 7f;
                        int damage = Main.expertMode ? 15 : 10;// if u want to change this, 15 is for expert mode, 10 is for normal mod
                        int type = mod.ProjectileType("IceBlast");
                        float rotation = (float)Math.Atan2(npc.Center.Y - (player.position.Y + (player.height * 0.5f)), npc.Center.X - (player.position.X + (player.width * 0.5f)));
                        Projectile.NewProjectile(npc.Center.X, npc.Center.Y, (float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1), type, damage, 0f, 0);
                        phase1attack1 = -30;
                    }
                    rotation /= 5f;
                    npc.velocity = (new Vector2(20, 20)).RotatedBy(MathHelper.ToRadians(rotation));
                }
                if (timerPhase1 >= 700 && timerPhase1 < 1000)
                {
                    Vector2 move = player.position - npc.Center;
                    npc.velocity.X = ((10 * npc.velocity.X + move.X) / 20f);
                    npc.velocity.Y = ((10 * npc.velocity.Y + move.Y - 300) / 20f);
                    int type = mod.ProjectileType("IcySpike");
                    int damage = Main.expertMode ? 15 : 10;// if u want to change this, 15 is for expert mode, 10 is for normal mod
                    float speedX = 10f;
                    float speedY = 10f;
                    Vector2 position = npc.Center;

                    phase1attack3++;
                    if (phase1attack3 >= 90)
                    {
                        Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(0));
                        Vector2 perturbedSpeed1 = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(45));
                        Vector2 perturbedSpeed2 = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(90));
                        Vector2 perturbedSpeed3 = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(135));
                        Vector2 perturbedSpeed4 = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(180));
                        Vector2 perturbedSpeed5 = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(225));
                        Vector2 perturbedSpeed6 = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(270));
                        Vector2 perturbedSpeed7 = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(315));
                        Vector2 perturbedSpeed8 = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(360));
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, 2f, player.whoAmI);
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed1.X, perturbedSpeed1.Y, type, damage, 2f, player.whoAmI);
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed2.X, perturbedSpeed2.Y, type, damage, 2f, player.whoAmI);
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed3.X, perturbedSpeed3.Y, type, damage, 2f, player.whoAmI);
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed4.X, perturbedSpeed4.Y, type, damage, 2f, player.whoAmI);
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed5.X, perturbedSpeed5.Y, type, damage, 2f, player.whoAmI);
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed6.X, perturbedSpeed6.Y, type, damage, 2f, player.whoAmI);
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed7.X, perturbedSpeed7.Y, type, damage, 2f, player.whoAmI);
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed8.X, perturbedSpeed8.Y, type, damage, 2f, player.whoAmI);
                        phase1attack3 = 0;
                    }






                }
                if (timerPhase1 >= 1000 && timerPhase1 < 1600)
                {
                    PlayerFollow();
                    phase1attack1++;
                    if (phase1attack1 >= 0)
                    {
                        float Speed = 7f;
                        int damage = Main.expertMode ? 15 : 10;// if u want to change this, 15 is for expert mode, 10 is for normal mod
                        int type = mod.ProjectileType("IceBlast");
                        float rotation = (float)Math.Atan2(npc.Center.Y - (player.position.Y + (player.height * 0.5f)), npc.Center.X - (player.position.X + (player.width * 0.5f)));
                        Projectile.NewProjectile(npc.Center.X, npc.Center.Y, (float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1), type, damage, 0f, 0);
                        phase1attack1 = -90;
                    }
                    phase1attack2++;
                    if (phase1attack2 >= 120 && phase1attack2 < 240)
                    {
                        npc.velocity.X = 0;
                        npc.velocity.Y = 0;
                        for (int i = 0; i < 20; i++)
                        {
                            Dust d = Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2CircularEdge(200, 200), DustID.Ice);
                            d.scale = 1.5f;
                            d.noGravity = true;
                        }
                    }
                    if (phase1attack2 >= 240)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            int dustType = DustID.Ice;
                            int dustIndex = Dust.NewDust(npc.Center, npc.width, npc.height, dustType);
                            Dust dust = Main.dust[dustIndex];
                            dust.velocity.X = dust.velocity.X + Main.rand.Next(-50, 51) * 0.01f;
                            dust.velocity.Y = dust.velocity.Y + Main.rand.Next(-50, 51) * 0.01f;
                            dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                            dust.noGravity = false;
                        }
                        Vector2 position = npc.Center;
                        float speedX = 10f;
                        float speedY = 10f;
                        int damage = Main.expertMode ? 15 : 10;
                        int type = mod.ProjectileType("IcySpike");
                        int type2 = mod.ProjectileType("IceBlast");
                        Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(0));
                        Vector2 perturbedSpeed1 = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(45));
                        Vector2 perturbedSpeed2 = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(90));
                        Vector2 perturbedSpeed3 = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(135));
                        Vector2 perturbedSpeed4 = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(180));
                        Vector2 perturbedSpeed5 = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(225));
                        Vector2 perturbedSpeed6 = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(270));
                        Vector2 perturbedSpeed7 = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(315));
                        Vector2 perturbedSpeed8 = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(360));
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, 2f, player.whoAmI);
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed1.X, perturbedSpeed1.Y, type, damage, 2f, player.whoAmI);
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed2.X, perturbedSpeed2.Y, type, damage, 2f, player.whoAmI);
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed3.X, perturbedSpeed3.Y, type, damage, 2f, player.whoAmI);
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed4.X, perturbedSpeed4.Y, type, damage, 2f, player.whoAmI);
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed5.X, perturbedSpeed5.Y, type, damage, 2f, player.whoAmI);
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed6.X, perturbedSpeed6.Y, type, damage, 2f, player.whoAmI);
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed7.X, perturbedSpeed7.Y, type, damage, 2f, player.whoAmI);
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed8.X, perturbedSpeed8.Y, type, damage, 2f, player.whoAmI);
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type2, damage, 2f, player.whoAmI);
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed1.X, perturbedSpeed1.Y, type2, damage, 2f, player.whoAmI);
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed2.X, perturbedSpeed2.Y, type2, damage, 2f, player.whoAmI);
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed3.X, perturbedSpeed3.Y, type2, damage, 2f, player.whoAmI);
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed4.X, perturbedSpeed4.Y, type2, damage, 2f, player.whoAmI);
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed5.X, perturbedSpeed5.Y, type2, damage, 2f, player.whoAmI);
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed6.X, perturbedSpeed6.Y, type2, damage, 2f, player.whoAmI);
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed7.X, perturbedSpeed7.Y, type2, damage, 2f, player.whoAmI);
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed8.X, perturbedSpeed8.Y, type2, damage, 2f, player.whoAmI);
                        phase1attack2 = 0;




                    }

                }
                if (timerPhase1 >= 1600)
                {
                    timerPhase1 = 0;
                }
            }
            if (PrePhase2 && !Phase2)
            {
                if (NPC.AnyNPCs(mod.NPCType("IceShield")))
                {
                    int damage = Main.expertMode ? 15 : 10;
                    int proj = mod.ProjectileType("IcySpikeNoGrav");
                    npc.velocity = new Vector2(0, 0);
                    phase1attack1++;
                    if (phase1attack1 >= 0)
                    {
                        float Speed = 7f;
                        int type = mod.ProjectileType("IceBlast");
                        float rotation = (float)Math.Atan2(npc.Center.Y - (player.position.Y + (player.height * 0.5f)), npc.Center.X - (player.position.X + (player.width * 0.5f)));
                        Projectile.NewProjectile(npc.Center.X, npc.Center.Y, (float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1), type, damage, 0f, 0);
                        phase1attack1 = -60;
                    }
                    npc.ai[0]++;
                    if (npc.ai[0] >= 60)
                    {
                        if (Main.rand.Next(2) == 0)
                        {
                            Projectile.NewProjectile(player.position.X - 1000, player.position.Y + Main.rand.Next(-100, 100), 4, 0, proj, damage, 0f, Main.myPlayer, 0f, 0f);
                            npc.ai[0] = 0;

                        }
                        else
                        {
                            Projectile.NewProjectile(player.position.X + 1000, player.position.Y + Main.rand.Next(-100, 100), -4, 0, proj, damage, 0f, Main.myPlayer, 0f, 0f);
                            npc.ai[0] = 0;
                        }
                    }
                }
            }
        }


        private void PlayerFollow()
        {
            // myes new player follow code, less boring and not plain like before
            npc.noGravity = true;
            npc.spriteDirection = npc.direction;
            if (npc.ai[0] == 0f)
            {
                npc.noGravity = false;
                npc.TargetClosest(true);
                if (Main.netMode != 1)
                {
                    if (npc.velocity.X == 0f && !(npc.velocity.Y < 0f) && !((double)npc.velocity.Y > 0.3))
                    {
                        Rectangle rectangle10 = new Rectangle((int)Main.player[npc.target].position.X, (int)Main.player[npc.target].position.Y, Main.player[npc.target].width, Main.player[npc.target].height);
                        if (new Rectangle((int)npc.position.X - 100, (int)npc.position.Y - 100, npc.width + 200, npc.height + 200).Intersects(rectangle10) || npc.life < npc.lifeMax)
                        {
                            npc.ai[0] = 1f;
                            npc.velocity.Y = npc.velocity.Y - 6f;
                            npc.netUpdate = true;
                        }
                    }
                    else
                    {
                        npc.ai[0] = 1f;
                        npc.netUpdate = true;
                    }
                }
            }
            else if (!Main.player[npc.target].dead)
            {

                npc.TargetClosest(true);
                if (npc.direction == -1 && npc.velocity.X > -3f)
                {
                    npc.velocity.X = npc.velocity.X - 0.1f;
                    if (npc.velocity.X > 3f)
                    {
                        npc.velocity.X = npc.velocity.X - 0.1f;
                    }
                    else if (npc.velocity.X > 0f)
                    {
                        npc.velocity.X = npc.velocity.X - 0.05f;
                    }
                    if (npc.velocity.X < -3f)
                    {
                        npc.velocity.X = -3f;
                    }
                }
                else if (npc.direction == 1 && npc.velocity.X < 3f)
                {
                    npc.velocity.X = npc.velocity.X + 0.1f;
                    if (npc.velocity.X < -3f)
                    {
                        npc.velocity.X = npc.velocity.X + 0.1f;
                    }
                    else if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X = npc.velocity.X + 0.05f;
                    }
                    if (npc.velocity.X > 3f)
                    {
                        npc.velocity.X = 3f;
                    }
                }
                float num3225 = Math.Abs(npc.position.X + (float)(npc.width / 2) - (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2)));
                float num3224 = Main.player[npc.target].position.Y - (float)(npc.height / 2);
                if (num3225 > 50f)
                {
                    num3224 -= 100f;
                }
                if (npc.position.Y < num3224)
                {
                    npc.velocity.Y = npc.velocity.Y + 0.05f;
                    if (npc.velocity.Y < 0f)
                    {
                        npc.velocity.Y = npc.velocity.Y + 0.01f;
                    }
                }
                else
                {
                    npc.velocity.Y = npc.velocity.Y - 0.05f;
                    if (npc.velocity.Y > 0f)
                    {
                        npc.velocity.Y = npc.velocity.Y - 0.01f;
                    }
                }
                if (npc.velocity.Y < -3f)
                {
                    npc.velocity.Y = -3f;
                }
                if (npc.velocity.Y > 3f)
                {
                    npc.velocity.Y = 3f;
                }
            }
            if (npc.wet)
            {
                if (npc.velocity.Y > 0f)
                {
                    npc.velocity.Y = npc.velocity.Y * 0.95f;
                }
                npc.velocity.Y = npc.velocity.Y - 0.5f;
                if (npc.velocity.Y < -4f)
                {
                    npc.velocity.Y = -4f;
                }
                npc.TargetClosest(true);
            }
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            for (int i = 0; i < 10; i++)
            {
                int dustType = DustID.Ice;
                int dustIndex = Dust.NewDust(npc.position, npc.width, npc.height, dustType);
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X = dust.velocity.X + Main.rand.Next(-50, 51) * 0.01f;
                dust.velocity.Y = dust.velocity.Y + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            {
                if (npc.frameCounter < 5)
                {
                    npc.frame.Y = 0 * frameHeight;
                }
                else if (npc.frameCounter < 10)
                {
                    npc.frame.Y = 1 * frameHeight;
                }
                else if (npc.frameCounter < 15)
                {
                    npc.frame.Y = 2 * frameHeight;
                }
                else if (npc.frameCounter < 20)
                {
                    npc.frame.Y = 3 * frameHeight;
                }
                else if (npc.frameCounter < 25)
                {
                    npc.frame.Y = 4 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
            if (npc.life <= npc.lifeMax / 2)
            {
                if (npc.frameCounter < 5)
                {
                    npc.frame.Y = 5 * frameHeight;
                }
                else if (npc.frameCounter < 10)
                {
                    npc.frame.Y = 6 * frameHeight;
                }
                else if (npc.frameCounter < 15)
                {
                    npc.frame.Y = 7 * frameHeight;
                }
                else if (npc.frameCounter < 20)
                {
                    npc.frame.Y = 8 * frameHeight;
                }
                else if (npc.frameCounter < 25)
                {
                    npc.frame.Y = 9 * frameHeight;
                }
                else if (npc.frameCounter < 30)
                {
                    npc.frame.Y = 10 * frameHeight;
                }
                else if (npc.frameCounter < 35)
                {
                    npc.frame.Y = 11 * frameHeight;
                }
                else if (npc.frameCounter < 40)
                {
                    npc.frame.Y = 12 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
            return null;
        }
    }
}
