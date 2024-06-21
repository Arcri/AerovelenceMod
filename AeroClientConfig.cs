using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace AerovelenceMod
{
	public class AeroClientConfig : ModConfig
	{
        public override ConfigScope Mode => ConfigScope.ClientSide;

        public static AeroClientConfig Instance;

        [Range(0f, 1f)]
        [Slider]
        [DefaultValue(1f)]
        public float ScreenshakeIntensity;



        //[LabelKey("$Mods.ExampleMod.Configs.Common.LocalizedLabel")]
        //TooltipKey("CyvercryDifficultyOverride")]

        [DrawTicks]
        [OptionStrings(new string[] { "None", "Normal", "Expert", "Master" })]
        [DefaultValue("None")]
        public string CyvercryAIOverride;

    }



}