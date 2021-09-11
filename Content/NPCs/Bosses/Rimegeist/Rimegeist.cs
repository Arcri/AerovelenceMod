using AerovelenceMod.Content.Items.TreasureBags;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using On.Terraria.GameContent.Events;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

//namespace AerovelenceMod.Content.NPCs.Bosses.Rimegeist
namespace AerovelenceMod.Content.NPCs.Bosses.Rimegeist
{
    [AutoloadBossHead]
    public class Rimegeist : ModNPC
    {
        public int t;
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
        private float whiteIn = 0f;
        private int stobit;
        private int ebic;
        public float hhhh;
        public static Rimegeist rimegeist;
        public const string AssetDirectory = "AerovelenceMod/Content/NPCs/Bosses/Rimegeist/";//AerovelenceMod/Content/NPCs/Bosses/Rimegeist/

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
            VortexOfRainbows = 8,
            IceBulletHell = 9,
            PhaseTwoTransition = 10,
            ShadowDash = 11,
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

        /// <summary>
        /// Manages the phase 2
        /// Gets and sets npc.ai[2] as tracker.
        /// </summary>
        public float PhaseTwo
        {
            get => npc.ai[2];
            set => npc.ai[2] = value;
        }


        /// <summary>
        /// Changes the npc state and attack timer
        /// Allows easier code injection later on to interupt change states
        /// </summary>
        public void ChangeState(int stateIndex, int attackTimer = 0)
        {
            State = (RimegeistState)stateIndex;
            if (PhaseTwo < 1 && npc.life < npc.lifeMax / 2)
                State = RimegeistState.PhaseTwoTransition;

            if (PhaseTwo > 2000)
            {
                State = RimegeistState.ShadowDash;
                PhaseTwo = Main.rand.Next(5, 200);

            }

            this.AttackTimer = attackTimer;

        }

