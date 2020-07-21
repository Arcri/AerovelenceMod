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
        private int shootTimer5;
        private int stobit;
        private int stobit2;
        private int plzstop;
        private int ebic;
        private int h;
        public float hhhh;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 10;    //boss frame/animation 
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
            npc.lifeMax = 11500;  //boss life scale in expertmode
            npc.damage = 40;  //boss damage increase in expermode
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
            if (npc.life <= npc.lifeMax / 2)
            {
                Phase2 = true;
            }

            npc.ai[0]++;
            if (npc.ai[0] >= 600)
            {
                NPC.NewNPC((int)npc.position.X, (int)npc.position.Y, ModContent.NPCType<ShiverSpirit>());
                npc.ai[0] = 0;
            }
            npc.TargetClosest(true);
            Vector2 vector8 = new Vector2(npc.position.X + (npc.width * 0.5f), npc.position.Y + (npc.height * 0.5f));
            Vector2 move = player.position - npc.Center;
            if (!Phase2)
            {
                progTimer1++;
                if (progTimer1 >= 0 && progTimer1 < 360)
                {
                    // follows player from the top and shoots a blast of icy spikes
                    npc.rotation = 0;
                    npc.velocity.X = ((10 * npc.velocity.X + move.X) / 20f);
                    npc.velocity.Y = ((10 * npc.velocity.Y + move.Y - 300) / 20f);

                    int type = mod.ProjectileType("IcySpike");
                    int damage = Main.expertMode ? 8 : 2;// if u want to change this, 15 is for expert mode, 10 is for normal mod
                    float speedX = 10f;
                    float speedY = 10f;
                    Vector2 position = npc.Center;
                    shootTimer++;
                    if (shootTimer >= 90)
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
                        shootTimer = 0;
                    }
                }
                if (progTimer1 >= 360 && progTimer1 < 660)
                {
                    // follows player from right and dash to left after 4 secs
                    dashTimer++;
                    if (dashTimer >= 0 && dashTimer < 240)
                    {
                        npc.velocity.X = ((10 * npc.velocity.X + move.X + 300) / 20f);
                        npc.velocity.Y = ((10 * npc.velocity.Y + move.Y) / 20f);
                        stobit++;
                        if (stobit >= 40)
                        {
                            float Speed = 7f;
                            int damage = Main.expertMode ? 11 : 2;// if u want to change this, 15 is for expert mode, 10 is for normal mod
                            int type = mod.ProjectileType("IceBlast");
                            float rotation = (float)Math.Atan2(vector8.Y - (player.position.Y + (player.height * 0.5f)), vector8.X - (player.position.X + (player.width * 0.5f)));
                            Projectile.NewProjectile(vector8.X, vector8.Y, (float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1), type, damage, 0f, 0);
                            stobit = 0;
                        }
                    }
                    if (dashTimer >= 240)
                    {
                        npc.velocity.Y = 0;
                        npc.velocity.X = -15f;
                    }
                }
                if (progTimer1 >= 660 && progTimer1 < 1260)
                {
                    // normal follow player and shoots a spead (3) icy bolt
                    dashTimer = 0;
                    Vector2 playerPos = player.position + new Vector2(0, -300);
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
                    int damage = Main.expertMode ? 7 : 2;// if u want to change this, 15 is for expert mode, 10 is for normal mod
                    Vector2 position = npc.Center;
                    int type = mod.ProjectileType("IceBolt");
                    float rotate = (float)Math.Atan2(vector8.Y - (player.position.Y + (player.height * 0.5f)), vector8.X - (player.position.X + (player.width * 0.5f)));
                    shootTimer2++;
                    if (shootTimer2 >= 120)
                    {
                        float numberProjectiles = 6;
                        float rotation = MathHelper.ToRadians(10);
                        position += Vector2.Normalize(new Vector2((float)((Math.Cos(rotate) * 5f) * -1), (float)((Math.Sin(rotate) * 5f) * -1))) * 45f;
                        for (int i = 0; i < numberProjectiles; i++)
                        {
                            Vector2 perturbedSpeed = new Vector2((float)((Math.Cos(rotate) * 5f) * -1), (float)((Math.Sin(rotate) * 5f) * -1)).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 2f;
                            if (i == 1 || i == 3)
                            {
                                int proj1 = Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, 2f, player.whoAmI);
                                Main.projectile[proj1].velocity *= 1.25f;
                                shootTimer2 = 0;
                            }
                            else if (i == 2)
                            {
                                int proj2 = Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, 2f, player.whoAmI);
                                Main.projectile[proj2].velocity *= 1.5f;
                                shootTimer2 = 0;
                            }
                            else
                            {
                                int proj3 = Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, 2f, player.whoAmI);
                                shootTimer2 = 0;
                            }

                        }
                    }
                }
                if (progTimer1 >= 1260 && progTimer1 < 1500)
                {
                    // spins around the player
                    someValue += 3f;
                    npc.position = player.position + new Vector2(400, 0).RotatedBy(MathHelper.ToRadians(someValue));
                    npc.ai[1]++;
                    if (npc.ai[1] >= 30)
                    {
                        float Speed = 4f;
                        int damage = Main.expertMode ? 9 : 2;// if u want to change this, 15 is for expert mode, 10 is for normal mod
                        int type = mod.ProjectileType("IceBlast");
                        float rotation = (float)Math.Atan2(vector8.Y - (player.position.Y + (player.height * 0.5f)), vector8.X - (player.position.X + (player.width * 0.5f)));
                        Projectile.NewProjectile(vector8.X, vector8.Y, (float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1), type, damage, 0f, 0);
                        npc.ai[1] = 0;
                    }
                }
                if (progTimer1 >= 1500 && progTimer1 < 2160)
                {
                    // random spread bullet hell
                    npc.velocity = new Vector2(0f, 0f);
                    shootTimer3++;
                    int type = mod.ProjectileType("IceBolt");
                    npc.rotation += 0.1f;
                    int damage = Main.expertMode ? 8 : 2;// if u want to change this, 15 is for expert mode, 10 is for normal mod
                    float speedX = 7f;
                    float speedY = 7f;
                    Vector2 position = npc.Center;
                    Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(360));
                    if (shootTimer3 >= 10)
                    {
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, 2f, player.whoAmI);
                        shootTimer3 = 0;
                    }
                }
                if (progTimer1 >= 2160)
                {
                    progTimer1 = 0;
                }
            }
            if (Phase2)
            {
                progTimer2++;
                if (progTimer2 >= 1 && progTimer2 < 361)
                {
                    // follow player's top and shoots blast of icy spikes faster
                    npc.rotation = 0;
                    npc.velocity.X = ((10 * npc.velocity.X + move.X) / 20f);
                    npc.velocity.Y = ((10 * npc.velocity.Y + move.Y - 300) / 20f);

                    int type = mod.ProjectileType("IcySpike");
                    int damage = Main.expertMode ? 15 : 2;// if u want to change this, 15 is for expert mode, 10 is for normal mod
                    float speedX = 10f;
                    float speedY = 10f;
                    Vector2 position = npc.Center;
                    shootTimer5++;
                    if (shootTimer5 >= 90)
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
                        shootTimer5 = 0;
                    }
                }
                if (progTimer2 >= 361 && progTimer2 < 631)
                {
                    // follow player's right and shoot ice bolts faster, after 2.5 secs, dash to player at a faster speed gremer
                    dashTimer2++;
                    if (dashTimer2 >= 0 && dashTimer2 < 150)
                    {
                        npc.velocity.X = ((10 * npc.velocity.X + move.X + 300) / 20f);
                        npc.velocity.Y = ((10 * npc.velocity.Y + move.Y) / 20f);
                        stobit2++;
                        if (stobit2 >= 20)
                        {
                            float Speed = 7f;
                            int damage = Main.expertMode ? 9 : 2;// if u want to change this, 15 is for expert mode, 10 is for normal mod
                            int type = ProjectileID.FrostBlastHostile;
                            float rotation = (float)Math.Atan2(vector8.Y - (player.position.Y + (player.height * 0.5f)), vector8.X - (player.position.X + (player.width * 0.5f)));
                            Projectile.NewProjectile(vector8.X, vector8.Y, (float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1), type, damage, 0f, 0);
                            stobit2 = 0;
                        }
                    }
                    if (dashTimer2 >= 150)
                    {
                        npc.velocity.Y = 0;
                        npc.velocity.X = -20f;
                    }
                }
                if (progTimer2 >= 631 && progTimer2 < 1231)
                {
                    // normal follow player and shoot a spread (6) ice bolts
                    dashTimer2 = 0;
                    Vector2 playerPos = player.position + new Vector2(0, -300);
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
                    int damage = Main.expertMode ? 5 : 2;// if u want to change this, 15 is for expert mode, 10 is for normal mod
                    Vector2 position = npc.Center;
                    int type = mod.ProjectileType("IceBolt");
                    float rotate = (float)Math.Atan2(vector8.Y - (player.position.Y + (player.height * 0.5f)), vector8.X - (player.position.X + (player.width * 0.5f)));
                    shootTimer2++;
                    if (shootTimer2 >= 120)
                    {
                        float numberProjectiles = 8;
                        float rotation = MathHelper.ToRadians(10);
                        position += Vector2.Normalize(new Vector2((float)((Math.Cos(rotate) * 5f) * -1), (float)((Math.Sin(rotate) * 5f) * -1))) * 45f;
                        for (int i = 0; i < numberProjectiles; i++)
                        {
                            Vector2 perturbedSpeed = new Vector2((float)((Math.Cos(rotate) * 5f) * -1), (float)((Math.Sin(rotate) * 5f) * -1)).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 2f;
                            if (i == 1 || i == 3)
                            {
                                int proj1 = Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, 2f, player.whoAmI);
                                Main.projectile[proj1].velocity *= 1.25f;
                                shootTimer2 = 0;
                            }
                            else if (i == 2)
                            {
                                int proj2 = Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, 2f, player.whoAmI);
                                Main.projectile[proj2].velocity *= 1.5f;
                                shootTimer2 = 0;
                            }
                            else
                            {
                                int proj3 = Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, 2f, player.whoAmI);
                                shootTimer2 = 0;
                            }

                        }
                    }
                }
                if (progTimer2 >= 1231 && progTimer2 < 1471)
                {
                    // spins around the player faster and shoots more ice bolts
                    someValue += 3f;
                    npc.position = player.position + new Vector2(400, 0).RotatedBy(MathHelper.ToRadians(someValue));
                    plzstop++;
                    if (plzstop >= 15)
                    {
                        float Speed = 3f;
                        int damage = Main.expertMode ? 5 : 2;// if u want to change this, 15 is for expert mode, 10 is for normal mod
                        int type = mod.ProjectileType("IceBlast");
                        float rotation = (float)Math.Atan2(vector8.Y - (player.position.Y + (player.height * 0.5f)), vector8.X - (player.position.X + (player.width * 0.5f)));
                        Projectile.NewProjectile(vector8.X, vector8.Y, (float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1), type, damage, 0f, 0);
                        plzstop = 0;
                    }
                }
                if (progTimer2 >= 1471 && progTimer2 < 1951)
                {
                    // normal follow player, shoots less ice bolts, spawns ice projectiles on player top that explodes into 4 ice bolts
                    Vector2 playerPos = player.position + new Vector2(0, -300);
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

                    ebic++;
                    if (ebic >= 60)
                    {
                        float Speed = 7f;
                        int damage = Main.expertMode ? 5 : 2;// if u want to change this, 15 is for expert mode, 10 is for normal mod
                        int type = mod.ProjectileType("IceBlast");
                        int type2 = mod.ProjectileType("IceCube");
                        float rotation = (float)Math.Atan2(vector8.Y - (player.position.Y + (player.height * 0.5f)), vector8.X - (player.position.X + (player.width * 0.5f)));
                        Projectile.NewProjectile(vector8.X, vector8.Y, (float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1), type, damage, 0f, 0);
                        Projectile.NewProjectile(player.Center + new Vector2(0, -300), new Vector2(0, 0), type2, damage, 2f, player.whoAmI);
                        ebic = 0;
                    }
                }
                if (progTimer2 >= 1951 && progTimer2 < 2611)
                {
                    // faster random spread bullet hell
                    npc.velocity = new Vector2(0f, 0f);
                    h++;
                    int type = mod.ProjectileType("IceBolt");
                    npc.rotation += 0.1f;
                    int damage = Main.expertMode ? 5 : 2;// if u want to change this, 15 is for expert mode, 10 is for normal mod
                    float speedX = 7f;
                    float speedY = 7f;
                    Vector2 position = npc.Center;
                    Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(360));
                    Vector2 perturbedSpeed1 = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(360));
                    if (h >= 15)
                    {
                        Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, 2f, player.whoAmI);
                        Projectile.NewProjectile(position, perturbedSpeed1, type, damage, 2f, player.whoAmI);
                        h = 0;
                    }
                }

                if (progTimer2 >= 2611)
                {
                    progTimer2 = 0;
                }
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
namespace AerovelenceMod.NPCs.Bosses.Snowrium
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
            npc.defense = 2;
            npc.lifeMax = 100;
            npc.knockBackResist = 0.5f;

        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.width = 46;
            npc.height = 31;
            npc.damage = 10;
            npc.defense = 3;
            npc.lifeMax = 175;
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
namespace AerovelenceMod.NPCs.Bosses.Snowrium
{
    public class IcySpike : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 52;
            projectile.height = 30;
            projectile.damage = 15;
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
            Dust dust;
            Vector2 position = projectile.Center;
            dust = Main.dust[Terraria.Dust.NewDust(position, projectile.width, projectile.height, DustID.AncientLight, 0f, 0f, 255)];
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
            Texture2D texture = mod.GetTexture("NPCs/Bosses/Snowrium/IcySpike_Glowmask");
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
                projectile.scale,SpriteEffects.None, //adjust this according to the sprite
                0f
                );
        }
    }
}


