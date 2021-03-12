using AerovelenceMod.Items.BossBags;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.NPCs.Bosses.Rimegeist
{
    [AutoloadBossHead]
    public class Rimegeist : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 1;    //(boss frame/animation) change this to whatever value once the boss got animations
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1; // be sure to keep the value of this to -1 or else the AI will be broken
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
            bossBag = ModContent.ItemType<RimegeistBag>();
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Rimegeist");
        }
        private enum RimegeistState
        {
            Attack1 = 0 // name will be changed

        }
        private RimegeistState State
        {
            get => (RimegeistState)npc.ai[0];
            set => npc.ai[0] = (float)value;
        }
        public int i;
        private float AttackTimer
        {
            get => npc.ai[1];
            set => npc.ai[1] = value;
        }
        private float StateTimer
        {
            get => npc.ai[2];
            set => npc.ai[2] = value;
        }
        public bool Spawn = true;
        public bool phaseTwo
        {
            get { return npc.life < npc.lifeMax / 2; }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 11500;  //boss life scale in expertmode
            npc.damage = 40;  //boss damage increase in expermode
        }

        public override void AI()
        {

            Player player = Main.player[npc.target];
            // npc.velocity = (player.Center - npc.Center) / 50;
            if (!player.active || player.dead) //player check
            {
                npc.noTileCollide = true;
                npc.TargetClosest(false);
                npc.velocity.Y = 20f;
                if (npc.timeLeft > 10)
                {
                    npc.timeLeft = 10;
                }
            }
            if (Spawn)// spawning visuals
            {
                npc.velocity = Vector2.Zero;
                npc.alpha = 30;

                CoolDust(npc.Center, 1, DustID.ApprenticeStorm);
                CoolDust(npc.Center, 2, DustID.ApprenticeStorm);


                if (++AttackTimer >= 200)
                {
                    CoolDust(npc.Center, 3, DustID.Ice);
                    Spawn = false;
                    AttackTimer = 0;
                }

            }
            else
            {
                StateCheck();
            }

            if (State == RimegeistState.Attack1)
            {


            }
        }
        ///<summary>
        ///Checks and changes states based on the timer.
        ///</summary>
        private void StateCheck()// better state management i guess
        {
            StateTimer++;
            if (StateTimer <= 200)
            {
                State = RimegeistState.Attack1;
            }
        }

        ///<summary>
        ///Spawn dust based on pre-made dust patterns.
        ///</summary>
        private void CoolDust(Vector2 pos, int pattern, int dustType, int size = 70, float fadeIn = 0f)// just don't want to clog AI() with dust codes (will be expanded)
        {                                                                                             
            float t = (float)Main.time * 0.1f;// this is synced i think


            if (pattern == 1) //infinity pattern
            {
                var dust = Dust.NewDustPerfect(pos, dustType, null, 100, Color.White, 2.5f);

                dust.noGravity = true;
                dust.position = pos + new Vector2(size * 2 * (float)Math.Cos(t), size * (float)Math.Sin(2 * t));
                dust.fadeIn = fadeIn;
            }

            if (pattern == 2) //dust vortex pattern
            {
                Vector2 dustPos = pos + Main.rand.NextVector2CircularEdge(size * 2, size * 2);

                float rotation = (float)Math.Atan2(dustPos.Y - npc.Center.Y, dustPos.X - npc.Center.X);

                float speedX = (float)((Math.Cos(rotation) * 10f) * -1);
                float speedY = (float)((Math.Sin(rotation) * 10f) * -1);

                for (int i = 0; i < 7; i++)
                {
                    Dust dust = Main.dust[Dust.NewDust(dustPos, 1, 1, dustType, speedX, speedY, 100, default, 2f)];

                    dust.noGravity = true;
                    dust.fadeIn = fadeIn;
                }
            }

            if (pattern == 3) //dust explosion
            {                 //NOTE: don't use this if the method doesn't execute for only one time

                for (int i = 0; i < 10; i++)
                {
                    int dustIndex = Dust.NewDust(npc.position, npc.width, npc.height, dustType);
                    Dust dust = Main.dust[dustIndex];

                    dust.velocity.X = dust.velocity.X + Main.rand.Next(-10, 10) * 0.01f;
                    dust.velocity.Y = dust.velocity.Y + Main.rand.Next(-65, -48) * 0.01f;

                    dust.scale *= 2f + Main.rand.Next(-30, 31) * 0.01f;
                    dust.noGravity = true;
                }

            }
        }
    }
}
