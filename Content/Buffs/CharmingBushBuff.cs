#region Using directives

using Terraria;
using Terraria.ModLoader;

#endregion

namespace AerovelenceMod.Content.Buffs
{
    public sealed class CharmingBushBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Charming Bush");
			Description.SetDefault("");

			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			AeroPlayer ap = player.GetModPlayer<AeroPlayer>();

			int projectileAmount =
				player.ownedProjectileCounts[ModContent.ProjectileType<Content.Projectiles.Weapons.Summoning.CharmingBushRedFlower>()] +
				player.ownedProjectileCounts[ModContent.ProjectileType<Content.Projectiles.Weapons.Summoning.CharmingBushGreenFlower>()] +
				player.ownedProjectileCounts[ModContent.ProjectileType<Content.Projectiles.Weapons.Summoning.CharmingBushYellowFlower>()];

			if (projectileAmount > 0)
			{
				ap.charmingBush = true;
			}

			if (!ap.charmingBush)
			{
				player.DelBuff(buffIndex--);
			}
			else
			{
				player.buffTime[buffIndex] = 18000;
			}
		}
	}
}
