using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AerovelenceMod.Events
{
	public class DarkNightWorld : ModWorld
	{
		public static bool DarkNight = false;
		private static bool daytime;
		public static bool dayToNight;

		public override void Initialize()
		{
			DarkNight = false;
			daytime = Main.dayTime;
			dayToNight = false;
		}
		public override void PostUpdate()
		{
			if (Main.dayTime != daytime)
			{
				dayToNight = true;
			}
			else
			{
				dayToNight = false;
			}
			daytime = Main.dayTime;
			bool old = DarkNight;
			if (dayToNight)
			{
				if (!Main.pumpkinMoon && !Main.snowMoon && !Main.bloodMoon && Main.rand.Next(10) == 0 && Main.netMode != NetmodeID.MultiplayerClient)
				{
					if (!Main.dayTime)
					{
						for (int i = 0; i < 255; i++)
						{
							if (Main.player[i].active && Main.player[i].statLifeMax > 120)
							{
								DarkNight = true;

								if (!old && DarkNight)
								{
									if (Main.netMode == NetmodeID.SinglePlayer)
									{
										Main.NewText("The essense of dark engulfs the moon", 000, 068, 193);
									}
									else if (Main.netMode == NetmodeID.Server)
									{
										NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("The essense of dark engulfs the moon"), new Microsoft.Xna.Framework.Color(000, 068, 193));
									}
								}
								break;
							}
						}
					}
				}
				else
				{
					DarkNight = false;
				}
			}
			if (Main.pumpkinMoon || Main.snowMoon || Main.bloodMoon)
			{
				DarkNight = false;
			}
		}
	}
}