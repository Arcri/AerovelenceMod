using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.LightningMoth
{
    [AutoloadBossHead]
    public class LightningMoth : ModNPC
    {

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 7;    //boss frame/animation 
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
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			var effects = npc.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			spriteBatch.Draw(Main.npcTexture[npc.type], new Vector2(npc.Center.X, npc.Center.Y) - Main.screenPosition + new Vector2(0, npc.gfxOffY), npc.frame, lightColor, npc.rotation, npc.frame.Size() / 2, npc.scale, effects, 0);
			return false;
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
        public bool phaseTwo => npc.life < npc.lifeMax / 2;
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
                    case CurrentAttack.BlastVolleys:
                        cooldownFrames = 2;
                        break;
                    case CurrentAttack.Divebomb:
                        DiveBomb();
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
                    int attack = grounded ? Main.rand.Next(3) + 6 : Main.rand.Next(6);
                    if (!grounded && phaseTwo && Main.rand.Next(8) == 0)
                    {
                        attack = 9;
                    }
                    currentAttack = (CurrentAttack)attack;
                }
                npc.velocity *= 0.97f;
                attackCounter = 0;
                cooldownFrames--;
                if (!grounded)
                {
                    UpdateFrame(0.3f, 0, 6);
                    npc.noGravity = true;
                    npc.noTileCollide = true;
                }
                else
                {
                    UpdateFrame(0.3f, 8, 17);
                    npc.noGravity = false;
                    npc.noTileCollide = false;
                }
            }
        }
        public float trueFrame;
        public override void FindFrame(int frameHeight)
		{
			npc.frame.Width = 268;
			npc.frame.X = ((int)trueFrame % 4) * npc.frame.Width;
			npc.frame.Y = (((int)trueFrame - ((int)trueFrame % 4)) / 4) * npc.frame.Height;
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
        Vector2 posToBe = Vector2.Zero;
        bool diveBombed = false;
        private void DiveBomb()
        {
            UpdateFrame(0.3f, 7, 7);
             Player player = Main.player[npc.target];
            if (attackCounter == 0)
            {
                diveBombed = false;
                attackCounter = 1;
            }   
            if (!diveBombed)
            {
                if (attackCounter == 1)
                {
                    posToBe = player.Center - new Vector2(0, 500);
                }
                Vector2 direction = posToBe - npc.Center;
                float lerpSpeed = (float)Math.Sqrt(direction.Length());
                direction.Normalize();
                direction *= lerpSpeed;
                npc.velocity = direction;
                if (lerpSpeed < 10)
                {
                    attackCounter++;
                    if (attackCounter > 30)
                    {
                        diveBombed = true;
                    }
                }
            }
            else
            {
                attackCounter++;
                npc.velocity.Y = 30;
                npc.velocity.X = 0;
                npc.noTileCollide = false;
                if (Main.netMode != NetmodeID.MultiplayerClient && (Main.tile[(int)(npc.Center.X / 16), (int)((npc.Center.Y + 150) / 16)].collisionType == 1 || attackCounter > 75))
                {
                    grounded = true;
                    cooldownFrames = 30;
                    return;
                }
            }
        }
        #endregion
    }
}