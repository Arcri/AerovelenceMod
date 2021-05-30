using DiscordRPC;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using AerovelenceMod.Common.Globals.Players;

namespace AerovelenceMod.Core
{
    public static class DiscordRichPresence
    {
        private static RichPresence Presence;
        private static DiscordRpcClient Client;

        public static void Initialize()
        {
            Presence = new RichPresence
            {
                Details = "In Main Menu",
                Assets = new Assets
                {
                    LargeImageText = "Aerovelence",
                    LargeImageKey = "aerodefault",
                    SmallImageKey = "helditem"
                },
                State = ""
            };

            if (Main.netMode != NetmodeID.SinglePlayer)
                Presence.Party = new Party
                {
                    Size = Main.ActivePlayersCount,
                    Max = Main.maxNetPlayers
                };

            Client = new DiscordRpcClient("828361668646928466");

            Presence.Timestamps = new Timestamps
            {
                Start = DateTime.UtcNow
            };

            Client?.Initialize();
            Client?.SetPresence(Presence);
        }

        private static int Cooldown;

        public static void Update()
        {
            if (AeroClientConfig.Instance.DiscordRPCEnabled)
                Cooldown++;

            if (Client == null || Cooldown < 60 || !AeroClientConfig.Instance.DiscordRPCEnabled)
                return;

            Client.Invoke();

            if (!Main.gameMenu)
            {
                var zonePlayer = Main.LocalPlayer.GetModPlayer<ZonePlayer>();
                if (Main.npc.Any(n => n.active && n.boss))
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.active && npc.boss)
                        {
                            Client.UpdateDetails("Fighting a Boss");
                            Client.UpdateLargeAsset("aerodefault", "Aerovelence - tModLoader");
                            Client.UpdateState("The " + npc.FullName.Replace("The ", ""));
                            Client.UpdateSmallAsset("helditem", Main.LocalPlayer.HeldItem.Name);
                        }
                    }
                }
                else if (zonePlayer.ZoneCrystalCaverns)
                {
                    Client.UpdateDetails("In the Crystal Caverns");
                    Client.UpdateLargeAsset("crystalcavern", "Aerovelence - tModLoader");
                    Client.UpdateState("");
                    Client.UpdateSmallAsset("helditem", Main.LocalPlayer.HeldItem.Name);
                }
                else
                {
                    Client.UpdateDetails("In Game");
                    Client.UpdateLargeAsset("aerodefault", "Aerovelence - tModLoader");
                    Client.UpdateState("");
                    Client.UpdateSmallAsset("helditem", Main.LocalPlayer.HeldItem.Name);
                }
            }
            else
            {
                Client.UpdateDetails("In Main Menu");
                Client.UpdateLargeAsset("aerodefault", "Aerovelence - tModLoader");
                Client.UpdateState("");
                Client.UpdateSmallAsset("terrariamod", "Terraria Mod");
            }
            //Client.SetPresence(Presence); It would constantly relaod the default presence
            Cooldown = 0;
        }
        public static void Deinitialize()
        {
            Client?.UpdateEndTime(DateTime.UtcNow);
            Client?.Dispose();
        }
    }
}