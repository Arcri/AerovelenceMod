using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Common.ShieldSystem;

namespace AerovelenceMod.Content.Items.Shields
{
	public class Firewall : ShieldItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("[WIP] Firewall");
			Tooltip.SetDefault("175 shield capacity\n18 recharge rate\n10s recharge delay\nNova shield\nHas a small chance to reflect projectiles\nGrants a buff when a projectile is deflected");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			item.accessory = true;
			item.rare = ItemRarityID.Orange;
			shield.capacity = 175;
			shield.rechargeRate = 18f;
			shield.rechargeDelay = 10;
			shield.type = ShieldTypes.nova;
	//		shield.projectileType = ModContent.ProjectileType<ShieldProjectile>();
			// This shield will have a small chance to reflect projectiles. Grants a buff when the shield deflects a projectile.
			// shield.capacity = 175; (Attacks do more damage to shield capacity unlike HP)
			// shield.rechargeRate = 5; (This is a fast regenerating shield)
			// shield.rechargeDelay = 10; (Measured in seconds)
			// shield.Bubble = true;
		}
	}
}
