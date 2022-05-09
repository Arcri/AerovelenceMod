#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

#endregion

namespace AerovelenceMod.Content.Buffs
{
	public sealed class BurnshockDefense : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Burnshock Shield Defense");
			Description.SetDefault("Increased temporary defense");
			
			Main.debuff[Type] = false;
			Main.buffNoTimeDisplay[Type] = false;

			BuffID.Sets.NurseCannotRemoveDebuff[Type] = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.statDefense += 10;
		}
	}
}
