using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.Rimegeist
{
    [AutoloadBossHead]
    public class Rimegeist : ModNPC
    {
        public int t;
        public bool Phase2;
        public int progTimer2;
        public int progTimer1;
        public float someValue;
        public float someValue2;
        public int timerAttack;
        public int shootTimer;
        public int shootTimer2;
        public int shootTimer3;
        public int shootTimer4;
        public int dashTimer;
        public int dashTimer2;
        private int stobit;
        private int ebic;
        public float hhhh;

        private enum RimegeistState
        {
            IdleMovement = 0,
            RadialIcicles = 1,
            IceBlast = 2,
            IcicleRain = 3,
            IceBoltBlast = 4,
            Dash = 5,
            HomingVoidSouls = 6,
            VoidStone = 7,
            VortexOfRainbows = 8

        }

        /// <summary>
        /// Manages the current AI state of the Crystal Tumbler.
        /// Gets and sets npc.ai[0] as tracker.
        /// </summary>
        private RimegeistState State
        {
            get => (RimegeistState)npc.ai[0];
            set => npc.ai[0] = (float)value;
        }

        /// <summary>
        /// Manages several AI state attack timers.
        /// Gets and sets npc.ai[1] as tracker.
        /// </summary>
        private float AttackTimer
        {
            get => npc.ai[1];
            set => npc.ai[1] = value;
        }
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 8;    //boss frame/animation 
        }
        public override void SetDefaults()
        {
            npc.aiStyle = -1;  //5 is the flying AI
            npc.lifeMax = 9000;   //boss life
            npc.damage = 32;  //boss damage
            npc.defense = 24;    //boss defense
            npc.knockBackResist = 0f;
            npc.width = 246;
            npc.height = 300;
            npc.value = Item.buyPrice(0, 5, 75, 45);
            npc.npcSlots = 1f;
            npc.boss = true;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit5;
            npc.DeathSound = SoundID.NPCHit5;
            npc.buffImmune[24] = true;
            //bossBag = ModContent.ItemType<RimegeistBag>();
            music = mod.GetSoundSlot(Terraria.ModLoader.SoundType.Music, "Sounds/Music/Rimegeist");
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
        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            float alpha = 1f;
            Texture2D texture = ModContent.GetTexture("AerovelenceMod/Content/NPCs/Bosses/Rimegeist/Glowmask");
            spriteBatch.Draw(texture, npc.Center - Main.screenPosition, npc.frame, Color.White * alpha, npc.rotation, npc.frame.Size() / 2f, npc.scale, SpriteEffects.None, 0);
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 10000;  //boss life scale in expertmode
            npc.damage = 40;  //boss damage increase in expermode
        }

        int moveSpeed = 0;
        bool text = false;
        int moveSpeedY = 0;

        public bool doingDash = false;

        public override void AI()
        {
            npc.rotation = npc.velocity.X * 0.07f;
            if (npc.life <= npc.lifeMax / 2)
            {
                Phase2 = true;
            }
            npc.TargetClosest(true);
            Player target = Main.player[npc.target];
            Vector2 vector8 = new Vector2(npc.position.X + (npc.width * 0.5f), npc.position.Y + (npc.height * 0.5f));
            var player = Main.player[npc.target];
            Vector2 move = player.position - npc.Center;

            if (State == RimegeistState.IdleMovement)
            {
                int RandomPos;
                if (Main.rand.NextBool(2))
                {
                    RandomPos = -300; 
                }
                else
                {
                    RandomPos = 300;
                }
                Vector2 playerPos = player.position + new Vector2(0, RandomPos);
                {
                    float speed = 4.6f;
                    Vector2 moving = playerPos - npc.Center;
                    float magnitude = (float)Math.Sqrt(moving.X * moving.X + moving.Y * moving.Y);
                    if (magnitude > speed)
                    {
                        moving *= speed / magnitude;
                    }
                    float turnResistance = 5f;
                    moving = (npc.velocity * turnResistance + moving) / (turnResistance + 1f);
                    magnitude = (float)Math.Sqrt(moving.X * moving.X + moving.Y * moving.Y);
                    if (magnitude > speed)
                    {
                        moving *= speed / magnitude;
                    }
                    npc.velocity = moving;
                    Main.NewText("Idle Movement");
                    if (++AttackTimer >= 1)
                    {
                        AttackTimer = 0;
                        npc.netUpdate = true;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            State = (RimegeistState)Main.rand.Next(2, 9);
                        }
                    }
                }
            }


            else if (State == RimegeistState.RadialIcicles)
            {
                if (++AttackTimer <= 180)
                {
                    Main.NewText("Radial Icicles");

                    // follows player from the top and shoots a blast of icy spikes
                    npc.velocity.X = ((10 * npc.velocity.X + move.X) / 20f);
                    npc.velocity.Y = ((10 * npc.velocity.Y + move.Y - 300) / 20f);

                    int type = mod.ProjectileType("IcySpike");
                    int damage = Main.expertMode ? 5 : 2;// if u want to change this, 15 is for expert mode, 10 is for normal mod
                    float speedX = 10f;
                    float speedY = 10f;
                    Vector2 position = npc.Center;


                    shootTimer++;

                    if (shootTimer >= 90)
                    {
                        if (Main.netMode != 1)
                            for (int i = 0; i < 8; i++)
                            {
                                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(45 * i));
                                Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, 2f, Main.myPlayer);
                            }
                        shootTimer = 0;
                    }
                    if (AttackTimer == 180)
                    {
                        npc.netUpdate = true;

                        AttackTimer = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            State = (RimegeistState)Main.rand.Next(2, 9);
                        }
                    }
                }
            }
            else if (State == RimegeistState.IceBlast)
            {

                if (++AttackTimer <= 180)
                {
                    dashTimer = 0;
                    Vector2 playerPos = player.position + new Vector2(500, 0);
                    float speed = 3.2f;
                    Vector2 moving = playerPos - npc.Center;
                    float magnitude = (float)Math.Sqrt(moving.X * moving.X + moving.Y * moving.Y);
                    if (magnitude > speed)
                    {
                        moving *= speed / magnitude;
                    }
                    float turnResistance = 5f;
                    moving = (npc.velocity * turnResistance + moving) / (turnResistance + 1f);
                    magnitude = (float)Math.Sqrt(moving.X * moving.X + moving.Y * moving.Y);
                    if (magnitude > speed)
                    {
                        moving *= speed / magnitude;
                    }
                    npc.velocity = moving;
                    Main.NewText("Ice Blast");
                    stobit++;
                    if (stobit >= 40)
                    {
                        float Speed = 7f;
                        int damage = Main.expertMode ? 5 : 2;// if u want to change this, 15 is for expert mode, 10 is for normal mod
                        int type = mod.ProjectileType("IceBlast");
                        float rotation = (float)Math.Atan2(vector8.Y - (player.position.Y + (player.height * 0.5f)), vector8.X - (player.position.X + (player.width * 0.5f)));
                        if (Main.netMode != 1)
                            Projectile.NewProjectile(vector8.X, vector8.Y, (float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1), type, damage, 0f, Main.myPlayer);
                        stobit = 0;
                    }
                }
                if (AttackTimer == 180)
                {
                    npc.netUpdate = true;

                    AttackTimer = 0;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        State = (RimegeistState)Main.rand.Next(2, 9);
                    }
                }
            }
            else if (State == RimegeistState.IceBoltBlast)
            {
                if (++AttackTimer <= 180)
                {
                    Main.NewText("IceBoltBlast");
                    /*Vector2 playerPos = player.position + new Vector2(0, -300);
                    float speed = 5.1f;
                    Vector2 moving = playerPos - npc.Center;
                    float magnitude = (float)Math.Sqrt(moving.X * moving.X + moving.Y * moving.Y);
                    if (magnitude > speed)
                    {
                        moving *= speed / magnitude;
                    }
                    float turnResistance = 5f;
                    moving = (npc.velocity * turnResistance + moving) / (turnResistance + 1f);
                    magnitude = (float)Math.Sqrt(moving.X * moving.X + moving.Y * moving.Y);
                    if (magnitude > speed)
                    {
                        moving *= speed / magnitude;
                    }
                    npc.velocity = moving;*/
                    int damage = Main.expertMode ? 5 : 2;// if u want to change this, 15 is for expert mode, 10 is for normal mod
                    Vector2 position = npc.Center;
                    int type = mod.ProjectileType("IceBolt");
                    float rotate = (float)Math.Atan2(vector8.Y - (player.position.Y + (player.height * 0.5f)), vector8.X - (player.position.X + (player.width * 0.5f)));
                    shootTimer2++;
                    if (shootTimer2 >= 50)
                    {
                        float numberProjectiles = 6;
                        float rotation = MathHelper.ToRadians(10);
                        position += Vector2.Normalize(new Vector2((float)((Math.Cos(rotate) * 5f) * -1), (float)((Math.Sin(rotate) * 5f) * -1))) * 45f;
                        for (int i = 0; i < numberProjectiles; i++)
                        {
                            Vector2 perturbedSpeed = new Vector2((float)((Math.Cos(rotate) * 5f) * -1), (float)((Math.Sin(rotate) * 5f) * -1)).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 2f;
                            if (Main.netMode != 1)
                            {
                                float speedMult = 1f;
                                if (i == 1 || i == 3)
                                {
                                    speedMult = 1.25f;
                                }
                                else if (i == 2)
                                {
                                    speedMult = 1.5f;
                                }
                                Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X * speedMult, perturbedSpeed.Y * speedMult, type, damage, 2f, Main.myPlayer);
                            }
                            shootTimer2 = 0;

                        }
                    }
                }
                if (AttackTimer == 180)
                {
                    npc.netUpdate = true;

                    AttackTimer = 0;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        State = (RimegeistState)Main.rand.Next(2, 9);
                    }
                }
            }

            else if (State == RimegeistState.IcicleRain)
            {
                if (++AttackTimer <= 0)
                {
                    progTimer1++;
                    Main.NewText("IcicleRain");

                }
                if (AttackTimer == 1)
                {
                    npc.netUpdate = true;

                    AttackTimer = 0;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        State = (RimegeistState)Main.rand.Next(2, 9);
                    }
                }
            }
            else if (State == RimegeistState.VortexOfRainbows)
            {

                int RandomPos;
                if (Main.rand.NextBool(2))
                {
                    RandomPos = -300;
                }
                else
                {
                    RandomPos = 300;
                }
                Vector2 playerPos = player.position + new Vector2(0, RandomPos);
                float speed = 4f;
                Vector2 moving = playerPos - npc.Center;
                float magnitude = (float)Math.Sqrt(moving.X * moving.X + moving.Y * moving.Y);
                if (magnitude > speed)
                {
                    moving *= speed / magnitude;
                }
                float turnResistance = 5f;
                moving = (npc.velocity * turnResistance + moving) / (turnResistance + 1f);
                magnitude = (float)Math.Sqrt(moving.X * moving.X + moving.Y * moving.Y);
                if (magnitude > speed)
                {
                    moving *= speed / magnitude;
                }
                npc.velocity = moving;
                if (++AttackTimer <= 180)
                {
                    Main.NewText("Vortex Of Rainbows");

                    if (progTimer2 <= 60)
                    {
                        npc.velocity *= 0.9f;
                        int type = mod.ProjectileType("RainbowBlast");
                        if (Main.netMode != 1 && progTimer2 % 10 == 0)
                        {
                            Vector2 circular = new Vector2(0, -8).RotatedBy(MathHelper.ToRadians(progTimer2 * 6));
                            Projectile.NewProjectile(npc.Center, circular, type, npc.damage, 2f, Main.myPlayer, npc.target, progTimer1 * 18);
                        }
                    }
                    progTimer2++;
                }
                if (AttackTimer == 180)
                {
                    progTimer2 = 0;
                    npc.netUpdate = true;

                    AttackTimer = 0;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        State = RimegeistState.RadialIcicles;
                    }
                }
            }
            else if (State == RimegeistState.HomingVoidSouls)
            {
                npc.velocity *= 0.99f;
                Main.NewText("HomingVoidSouls");
                if (++AttackTimer <= 180)
                {
                    ebic++;
                    if (ebic >= 100)
                    {
                        float Speed = 7f;
                        int damage = Main.expertMode ? 5 : 2;// if u want to change this, 15 is for expert mode, 10 is for normal mod
                        int type = mod.ProjectileType("WispSouls");
                        int type2 = mod.ProjectileType("IceCube");
                        float rotation = (float)Math.Atan2(vector8.Y - (player.position.Y + (player.height * 0.5f)), vector8.X - (player.position.X + (player.width * 0.5f)));
                        Projectile.NewProjectile(vector8.X, vector8.Y, (float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1), type, damage, 0f, 0);
                        Projectile.NewProjectile(player.Center + new Vector2(0, -300), new Vector2(0, 0), type2, damage, 2f, player.whoAmI);
                        ebic = 0;
                    }
                }
                if (AttackTimer == 180)
                {
                    npc.netUpdate = true;

                    AttackTimer = 0;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        State = RimegeistState.Dash;
                    }
                }
            }


            else if (State == RimegeistState.Dash)
            {
                Main.NewText("Dash");
                if (++AttackTimer >= 0 && AttackTimer < 300)
                {
                    dashTimer++;
                    if (dashTimer >= 0 && dashTimer < 240)
                    {
                        int RandomPos;
                        if(Main.rand.NextBool(2))
                        {
                            RandomPos = 300;
                        }
                        else
                        {
                            RandomPos = -300;
                        }
                        npc.velocity.X = ((10 * npc.velocity.X + move.X + RandomPos) / 20f);
                        npc.velocity.Y = ((10 * npc.velocity.Y + move.Y) / 20f);
                        stobit++;
                        if (stobit >= 40)
                        {
                            float Speed = 7f;
                            int damage = Main.expertMode ? 5 : 2;// if u want to change this, 15 is for expert mode, 10 is for normal mod
                            int type = mod.ProjectileType("IceBlast");
                            float rotation = (float)Math.Atan2(vector8.Y - (player.position.Y + (player.height * 0.5f)), vector8.X - (player.position.X + (player.width * 0.5f)));
                            if (Main.netMode != 1)
                                Projectile.NewProjectile(vector8.X, vector8.Y, (float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1), type, damage, 0f, Main.myPlayer);
                            stobit = 0;
                        }
                    }
                    if (dashTimer >= 240)
                    {
                        Vector2 homeCenter = player.Center;
                        homeCenter.X += (npc.Center.X < player.Center.X) ? -280 : 280;
                        homeCenter.Y -= 30;

                        float vel = MathHelper.Clamp(npc.Distance(homeCenter) / 12, 8, 30);
                        npc.velocity = Vector2.Lerp(npc.velocity, npc.DirectionTo(homeCenter) * vel, 0.08f);
                    }
                }
                if (AttackTimer == 300)
                {
                    npc.netUpdate = true;

                    AttackTimer = 0;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        State = RimegeistState.RadialIcicles;
                    }
                }
            }
            else if (State == RimegeistState.VoidStone)
            {
                Main.NewText("Void Stone");
                if (++AttackTimer <= 80)
                {
                    npc.velocity *= 0.99f;
                    npc.velocity.Y -= 0.02f;
                    float numberProjectiles = 2;
                    float rotation = MathHelper.ToRadians(10);
                    float rotate = (float)Math.Atan2(vector8.Y - (player.position.Y + (player.height * 0.5f)), vector8.X - (player.position.X + (player.width * 0.5f)));

                    Vector2 position = npc.Center;
                    someValue += 3f;
                    shootTimer2++;
                    position += Vector2.Normalize(new Vector2((float)((Math.Cos(rotate) * 5f) * -1), (float)((Math.Sin(rotate) * 5f) * -1))) * 45f;
                    if (shootTimer2 > 40)
                    {
                        if (Main.netMode != 1)
                        {
                            Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0, 0, ModContent.ProjectileType<VoidStone>(), 15, 2f, Main.myPlayer, npc.target);

                        }
                        shootTimer2 = -30;
                    }
                }
                if (AttackTimer == 80)
                {
                    npc.netUpdate = true;

                    AttackTimer = 0;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        State = (RimegeistState)Main.rand.Next(2, 9);
                    }
                }
            }
        }
        void DashAttack(Player player)
        {
//supposed to go to left or right and then dash into the player.
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
            if (npc.life <= npc.lifeMax / 2)
            {
                int frameSpeed = 10;
                int frameC = (int)(npc.frameCounter / frameSpeed) % 7;
                npc.frame.Y = frameHeight * (frameC + 1);
            }
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
            return null;
        }
    }
}
namespace AerovelenceMod.Content.NPCs.Bosses.Rimegeist
{
    public class ShiverSpirit : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 3;
        }
        public override void SetDefaults()
        {
            npc.CloneDefaults(NPCID.IceElemental);
            npc.width = 46;
            npc.height = 31;
            npc.damage = 7;
            npc.defense = 6;
            npc.lifeMax = 30;
            npc.knockBackResist = 0.5f;
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
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }
    }
}

