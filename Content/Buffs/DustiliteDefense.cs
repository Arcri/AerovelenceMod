#region Using directives

using Terraria;
using Terraria.ModLoader;

#endregion

namespace AerovelenceMod.Content.Buffs
{
	public sealed class DustiliteDefense : ModBuff
	{
		// TODO: Eldrazi - Implement correct sprites.
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Dustilite Defense");
			Description.SetDefault("Increased temporary defense");
			
			Main.debuff[Type] = false;
			Main.buffNoTimeDisplay[Type] = false;
			
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.statDefense += 10;
		}
	}
}
