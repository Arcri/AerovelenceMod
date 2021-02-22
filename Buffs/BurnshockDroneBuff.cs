using AerovelenceMod.Projectiles.Minions;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Buffs
{
    public class BurnshockDroneBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Starshock Drone");
            Description.SetDefault("A Starshock drone is protecting you");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

		public override void Update(Player player, ref int buffIndex)
		{
			AeroPlayer modPlayer = (AeroPlayer)player.GetModPlayer(mod, "AeroPlayer");
			if (player.ownedProjectileCounts[ModContent.ProjectileType<ShockburnDrone>()] > 0)
			{
				modPlayer.StarDrone = true;
			}

			if (!modPlayer.StarDrone)
			{
				player.DelBuff(buffIndex);
				buffIndex--;
				return;
			}

			player.buffTime[buffIndex] = 18000;
		}
	}
}