        public override bool Autoload(ref string name)
        {
            ScreenObstruction.Draw += DrawOverBlackout;
            return true;
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 8;    //boss frame/animation 
        }
        public override void SetDefaults()
        {
            npc.aiStyle = -1;  //5 is the flying AI
            npc.lifeMax = 10000;   //boss life
            npc.damage = 40;  //boss damage
            npc.defense = 35;    //boss defense
            npc.knockBackResist = 0f;
            npc.width = 220;
            npc.height = 260;
            npc.value = Item.buyPrice(0, 5, 75, 45);
            npc.npcSlots = 1f;
            npc.boss = true;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit5;
            npc.DeathSound = SoundID.NPCHit5;
            npc.buffImmune[24] = true;
            bossBag = ModContent.ItemType<RimegeistBag>();
            music = mod.GetSoundSlot(Terraria.ModLoader.SoundType.Music, "Sounds/Music/Rimegeist");
        }
        public override void NPCLoot()
        {
            if (Main.expertMode)
            {
                npc.DropBossBags();
            }
            else
            {
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
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return npc.localAI[2] > -30f && State != RimegeistState.ShadowDash ? false : base.CanHitPlayer(target, ref cooldownSlot);
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 12000;  //boss life scale in expertmode
            npc.damage = 30;  //boss damage increase in expermode
            npc.defense = 40;
        }

        int moveSpeed = 0;
        bool text = false;
        int moveSpeedY = 0;

        public bool doingDash = false;

        public override void AI()
        {


            npc.dontTakeDamage = npc.localAI[2] > 0f && State != RimegeistState.ShadowDash;
            npc.localAI[0] = npc.velocity.X * 0.025f;
            npc.rotation += (npc.localAI[0] - npc.rotation) * 0.05f;

            npc.localAI[2]--;
            if (npc.localAI[2] < 1)
                npc.localAI[3] *= 0.95f;

            rimegeist = this;

            if (PhaseTwo > 0 && State != RimegeistState.PhaseTwoTransition)
            {
                PhaseTwo += 1;
            }

            npc.TargetClosest(true);
            Player target = Main.player[npc.target];
            Vector2 vector8 = new Vector2(npc.position.X + (npc.width * 0.5f), npc.position.Y + (npc.height * 0.5f));
            var player = Main.player[npc.target];
            Vector2 move = player.position - npc.Center;

            float speed = 4.6f;
            Vector2 playerPos = player.position + new Vector2(0, 0);
            Vector2 moving = playerPos - npc.Center;
            float magnitude = 1f;
            int RandomPos;
            float turnResistance = 5f;
            if (Main.dayTime || player.dead)
            {
                npc.rotation *= 0.99f;
                npc.velocity.Y -= 0.09f;
                npc.timeLeft = 100;
                if (npc.position.Y <= 16 * 20) //checking for top of the world practically
                    npc.active = false;
                return;
            }
            switch (State)
            {

                case RimegeistState.IdleMovement:

                    if (AttackTimer % 120 == 0)
                    {
                        RandomPos = 300;
                    }
                    else
                    {
                        RandomPos = -300;
                    }
                    playerPos = player.position + new Vector2(0, RandomPos);
                    {
                        speed = 4.6f;
                        moving = playerPos - npc.Center;
                        magnitude = (float)Math.Sqrt(moving.X * moving.X + moving.Y * moving.Y);
                        if (magnitude > speed)
                        {
                            moving *= speed / magnitude;
                        }
                        turnResistance = 5f;
                        moving = (npc.velocity * turnResistance + moving) / (turnResistance + 1f);
                        magnitude = (float)Math.Sqrt(moving.X * moving.X + moving.Y * moving.Y);
                        if (magnitude > speed)
                        {
                            moving *= speed / magnitude;
                        }
                        npc.velocity = moving;
                        // Main.NewText("Idle Movement");
                        if (++AttackTimer >= 1)
                        {
                            npc.netUpdate = true;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                ChangeState((int)RimegeistState.RadialIcicles);
                            }
                        }
                    }

                    break;
                case RimegeistState.RadialIcicles:

                    if (++AttackTimer <= 220)
                    {
                        //Main.NewText("Radial Icicles");

                        // follows player from the top and shoots a blast of icy spikes

                        float scalespeed = (PhaseTwo > 0 ? 0.2f : MathHelper.Clamp(AttackTimer / 300f, 0f, 1f)) * 10f;

                        if (AttackTimer < 30 && PhaseTwo > 0)
                        {
                            if (AttackTimer < 80)
                            {
                                npc.localAI[2] = 2;
                                npc.localAI[3] += (60f - npc.localAI[3]) * 0.75f;
                            }
                            else
                            {
                                npc.localAI[2] = 2;
                                npc.localAI[3] += (0f - npc.localAI[3]) * 0.75f;
                            }
                        }

                        npc.velocity.X = (((npc.velocity.X + move.X) / 20f)) * scalespeed;
                        npc.velocity.Y = (((npc.velocity.Y + move.Y - 300) / 20f)) * scalespeed;

                        int type = mod.ProjectileType("IcySpike");
                        int damage = Main.expertMode ? 25 : 15;// if u want to change this, 15 is for expert mode, 10 is for normal mod
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
                        if (AttackTimer == 220)
                        {
                            npc.netUpdate = true;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                ChangeState(Main.rand.Next(2, 10));
                            }
                        }
                    }

                    break;
                case RimegeistState.IceBlast:

                    if (++AttackTimer <= 180)
                    {
                        dashTimer = 0;
                        playerPos = player.position + new Vector2(Math.Sign(-move.X) * 500, 0);
                        speed = 3.2f;
                        moving = playerPos - npc.Center;
                        magnitude = (float)Math.Sqrt(moving.X * moving.X + moving.Y * moving.Y);
                        if (magnitude > speed)
                        {
                            moving *= speed / magnitude;
                        }
                        turnResistance = 5f;
                        moving = (npc.velocity * turnResistance + moving) / (turnResistance + 1f);
                        magnitude = (float)Math.Sqrt(moving.X * moving.X + moving.Y * moving.Y);
                        if (magnitude > speed)
                        {
                            moving *= speed / magnitude;
                        }
                        npc.velocity = moving;
                        // Main.NewText("Ice Blast");
                        stobit++;
                        if (stobit >= 40)
                        {
                            float Speed = 7f;
                            int damage = Main.expertMode ? 20 : 15;// if u want to change this, 15 is for expert mode, 10 is for normal mod
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

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            ChangeState(Main.rand.Next(2, 10));
                        }
                    }

                    break;
                case RimegeistState.IceBoltBlast:

                    if (++AttackTimer <= 180)
                    {
                        // Main.NewText("IceBoltBlast");
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
                        int damage = Main.expertMode ? 20 : 10;// if u want to change this, 15 is for expert mode, 10 is for normal mod
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
                                    Main.PlaySound(SoundID.Item101);
                                    Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X * speedMult, perturbedSpeed.Y * speedMult, type, damage, 2f, Main.myPlayer);

                                }
                                shootTimer2 = 0;

                            }
                        }
                    }
                    if (AttackTimer == 180)
                    {
                        npc.netUpdate = true;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            ChangeState(Main.rand.Next(2, 10));
                        }
                    }

                    break;
                case RimegeistState.IcicleRain:

                    if (++AttackTimer <= 0)
                    {
                        progTimer1++;
                        //Main.NewText("IcicleRain");

                    }
                    if (AttackTimer == 1)
                    {
                        npc.netUpdate = true;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            ChangeState(Main.rand.Next(2, 10));
                        }
                    }

                    break;
                case RimegeistState.VortexOfRainbows:


                    if (AttackTimer % 120 == 0)
                    {
                        RandomPos = 300;
                    }
                    else
                    {
                        RandomPos = -300;
                    }
                    playerPos = player.position + new Vector2(0, RandomPos);
                    speed = 4f;
                    moving = playerPos - npc.Center;
                    magnitude = (float)Math.Sqrt(moving.X * moving.X + moving.Y * moving.Y);
                    if (magnitude > speed)
                    {
                        moving *= speed / magnitude;
                    }
                    turnResistance = 5f;
                    moving = (npc.velocity * turnResistance + moving) / (turnResistance + 1f);
                    magnitude = (float)Math.Sqrt(moving.X * moving.X + moving.Y * moving.Y);
                    if (magnitude > speed)
                    {
                        moving *= speed / magnitude;
                    }
                    npc.velocity = moving;
                    if (++AttackTimer <= 180)
                    {
                        // Main.NewText("Vortex Of Rainbows");

                        if (progTimer2 <= 60)
                        {
                            int damage = Main.expertMode ? 75 : 65;// if u want to change this, 15 is for expert mode, 10 is for normal mod
                            npc.velocity *= 0.9f;
                            int type = mod.ProjectileType("RainbowBlast");
                            if (Main.netMode != 1 && progTimer2 % 10 == 0)
                            {
                                Vector2 circular = new Vector2(0, -8).RotatedBy(MathHelper.ToRadians(progTimer2 * 6));
                                Projectile.NewProjectile(npc.Center, circular, type, damage, 2f, Main.myPlayer, npc.target, progTimer1 * 18);
                            }
                        }
                        progTimer2++;
                    }
                    if (AttackTimer == 180)
                    {
                        progTimer2 = 0;
                        npc.netUpdate = true;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            ChangeState((int)RimegeistState.RadialIcicles);
                        }
                    }

                    break;
                case RimegeistState.HomingVoidSouls:

                    npc.velocity *= 0.99f;
                    //Main.NewText("HomingVoidSouls");
                    if (++AttackTimer <= 180)
                    {
                        ebic++;
                        if (ebic >= 100)
                        {
                            int damage = Main.expertMode ? 10 : 5;// if u want to change this, 15 is for expert mode, 10 is for normal mod
                            int type2 = mod.ProjectileType("IceCube");
                            Projectile.NewProjectile(player.Center + new Vector2(0, -300), new Vector2(0, 0), type2, damage, 2f, player.whoAmI);
                            ebic = 0;
                        }
                        shootTimer2++;
                        if (shootTimer2 >= 150)
                        {
                            float numberProjectiles = 4;
                            for (int i = 0; i < numberProjectiles; i++)
                            {
                                if (Main.netMode != 1)
                                {
                                    FireLaser(ModContent.ProjectileType<HomingWispSouls>(), 1, 0, player.whoAmI);
                                }
                                shootTimer2 = 0;

                            }
                        }
                    }
                    if (AttackTimer == 180)
                    {
                        npc.netUpdate = true;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            ChangeState((int)RimegeistState.Dash);
                        }
                    }

                    break;
                case RimegeistState.Dash:


                    Vector2 movewhere = PhaseTwo > 0 ? move + new Vector2(Math.Sign(player.Center.X - npc.Center.X) * -200, -200) : move;

                    //Main.NewText("Dash");
                    if (++AttackTimer >= 0 && AttackTimer < 480)
                    {
                        dashTimer++;

                        if (dashTimer % 240 < 150)
                        {

                            npc.velocity = movewhere * MathHelper.Clamp((dashTimer % 240) / 100f, 0f, 1f) * (PhaseTwo > 0 ? 0.075f : 0.02f);
                            npc.velocity *= 0.99f;//Friction

                        }
                        else if (dashTimer % 240 < 160 || dashTimer % 240 > 220)
                        {
                            //  Main.NewText("Dash Wait");
                            if (PhaseTwo > 0)
                            {
                                npc.velocity = -Vector2.Normalize(move) * (12 * MathHelper.Clamp(AttackTimer / 60f, 0f, 1f));

                            }
                            npc.velocity *= 0.75f;
                            moveSpeed = 0;
                        }
                        else
                        {
                            // Main.NewText("Dash Fly");
                            if (moveSpeed == 0)
                            {
                                Main.PlaySound(new LegacySoundStyle(SoundID.Roar, 0), npc.Center);
                                moveSpeed = Math.Sign(player.Center.X - npc.Center.X) * 10;
                                if (PhaseTwo < 1)
                                {
                                    npc.velocity.X = moveSpeed;
                                }
                                else
                                {
                                    npc.velocity = Vector2.Normalize(move) * 16;
                                }
                            }
                        }
                    }

                    if (AttackTimer == 480)
                    {
                        npc.netUpdate = true;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            ChangeState((int)RimegeistState.RadialIcicles);
                        }
                    }

                    break;
                case RimegeistState.VoidStone:

                    // Main.NewText("Void Stone");
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

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            ChangeState((int)Main.rand.Next(2, 10));
                        }
                    }
                    break;
                case RimegeistState.IceBulletHell:

                    playerPos = player.position + new Vector2(-400, 0);
                    {
                        speed = 4.6f;
                        moving = playerPos - npc.Center;
                        magnitude = (float)Math.Sqrt(moving.X * moving.X + moving.Y * moving.Y);
                        if (magnitude > speed)
                        {
                            moving *= speed / magnitude;
                        }
                        turnResistance = 5f;
                        moving = (npc.velocity * turnResistance + moving) / (turnResistance + 1f);
                        magnitude = (float)Math.Sqrt(moving.X * moving.X + moving.Y * moving.Y);
                        if (magnitude > speed)
                        {
                            moving *= speed / magnitude;
                        }
                        npc.velocity = moving;
                        float length = 850f;
                        {
                            Vector2 projectilePos = player.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * length;
                            Vector2 projectileVelocity = Vector2.Normalize(target.Center - projectilePos) * 16;

                            // Main.NewText("Laser Bullet Hell");
                            if (++AttackTimer % 5 == 0 && AttackTimer < 40)
                            {
                                npc.dontTakeDamage = true;

                                Projectile.NewProjectile(projectilePos, projectileVelocity, ModContent.ProjectileType<IcyLaserProj>(), npc.damage, 1);
                            }
                        }
                        if (AttackTimer == 50)
                        {
                            npc.dontTakeDamage = false;
                            npc.netUpdate = true;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                ChangeState((int)Main.rand.Next(2, 10));
                            }
                        }
                    }

                    break;
                case RimegeistState.PhaseTwoTransition:

                    npc.velocity *= 0.95f;
                    npc.dontTakeDamage = true;

                    whiteIn = MathHelper.Clamp(whiteIn + (AttackTimer < 120 ? 0.025f : -0.15f), 0, 1f);

                    if (++AttackTimer == 120)
                    {
                        PhaseTwo = 1;

                        Microsoft.Xna.Framework.Audio.SoundEffectInstance snd = Main.PlaySound(new LegacySoundStyle(SoundID.Roar, 2), npc.Center);

                        if (snd != null)
                        {
                            snd.Pitch = -0.25f;
                        }

                        for (int i = 0; i < 3; i++)
                            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/RimegeistGore" + i));

                    }

                    if (AttackTimer == 200)
                    {
                        npc.netUpdate = true;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            ChangeState((int)RimegeistState.Dash);
                        }
                    }

                    break;
                case RimegeistState.ShadowDash:

                    //Main.NewText("Dash");

                    AttackTimer += 1;

                    npc.localAI[2] = 20;
                    npc.localAI[3] += (60f - npc.localAI[3]) * 0.15f;

                    if (AttackTimer < 600)
                        Terraria.GameContent.Events.ScreenObstruction.screenObstruction = MathHelper.Clamp(Terraria.GameContent.Events.ScreenObstruction.screenObstruction + 0.10f, 0f, 0.95f);

                    dashTimer++;

                    int dashindex = 200;

                    if (dashTimer % dashindex >= 140)
                    {

                        //Main.NewText(npc.velocity);

                        if (dashTimer % dashindex == 140)
                        {
                            npc.velocity = Vector2.Normalize(move) * 32f;

                            Microsoft.Xna.Framework.Audio.SoundEffectInstance snd = Main.PlaySound(new LegacySoundStyle(SoundID.Roar, 0), npc.Center);

                            if (snd != null)
                            {
                                snd.Pitch = 0.50f;
                            }

                        }

                        if (dashTimer % 10 == 0)
                        {
                            Projectile.NewProjectile(npc.Center, -Vector2.Normalize(move) * 1f, ModContent.ProjectileType<HomingWispSouls>(), 20, 0, Main.myPlayer);
                        }
                    }
                    else
                    {
                        if (dashTimer % dashindex < 100)
                        {
                            npc.velocity += ((move + Vector2.UnitX.RotatedBy(move.ToRotation() + MathHelper.Pi) * (MathHelper.Max((dashTimer % dashindex * 12f) - 200, 240f))) - npc.velocity) * 0.025f;
                            npc.velocity = Vector2.Normalize(npc.velocity) * MathHelper.Clamp(npc.velocity.Length(), 0f, 12f);

                        }
                        npc.velocity *= 0.95f;
                    }

                    if (AttackTimer == 650)
                    {
                        dashTimer = 0;
                        npc.netUpdate = true;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            ChangeState((int)RimegeistState.VortexOfRainbows);
                        }
                    }

                    break;
                default:

                    //when non is true
                    break;

            }
        }

        Texture2D GlowTexture => ModContent.GetTexture(AssetDirectory + "Glowmask");

        public float EyesFade => 1f;
        public float Alpha => MathHelper.Clamp(3f - (npc.localAI[3]) / 20f, 0f, 1f);
        public float ShadowTrailEffect => MathHelper.Clamp((float)Math.Sin(Math.Min((npc.localAI[3] / 30f) * MathHelper.Pi, MathHelper.PiOver2)), 0f, 1f);
        public Color EyesColor => Color.Lerp(Color.Black, Color.White, EyesFade) * EyesFade;


        public void DrawOverBlackout(On.Terraria.GameContent.Events.ScreenObstruction.orig_Draw orig, SpriteBatch spriteBatch)
        {
            orig(spriteBatch);

            foreach (Projectile homingshot in Main.projectile.Where(testby => testby.active && testby.type == ModContent.ProjectileType<HomingWispSouls>()))
            {
                homingshot.modProjectile.PreDraw(spriteBatch, Color.White);
            }

            if (Rimegeist.rimegeist != null && Rimegeist.rimegeist.npc.active)
            {
                float alpha = 1f;
                spriteBatch.Draw(ModContent.GetTexture(AssetDirectory + "Glowmask"), Rimegeist.rimegeist.npc.Center - Main.screenPosition, Rimegeist.rimegeist.npc.frame, Rimegeist.rimegeist.EyesColor, Rimegeist.rimegeist.npc.rotation, Rimegeist.rimegeist.npc.frame.Size() / 2f, Rimegeist.rimegeist.npc.scale, SpriteEffects.None, 0);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D solidColor = ModContent.GetTexture(AssetDirectory + "RimegeistColorOver");
            if (ShadowTrailEffect > 0)
            {
                for (float f = 0; f < npc.velocity.Length(); f += 0.10f)
                {
                    float scale = 3f;
                    spriteBatch.Draw(solidColor, npc.Center + (Vector2.Normalize(npc.velocity) * -f * scale) - Main.screenPosition, npc.frame, Color.Black * 0.075f * ShadowTrailEffect * (1f - (f / npc.velocity.Length())), npc.rotation, npc.frame.Size() / 2f, npc.scale, SpriteEffects.None, 0);
                }
            }
            if (Alpha > 0)
            {
                spriteBatch.Draw(Main.npcTexture[npc.type], npc.Center - Main.screenPosition, npc.frame, drawColor * Alpha, npc.rotation, npc.frame.Size() / 2f, npc.scale, SpriteEffects.None, 0);
                spriteBatch.Draw(GlowTexture, npc.Center - Main.screenPosition, npc.frame, Color.White * Alpha, npc.rotation, npc.frame.Size() / 2f, npc.scale, SpriteEffects.None, 0);

            }
            if (Alpha * whiteIn > 0)
                spriteBatch.Draw(solidColor, npc.Center - Main.screenPosition, npc.frame, Color.White * Alpha * whiteIn, npc.rotation, npc.frame.Size() / 2f, npc.scale, SpriteEffects.None, 0);

            return false;

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
            if (PhaseTwo > 0)
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
        public void FireLaser(int type, float speed = 6f, float recoilMult = 2f, float ai1 = 0, float ai2 = 0)
        {
            Player player = Main.player[npc.target];
            Vector2 toPlayer = player.Center - npc.Center;
            toPlayer = toPlayer.SafeNormalize(new Vector2(1, 0));
            toPlayer *= speed;
            Vector2 from = npc.Center - new Vector2(96, 0).RotatedBy(npc.rotation);
            int damage = 60;
            if (Main.expertMode)
            {
                damage = (int)(damage / Main.expertDamage);
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(from, toPlayer, type, damage, 3, Main.myPlayer, ai1, ai2);
            }
            npc.velocity -= toPlayer * recoilMult;
        }
    }

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
                Color color = projectile.GetAlpha(FetchRainbow()) * ((projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
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
        public Color FetchRainbow()
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
        private bool spawned;
        public override void AI()
        {
            if (!spawned)
            {
                spawned = true;
                Main.PlaySound(SoundID.Item75, projectile.Center);
            }

            counter++;
            projectile.velocity *= 0.955f + 0.000175f * counter;

            Player player = Main.player[(int)projectile.ai[0]];
            Vector2 toPlayer = player.Center - projectile.Center;
            Vector2 circular = new Vector2(1, 0).RotatedBy(MathHelper.ToRadians(projectile.ai[1] * 2));
            projectile.velocity += toPlayer.SafeNormalize(Vector2.Zero) * (counter * 0.0004f) + circular * 0.05f;
            projectile.ai[1] += 2f;
            Color rainbow = FetchRainbow();
            Lighting.AddLight(new Vector2(projectile.Center.X, projectile.Center.Y), rainbow.R / 255f, rainbow.G / 255f, rainbow.B / 255f);
            if (Main.rand.NextBool(10))
            {
                int num2 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 267);
                Dust dust = Main.dust[num2];
                Color color2 = new Color(110, 110, 110, 0).MultiplyRGBA(rainbow);
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
        private bool spawned;
        public override void AI()
        {
            if (!spawned)
            {
                spawned = true;
                Main.PlaySound(SoundID.Item30);
            }
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

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = ModContent.GetTexture(Rimegeist.AssetDirectory + "IcySpike_Glowmask");
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

    public class IceCube : WispSouls
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
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (Rimegeist.rimegeist != null && Rimegeist.rimegeist.PhaseTwo > 0)
                base.PreDraw(spriteBatch, lightColor);

            return true;
        }
        public override void AI()
        {
            if (Rimegeist.rimegeist != null && Rimegeist.rimegeist.PhaseTwo > 0)
            {
                if (projectile.velocity.Length() < 3)
                {
                    projectile.velocity = -Vector2.UnitY * 8f;
                }
                base.AI();
            }

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
            Texture2D texture = ModContent.GetTexture(Rimegeist.AssetDirectory + "IceBolt_Glowmask");
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
    public class HomingWispSouls : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 25;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            Main.projFrames[projectile.type] = 6;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            // Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width, Main.projectileTexture[projectile.type].Height);
            Texture2D texture2D = mod.GetTexture("Assets/Glow");
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                float scale = projectile.scale * (projectile.oldPos.Length - k) / projectile.oldPos.Length * .45f;
                Vector2 drawPos = projectile.oldPos[k] + projectile.Hitbox.Size() / 2f - Main.screenPosition;
                Color color = projectile.GetAlpha(Color.Black) * ((projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);

                spriteBatch.Draw(texture2D, drawPos, null, color, projectile.rotation, texture2D.Size() / 2f, scale, SpriteEffects.None, 0f);
            }

            texture2D = Main.projectileTexture[projectile.type];

            Rectangle rect = new Rectangle(0, (projectile.frame % (Main.projFrames[projectile.type])) * (texture2D.Height / Main.projFrames[projectile.type]), texture2D.Width, texture2D.Height / Main.projFrames[projectile.type]);

            spriteBatch.Draw(texture2D, projectile.Center - Main.screenPosition, rect, Color.White * projectile.Opacity, projectile.velocity.X / 20f, rect.Size() / 2f, projectile.scale, SpriteEffects.None, 0);

            return false;
        }
        public override void SetDefaults()
        {
            projectile.width = 44;
            projectile.height = 56;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.aiStyle = -1;
            projectile.tileCollide = false;
            projectile.ranged = projectile.friendly == true;
            projectile.timeLeft = 180;
        }
        int radians = 16;
        int Timer = 0;
        int counter = 10;
        int i;
        private bool spawned;
        public override void AI()
        {

            projectile.Opacity = MathHelper.Clamp(projectile.timeLeft / 20f, 0f, 1f);

            if (!spawned)
            {
                spawned = true;
                Main.PlaySound(SoundID.Item105);
            }

            projectile.rotation = MathHelper.ToRadians(180) + projectile.velocity.ToRotation();
            projectile.frameCounter++;
            if (projectile.frameCounter >= 4)
            {
                projectile.frameCounter = 0;
                projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
            }
            float approaching = ((540f - projectile.timeLeft) / 540f);

            Player player = Main.player[(int)projectile.ai[0]];
            if (player.active)
            {
                float x = Main.rand.Next(-10, 11) * 0.001f * approaching;
                float y = Main.rand.Next(-10, 11) * 0.001f * approaching;
                Vector2 toPlayer = projectile.Center - player.Center;
                toPlayer = toPlayer.SafeNormalize(Vector2.Zero);
                projectile.velocity += -toPlayer * (0.155f * projectile.timeLeft / 540f) + new Vector2(x, y);
            }
        }
    }
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
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.height = 18;
            projectile.aiStyle = -1;
            projectile.tileCollide = false;
            projectile.ranged = projectile.friendly == true;
            projectile.timeLeft = 180;
        }
        int radians = 16;
        int Timer = 0;
        private bool spawned;
        public override void AI()
        {

            projectile.Opacity = MathHelper.Clamp(projectile.timeLeft / 20f, 0f, 1f);

            if (!spawned)
            {
                spawned = true;
                Main.PlaySound(SoundID.Item104);
            }
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
                Main.PlaySound(SoundID.Item103);
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



    public class IcyLaserProj : ModProjectile
    {

        public override string Texture => "Terraria/Projectile_" + ProjectileID.None;

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 8;

            projectile.aiStyle = -1;
            projectile.friendly = projectile.melee = projectile.tileCollide = false;

            projectile.penetrate = 4;

            projectile.timeLeft = 180;
        }

        int Timer = 0;
        private bool spawned;
        public override void AI()
        {
            if (!spawned)
            {
                spawned = true;
                Main.PlaySound(SoundID.Item30);
            }
            projectile.velocity *= 1.003f;

            for (int j = 0; j < 80; j++)
            {
                float x = projectile.position.X - projectile.velocity.X / 10f * (float)j;
                float y = projectile.position.Y - projectile.velocity.Y / 10f * (float)j;
                Dust dust = Dust.NewDustDirect(new Vector2(x, y), 1, 1, 20, 0, 0, 0, Color.Blue, 0.9f);
                dust.position.X = x;
                dust.position.Y = y;
                dust.velocity *= 0f;
                dust.noGravity = true;
            }
            if (++projectile.localAI[1] > 10)
            {
                float amountOfDust = 16f;
                for (int i = 0; i < amountOfDust; ++i)
                {
                    Vector2 spinningpoint5 = -Vector2.UnitY.RotatedBy(i * (MathHelper.TwoPi / amountOfDust)) * new Vector2(1f, 4f);
                    spinningpoint5 = spinningpoint5.RotatedBy(projectile.velocity.ToRotation());

                    Dust dust = Dust.NewDustPerfect(projectile.Center + spinningpoint5, 20, spinningpoint5, 0, Color.Blue, 1.3f);
                    dust.noGravity = true;
                }

                projectile.localAI[1] = 0;
            }
        }
    }
}

