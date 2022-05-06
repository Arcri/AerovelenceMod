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
			Item.width = 24;
			Item.height = 32;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(0, 0, 18, 30);

			Item.crit = 4;
			Item.mana = 15;
			Item.damage = 25;
			Item.knockBack = 1;

			Item.useTime = Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.HoldUp;
			
			Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;
			Item.autoReuse = true;
			
			Item.shootSpeed = 14;
			Item.shoot = ModContent.ProjectileType<Projectiles.Weapons.Summoning.ShiningCrystalCore_Proj>();
			
			Item.UseSound = SoundID.Item101;
		}

		public override bool CanUseItem(Player player)
			=> player.ownedProjectileCounts[Item.shoot] < 5;

	}
}
