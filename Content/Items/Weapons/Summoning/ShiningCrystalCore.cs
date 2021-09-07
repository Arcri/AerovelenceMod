#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

#endregion

namespace AerovelenceMod.Content.Items.Weapons.Summoning
{
	public sealed class ShiningCrystalCore : ModItem
	{
		public override void SetDefaults()
		{
			item.width = 24;
			item.height = 32;
			item.rare = ItemRarityID.Blue;
			item.value = Item.sellPrice(0, 0, 18, 30);

			item.crit = 4;
			item.mana = 15;
			item.damage = 25;
			item.knockBack = 1;

			item.useTime = item.useAnimation = 35;
			item.useStyle = ItemUseStyleID.HoldingUp;
			
			item.summon = true;
			item.noMelee = true;
			item.autoReuse = true;
			
			item.shootSpeed = 10;
			item.shoot = ModContent.ProjectileType<Projectiles.Weapons.Summoning.ShiningCrystalCore_Proj>();
			
			item.UseSound = SoundID.Item101;
		}

		public override bool CanUseItem(Player player)
			=> player.ownedProjectileCounts[item.shoot] < 5;

	}
}
