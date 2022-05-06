using AerovelenceMod.Content.Projectiles.Weapons.Summoning;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Buffs
{
    public class BurnshineStaffBuff: ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Neutron Star");
			Description.SetDefault("A neutron star is protecting you");
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			AeroPlayer modPlayer = player.GetModPlayer<AeroPlayer>();
			if (player.ownedProjectileCounts[ModContent.ProjectileType<BurningNeutronStar>()] > 0)
			{
				modPlayer.NeutronMinion = true;
			}

			if (!modPlayer.NeutronMinion)
			{
				player.DelBuff(buffIndex);
				buffIndex--;
				return;
			}

			player.buffTime[buffIndex] = 18000;
		}
	}
}