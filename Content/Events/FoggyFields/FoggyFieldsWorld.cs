using AerovelenceMod.Common.Globals.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Events.FoggyFields
{
    public class FoggyFieldsWorld : ModWorld
    {
        public static bool FoggyFields;
        private static bool nighttime;
        public static bool nightToDay;

        public override void Initialize()
        {
            FoggyFields = false;
            nighttime = !Main.dayTime;
            nightToDay = false;
        }

        public override void PostUpdate()
        {
            if (Main.dayTime != nighttime)
                nightToDay = true;
            else
                nightToDay = false;

            nighttime = Main.dayTime;
            var old = FoggyFields;

            if (nightToDay)
                if (Main.rand.NextBool(5))
                    if (Main.dayTime)
                    {
                        for (var i = 0; i < 255; i++)
                            if (Main.player[i].active)
                            {
                                if (Main.player[i].GetModPlayer<ZonePlayer>().ZoneCrystalCaverns)
                                {
                                    FoggyFields = true;
                                    break;
                                }

                                if (!old && FoggyFields)
                                {
                                    if (Main.netMode == NetmodeID.SinglePlayer)
                                        Main.NewText("A light fog covers the crystal fields", 176, 217, 232);
                                    else if (Main.netMode == NetmodeID.Server)
                                        NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("A light fog covers the crystal fields"),
                                            new Color(176, 217, 232));
                                }

                                break;
                            }
                    }
                    else
                        FoggyFields = false;
        }
    }
}
