using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace AerovelenceMod
{
	public class AeroClientConfig : ModConfig
	{
        public override ConfigScope Mode => ConfigScope.ClientSide;

        public static AeroClientConfig Instance;

        [DefaultValue(true)]
        [Label("Discord Rich Presence")]
        public bool DiscordRPCEnabled;
    }
}