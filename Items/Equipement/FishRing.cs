using AerovelenceMod.Projectiles.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Equipement
{
    public class FishRing : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fish Ring");
			Tooltip.SetDefault("You may now kiss the fish\n" +
							   "Summons an Electric Tetra to partner you in life");
		}

		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.ZephyrFish);
			item.shoot = ModContent.ProjectileType<LightningFish>();
			item.buffType = ModContent.BuffType<Buffs.Pets.LightningFishBuff>();
		}

		public override void UseStyle(Player player)
		{
			if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
			{
				player.AddBuff(item.buffType, 2, true);
			}
		}
	}
}