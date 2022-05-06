#region Using directives

using Terraria;
using Terraria.ModLoader;

#endregion

namespace AerovelenceMod.Content.Buffs
{
	public sealed class HuntressBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Huntress");
			Description.SetDefault("");

			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			AeroPlayer ap = player.GetModPlayer<AeroPlayer>();

			if (player.ownedProjectileCounts[ModContent.ProjectileType<Content.Projectiles.Weapons.Summoning.Huntress>()] > 0)
			{
				ap.huntressSummon = true;
			}

			if (!ap.huntressSummon)
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
