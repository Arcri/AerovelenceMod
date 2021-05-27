using AerovelenceMod.Content.Buffs.Pets;
using AerovelenceMod.Content.Projectiles.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Equipment
{
    public class StarScope : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Star Scope");
			Tooltip.SetDefault("You can zoom farther while holding this");
		}

		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.ZephyrFish);
			item.shoot = ModContent.ProjectileType<LightningFish>();
			item.buffType = ModContent.BuffType<LightningFishBuff>();
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