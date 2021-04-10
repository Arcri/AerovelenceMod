using AerovelenceMod.Content.Projectiles.Weapons.Summoning;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Buffs
{
	public class ShiverMinionBuff : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Shiver Spirit");
			Description.SetDefault("The icy spirit will fight for you");
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			AeroPlayer modPlayer = player.GetModPlayer<AeroPlayer>();
			if (player.ownedProjectileCounts[ModContent.ProjectileType<ShiverMinion>()] > 0)
			{
				modPlayer.ShiverMinion = true;
			}
			if (!modPlayer.ShiverMinion)
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