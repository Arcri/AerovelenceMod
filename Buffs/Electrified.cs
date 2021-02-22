using AerovelenceMod.NPCs;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Buffs
{
	public class Electrified : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Electrified");
			Description.SetDefault("Hmm");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<AeroPlayer>().Electrified = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.GetGlobalNPC<AeroGlobalNPC>().Electrified = true;
		}
	}
}