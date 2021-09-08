#region Using directives

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
			item.width = 24;
			item.height = 32;
			item.rare = ItemRarityID.Blue;
			item.value = Item.sellPrice(0, 0, 18, 30);

			item.crit = 4;
			item.mana = 15;
			item.damage = 35;
			item.knockBack = 1;

			item.useTime = item.useAnimation = 25;
			item.useStyle = ItemUseStyleID.HoldingUp;
			
			item.summon = true;
			item.noMelee = true;
			item.autoReuse = true;
			
			item.shootSpeed = 10;
			item.shoot = ModContent.ProjectileType<Projectiles.Weapons.Summoning.OmegaFireCrystalCore_Proj>();
			
			item.UseSound = SoundID.Item101;
		}
		public override void AddRecipes()
		{
			ModRecipe modRecipe = new ModRecipe(mod);
			modRecipe.AddIngredient(ItemID.SoulofNight, 15);
			modRecipe.AddIngredient(ModContent.ItemType<ShiningCrystalCore>(), 1);
			modRecipe.AddRecipeGroup("AerovelenceMod:CobaltBars", 15);
			modRecipe.SetResult(this, 1);
			modRecipe.AddRecipe();
		}

		public override bool CanUseItem(Player player)
			=> player.ownedProjectileCounts[item.shoot] < 10;

	}
}
