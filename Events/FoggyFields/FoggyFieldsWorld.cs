using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AerovelenceMod.Events
{
	public class FoggyFieldsWorld : ModWorld
	{
		public static bool FoggyFields = false;
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
			{
				nightToDay = true;
			}
			else
			{
				nightToDay = false;
			}
			nighttime = Main.dayTime;
			bool old = FoggyFields;
			if (nightToDay)
			{
				if (Main.rand.Next(5) == 0)
				{
					if (Main.dayTime)
					{
						for (int i = 0; i < 255; i++)
						{
							if (Main.player[i].active)
							{
								if (Main.player[i].GetModPlayer<AeroPlayer>().ZoneCrystalCaverns)
                                {
									FoggyFields = true;
									break;
								}
								if (!old && FoggyFields)
								{
									if (Main.netMode == NetmodeID.SinglePlayer)
									{
										Main.NewText("A light fog covers the crystal fields", 176, 217, 232);
									}
									else if (Main.netMode == NetmodeID.Server)
									{
										NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("A light fog covers the crystal fields"), new Microsoft.Xna.Framework.Color(176, 217, 232));
									}
								}
								break;
							}
						}
					}
				}
				else
				{
					FoggyFields = false;
				}
			}
		}
	}
}