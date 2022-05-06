using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Common.ShieldSystem;

namespace AerovelenceMod.Content.Items.Shields
{
	public class SkyGuard : ShieldItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("[WIP] Sky Guard");
			Tooltip.SetDefault("250 shield capacity\n30 recharge rate\n11s recharge delay\nBubble shield\nBlocks projectile damage\nAlmost no protection from contact damage\nProtects against electricity, but consumes high shield capacity.");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			Item.accessory = true;
			Item.rare = ItemRarityID.Orange;
			shield.capacity = 250;
			shield.rechargeRate = 30f;
			shield.rechargeDelay = 11;
			shield.type = ShieldTypes.Bubble;

			// This shield will protect the player from almost all projectiles, but has no protection to contact damage. Electricity can't pass through the shield but will greatly take down shield capacity.
			// shield.capacity = 250; (Attacks do more damage to shield capacity unlike HP)
			// shield.rechargeRate = 30; (This is a fast regenerating shield)
			// shield.rechargeDelay = 7; (Measured in seconds)
			// shield.Bubble = true;
		}
	}

}
