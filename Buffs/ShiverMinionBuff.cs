using AerovelenceMod.Projectiles.Minions;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Buffs
{
	public class ShiverMinionBuff: ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Shiver");
			Description.SetDefault("The lost icy spirit will fight for you");
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			AeroPlayer modPlayer = player.GetModPlayer<AeroPlayer>();
			if (player.ownedProjectileCounts[ModContent.ProjectileType<ShiverMinion>()] > 0)
			{
				modPlayer.FrostMinion = true;
			}
			if (!modPlayer.FrostMinion)
			{
				player.DelBuff(buffIndex);
				buffIndex--;
			}
			else
			{
				player.buffTime[buffIndex] = 18000;
			}
		}
	}
}