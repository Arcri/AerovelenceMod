using AerovelenceMod.Content.Buffs.Pets;
using AerovelenceMod.Content.Projectiles.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Equipment
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
			Item.CloneDefaults(ItemID.ZephyrFish);
			Item.shoot = ModContent.ProjectileType<LightningFish>();
			Item.buffType = ModContent.BuffType<LightningFishBuff>();
		}

		public override void UseStyle(Player player)
		{
			if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
			{
				player.AddBuff(Item.buffType, 2, true);
			}
		}
	}
}