namespace AerovelenceMod.NPCs.Bosses.Snowrium
{
    public class IceCube : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
            projectile.damage = 30;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 120;
        }

        public override void AI()
        {

            Dust dust;
            Vector2 position = projectile.Center;
            dust = Main.dust[Dust.NewDust(position, projectile.width, projectile.height, DustID.AncientLight, 0f, 0f, 255)];
            dust.noGravity = true;


        }
        public override void Kill(int timeLeft)
        {
            int type = ProjectileID.FrostShard;
            float speed = 6f;
            int damage = 10;
            Vector2 position = projectile.Center;
            Vector2 perturbedSpeed = new Vector2(speed, speed).RotatedBy(MathHelper.ToRadians(0));
            Vector2 perturbedSpeed1 = new Vector2(speed, speed).RotatedBy(MathHelper.ToRadians(45));
            Vector2 perturbedSpeed2 = new Vector2(speed, speed).RotatedBy(MathHelper.ToRadians(90));
            Vector2 perturbedSpeed3 = new Vector2(speed, speed).RotatedBy(MathHelper.ToRadians(135));
            Vector2 perturbedSpeed4 = new Vector2(speed, speed).RotatedBy(MathHelper.ToRadians(180));
            Vector2 perturbedSpeed5 = new Vector2(speed, speed).RotatedBy(MathHelper.ToRadians(225));
            Vector2 perturbedSpeed6 = new Vector2(speed, speed).RotatedBy(MathHelper.ToRadians(270));
            Vector2 perturbedSpeed7 = new Vector2(speed, speed).RotatedBy(MathHelper.ToRadians(315));
            Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, 2f);
            Projectile.NewProjectile(position.X, position.Y, perturbedSpeed1.X, perturbedSpeed1.Y, type, damage, 2f);
            Projectile.NewProjectile(position.X, position.Y, perturbedSpeed2.X, perturbedSpeed2.Y, type, damage, 2f);
            Projectile.NewProjectile(position.X, position.Y, perturbedSpeed3.X, perturbedSpeed3.Y, type, damage, 2f);
            Projectile.NewProjectile(position.X, position.Y, perturbedSpeed4.X, perturbedSpeed4.Y, type, damage, 2f);
            Projectile.NewProjectile(position.X, position.Y, perturbedSpeed5.X, perturbedSpeed5.Y, type, damage, 2f);
            Projectile.NewProjectile(position.X, position.Y, perturbedSpeed6.X, perturbedSpeed6.Y, type, damage, 2f);
            Projectile.NewProjectile(position.X, position.Y, perturbedSpeed7.X, perturbedSpeed7.Y, type, damage, 2f);
        }
    }
}
namespace AerovelenceMod.NPCs.Bosses.Snowrium
{
    public class IceBolt : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 12;
            projectile.damage = 30;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();
            Dust dust;
            Vector2 position = projectile.Center;
            dust = Main.dust[Terraria.Dust.NewDust(position, projectile.width, projectile.height, DustID.AncientLight, 0f, 0f, 255)];
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
            Texture2D texture = mod.GetTexture("NPCs/Bosses/Snowrium/IceBolt_Glowmask");
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



namespace AerovelenceMod.NPCs.Bosses.Snowrium
{
    public class IceBlast : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.FrostBlastHostile;
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.FrostBlastHostile);
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.damage = 25;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            aiType = ProjectileID.FrostBlastHostile;

        }

    }
}

