#region Using directives

using Terraria;
using Terraria.ModLoader;

#endregion

namespace AerovelenceMod.Content.Buffs
{
	public sealed class BurnshockShield : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Burnshock Shield");
			Description.SetDefault("A protective shield");
			
			Main.debuff[Type] = false;
			Main.buffNoTimeDisplay[Type] = true;
			
			canBeCleared = false;
		}
	}
}