namespace AerovelenceMod.Content.NPCs.Bosses.Rimegeist
{
    public class RainbowBlast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rainbow Good");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 40;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            // Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width, Main.projectileTexture[projectile.type].Height);
            Texture2D texture2D = mod.GetTexture("Assets/Glow");
            Vector2 origin = new Vector2(texture2D.Width / 2, texture2D.Height / 2);
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                float scale = projectile.scale * (projectile.oldPos.Length - k) / projectile.oldPos.Length * 1.0f;
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + Main.projectileTexture[projectile.type].Size() / 3f;
                Color color = projectile.GetAlpha(fetchRainbow()) * ((projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
                for (int i = 0; i < 6; i++)
                {
                    if (i == 0)
                        spriteBatch.Draw(texture2D, drawPos, null, color, projectile.rotation, origin, scale, SpriteEffects.None, 0f);
                    spriteBatch.Draw(texture2D, drawPos + new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)), null, Color.White.MultiplyRGBA(color * 0.5f), projectile.rotation, origin, scale * 0.6f, SpriteEffects.None, 0f);
                }
            }
            return false;
        }
        public override void SetDefaults()
        {
            projectile.width = 68;
            projectile.height = 68;
            projectile.penetrate = -1;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.aiStyle = -1;
            projectile.scale = 0.8f;
            projectile.timeLeft = 720;
            projectile.extraUpdates = 1;
            projectile.tileCollide = false;
        }
        public Color fetchRainbow()
        {
            float sin1 = (float)Math.Sin(MathHelper.ToRadians(projectile.ai[1]));
            float sin2 = (float)Math.Sin(MathHelper.ToRadians(projectile.ai[1] + 120));
            float sin3 = (float)Math.Sin(MathHelper.ToRadians(projectile.ai[1] + 240));
            int middle = 180;
            int length = 75;
            float r = middle + length * sin1;
            float g = middle + length * sin2;
            float b = middle + length * sin3;
            Color color = new Color((int)r, (int)g, (int)b);
            return color;
        }
        int counter = 30;
        public override void AI()
        {
            counter++;
            projectile.velocity *= 0.955f + 0.000175f * counter;
            Player player = Main.player[(int)projectile.ai[0]];
            Vector2 toPlayer = player.Center - projectile.Center;
            Vector2 circular = new Vector2(1, 0).RotatedBy(MathHelper.ToRadians(projectile.ai[1] * 2));
            projectile.velocity += toPlayer.SafeNormalize(Vector2.Zero) * (counter * 0.0004f) + circular * 0.05f;
            projectile.ai[1] += 2f;
            Lighting.AddLight(new Vector2(projectile.Center.X, projectile.Center.Y), fetchRainbow().R / 255f, fetchRainbow().G / 255f, fetchRainbow().B / 255f);
            if (Main.rand.NextBool(10))
            {
                int num2 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 267);
                Dust dust = Main.dust[num2];
                Color color2 = new Color(110, 110, 110, 0).MultiplyRGBA(fetchRainbow());
                dust.color = color2;
                dust.noGravity = true;
                dust.fadeIn = 0.1f;
                dust.scale *= 2f;
                dust.alpha = 255 - (int)(255 * (projectile.timeLeft / 720f));
                dust.velocity *= 0.5f;
                dust.velocity += projectile.velocity * 0.4f;
            }
        }
    }
}
namespace AerovelenceMod.Content.NPCs.Bosses.Rimegeist
{
    public class IcySpike : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 52;
            projectile.height = 30;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();
            projectile.velocity.Y = projectile.velocity.Y + 0.15f;
            Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.AncientLight, 0f, 0f, 255);
            dust.noGravity = true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
                spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
    }
}

