using System.IO;
using AerovelenceMod.Common.Globals.Worlds;
using AerovelenceMod.Content.Biomes;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace AerovelenceMod.Common.Globals.Players
{
    public class ScreenPlayer : ModPlayer
    {

        public bool cutscene = false;
        public Vector2 ScreenGoalPos;
        public float interpolant = 0f;
        public bool lerpBackToPlayer = false;

        public float DirectionalScreenShakePower;
        public float DirectionalScreenShakeDirection;

        public DSSB DSSBehavior = DSSB.RandomTwoDirections;
        //Default behavoir is Base with preset values
        public enum DSSB
        {
            RandomTwoDirections = 0,
            RandomOneDirection = 1,
            NoRandom = 2,
        }

        public override void PostUpdate()
        {
            //If we are in a cutscene, lerp the interpolant value to 1, otherwise lerp it to zero
            if (cutscene)
            {
                interpolant = MathHelper.Clamp(MathHelper.Lerp(interpolant, 1.1f, 0.04f), 0f, 1f);
            }
            else if (lerpBackToPlayer)
            {
                interpolant = MathHelper.Clamp(MathHelper.Lerp(interpolant, -0.1f, 0.02f), 0f, 1f);
                if (interpolant == 0f)
                    lerpBackToPlayer = false;
            }

        }

        public override void ModifyScreenPosition()
        {
            if (cutscene || lerpBackToPlayer)
            {
                float shakeVal = Player.GetModPlayer<AeroPlayer>().ScreenShakePower;
                Vector2 rand = new Vector2(Main.rand.NextFloat(shakeVal), Main.rand.NextFloat(shakeVal));
                Vector2 TrueGoalPos = (ScreenGoalPos - new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f) + rand;
                Main.screenPosition = Vector2.SmoothStep(Main.screenPosition, TrueGoalPos, interpolant);

            }

            //NoRandom
            
            if (DirectionalScreenShakePower > 0.1f)
            {

                switch (DSSBehavior)
                {
                    case DSSB.RandomOneDirection:
                        Main.screenPosition += new Vector2(Main.rand.NextFloat(DirectionalScreenShakePower), 0f).RotatedBy(DirectionalScreenShakeDirection);
                        DirectionalScreenShakePower *= 0.9f;
                        break;

                    case DSSB.RandomTwoDirections:
                        Main.screenPosition += new Vector2(Main.rand.NextFloat(-DirectionalScreenShakePower, DirectionalScreenShakePower), 0f).RotatedBy(DirectionalScreenShakeDirection);
                        DirectionalScreenShakePower *= 0.9f;
                        break;

                    case DSSB.NoRandom:
                        Main.screenPosition -= new Vector2(DirectionalScreenShakePower, 0f).RotatedBy(DirectionalScreenShakeDirection);
                        DirectionalScreenShakePower *= 0.9f;
                        break;
                }


            }
        }

        //Would be cringe if the player could die during a boss animation
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (cutscene)
            {
                Player.statLife = 1;
                return false;
            }
            return true;
        }
    }
}
