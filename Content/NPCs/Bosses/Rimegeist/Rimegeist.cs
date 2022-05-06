using AerovelenceMod.Content.Items.Armor.Vanity;
using AerovelenceMod.Content.Items.Placeables.Trophies;
using AerovelenceMod.Content.Items.TreasureBags;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using On.Terraria.GameContent.Events;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

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
        public const string AssetDirectory = "AerovelenceMod/Content/NPCs/Bosses/Rimegeist/";

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
            get => (RimegeistState)NPC.ai[0];
            set => NPC.ai[0] = (float)value;
        }

        /// <summary>
        /// Manages several AI state attack timers.
        /// Gets and sets npc.ai[1] as tracker.
        /// </summary>
        private float AttackTimer
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        /// <summary>
        /// Manages the phase 2
        /// Gets and sets npc.ai[2] as tracker.
        /// </summary>
        public float PhaseTwo
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }


        /// <summary>
        /// Changes the npc state and attack timer
        /// Allows easier code injection later on to interupt change states
        /// </summary>
        public void ChangeState(int stateIndex, int attackTimer = 0)
        {
            State = (RimegeistState)stateIndex;
            if (PhaseTwo < 1 && NPC.life < NPC.lifeMax / 2)
                State = RimegeistState.PhaseTwoTransition;

            if (PhaseTwo > 2000)
            {
                State = RimegeistState.ShadowDash;
                PhaseTwo = Main.rand.Next(5,200);

            }

            this.AttackTimer = attackTimer;

        }

        /*public override bool Autoload(ref string name)
        {
            ScreenObstruction.Draw += DrawOverBlackout;
            return true;
        }*/

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;    //boss frame/animation 
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;  //5 is the flying AI
            NPC.lifeMax = 9000;   //boss life
            NPC.damage = 32;  //boss damage
            NPC.defense = 24;    //boss defense
            NPC.knockBackResist = 0f;
            NPC.width = 220;
            NPC.height = 260;
            NPC.value = Item.buyPrice(0, 5, 75, 45);
            NPC.npcSlots = 1f;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCHit5;
            NPC.buffImmune[24] = true;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Rimegeist");
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            var entitySource = NPC.GetSource_Death();

            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<RimegeistBag>()));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RimegeistTrophy>(), 10));

            //npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<RimegeistRelic>()));

            //npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<RimegeistPetItem>(), 4));

            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<RimegeistMask>(), 7));

            npcLoot.Add(notExpertRule);


            Item.NewItem(entitySource, (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, Mod.Find<ModItem>("FrostShard").Type, Main.rand.Next(10, 20), false, 0, false, false);
            switch (Main.rand.Next(5))
            {
                case 0:
                    Item.NewItem(entitySource, (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, Mod.Find<ModItem>("CrystalArch").Type, 1, false, 0, false, false);
                    break;
                case 1:
                    Item.NewItem(entitySource, (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, Mod.Find<ModItem>("DeepFreeze").Type, 1, false, 0, false, false);
                    break;
                case 2:
                    Item.NewItem(entitySource, (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, Mod.Find<ModItem>("IcySaber").Type, 1, false, 0, false, false);
                    break;
                case 3:
                    Item.NewItem(entitySource, (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, Mod.Find<ModItem>("CryoBall").Type, 1, false, 0, false, false);
                    break;
                case 4:
                    Item.NewItem(entitySource, (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, Mod.Find<ModItem>("Snowball").Type, 1, false, 0, false, false);
                    break;
            }
        }
        
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return NPC.localAI[2] > -30f && State != RimegeistState.ShadowDash ? false : base.CanHitPlayer(target, ref cooldownSlot);
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = 10000;  //boss life scale in expertmode
            NPC.damage = 40;  //boss damage increase in expermode
        }

        int moveSpeed = 0;
        bool text = false;
        int moveSpeedY = 0;

        public bool doingDash = false;

        public override void AI()
        {

            NPC.dontTakeDamage = NPC.localAI[2] > 0f && State != RimegeistState.ShadowDash;
            NPC.localAI[0] = NPC.velocity.X * 0.025f;
            NPC.rotation += (NPC.localAI[0]-NPC.rotation)*0.05f;

            NPC.localAI[2]--;
            if (NPC.localAI[2]<1)
            NPC.localAI[3] *= 0.95f;

            rimegeist = this;

            if (PhaseTwo > 0 && State != RimegeistState.PhaseTwoTransition)
            {
                PhaseTwo += 1;
            }

            NPC.TargetClosest(true);
            Player target = Main.player[NPC.target];
            Vector2 vector8 = new Vector2(NPC.position.X + (NPC.width * 0.5f), NPC.position.Y + (NPC.height * 0.5f));
            var player = Main.player[NPC.target];
            Vector2 move = player.position - NPC.Center;

            float speed = 4.6f;
            Vector2 playerPos = player.position + new Vector2(0, 0);
            Vector2 moving = playerPos - NPC.Center;
            float magnitude = 1f;
            int RandomPos;
            float turnResistance = 5f;

            if (Main.dayTime || player.dead)
            {
                NPC.rotation *= 0.99f;
                NPC.velocity.Y -= 0.09f;
                NPC.timeLeft = 100;
                if (NPC.position.Y <= 16 * 20) //checking for top of the world practically
                    NPC.active = false;
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
                        moving = playerPos - NPC.Center;
                        magnitude = (float)Math.Sqrt(moving.X * moving.X + moving.Y * moving.Y);
                        if (magnitude > speed)
                        {
                            moving *= speed / magnitude;
                        }
                        turnResistance = 5f;
                        moving = (NPC.velocity * turnResistance + moving) / (turnResistance + 1f);
                        magnitude = (float)Math.Sqrt(moving.X * moving.X + moving.Y * moving.Y);
                        if (magnitude > speed)
                        {
                            moving *= speed / magnitude;
                        }
                        NPC.velocity = moving;
                        // Main.NewText("Idle Movement");
                        if (++AttackTimer >= 1)
                        {
                            NPC.netUpdate = true;

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

                        float scalespeed = (PhaseTwo>0 ? 0.2f : MathHelper.Clamp(AttackTimer / 300f, 0f, 1f))*10f;

                        if (AttackTimer < 30 && PhaseTwo>0)
                        {
                            if (AttackTimer < 80)
                            {
                                NPC.localAI[2] = 2;
                                NPC.localAI[3] += (60f - NPC.localAI[3]) * 0.75f;
                            }
                            else
                            {
                                NPC.localAI[2] = 2;
                                NPC.localAI[3] += (0f - NPC.localAI[3]) * 0.75f;
                            }
                        }

                        NPC.velocity.X = (((NPC.velocity.X + move.X) / 20f))* scalespeed;
                        NPC.velocity.Y = (((NPC.velocity.Y + move.Y - 300) / 20f)) * scalespeed;

                        int type = Mod.Find<ModProjectile>("IcySpike").Type;
                        int damage = Main.expertMode ? 10 : 5;// if u want to change this, 15 is for expert mode, 10 is for normal mod
                        float speedX = 10f;
                        float speedY = 10f;
                        Vector2 position = NPC.Center;


                        shootTimer++;

                        if (shootTimer >= 90)
                        {
                            if (Main.netMode != 1)
                                for (int i = 0; i < 8; i++)
                                {
                                    Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(45 * i));
                                    Projectile.NewProjectile(Projectile.InheritSource(NPC), position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, 2f, Main.myPlayer);
                                }
                            shootTimer = 0;
                        }
                        if (AttackTimer == 220)
                        {
                            NPC.netUpdate = true;

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
                        playerPos = player.position + new Vector2(Math.Sign(-move.X)*500, 0);
                        speed = 3.2f;
                        moving = playerPos - NPC.Center;
                        magnitude = (float)Math.Sqrt(moving.X * moving.X + moving.Y * moving.Y);
                        if (magnitude > speed)
                        {
                            moving *= speed / magnitude;
                        }
                        turnResistance = 5f;
                        moving = (NPC.velocity * turnResistance + moving) / (turnResistance + 1f);
                        magnitude = (float)Math.Sqrt(moving.X * moving.X + moving.Y * moving.Y);
                        if (magnitude > speed)
                        {
                            moving *= speed / magnitude;
                        }
                        NPC.velocity = moving;
                        // Main.NewText("Ice Blast");
                        stobit++;
                        if (stobit >= 40)
                        {
                            float Speed = 7f;
                            int damage = Main.expertMode ? 10 : 5;// if u want to change this, 15 is for expert mode, 10 is for normal mod
                            int type = Mod.Find<ModProjectile>("IceBlast").Type;
                            float rotation = (float)Math.Atan2(vector8.Y - (player.position.Y + (player.height * 0.5f)), vector8.X - (player.position.X + (player.width * 0.5f)));
                            if (Main.netMode != 1)
                                Projectile.NewProjectile(Projectile.InheritSource(NPC), vector8.X, vector8.Y, (float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1), type, damage, 0f, Main.myPlayer);
                            stobit = 0;
                        }
                    }
                    if (AttackTimer == 180)
                    {
                        NPC.netUpdate = true;

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
                        int damage = Main.expertMode ? 10 : 5;// if u want to change this, 15 is for expert mode, 10 is for normal mod
                        Vector2 position = NPC.Center;
                        int type = Mod.Find<ModProjectile>("IceBolt").Type;
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
                                    SoundEngine.PlaySound(SoundID.Item101);
                                    Projectile.NewProjectile(Projectile.InheritSource(NPC), position.X, position.Y, perturbedSpeed.X * speedMult, perturbedSpeed.Y * speedMult, type, damage, 2f, Main.myPlayer);

                                }
                                shootTimer2 = 0;

                            }
                        }
                    }
                    if (AttackTimer == 180)
                    {
                        NPC.netUpdate = true;

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
                        NPC.netUpdate = true;

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
                    moving = playerPos - NPC.Center;
                    magnitude = (float)Math.Sqrt(moving.X * moving.X + moving.Y * moving.Y);
                    if (magnitude > speed)
                    {
                        moving *= speed / magnitude;
                    }
                    turnResistance = 5f;
                    moving = (NPC.velocity * turnResistance + moving) / (turnResistance + 1f);
                    magnitude = (float)Math.Sqrt(moving.X * moving.X + moving.Y * moving.Y);
                    if (magnitude > speed)
                    {
                        moving *= speed / magnitude;
                    }
                    NPC.velocity = moving;
                    if (++AttackTimer <= 180)
                    {
                        // Main.NewText("Vortex Of Rainbows");

                        if (progTimer2 <= 60)
                        {
                            NPC.velocity *= 0.9f;
                            int type = Mod.Find<ModProjectile>("RainbowBlast").Type;
                            if (Main.netMode != 1 && progTimer2 % 10 == 0)
                            {
                                Vector2 circular = new Vector2(0, -8).RotatedBy(MathHelper.ToRadians(progTimer2 * 6));
                                Projectile.NewProjectile(Projectile.InheritSource(NPC), NPC.Center, circular, type, NPC.damage, 2f, Main.myPlayer, NPC.target, progTimer1 * 18);
                            }
                        }
                        progTimer2++;
                    }
                    if (AttackTimer == 180)
                    {
                        progTimer2 = 0;
                        NPC.netUpdate = true;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            ChangeState((int)RimegeistState.RadialIcicles);
                        }
                    }

                    break;
                case RimegeistState.HomingVoidSouls:

                    NPC.velocity *= 0.99f;
                    //Main.NewText("HomingVoidSouls");
                    if (++AttackTimer <= 180)
                    {
                        ebic++;
                        if (ebic >= 100)
                        {
                            int damage = Main.expertMode ? 10 : 5;// if u want to change this, 15 is for expert mode, 10 is for normal mod
                            int type2 = Mod.Find<ModProjectile>("IceCube").Type;
                            Projectile.NewProjectile(Projectile.InheritSource(NPC), player.Center + new Vector2(0, -300), new Vector2(0, 0), type2, damage, 2f, player.whoAmI);
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
                        NPC.netUpdate = true;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            ChangeState((int)RimegeistState.Dash);
                        }
                    }

                    break;
                case RimegeistState.Dash:


                    Vector2 movewhere = PhaseTwo > 0 ? move + new Vector2(Math.Sign(player.Center.X - NPC.Center.X) * -200, -200) : move;

                    //Main.NewText("Dash");
                    if (++AttackTimer >= 0 && AttackTimer < 480)
                    {
                        dashTimer++;

                        if (dashTimer % 240 < 150)
                        {

                            NPC.velocity = movewhere*MathHelper.Clamp((dashTimer%240)/100f,0f,1f) * (PhaseTwo > 0 ? 0.075f : 0.02f);
                            NPC.velocity *= 0.99f;//Friction

                        }
                        else if (dashTimer % 240 < 160 || dashTimer % 240 > 220)
                        {
                            //  Main.NewText("Dash Wait");
                            if (PhaseTwo > 0)
                            {
                                NPC.velocity = -Vector2.Normalize(move) * (12*MathHelper.Clamp(AttackTimer/60f,0f,1f));

                            }
                            NPC.velocity *= 0.75f;
                            moveSpeed = 0;
                        }
                        else
                        {
                            // Main.NewText("Dash Fly");
                            if (moveSpeed == 0)
                            {
                                SoundEngine.PlaySound(new LegacySoundStyle(SoundID.Roar, 0), NPC.Center);
                                moveSpeed = Math.Sign(player.Center.X - NPC.Center.X) * 10;
                                if (PhaseTwo < 1)
                                {
                                    NPC.velocity.X = moveSpeed;
                                }
                                else
                                {
                                    NPC.velocity = Vector2.Normalize(move) * 16;
                                }
                            }
                        }
                    }

                    if (AttackTimer == 480)
                    {
                        NPC.netUpdate = true;

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
                        NPC.velocity *= 0.99f;
                        NPC.velocity.Y -= 0.02f;
                        float numberProjectiles = 2;
                        float rotation = MathHelper.ToRadians(10);
                        float rotate = (float)Math.Atan2(vector8.Y - (player.position.Y + (player.height * 0.5f)), vector8.X - (player.position.X + (player.width * 0.5f)));

                        Vector2 position = NPC.Center;
                        someValue += 3f;
                        shootTimer2++;
                        position += Vector2.Normalize(new Vector2((float)((Math.Cos(rotate) * 5f) * -1), (float)((Math.Sin(rotate) * 5f) * -1))) * 45f;
                        if (shootTimer2 > 40)
                        {
                            if (Main.netMode != 1)
                            {
                                Projectile.NewProjectile(Projectile.InheritSource(NPC), NPC.Center.X, NPC.Center.Y, 0, 0, ModContent.ProjectileType<VoidStone>(), 15, 2f, Main.myPlayer, NPC.target);

                            }
                            shootTimer2 = -30;
                        }
                    }
                    if (AttackTimer == 80)
                    {
                        NPC.netUpdate = true;

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
                        moving = playerPos - NPC.Center;
                        magnitude = (float)Math.Sqrt(moving.X * moving.X + moving.Y * moving.Y);
                        if (magnitude > speed)
                        {
                            moving *= speed / magnitude;
                        }
                        turnResistance = 5f;
                        moving = (NPC.velocity * turnResistance + moving) / (turnResistance + 1f);
                        magnitude = (float)Math.Sqrt(moving.X * moving.X + moving.Y * moving.Y);
                        if (magnitude > speed)
                        {
                            moving *= speed / magnitude;
                        }
                        NPC.velocity = moving;
                        float length = 850f;
                        {
                            Vector2 projectilePos = player.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * length;
                            Vector2 projectileVelocity = Vector2.Normalize(target.Center - projectilePos) * 16;

                            // Main.NewText("Laser Bullet Hell");
                            if (++AttackTimer % 5 == 0 && AttackTimer < 40)
                            {
                                NPC.dontTakeDamage = true;

                                Projectile.NewProjectile(Projectile.InheritSource(NPC), projectilePos, projectileVelocity, ModContent.ProjectileType<IcyLaserProj>(), NPC.damage, 1);
                            }
                        }
                        if (AttackTimer == 50)
                        {
                            NPC.dontTakeDamage = false;
                            NPC.netUpdate = true;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                ChangeState((int)Main.rand.Next(2, 10));
                            }
                        }
                    }

                    break;
                case RimegeistState.PhaseTwoTransition:

                    var entitySource = NPC.GetSource_Death();
                    NPC.velocity *= 0.95f;
                    NPC.dontTakeDamage = true;

                    whiteIn = MathHelper.Clamp(whiteIn + (AttackTimer < 120 ? 0.025f : -0.15f), 0, 1f);

                    if (++AttackTimer == 120)
                    {
                        PhaseTwo = 1;

                        Microsoft.Xna.Framework.Audio.SoundEffectInstance snd = SoundEngine.PlaySound(new LegacySoundStyle(SoundID.Roar, 2), NPC.Center);

                        if (snd != null)
                        {
                            snd.Pitch = -0.25f;
                        }

                        for (int i = 0; i < 7; i++)
                            if (Main.netMode == NetmodeID.Server)
                                Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/TumblerGore" + i).Type);

                    }

                    if (AttackTimer == 200)
                    {
                        NPC.netUpdate = true;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            ChangeState((int)RimegeistState.Dash);
                        }
                    }

                    break;
                case RimegeistState.ShadowDash:

                    //Main.NewText("Dash");

                    AttackTimer += 1;

                    NPC.localAI[2] = 20;
                    NPC.localAI[3] += (60f - NPC.localAI[3]) * 0.15f;

                    if (AttackTimer<600)
                    Terraria.GameContent.Events.ScreenObstruction.screenObstruction = MathHelper.Clamp(Terraria.GameContent.Events.ScreenObstruction.screenObstruction + 0.10f, 0f, 0.95f);

                    dashTimer++;

                    int dashindex = 200;

                    if (dashTimer % dashindex >= 140)
                    {

                        //Main.NewText(npc.velocity);

                        if (dashTimer % dashindex == 140)
                        {
                            NPC.velocity = Vector2.Normalize(move) * 32f;

                            Microsoft.Xna.Framework.Audio.SoundEffectInstance snd = SoundEngine.PlaySound(new LegacySoundStyle(SoundID.Roar, 0), NPC.Center);

                            if (snd != null)
                            {
                                snd.Pitch = 0.50f;
                            }

                        }

                        if (dashTimer % 10 == 0)
                        {
                            Projectile.NewProjectile(Projectile.InheritSource(NPC), NPC.Center, -Vector2.Normalize(move)*1f, ModContent.ProjectileType<HomingWispSouls>(), 20, 0, Main.myPlayer);
                        }
                    }
                    else
                    {
                        if (dashTimer% dashindex < 100)
                        {
                            NPC.velocity += ((move + Vector2.UnitX.RotatedBy(move.ToRotation()+MathHelper.Pi)*(MathHelper.Max((dashTimer% dashindex * 12f)-200,240f)))-NPC.velocity) * 0.025f;
                            NPC.velocity = Vector2.Normalize(NPC.velocity) * MathHelper.Clamp(NPC.velocity.Length(), 0f, 12f);

                        }
                        NPC.velocity *= 0.95f;
                    }

                    if (AttackTimer == 650)
                    {
                        dashTimer = 0;
                        NPC.netUpdate = true;

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

        Texture2D GlowTexture => (Texture2D)ModContent.Request<Texture2D>(AssetDirectory + "Glowmask");

        public float EyesFade => 1f;
        public float Alpha => MathHelper.Clamp(3f - (NPC.localAI[3]) / 20f, 0f, 1f);
        public float ShadowTrailEffect => MathHelper.Clamp((float)Math.Sin(Math.Min((NPC.localAI[3]/30f)*MathHelper.Pi,MathHelper.PiOver2)), 0f, 1f);
        public Color EyesColor => Color.Lerp(Color.Black,Color.White,EyesFade) * EyesFade;


        public void DrawOverBlackout(On.Terraria.GameContent.Events.ScreenObstruction.orig_Draw orig, SpriteBatch spriteBatch)
        {
            orig(spriteBatch);

            foreach(Projectile homingshot in Main.projectile.Where(testby => testby.active && testby.type == ModContent.ProjectileType<HomingWispSouls>()))
            {
                //homingshot.ModProjectile.PreDraw(Color.White);
            }

            if (Rimegeist.rimegeist != null && Rimegeist.rimegeist.NPC.active)
            {
                float alpha = 1f;
                Main.EntitySpriteDraw((Texture2D)ModContent.Request<Texture2D>(AssetDirectory + "Glowmask"), Rimegeist.rimegeist.NPC.Center - Main.screenPosition, Rimegeist.rimegeist.NPC.frame, Rimegeist.rimegeist.EyesColor, Rimegeist.rimegeist.NPC.rotation, Rimegeist.rimegeist.NPC.frame.Size() / 2f, Rimegeist.rimegeist.NPC.scale, SpriteEffects.None, 0);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
        Texture2D solidColor = (Texture2D)ModContent.Request<Texture2D>(AssetDirectory + "RimegeistColorOver");
            if (ShadowTrailEffect > 0)
            {
                for (float f = 0; f < NPC.velocity.Length(); f += 0.10f)
                {
                    float scale = 3f;
                    Main.EntitySpriteDraw(solidColor, NPC.Center + (Vector2.Normalize(NPC.velocity) * -f*scale) - Main.screenPosition, NPC.frame, Color.Black*0.075f * ShadowTrailEffect*(1f-(f/NPC.velocity.Length())), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
                }
            }
            if (Alpha > 0)
            {
                Main.EntitySpriteDraw((Texture2D)TextureAssets.Npc[NPC.type], NPC.Center - Main.screenPosition, NPC.frame, drawColor * Alpha, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(GlowTexture, NPC.Center - Main.screenPosition, NPC.frame, Color.White * Alpha, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);

            }
            if (Alpha * whiteIn > 0)
                Main.EntitySpriteDraw(solidColor, NPC.Center - Main.screenPosition, NPC.frame, Color.White * Alpha * whiteIn, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);

            return false;

        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int i = 0; i < 10; i++)
            {
                int dustType = DustID.Ice;
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, dustType);
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X = dust.velocity.X + Main.rand.Next(-50, 51) * 0.01f;
                dust.velocity.Y = dust.velocity.Y + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (PhaseTwo > 0)
            {
                int frameSpeed = 10;
                int frameC = (int)(NPC.frameCounter / frameSpeed) % 7;
                NPC.frame.Y = frameHeight * (frameC + 1);
            }
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
            return null;
        }
        public void FireLaser(int type, float speed = 6f, float recoilMult = 2f, float ai1 = 0, float ai2 = 0)
        {
            Player player = Main.player[NPC.target];
            Vector2 toPlayer = player.Center - NPC.Center;
            toPlayer = toPlayer.SafeNormalize(new Vector2(1, 0));
            toPlayer *= speed;
            Vector2 from = NPC.Center - new Vector2(96, 0).RotatedBy(NPC.rotation);
            int damage = 75;
            /*if (Main.expertMode)
            {
                damage = (int)(damage / Main.expertDamage);
            }*/
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(Projectile.InheritSource(NPC), from, toPlayer, type, damage, 3, Main.myPlayer, ai1, ai2);
            }
            NPC.velocity -= toPlayer * recoilMult;
        }
    }

    public class RainbowBlast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rainbow Good");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
                // Vector2 drawOrigin = new Vector2((Texture2D)TextureAssets.Projectile[projectile.type].Width, (Texture2D)TextureAssets.Projectile[projectile.type].Height);
                Texture2D texture2D = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
                Vector2 origin = new Vector2(texture2D.Width / 2, texture2D.Height / 2);
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    float scale = Projectile.scale * (Projectile.oldPos.Length - k) / Projectile.oldPos.Length * 1.0f;
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + TextureAssets.Projectile[Projectile.type].Size() / 3f;
                    Color color = Projectile.GetAlpha(FetchRainbow()) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    for (int i = 0; i < 6; i++)
                    {
                        if (i == 0)
                            Main.EntitySpriteDraw(texture2D, drawPos, null, color, Projectile.rotation, origin, scale, SpriteEffects.None, 0);
                        Main.EntitySpriteDraw(texture2D, drawPos + new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)), null, Color.White.MultiplyRGBA(color * 0.5f), Projectile.rotation, origin, scale * 0.6f, SpriteEffects.None, 0);
                    }
                }
                return false;
            }
        public override void SetDefaults()
        {
            Projectile.width = 68;
            Projectile.height = 68;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.scale = 0.8f;
            Projectile.timeLeft = 720;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
        }
        public Color FetchRainbow()
        {
            float sin1 = (float)Math.Sin(MathHelper.ToRadians(Projectile.ai[1]));
            float sin2 = (float)Math.Sin(MathHelper.ToRadians(Projectile.ai[1] + 120));
            float sin3 = (float)Math.Sin(MathHelper.ToRadians(Projectile.ai[1] + 240));
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
                SoundEngine.PlaySound(SoundID.Item75);
            }

            counter++;
            Projectile.velocity *= 0.955f + 0.000175f * counter;

            Player player = Main.player[(int)Projectile.ai[0]];
            Vector2 toPlayer = player.Center - Projectile.Center;
            Vector2 circular = new Vector2(1, 0).RotatedBy(MathHelper.ToRadians(Projectile.ai[1] * 2));
            Projectile.velocity += toPlayer.SafeNormalize(Vector2.Zero) * (counter * 0.0004f) + circular * 0.05f;
            Projectile.ai[1] += 2f;
            Color rainbow = FetchRainbow();
            Lighting.AddLight(new Vector2(Projectile.Center.X, Projectile.Center.Y), rainbow.R / 255f, rainbow.G / 255f, rainbow.B / 255f);
            if (Main.rand.NextBool(10))
            {
                int num2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 267);
                Dust dust = Main.dust[num2];
                Color color2 = new Color(110, 110, 110, 0).MultiplyRGBA(rainbow);
                dust.color = color2;
                dust.noGravity = true;
                dust.fadeIn = 0.1f;
                dust.scale *= 2f;
                dust.alpha = 255 - (int)(255 * (Projectile.timeLeft / 720f));
                dust.velocity *= 0.5f;
                dust.velocity += Projectile.velocity * 0.4f;
            }
        }
    }

    public class IcySpike : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 30;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        private bool spawned;
        public override void AI()
        {
            if (!spawned)
            {
                spawned = true;
                SoundEngine.PlaySound(SoundID.Item30);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity.Y = Projectile.velocity.Y + 0.15f;
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight, 0f, 0f, 255);
            dust.noGravity = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Width() * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw((Texture2D)TextureAssets.Projectile[Projectile.type], drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            return true;
        }

        public override void PostDraw(Color lightColor)
        {
        Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Rimegeist.AssetDirectory + "IcySpike_Glowmask");
            Vector2 drawPos = Projectile.Center + new Vector2(0, Projectile.gfxOffY) - Main.screenPosition;
            //keep an eye on the width and height when doing this. It matters
            Main.EntitySpriteDraw
            (
                texture,
                drawPos,
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                Projectile.rotation,
                texture.Size() * 0.5f,
                Projectile.scale,
                SpriteEffects.None, //adjust this according to the sprite
                0
                );
        }

    }

    public class IceCube : WispSouls
    {
        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
        }
		public override bool PreDraw(ref Color lightColor)
        {
            if (Rimegeist.rimegeist != null && Rimegeist.rimegeist.PhaseTwo > 0)
                base.PreDraw(ref lightColor);

                return true;
        }
        public override void AI()
        {
            if (Rimegeist.rimegeist != null && Rimegeist.rimegeist.PhaseTwo>0)
            {
                if (Projectile.velocity.Length() < 3)
                {
                    Projectile.velocity = -Vector2.UnitY * 8f;
                }
                base.AI();
            }

            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight, 0f, 0f, 255);
            dust.noGravity = true;
        }
        public override void Kill(int timeLeft)
        {

            if (Main.netMode != 1)
            {
                int type = ProjectileID.FrostShard;
                float speed = 6f;
                int damage = 10;
                Vector2 position = Projectile.Center;
                for (int i = 0; i < 8; i++)
                {
                    Vector2 perturbedSpeed = new Vector2(speed, speed).RotatedBy(MathHelper.ToRadians(i * 45));
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, 2f, Main.myPlayer);
                }
            }
        }
    }

    public class IceBolt : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 12;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {

            Projectile.rotation = Projectile.velocity.ToRotation();
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight, 0f, 0f, 255);
            dust.noGravity = true;
        }
		public override bool PreDraw(ref Color lightColor)
        {

            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Width() * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw((Texture2D)TextureAssets.Projectile[Projectile.type], drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            return true;
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Rimegeist.AssetDirectory+"IceBolt_Glowmask");
            Vector2 drawPos = Projectile.Center + new Vector2(0, Projectile.gfxOffY) - Main.screenPosition;
            //keep an eye on the width and height when doing this. It matters
            Main.EntitySpriteDraw
            (
                texture,
                drawPos,
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                Projectile.rotation,
                texture.Size() * 0.5f,
                Projectile.scale,
                SpriteEffects.None, //adjust this according to the sprite
                0
                );
        }
    }

    public class IceBlast : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.FrostBlastHostile;
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.FrostBlastHostile);
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            AIType = ProjectileID.FrostBlastHostile;
        }
    }
    public class HomingWispSouls : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 25;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            Main.projFrames[Projectile.type] = 6;
        }
		public override bool PreDraw(ref Color lightColor)
        {
            // Vector2 drawOrigin = new Vector2((Texture2D)TextureAssets.Projectile[projectile.type].Width, (Texture2D)TextureAssets.Projectile[projectile.type].Height);
            Texture2D texture2D = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                float scale = Projectile.scale * (Projectile.oldPos.Length - k) / Projectile.oldPos.Length * .45f;
                Vector2 drawPos = Projectile.oldPos[k]+Projectile.Hitbox.Size()/2f - Main.screenPosition;
                Color color = Projectile.GetAlpha(Color.Black) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);

                Main.EntitySpriteDraw(texture2D, drawPos, null, color, Projectile.rotation, texture2D.Size()/2f, scale, SpriteEffects.None, 0);
            }

            texture2D = (Texture2D)TextureAssets.Projectile[Projectile.type];

            Rectangle rect = new Rectangle(0, (Projectile.frame % (Main.projFrames[Projectile.type])) * (texture2D.Height/Main.projFrames[Projectile.type]), texture2D.Width, texture2D.Height/ Main.projFrames[Projectile.type]);

            Main.EntitySpriteDraw(texture2D, Projectile.Center - Main.screenPosition, rect, Color.White * Projectile.Opacity, Projectile.velocity.X/20f, rect.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 56;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.timeLeft = 180;
        }
        int radians = 16;
        int Timer = 0;
        int counter = 10;
        int i;
        private bool spawned;
        public override void AI()
        {

            Projectile.Opacity = MathHelper.Clamp(Projectile.timeLeft/20f,0f,1f);

            if (!spawned)
            {
                spawned = true;
                SoundEngine.PlaySound(SoundID.Item105);
            }

            Projectile.rotation = MathHelper.ToRadians(180) + Projectile.velocity.ToRotation();
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
            }
            float approaching = ((540f - Projectile.timeLeft) / 540f);

            Player player = Main.player[(int)Projectile.ai[0]];
            if (player.active)
            {
                float x = Main.rand.Next(-10, 11) * 0.001f * approaching;
                float y = Main.rand.Next(-10, 11) * 0.001f * approaching;
                Vector2 toPlayer = Projectile.Center - player.Center;
                toPlayer = toPlayer.SafeNormalize(Vector2.Zero);
                Projectile.velocity += -toPlayer * (0.155f * Projectile.timeLeft / 540f) + new Vector2(x, y);
            }
        }
    }
    public class WispSouls : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 25;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
		public override bool PreDraw(ref Color lightColor)
        {
            // Vector2 drawOrigin = new Vector2((Texture2D)TextureAssets.Projectile[projectile.type].Width, (Texture2D)TextureAssets.Projectile[projectile.type].Height);
            Texture2D texture2D = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
            Vector2 origin = new Vector2(texture2D.Width / 2, texture2D.Height / 2);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                float scale = Projectile.scale * (Projectile.oldPos.Length - k) / Projectile.oldPos.Length * .45f;
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + TextureAssets.Projectile[Projectile.type].Size() / 3f;
                Color color = Projectile.GetAlpha(Color.Black) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture2D, drawPos, null, color, Projectile.rotation, origin, scale, SpriteEffects.None, 0);
            }
            return true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 17;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.height = 18;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.timeLeft = 180;
        }
        int radians = 16;
        int Timer = 0;
        private bool spawned;
        public override void AI()
        {

            Projectile.Opacity = MathHelper.Clamp(Projectile.timeLeft / 20f, 0f, 1f);

            if (!spawned)
            {
                spawned = true;
                SoundEngine.PlaySound(SoundID.Item104);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity *= 1.03f;

            if (Timer < 60)
                Projectile.velocity /= 1.03f;


            Projectile.ai[0] += 1;

            if (Projectile.ai[0] >= 8)
            {
                Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedByRandom(MathHelper.ToRadians(radians));
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
                Projectile.velocity.Y = perturbedSpeed.Y;
                Projectile.velocity.X = perturbedSpeed.X;
                Projectile.ai[0] = 0;
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
            Projectile.width = 76;
            Projectile.height = 60;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
        }
        public override void AI()
        {
            Player player = Main.player[(int)Projectile.ai[0]];
            if (!player.active || player.dead)
            {
                Projectile.Kill();
                return;
            }
            Projectile.velocity *= 0.985f;
            Vector2 direction = Projectile.DirectionTo(player.Center);
            Projectile.velocity += direction * 0.02f;
            Projectile.velocity.Y += 0.09f;
            if (runOnce)
            {
                SoundEngine.PlaySound(SoundID.Item103);
                randomModifier1 = Main.rand.NextFloat(-1f, 1.75f);
                randomModifier2 = Main.rand.NextFloat(-24, 24);
                rotationCounter = Main.rand.NextFloat(360);
                runOnce = false;
                if (Main.myPlayer == player.whoAmI)
                    Projectile.netUpdate = true;
            }
            if (Main.rand.NextBool(25) && rotationCounter > 10)
            {
                int num1 = Dust.NewDust(new Vector2(Projectile.Center.X, Projectile.Center.Y) - new Vector2(5), 0, 0, DustID.RainbowMk2);
                Dust dust = Main.dust[num1];
                dust.velocity *= 0.7f;
                dust.noGravity = true;
                dust.color = new Color(130, 130, 130, 0);
                dust.fadeIn = 0.2f;
                dust.scale = 1.2f;
            }
        }
		public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = (Texture2D)TextureAssets.Projectile[Projectile.type];
            Vector2 origin = new Vector2(texture.Width / 2, Projectile.height / 2);
            Color color = new Color(130, 130, 150, 0);
            for (int i = 0; i < 360; i += 60)
            {
                Vector2 circular = new Vector2(Main.rand.NextFloat(1.5f, 2.5f), 0).RotatedBy(MathHelper.ToRadians(i * 2.5f));
                Main.EntitySpriteDraw(texture, Projectile.Center + circular - Main.screenPosition, null, color * ((255f - Projectile.alpha) / 255f), Projectile.rotation, origin, Projectile.scale * 0.8f, SpriteEffects.None, 0);
            }
            //color = projectile.GetAlpha(Color.White);
            //Main.Main.EntitySpriteDraw(texture, projectile.Center - Main.screenPosition, null, color, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0.0f);
            return false;
        }
        public override void Kill(int timeLeft)
        {
            if (Main.netMode != 1)
            {
                int type = ModContent.ProjectileType<WispSouls>();
                float speed = 6f;
                int damage = 10;
                Vector2 position = Projectile.Center;
                for (int i = 0; i < 8; i++)
                {
                    Vector2 perturbedSpeed = new Vector2(speed, speed).RotatedBy(MathHelper.ToRadians(i * 45));
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, 2f, Main.myPlayer);
                }
            }
        }
    }



    public class IcyLaserProj : ModProjectile
    {

        public override string Texture => "Terraria/Projectile_" + ProjectileID.None;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;

            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 5;

            Projectile.timeLeft = 180;
        }

        int Timer = 0;
        private bool spawned;
        public override void AI()
        {
            if (!spawned)
            {
                spawned = true;
                SoundEngine.PlaySound(SoundID.Item30);
            }
            Projectile.velocity *= 1.003f;

            for (int j = 0; j < 80; j++)
            {
                float x = Projectile.position.X - Projectile.velocity.X / 10f * (float)j;
                float y = Projectile.position.Y - Projectile.velocity.Y / 10f * (float)j;
                Dust dust = Dust.NewDustDirect(new Vector2(x, y), 1, 1, 20, 0, 0, 0, Color.Blue, 0.9f);
                dust.position.X = x;
                dust.position.Y = y;
                dust.velocity *= 0f;
                dust.noGravity = true;
            }
            if (++Projectile.localAI[1] > 10)
            {
                float amountOfDust = 16f;
                for (int i = 0; i < amountOfDust; ++i)
                {
                    Vector2 spinningpoint5 = -Vector2.UnitY.RotatedBy(i * (MathHelper.TwoPi / amountOfDust)) * new Vector2(1f, 4f);
                    spinningpoint5 = spinningpoint5.RotatedBy(Projectile.velocity.ToRotation());

                    Dust dust = Dust.NewDustPerfect(Projectile.Center + spinningpoint5, 20, spinningpoint5, 0, Color.Blue, 1.3f);
                    dust.noGravity = true;
                }

                Projectile.localAI[1] = 0;
            }
        }
    }
}