namespace AerovelenceMod.Content.NPCs.Bosses.Rimegeist
{
    public class IceCube : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 120;
        }
        public override void AI()
        {
            Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.AncientLight, 0f, 0f, 255);
            dust.noGravity = true;
        }
        public override void Kill(int timeLeft)
        {
            if (Main.netMode != 1)
            {
                int type = ProjectileID.FrostShard;
                float speed = 6f;
                int damage = 10;
                Vector2 position = projectile.Center;
                for (int i = 0; i < 8; i++)
                {
                    Vector2 perturbedSpeed = new Vector2(speed, speed).RotatedBy(MathHelper.ToRadians(i * 45));
                    Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, 2f, Main.myPlayer);
                }
            }
        }
    }
}
namespace AerovelenceMod.Content.NPCs.Bosses.Rimegeist
{
    public class IceBolt : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 12;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();
            Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.AncientLight, 0f, 0f, 255);
            dust.noGravity = true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
                spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = mod.GetTexture("Content/NPCs/Bosses/Rimegeist/IceBolt_Glowmask");
            Vector2 drawPos = projectile.Center + new Vector2(0, projectile.gfxOffY) - Main.screenPosition;
            //keep an eye on the width and height when doing this. It matters
            spriteBatch.Draw
            (
                texture,
                drawPos,
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                projectile.rotation,
                texture.Size() * 0.5f,
                projectile.scale,
                SpriteEffects.None, //adjust this according to the sprite
                0f
                );
        }
    }
}



