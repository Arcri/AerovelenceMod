using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
namespace AerovelenceMod.CrystalTorrent
{
    public class CrystalTorrentInvasion
    {
        public static int[] invaders = {
            NPCID.Zombie,
            NPCID.BlueSlime
        };
        public static void StartCrystalTorrent()
        {
            if (Main.invasionType != 0 && Main.invasionSize == 0)
            {
                Main.invasionType = 0;
            }
            if (Main.invasionType == 0)
            {
                int numPlayers = 0;
                for (int i = 0; i < 255; i++)
                {
                    if (Main.player[i].active && Main.player[i].statLifeMax >= 200)
                    {
                        numPlayers++;
                    }
                }
                if (numPlayers > 0)
                {
                    Main.invasionType = -1;
                    CrystalTorrentWorld.CrystalTorrentUp = true;
                    Main.invasionSize = 100 * numPlayers;
                    Main.invasionSizeStart = Main.invasionSize;
                    Main.invasionProgress = 0;
                    Main.invasionProgressIcon = 0 + 3;
                    Main.invasionProgressWave = 0;
                    Main.invasionProgressMax = Main.invasionSizeStart;
                    Main.invasionWarn = 3600;
                    if (Main.rand.Next(2) == 0)
                    {
                        Main.invasionX = 0.0;
                        return;
                    }
                    Main.invasionX = Main.maxTilesX;
                }
            }
        }
        [Obsolete]
        public static void CrystalTorrentWarning()
        {
            String text = "";
            if (Main.invasionX == Main.spawnTileX)
            {
                text = "The Crystal Torrents have begun!";
            }
            if (Main.invasionSize <= 0)
            {
                text = "The storm subsides...";
            }
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(text, 175, 75, 255, false);
                return;
            }
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.ChatText, -1, -1, NetworkText.FromLiteral(text), 255, 175f, 75f, 255f, 0, 0, 0);
            }
        }
        [Obsolete]
        public static void UpdateCrystalTorrent()
        {
            if (CrystalTorrentWorld.CrystalTorrentUp)
            {
                if (Main.invasionSize <= 0)
                {
                    CrystalTorrentWorld.CrystalTorrentUp = false;
                    CrystalTorrentWarning();
                    Main.invasionType = 0;
                    Main.invasionDelay = 0;
                }
                if (Main.invasionX == Main.spawnTileX)
                {
                    return;
                }
                float moveRate = Main.dayRate;
                if (Main.invasionX > Main.spawnTileX)
                {
                    Main.invasionX -= moveRate;
                    if (Main.invasionX <= Main.spawnTileX)
                    {
                        Main.invasionX = Main.spawnTileX;
                        CrystalTorrentWarning();
                    }
                    else
                    {
                        Main.invasionWarn--;
                    }
                }
                else
                {
                    if (Main.invasionX < Main.spawnTileX)
                    {
                        Main.invasionX += moveRate;
                        if (Main.invasionX >= Main.spawnTileX)
                        {
                            Main.invasionX = Main.spawnTileX;
                            CrystalTorrentWarning();
                        }
                        else
                        {
                            Main.invasionWarn--;
                        }
                    }
                }
            }
        }
        public static void CheckCrystalTorrentProgress()
        {
            if (Main.invasionProgressMode != 2)
            {
                Main.invasionProgressNearInvasion = false;
                return;
            }
            bool flag = false;
            Player player = Main.player[Main.myPlayer];
            Rectangle rectangle = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
            int num = 5000;
            int icon = 0;
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].active)
                {
                    icon = 0;
                    int type = Main.npc[i].type;
                    for (int n = 0; n < invaders.Length; n++)
                    {
                        if (type == invaders[n])
                        {
                            Rectangle value = new Rectangle((int)(Main.npc[i].position.X + Main.npc[i].width / 2) - num, (int)(Main.npc[i].position.Y + Main.npc[i].height / 2) - num, num * 2, num * 2);
                            if (rectangle.Intersects(value))
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                }
            }
            Main.invasionProgressNearInvasion = flag;
            int progressMax3 = 1;
            if (CrystalTorrentWorld.CrystalTorrentUp)
            {
                progressMax3 = Main.invasionSizeStart;
            }
            if (CrystalTorrentWorld.CrystalTorrentUp && (Main.invasionX == Main.spawnTileX))
            {
                Main.ReportInvasionProgress(Main.invasionSizeStart - Main.invasionSize, progressMax3, icon, 0);
            }
            foreach (Player p in Main.player)
            {
                NetMessage.SendData(MessageID.InvasionProgressReport, p.whoAmI, -1, null, Main.invasionSizeStart - Main.invasionSize, Main.invasionSizeStart, Main.invasionType + 3, 0f, 0, 0, 0);
            }
        }
    }
}