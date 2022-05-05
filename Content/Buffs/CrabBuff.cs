#region Using directives

using AerovelenceMod.Content.Items.Armor.Seashine;
using Terraria;
using Terraria.ModLoader;

#endregion

namespace AerovelenceMod.Content.Buffs
{
	public sealed class CrabBuff : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Seashine Crab");
			Description.SetDefault("");

			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}



		public override void Update(Player player, ref int buffIndex)
		{
			AeroPlayer ap = player.GetModPlayer<AeroPlayer>();

			Item head = player.armor[0];
			Item body = player.armor[1];
			Item legs = player.armor[2];

			if (body.type == ModContent.ItemType<SeashineBodyArmor>() && legs.type == ModContent.ItemType<SeashineLeggings>() && head.type == ModContent.ItemType<SeashineHelmet>())

			if (player.ownedProjectileCounts[ModContent.ProjectileType<Content.Projectiles.Weapons.Summoning.SeaCrab>()] > 0)
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