namespace AerovelenceMod.Content.NPCs.Bosses.Rimegeist
{
    public class IceBlast : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.FrostBlastHostile;
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.FrostBlastHostile);
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            aiType = ProjectileID.FrostBlastHostile;
        }
    }
}

namespace AerovelenceMod.Content.NPCs.Bosses.Rimegeist
{
    public class WispSouls : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 25;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            // Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width, Main.projectileTexture[projectile.type].Height);
            Texture2D texture2D = mod.GetTexture("Assets/Glow");
            Vector2 origin = new Vector2(texture2D.Width / 2, texture2D.Height / 2);
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                float scale = projectile.scale * (projectile.oldPos.Length - k) / projectile.oldPos.Length * .45f;
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + Main.projectileTexture[projectile.type].Size() / 3f;
                Color color = projectile.GetAlpha(Color.Black) * ((projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
                spriteBatch.Draw(texture2D, drawPos, null, color, projectile.rotation, origin, scale, SpriteEffects.None, 0f);
            }
            return true;
        }
        public override void SetDefaults()
        {
            projectile.width = 17;
            projectile.height = 18;
            projectile.aiStyle = -1;
            projectile.tileCollide = false;
            projectile.ranged = projectile.friendly = true;
            projectile.timeLeft = 180;
        }
        int radians = 16;
        int Timer = 0;
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();
            projectile.velocity *= 1.03f;

