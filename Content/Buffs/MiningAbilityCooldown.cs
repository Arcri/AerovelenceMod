using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Buffs
{
	public class MiningAbilityCooldown : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mining Power Withdrawl");
			Description.SetDefault("Can't use Ambrosia mining set bonus while this is active");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<AeroPlayer>().MiningAbilityCooldown = true;
		}
	}
}