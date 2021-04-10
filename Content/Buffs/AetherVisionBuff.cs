using AerovelenceMod.Content.Projectiles.Weapons.Summoning;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Buffs
{
	public class AetherVisionBuff : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Mini Cyvercry");
			Description.SetDefault("A mini Cyvercry is protecting you");
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			AeroPlayer modPlayer = player.GetModPlayer<AeroPlayer>();
			if (player.ownedProjectileCounts[ModContent.ProjectileType<Minicry>()] > 0)
			{
				modPlayer.Minicry = true;
			}

			if (!modPlayer.Minicry)
			{
				player.DelBuff(buffIndex);
				buffIndex--;
				return;
			}

			player.buffTime[buffIndex] = 18000;
		}
	}
}