            if (Timer < 60)
                projectile.velocity /= 1.03f;


            projectile.ai[0] += 1;

            if (projectile.ai[0] >= 8)
            {
                Vector2 perturbedSpeed = new Vector2(projectile.velocity.X, projectile.velocity.Y).RotatedByRandom(MathHelper.ToRadians(radians));
                if (radians >= 16)
                {
                    radians = -24;
                }
                if (radians <= -16)
                {
                    radians = 16;
                }
                float scale = 1f - (Main.rand.NextFloat() * .3f);
                perturbedSpeed = perturbedSpeed * scale;
                projectile.velocity.Y = perturbedSpeed.Y;
                projectile.velocity.X = perturbedSpeed.X;
                projectile.ai[0] = 0;
            }
        }
    }
}
namespace AerovelenceMod.Content.NPCs.Bosses.Rimegeist
{
    public class VoidStone : ModProjectile
    {

        float rotationCounter = 0;
        float length = 34;
        bool runOnce = true;
        float randomModifier1 = 0;
        float randomModifier2 = 0;

        public override void SetDefaults()
        {
            projectile.width = 76;
            projectile.height = 60;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 120;
        }
        public override void AI()
        {
            Player player = Main.player[(int)projectile.ai[0]];
            if (!player.active || player.dead)
            {
                projectile.Kill();
                return;
            }
            projectile.velocity *= 0.985f;
            Vector2 direction = projectile.DirectionTo(player.Center);
            projectile.velocity += direction * 0.02f;
            projectile.velocity.Y += 0.09f;
            if (runOnce)
            {
                Main.PlaySound(2, (int)projectile.Center.X, (int)projectile.Center.Y, 30, 0.75f, -0.4f);
                randomModifier1 = Main.rand.NextFloat(-1f, 1.75f);
                randomModifier2 = Main.rand.NextFloat(-24, 24);
                rotationCounter = Main.rand.NextFloat(360);
                runOnce = false;
                if (Main.myPlayer == player.whoAmI)
                    projectile.netUpdate = true;
            }
            if (Main.rand.NextBool(25) && rotationCounter > 10)
            {
                int num1 = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y) - new Vector2(5), 0, 0, DustID.RainbowMk2);
                Dust dust = Main.dust[num1];
                dust.velocity *= 0.7f;
                dust.noGravity = true;
                dust.color = new Color(130, 130, 130, 0);
                dust.fadeIn = 0.2f;
                dust.scale = 1.2f;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 origin = new Vector2(texture.Width / 2, projectile.height / 2);
            Color color = new Color(130, 130, 150, 0);
            for (int i = 0; i < 360; i += 60)
            {
                Vector2 circular = new Vector2(Main.rand.NextFloat(1.5f, 2.5f), 0).RotatedBy(MathHelper.ToRadians(i * 2.5f));
                Main.spriteBatch.Draw(texture, projectile.Center + circular - Main.screenPosition, null, color * ((255f - projectile.alpha) / 255f), projectile.rotation, origin, projectile.scale * 0.8f, SpriteEffects.None, 0.0f);
            }
            //color = projectile.GetAlpha(Color.White);
            //Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, color, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0.0f);
            return false;
        }
        public override void Kill(int timeLeft)
        {
            if (Main.netMode != 1)
            {
                int type = ModContent.ProjectileType<WispSouls>();
                float speed = 6f;
                int damage = 10;
                Vector2 position = projectile.Center;
                for (int i = 0; i < 8; i++)
                {
                    Vector2 perturbedSpeed = new Vector2(speed, speed).RotatedBy(MathHelper.ToRadians(i * 45));
                    Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, 2f, Main.myPlayer);
                }
            }
        }
    }
}