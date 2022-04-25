using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Common.ShieldSystem;

namespace AerovelenceMod.Content.Items.Shields
{
	public class FallingDefense : ShieldItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("[WIP] Falling Defense");
			Tooltip.SetDefault("300 shield capacity\n15 recharge rate\n5s recharge delay\nNova shield\nBlocks most contact damage\nAlmost no protection from projectiles");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			item.accessory = true;
			item.rare = ItemRarityID.Orange;
			shield.capacity = 300;
			shield.rechargeRate = 15f;
			shield.rechargeDelay = 5;
			shield.type = ShieldTypes.Nova;
		//	shield.projectileType = ModContent.ProjectileType<FallingDefenseShieldProjectile>();

			// This shield will protect the player from almost all contact damage, has a ton of shield capacity. But, recharges incredibly slow. Projectiles don't affect the shield, and will pass right through it.
			// shield.capacity = 300; (Attacks do more damage to shield capacity unlike HP)
			// shield.rechargeRate = 5; (This is a fast regenerating shield)
			// shield.rechargeDelay = 5; (Measured in seconds)
			// shield.Turtle = true;
		}
	}
	/*public class FallingDefenseShieldProjectile : ShieldProjectile
	{
	/*	protected override void NovaExplosionEffect()
		{
			Main.NewText("hello");
			//base.NovaExplosion();
		}
	}*/
}
