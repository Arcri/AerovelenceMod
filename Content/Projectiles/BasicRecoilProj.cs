using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.DataStructures;
using System.Runtime.InteropServices;
using Terraria.GameContent;
using AerovelenceMod.Common.Utilities;


namespace VFXPlus.Content.Projectiles
{

    public class BasicRecoilProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 999999;

            Projectile.DamageType = DamageClass.Ranged;

            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;

        //Which muzzle flash texture to use
        public int muzzleFlashNum = 1;

        //How long should the recoil take
        public int AnimationTime = 25;

        //The item ID of the weapon we are using
        public int gunID = 1;

        public float BaseXOffset = 18f;
        public float GoalXOffset = 4f;
        public float yRecoilPower = 0.075f;

        public Vector2 HoldoutOffset = Vector2.Zero; // 0 1
        public Vector2 TipPosition = Vector2.Zero; // 33 -3
        public Vector2 StarPosition = Vector2.Zero; // 30 -3

        public int timeToStartFade = 1;

        public bool doCompositeArm = true;

        //Whether composite arms should always be at max stretch 
        public bool compositeArmAlwaysFull = false;

        public void SetProjInfo(int GunID, int AnimTime, float NormalXOffset, float DestXOffset, float YRecoilAmount,
            Vector2 HoldOffset)
        {
            gunID = GunID;
            AnimationTime = AnimTime;
            BaseXOffset = NormalXOffset;
            GoalXOffset = DestXOffset;
            yRecoilPower = YRecoilAmount;
            HoldoutOffset = HoldOffset;
        }

        public void SetProjInfo(int GunID, int AnimTime, float NormalXOffset, float DestXOffset, float YRecoilAmount, 
            Vector2 HoldOffset, Vector2 TipPos, Vector2 StarPos)
        {
            gunID = GunID;
            AnimationTime = AnimTime;
            BaseXOffset = NormalXOffset;
            GoalXOffset = DestXOffset;
            yRecoilPower = YRecoilAmount;
            HoldoutOffset = HoldOffset;
            TipPosition = TipPos;
            StarPosition = StarPos;
        }

        //The angle of the gun shot
        float shotAngle = 0f;

        int timer = 0;
        public override void AI()
        {
            Player Player = Main.player[Projectile.owner];
            ProjectileExtensions.KillHeldProjIfPlayerDeadOrStunned(Projectile);

            Projectile.velocity = Vector2.Zero;

            //Kill proj if player is done with item use
            if (Player.itemAnimation <= 1)
                Projectile.active = false;

            //Or if we have reached the end of the animation for a long time
            if (timer == AnimationTime * 2f)
                Projectile.active = false;

            //Store the shot angle
            if (timer == 0 && Projectile.owner == Main.myPlayer)
            {
                shotAngle = (Main.MouseWorld - Player.Center).ToRotation();
                XOffset = BaseXOffset;

                bonusPower = 1f;
                muzzleFlashNum = Main.rand.Next(1, 4);
            }

            GunDirection = shotAngle.ToRotationVector2();
            Player.ChangeDir(GunDirection.X > 0 ? 1 : -1);

            #region XRecoil
            int XAnimTime = AnimationTime;
            float goalX = GoalXOffset;
            float baseX = BaseXOffset; //The Normal XOffset of the gun

            //Should add up to 1 (but does not need to)
            Vector2 animRatioX = new Vector2(0.25f, 0.75f);

            float xAnimProgress = (float)(Math.Clamp(timer, 0f, XAnimTime) / XAnimTime);

            //Move Out
            if (xAnimProgress < animRatioX.X)
            {
                float prog = xAnimProgress / animRatioX.X;
                XOffset = MathHelper.Lerp(baseX, goalX, Easings.easeInOutQuad(prog));
            }
            //Move back in
            else
            {
                float prog = (xAnimProgress - animRatioX.X) / animRatioX.Y;
                XOffset = MathHelper.Lerp(goalX, baseX, Easings.easeOutQuad(prog));
            }
            #endregion

            #region YRecoil
            int timeToStartYAnim = 3;
            int YAnimTime = AnimationTime;
            float goalY = yRecoilPower; //The amount of recoil (radians) of the shot
            float baseY = 0f;

            //Should add up to 1 (but does not need to)
            Vector2 animRatioY = new Vector2(0.15f, 0.85f);

            if (timer >= timeToStartYAnim)
            {
                float yAnimProgress = (float)(Math.Clamp((timer - timeToStartYAnim), 0f, YAnimTime) / YAnimTime);

                //RecoilUp
                if (yAnimProgress < animRatioY.X)
                {
                    float prog = yAnimProgress / animRatioY.X;
                    YRecoil = MathHelper.Lerp(baseY, goalY, Easings.easeOutCubic(prog));
                }
                //RecoilDown
                else
                {
                    float prog = (yAnimProgress - animRatioY.X) / animRatioY.Y;
                    YRecoil = MathHelper.Lerp(goalY, baseY, Easings.easeInOutBack(prog, 0f, 1f)); //
                }
            }
            #endregion


            if (timer > timeToStartFade)
                muzzleFlashPower = Math.Clamp(MathHelper.Lerp(muzzleFlashPower, -0.5f, 0.15f), 0f, 1f);

            //StandardHeldProjCode
            GunDirection = shotAngle.ToRotationVector2().RotatedBy(YRecoil * Player.direction * -1f); ;
            Projectile.Center = Player.MountedCenter + (GunDirection * XOffset);
            Projectile.velocity = Vector2.Zero;
            Player.itemRotation = shotAngle;

            if (Player.direction != 1)
                Player.itemRotation -= 3.14f;

            Player.itemRotation = MathHelper.WrapAngle(Player.itemRotation);

            #region compositeArms
            
            if (doCompositeArm)
            {
                if (compositeArmAlwaysFull)
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, GunDirection.ToRotation() - MathHelper.PiOver2);
                else
                {
                    float Xprog = Utils.GetLerpValue(goalX, baseX, XOffset, true);

                    if (Xprog > 0.75f)
                        Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, GunDirection.ToRotation() - MathHelper.PiOver2);
                    else if (Xprog > 0.5f)
                        Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, GunDirection.ToRotation() - MathHelper.PiOver2);
                    else if (Xprog > 0.25f)
                        Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, GunDirection.ToRotation() - MathHelper.PiOver2);
                    else
                        Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.None, GunDirection.ToRotation() - MathHelper.PiOver2);
                }
            }

            #endregion

            Player.heldProj = Projectile.whoAmI;
            Projectile.rotation = GunDirection.ToRotation();

            bonusPower *= 0.8f;

            timer++;
        }

        public Vector2 GunDirection = Vector2.Zero;

        float XOffset = 0f;
        float YRecoil = 0f;

        //Does nothing by default, but useful for adding a muzzle flash
        public float bonusPower = 0f;
        public float muzzleFlashPower = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Texture = TextureAssets.Item[gunID].Value;
            Player Player = Main.player[Projectile.owner];
            SpriteEffects mySE = Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Vector2 heldOffset = new Vector2(HoldoutOffset.X, HoldoutOffset.Y * Player.direction).RotatedBy(Projectile.rotation);         
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Player.gfxOffY) + heldOffset;


            Main.spriteBatch.Draw(Texture, drawPos, null, lightColor, Projectile.rotation, Texture.Size() / 2, Projectile.scale, mySE, 0f);

            return false;
        }

    }

}
