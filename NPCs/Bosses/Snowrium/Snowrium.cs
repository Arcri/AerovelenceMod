using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Projectiles;
using AerovelenceMod.Items.BossBags;
using Microsoft.Xna.Framework.Graphics;
using AerovelenceMod.NPCs.Bosses.Snowrium.Projectiles;
using System.Security.Cryptography.X509Certificates;

namespace AerovelenceMod.NPCs.Bosses.Snowrium
{
    [AutoloadBossHead]
    public class Snowrium : ModNPC
    {
        int t;
        bool PhaseTwo;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 8;    //boss frame/animation 
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;  //5 is the flying AI
            npc.lifeMax = 4800;   //boss life
            npc.damage = 32;  //boss damage
            npc.defense = 24;    //boss defense
            npc.knockBackResist = 0f;
            npc.width = 130;
            npc.height = 98;
            npc.value = Item.buyPrice(0, 5, 75, 45);
            npc.npcSlots = 1f;
            npc.boss = true;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit5;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.buffImmune[24] = true;
            bossBag = ModContent.ItemType<SnowriumBag>();
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Snowrium");
        }


        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = mod.GetTexture("NPCs/Bosses/Snowrium/Glowmask");
            Vector2 drawPos = npc.Center + new Vector2(0, npc.gfxOffY) - Main.screenPosition;
            //keep an eye on the width and height when doing this. It matters
            spriteBatch.Draw
            (
                texture,
                drawPos,
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                npc.rotation,
                texture.Size() * 0.5f,
                npc.scale,
                npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, //adjust this according to the sprite
                0f
                );
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
            npc.lifeMax = 5600;  //boss life scale in expertmode
            npc.damage = 20;  //boss damage increase in expermode
        }

        public override void AI()
        {
            t++;
            npc.TargetClosest(true);
            var player = Main.player[npc.target];
            if (player.Center.X > npc.Center.X)
            {
                if (npc.velocity.X < 6)
                {
                    npc.velocity.X += 0.15f;
                }
            }
            if (player.Center.X < npc.Center.X)
            {
                if (npc.velocity.X > -6)
                {
                    npc.velocity.X -= 0.15f;
                }
            }

            if (!(t % 400 >= 0 && t % 500 <= 50))
            {
                if (player.Center.Y > npc.Center.Y + 250)
                {
                    if (npc.velocity.Y < 4)
                    {
                        npc.velocity.Y += 0.2f;
                    }
                }
            }
            else
            {
                if (player.Center.Y > npc.Center.Y)
                {
                    if (npc.velocity.Y < 4)
                    {
                        npc.velocity.Y += 0.2f;
                    }
                }
            }
            if (!(t % 400 >= 0 && t % 500 <= 50))
            {
                if (player.Center.Y < npc.Center.Y + 250)
                {
                    if (npc.velocity.Y > -4)
                    {
                        npc.velocity.Y -= 0.2f;
                    }
                }
            }
            else
            {
                if (player.Center.Y < npc.Center.Y)
                {
                    if (npc.velocity.Y > -4)
                    {
                        npc.velocity.Y -= 0.2f;
                    }
                }
            }



            npc.rotation = npc.velocity.X * 0.1f;
            if (t % 100 == 0)
            {
                Projectile.NewProjectile(npc.Center, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, 5 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<SnowriumFlailProjectile>(), 12, 1f, Main.myPlayer);

            }
        }


        private const int Frame_Snowrium_1 = 1;
        private const int Frame_Snowrium_2 = 2;
        private const int Frame_Snowrium_3 = 3;
        private const int Frame_Snowrium_4 = 4;
        private const int Frame_Snowrium_5 = 5;
        private const int Frame_Snowrium_6 = 6;
        private const int Frame_Snowrium_7 = 7;
        private const int Frame_Snowrium_8 = 8;


        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter < 0)
            {
                npc.frame.Y = Frame_Snowrium_1 * frameHeight;
            }
            else if (npc.frameCounter < 10)
            {
                npc.frame.Y = Frame_Snowrium_2 * frameHeight;
            }
            else if (npc.frameCounter < 20)
            {
                npc.frame.Y = Frame_Snowrium_3 * frameHeight;
            }
            else if (npc.frameCounter < 30)
            {
                npc.frame.Y = Frame_Snowrium_4 * frameHeight;
            }
            else
            {
                npc.frameCounter = 0;
            }
            if (npc.life < 2000)
            {
                

                if (npc.frameCounter < 50)
                {
                    npc.frame.Y = Frame_Snowrium_5 * frameHeight;
                }
                else if (npc.frameCounter < 60)
                {
                    npc.frame.Y = Frame_Snowrium_6 * frameHeight;
                }
                else if (npc.frameCounter < 70)
                {
                    npc.frame.Y = Frame_Snowrium_7 * frameHeight;
                }
                else if (npc.frameCounter < 80)
                {
                    npc.frame.Y = Frame_Snowrium_8 * frameHeight;
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