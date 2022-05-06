using AerovelenceMod.Content.Items.TreasureBags;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.Rimegeist
{
    public class GhastlyGlacier : ModNPC
    {


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
            NPC.width = 246;
            NPC.height = 300;
            NPC.value = Item.buyPrice(0, 5, 75, 45);
            NPC.npcSlots = 1f;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCHit5;
            NPC.buffImmune[24] = true;
            bossBag = ModContent.ItemType<RimegeistBag>();
            music = Mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Rimegeist");
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Rimegeist/Glowmask").Value;
            Vector2 drawPos = NPC.Center + new Vector2(0, NPC.gfxOffY) - Main.screenPosition;
            //keep an eye on the width and height when doing this. It matters
            spriteBatch.Draw(texture, drawPos, NPC.frame, Color.White, NPC.rotation, texture.Size() * 0.5f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        }
        public override void NPCLoot()
        {
            if (Main.expertMode)
            {
                NPC.DropBossBags();
            }
            if (!Main.expertMode)
            {
                Item.NewItem((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.HealingPotion, Main.rand.Next(4, 12), false, 0, false, false);
                Item.NewItem((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, Mod.Find<ModItem>("FrostShard").Type, Main.rand.Next(10, 20), false, 0, false, false);
                switch (Main.rand.Next(5))
                {
                    case 0:
                        Item.NewItem((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, Mod.Find<ModItem>("CrystalArch").Type, 1, false, 0, false, false);
                        break;
                    case 1:
                        Item.NewItem((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, Mod.Find<ModItem>("DeepFreeze").Type, 1, false, 0, false, false);
                        break;
                    case 2:
                        Item.NewItem((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, Mod.Find<ModItem>("IcySaber").Type, 1, false, 0, false, false);
                        break;
                    case 3:
                        Item.NewItem((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, Mod.Find<ModItem>("CryoBall").Type, 1, false, 0, false, false);
                        break;
                    case 4:
                        Item.NewItem((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, Mod.Find<ModItem>("Snowball").Type, 1, false, 0, false, false);
                        break;
                }
            }
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = 10000;  //boss life scale in expertmode
            NPC.damage = 40;  //boss damage increase in expermode
        }
    }
}