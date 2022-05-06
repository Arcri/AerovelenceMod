#region Using directives

using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

#endregion

namespace AerovelenceMod.Content.Items.Weapons.Summoning
{
	public sealed class OmegaFireCrystalCore : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 32;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(0, 0, 18, 30);

			Item.crit = 4;
			Item.mana = 15;
			Item.damage = 35;
			Item.knockBack = 1;

			Item.useTime = Item.useAnimation = 25;
			Item.useStyle = ItemUseStyleID.HoldUp;
			
			Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;
			Item.autoReuse = true;
			
			Item.shootSpeed = 10;
			Item.shoot = ModContent.ProjectileType<Projectiles.Weapons.Summoning.OmegaFireCrystalCore_Proj>();
			
			Item.UseSound = SoundID.Item101;
		}
		public override void AddRecipes()
		{
			CreateRecipe(1)
				.AddIngredient(ModContent.ItemType<EmberFragment>(), 15)
				.AddIngredient(ModContent.ItemType<ShiningCrystalCore>(), 1)
				.AddRecipeGroup("AerovelenceMod:CobaltBars", 15)
				.Register();
		}

		public override bool CanUseItem(Player player)
			=> player.ownedProjectileCounts[Item.shoot] < 10;

	}
}