namespace AerovelenceMod.NPCs.Bosses.Snowrium
{
    public class SnowriumFlailProjectile : ModProjectile
    {
        NPC npc1;


        private const string ChainTexturePath = "AerovelenceMod/NPCs/Bosses/Snowrium/Projectiles/SnowriumFlailProjectileChain";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Snowrium Flail");
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 28;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.melee = true;
        }
        public override void AI()
        {

            var dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 172, projectile.velocity.X * 0.4f, projectile.velocity.Y * 0.4f, 100, default, 1.5f);
            dust.noGravity = true;
            dust.velocity /= 2f;
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].FullName == "Snowrium")
                {
                    npc1 = Main.npc[i];
                }
            }
            var npc = npc1;
            npc1 = npc;
            if (!npc.active)
            {
                projectile.Kill();
                return;
            }
            int newDirection = projectile.Center.X > npc.Center.X ? 1 : -1;
            projectile.direction = newDirection;

            var vectorToPlayer = npc.Center - projectile.Center;
            float currentChainLength = vectorToPlayer.Length();
            if (projectile.ai[0] == 0f)
            {
                float maxChainLength = 600f;
                projectile.tileCollide = false;

                if (currentChainLength > maxChainLength)
                {
                    projectile.ai[0] = 1f;
                    projectile.netUpdate = true;
                }
            }
            else if (projectile.ai[0] == 1f)
            {
                float elasticFactorA = 10f;
                float elasticFactorB = 10f;
                float maxStretchLength = 600f;

                if (projectile.ai[1] == 1f)
                    projectile.tileCollide = false;

                if (currentChainLength > maxStretchLength || !projectile.tileCollide)
                {
                    projectile.ai[1] = 1f;

                    if (projectile.tileCollide)
                        projectile.netUpdate = true;

                    projectile.tileCollide = false;

                    if (currentChainLength < 20f)
                        projectile.Kill();
                }

                if (!projectile.tileCollide)
                    elasticFactorB *= 2f;

                int restingChainLength = 600;

                if (currentChainLength > restingChainLength || !projectile.tileCollide)
                {
                    var elasticAcceleration = vectorToPlayer * elasticFactorA / currentChainLength - projectile.velocity;
                    elasticAcceleration *= elasticFactorB / elasticAcceleration.Length();
                    projectile.velocity *= 0.98f;
                    projectile.velocity += elasticAcceleration;
                }
                else
                {
                    if (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y) < 6f)
                    {
                        projectile.velocity.X *= 0.96f;
                        projectile.velocity.Y += 0.2f;
                    }
                    if (npc.velocity.X == 0f)
                        projectile.velocity.X *= 0.96f;
                }
            }
            projectile.rotation = vectorToPlayer.ToRotation() - projectile.velocity.X * 0.1f;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(1f, 1f, 1f, 1f);
        }



        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bool shouldMakeSound = false;

            if (oldVelocity.X != projectile.velocity.X)
            {
                if (Math.Abs(oldVelocity.X) > 4f)
                {
                    shouldMakeSound = true;
                }

                projectile.position.X += projectile.velocity.X;
                projectile.velocity.X = -oldVelocity.X * 0.2f;
            }

            if (oldVelocity.Y != projectile.velocity.Y)
            {
                if (Math.Abs(oldVelocity.Y) > 4f)
                {
                    shouldMakeSound = true;
                }

                projectile.position.Y += projectile.velocity.Y;
                projectile.velocity.Y = -oldVelocity.Y * 0.2f;
            }
            projectile.ai[0] = 1f;

            if (shouldMakeSound)
            {
                projectile.netUpdate = true;
                Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
                Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y);
            }

            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var npc = npc1;

            Vector2 mountedCenter = npc.Center;
            Texture2D chainTexture = ModContent.GetTexture(ChainTexturePath);

            var drawPosition = projectile.Center;
            var remainingVectorToNpc = mountedCenter - drawPosition;

            float rotation = remainingVectorToNpc.ToRotation() - MathHelper.PiOver2;

            if (projectile.alpha == 0)
            {
                int direction = -1;

                if (projectile.Center.X < mountedCenter.X)
                    direction = 1;

                //player.itemRotation = (float)Math.Atan2(remainingVectorToPlayer.Y * direction, remainingVectorToPlayer.X * direction);
            }
            while (true)
            {
                float length = remainingVectorToNpc.Length();
                if (length < 25f || float.IsNaN(length))
                    break;
                drawPosition += remainingVectorToNpc * 12 / length;
                remainingVectorToNpc = mountedCenter - drawPosition;
                Color color = new Color(1f, 1f, 1f, 1f);
                spriteBatch.Draw(chainTexture, drawPosition - Main.screenPosition, null, color, rotation, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
            }

            return true;
        }
    }
}

