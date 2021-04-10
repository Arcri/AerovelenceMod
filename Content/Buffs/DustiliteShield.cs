#region Using directives

using Terraria;
using Terraria.ModLoader;

#endregion

namespace AerovelenceMod.Content.Buffs
{
	// TODO: Eldrazi - Implement correct sprites.
	public sealed class DustiliteShield : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Dustilite Shield");
			Description.SetDefault("A protective shield");
			
			Main.debuff[Type] = false;
			Main.buffNoTimeDisplay[Type] = true;
			
			canBeCleared = false;
		}
	}
}
