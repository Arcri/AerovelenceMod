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
                Vector2 TrueGoalPos = ScreenGoalPos - new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
                Main.screenPosition = Vector2.SmoothStep(Main.screenPosition, TrueGoalPos, interpolant);

            }
        }

        //Would be bad if the player could die during a boss animation
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
