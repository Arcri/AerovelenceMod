using AerovelenceMod.Projectiles.Minions;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Buffs
{
	public class BurnshineStaffBuff: ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Neutron Star");
			Description.SetDefault("A neutron star is protecting you");
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<BurningNeutronStar>()] > 0)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}