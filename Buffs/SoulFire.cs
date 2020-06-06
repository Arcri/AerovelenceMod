using AerovelenceMod.NPCs;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Buffs
{
	public class SoulFire : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Soul Fire");
			Description.SetDefault("You are burning with rage deep within");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<AeroPlayer>().SoulFire = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.GetGlobalNPC<AeroGlobalNPC>().SoulFire = true;
		}
	}
}