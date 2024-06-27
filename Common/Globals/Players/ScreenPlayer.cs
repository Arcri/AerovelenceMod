using System.IO;
using System.Threading;
using AerovelenceMod.Common.Globals.Worlds;
using AerovelenceMod.Content.Biomes;
using AerovelenceMod.Content.NPCs.Bosses.Cyvercry;
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

        int safetyTimer = 0;

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

            //This is a safety check to make sure that a cutscene stops if the boss/event despawns unexpectedly (like via dragonlens)
            //Only check the npc array once every 2 seconds
            if (safetyTimer % 120 == 0)
            {
                bool foundNPC = false;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];

                    if (npc.type == ModContent.NPCType<Cyvercry2>())
                    {
                        if (npc.active == true)
                            foundNPC = true;
                    }

                }
                if (!foundNPC)
                {
                    cutscene = false;
                    lerpBackToPlayer = true;
                }
            }

            safetyTimer++;
        }

        public override void ModifyScreenPosition()
        {
            if (cutscene || lerpBackToPlayer)
            {
                float shakeVal = Player.GetModPlayer<AeroPlayer>().ScreenShakePower;

                //This runs less often at lower frame rates (and vice versa) so this normalizes that 
                float adjustedShakeVal = shakeVal * (Main.frameRate / 144f);

                float totalIntensity = adjustedShakeVal * ModContent.GetInstance<AeroClientConfig>().ScreenshakeIntensity;

                Vector2 rand = new Vector2(Main.rand.NextFloat(totalIntensity), Main.rand.NextFloat(totalIntensity));
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
            }
            return true;
        }
    }
}
