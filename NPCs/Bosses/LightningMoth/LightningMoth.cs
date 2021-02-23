using AerovelenceMod.Items.BossBags;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.NPCs.Bosses.LightningMoth
{
    [AutoloadBossHead]
    public class LightningMoth : ModNPC
    {

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 26;    //boss frame/animation 
        }
        public override void SetDefaults()
        {
            npc.lifeMax = 9500;   //boss life
            npc.damage = 32;  //boss damage
            npc.defense = 9;    //boss defense
            npc.alpha = 0;
            npc.knockBackResist = 0f;
            npc.width = 222;
            npc.height = 174;
            npc.value = Item.buyPrice(0, 5, 75, 45);
            npc.npcSlots = 1f;
            npc.boss = true;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit44;
            npc.DeathSound = SoundID.NPCHit46;
            npc.buffImmune[24] = true;
       //     bossBag = ModContent.ItemType<SnowriumBag>();
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Snowrium");
        }
        private int cooldownFrames
        {
            get
            {
                return (int)npc.ai[0];
            }
            set
            {
                npc.ai[0] = value;
            }
        }
        private CurrentAttack currentAttack
        {
            get
            {
                return (CurrentAttack)(int)npc.ai[1];
            }
            set
            {
                npc.ai[1] = (int)value;
            }
        }
        private enum CurrentAttack
        {
            IdleFloat = 0,
            Dash = 1,
            LightningStorm = 2,
            SummonMoths = 3,
            BlastVolleys = 4,
            Divebomb = 5,
            CrystalStomp = 6,
            CrystalSpin = 7,
            JumpAtPlayer = 8,
            SummonJolts = 9
        }
        bool grounded = false;
        float ZaxisRotation = 0;
        public bool phaseTwo
        {
            get{return npc.life < npc.lifeMax / 2;}
        }
        public override void AI()
        {
            npc.TargetClosest();

            if (cooldownFrames <= 0)
            {
                Main.NewText(currentAttack);
                switch(currentAttack)
                {
                    case CurrentAttack.IdleFloat:
                        IdleFloat();
                        break;
                    case CurrentAttack.Dash:
                        Dash();
                        break;
                    case CurrentAttack.LightningStorm:
                        LightningStorm();
                        break;
                    case CurrentAttack.SummonMoths:
                        cooldownFrames = 2;
                        break;
                    case CurrentAttack.Divebomb:
                        cooldownFrames = 2;
                        break;
                    case CurrentAttack.CrystalStomp:
                        cooldownFrames = 2;
                        break;
                    case CurrentAttack.CrystalSpin:
                        cooldownFrames = 2;
                        break;
                    case CurrentAttack.JumpAtPlayer:
                        cooldownFrames = 2;
                        break;
                    case CurrentAttack.SummonJolts:
                        cooldownFrames = 2;
                        break;
                    default:
                        Main.NewText("Error");
                        cooldownFrames = 2;
                        break;
                }
            }
            else
            {
                if (cooldownFrames == 1)
                {
                    int attack = grounded ? Main.rand.Next(3) + 5 : Main.rand.Next(5);
                    if (!grounded && phaseTwo && Main.rand.Next(8) == 0)
                    {
                        attack = 8;
                    }
                    currentAttack = (CurrentAttack)attack;
                }
                npc.velocity *= 0.97f;
                attackCounter = 0;
                cooldownFrames--;
                if (!grounded)
                {
                    UpdateFrame(0.3f, 0, 6);
                }
            }
        }
        public float trueFrame;
        public override void FindFrame(int frameHeight)
        {
            npc.frame.Y = (int)trueFrame * frameHeight;
        }
        internal void UpdateFrame(float speed, int minFrame, int maxFrame)
        {
            trueFrame += speed;
			if (trueFrame < minFrame) 
			{
				trueFrame = minFrame;
			}
			if (trueFrame > maxFrame) 
			{
				trueFrame = minFrame;
			}
        }

        #region attacks
        int attackCounter;
        private void IdleFloat()
        {
            UpdateFrame(0.3f, 0, 6);
            Player player = Main.player[npc.target];
            attackCounter++;
            if (attackCounter > 200)
            {
                cooldownFrames = 30;
                return;
            }

            npc.velocity.Y += Math.Sign((player.position.Y - 300) - npc.Center.Y) * (phaseTwo ? 0.2f : 0.3f);
            if (npc.velocity.Y > 20)
                npc.velocity.Y = 20;
            if (npc.velocity.Y < -20)
                npc.velocity.Y = -20;

            npc.velocity.X += Math.Sign(player.position.X - npc.Center.X) * (phaseTwo ? 0.2f : 0.3f);
             if (npc.velocity.X > 20)
                npc.velocity.X = 20;
            if (npc.velocity.X < -20)
                npc.velocity.X = -20;
        }
        Vector2 dashDirection;
        private void Dash()
        {
            Player player = Main.player[npc.target];

            attackCounter++;
            if (attackCounter >= 300)
            {
                cooldownFrames = 30;
                return;
            }
            if (attackCounter % 90 == 35)
            {
               dashDirection = player.Center - npc.Center;
               dashDirection.Normalize();
                dashDirection*= 25;
            }
            if (attackCounter % 90 < 50)
            {
                UpdateFrame(0.1f, 0, 6);
               npc.velocity *= 0.98f;
            }
            else
            {
                UpdateFrame(0.3f, 0, 6);
                 npc.velocity = dashDirection;
            }

        }

        private void LightningStorm()
        {
            UpdateFrame(0.3f, 0, 6);
            attackCounter++;
            if (attackCounter >= 300)
            {
                cooldownFrames = 30;
                return;
            }
            npc.velocity = Vector2.Zero;
        }
        #endregion
    }
}