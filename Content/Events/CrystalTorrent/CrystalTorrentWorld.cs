using Terraria.ModLoader;

namespace AerovelenceMod.Content.Events.CrystalTorrent
{
    public class CrystalTorrentWorld : ModWorld
	{
		public static bool CrystalTorrents;

		public override void Initialize()
		{
			CrystalTorrents = false;
		}
		/*public override void PostUpdate()
		{
			bool old = CrystalTorrents;

			if (Main.rand.Next(2) == 1 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (Main.dayTime && !Main.eclipse)
				{
					for (int i = 0; i < 255; i++)
					{
						if (Main.player[i].active && Main.player[i].statLifeMax > 120)
						{
							CrystalTorrents = true;

							if (!old && CrystalTorrents)
							{
								if (Main.netMode == NetmodeID.SinglePlayer)
								{
									Main.NewText("A violent electrical storm floods the land", 000, 230, 255);
								}
								else if (Main.netMode == NetmodeID.Server)
								{
									NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("A violent electrical storm floods the land"), new Microsoft.Xna.Framework.Color(000, 230, 255));
								}
							}
							break;

						}
					}
				}
				else
				{
					CrystalTorrents = false;
				}
			}
			if (Main.eclipse)
			{
				CrystalTorrents = false;
			}
		}*/
	}
}