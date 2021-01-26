using AerovelenceMod.Projectiles.Minions;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Buffs
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
			AeroPlayer modPlayer = (AeroPlayer)player.GetModPlayer(mod, "AeroPlayer");
			if (player.ownedProjectileCounts[ModContent.ProjectileType<Minicry>()] > 0)
			{
				modPlayer.Minicry = true;
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