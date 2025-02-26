using System;
using System.Linq;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AerovelenceMod.Content.Projectiles
{
	public abstract class BaseSwingSwordProj : ModProjectile
	{
        #region variables
        // ------ Things you probably want to change ------

        //The angle of Half the swing arc (IN DEGREES)
        public float SwingHalfAngle = 145;

        //The progress you want easingProgress to start at
        public float startingProgress = 0.0f;

        //Distance projectile should be from player
        public float offset = 40;

        //Move the position of the sword by a bit
        public Vector2 positionOffset = Vector2.Zero;

        //Adds a delay before starting the swing
        public int frameToStartSwing = 2;

        //How much to add to easingProgress per frame
        public float easingAdditionAmount = 0.01f;

        //Adds a delay to the projectile dying after the swing is done
        public int timeAfterEnd = 4;

        public float progressToKill = 0.99f;

        public bool useMeleeSpeed = true;

        // ------ Things used locally ------
        public int timer = 0;

        public float startingAngle;

        public float currentAngle;

        //Stores the angle to mouse on frame 1, mostly used to help with vfx
        public float originalAngle;

        private bool firstFrame = true;

        //Progress should be from 0 to 1
        public float easingProgress;

        //Player direction on the first frame
        public int storedDirection;

        //Can't decrement timeAfterEnd because we are constantly setting it in the projectile, so we use this to store the value 
        private int storedTimeAfterEnd = 4;

        //For hitlag
        public int justHitTime = 0;
        #endregion

        public void StandardHeldProjCode()
        {
            Player player = Main.player[Projectile.owner];
            player.heldProj = Projectile.whoAmI;

            float angleToProj = (Projectile.Center - player.MountedCenter).ToRotation();

			//Store player direction
			if (firstFrame)
                storedDirection = player.direction;

            //Make sure itemRotation is right
            float itemrotate = storedDirection < 0 ? MathHelper.Pi : 0;
            if (player.direction != storedDirection)
                itemrotate += MathHelper.Pi;
            player.itemRotation = MathHelper.WrapAngle(angleToProj + itemrotate);

            //Composite arms
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (Projectile.Center - player.MountedCenter).ToRotation() + (MathHelper.Pi + MathHelper.PiOver2));
            

            //Delete proj if it shouldn't be there
            if (!player.active || player.dead || player.CCed || player.noItems || player.frozen)
            {
				Projectile.Kill();
            }
        }

        public void StandardSwingUpdate()
        {
			if (Projectile.owner != Main.myPlayer) return;
            Player player = Main.player[Projectile.owner];

            //This is were we set the beginning and ending angle of the sword 
            if (firstFrame)
            {
                //For drawing correctly
                Projectile.spriteDirection = Main.MouseWorld.X > Main.player[Projectile.owner].MountedCenter.X ? 1 : -1;

                storedTimeAfterEnd = timeAfterEnd;
                easingProgress = startingProgress;

				Vector2 mouseDir = player.DirectionTo(Main.MouseWorld);
                startingAngle = (-Vector2.UnitX).AngleTo(mouseDir) * 2;
                originalAngle = startingAngle;

                //Adjust projectile rotation for swing direction
                if (storedDirection == -1)
                {
                    startingAngle += MathHelper.Pi;
                    originalAngle = startingAngle;
                }

				//we set Projectile.ai[0] in the weapon. This is so the sword alternates direction
				//Change Projectile.ai[0] to be -1 or +1 to make this simpler
				startingAngle -= (Projectile.ai[0] * 2 - 1) * MathHelper.ToRadians(-SwingHalfAngle);

                currentAngle = startingAngle;
                firstFrame = false;
            }



            if (timer >= frameToStartSwing && justHitTime < 1)
            {
				//Change Projectile.ai[0] to be -1 or +1 to make this simpler
				currentAngle = startingAngle - ((Projectile.ai[0] * 2 - 1) * MathHelper.ToRadians(SwingHalfAngle * 2 * getProgress(easingProgress)));

                float meleeSpeed = useMeleeSpeed ? Main.player[Projectile.owner].GetTotalAttackSpeed(DamageClass.Melee) : 1f;
                easingProgress = Math.Clamp(easingProgress + easingAdditionAmount * meleeSpeed, 0.01f, 1f);
            }

            Projectile.rotation = currentAngle + MathHelper.PiOver4;

			Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + (currentAngle.ToRotationVector2() * offset);
            player.itemTime = 10;
            player.itemAnimation = 10;

            justHitTime--;
            timer++;


            
            if (getProgress(easingProgress) >= progressToKill)
            {
                if (storedTimeAfterEnd < 1)
                {
                    player.itemTime = 0;
                    player.itemAnimation = 0;
					Projectile.Kill();
					return;
                }
                storedTimeAfterEnd--;
            }
        }

        //input will be from 0-1
        //use with functions from here: https://easings.net
        //TODO at easing function presets to this
        public virtual float getProgress(float x) //From 0 to 1 and returns 0-1
        {
            float toReturn = 0f;

            //easeInOutExpo
            if (x <= 0.5f)
                toReturn = (float)Math.Pow(2, (16 * x) - 8) * 0.5f;
            else if (x > 0.5)
                toReturn = (float)(2 - Math.Pow(2, (-16 * x) + 8)) * 0.5f;

            //post 0.5
            if (x <= 0)
                toReturn = 0;
            if (x >= 1)
                toReturn = 1;

            return toReturn;
        }

    }
}