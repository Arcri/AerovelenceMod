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
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
        Texture2D texture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Rimegeist/Glowmask").Value;
            Vector2 drawPos = NPC.Center + new Vector2(0, NPC.gfxOffY) - Main.screenPosition;
            //keep an eye on the width and height when doing this. It matters
            Main.EntitySpriteDraw(texture, drawPos, NPC.frame, Color.White, NPC.rotation, texture.Size() * 0.5f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            NPC.lifeMax = 10000;  //boss life scale in expertmode
            NPC.damage = 40;  //boss damage increase in expermode
        }
    }
}