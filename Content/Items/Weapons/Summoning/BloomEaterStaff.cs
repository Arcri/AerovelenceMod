#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

#endregion

namespace AerovelenceMod.Content.Items.Weapons.Summoning
{
	// TODO: Eldrazi - Implement projectile and logic.
	public sealed class BloomEaterStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.staff[Item.type] = true;
		}
		public override void SetDefaults()
		{
			Item.width = Item.height = 16;

			Item.mana = 40;
			Item.damage = 5;

			Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;

			Item.shootSpeed = 10f;
		}

		// - Bloom Eater Staff: Summons a blossomed man eater plant. It can charge up its mouth and spit a burst of charmed petals at the enemies.

		/*public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<Materials.Leaf>(), 5);
			recipe.AddIngredient(ModContent.ItemType<Materials.ScatteredFlower>(), 24);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}*/
	}
}
