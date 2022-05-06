#region Using directives

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

#endregion

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
	public sealed class TomeOfGrowth : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Shoots a budding red seed");
		}
		public override void SetDefaults()
		{
			Item.height = 41;
			Item.width = 39;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.sellPrice(0, 0, 60, 0);

			Item.mana = 10;
			Item.damage = 20;
			Item.knockBack = 5;
			
			Item.useTime = Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Shoot;
			
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.autoReuse = true;

			Item.shootSpeed = 7.5f;
			Item.shoot = ModContent.ProjectileType<Projectiles.Weapons.Magic.RedSeed>();
			
			Item.UseSound = SoundID.Item20;
		}

        public override bool CanUseItem(Player player)
			=> player.ownedProjectileCounts[Item.shoot] < 8;

        /*public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Book);
			recipe.AddIngredient(ModContent.ItemType<Materials.Leaf>(), 3);
			recipe.AddIngredient(ModContent.ItemType<Materials.ScatteredFlower>(), 20);
			recipe.AddTile(TileID.LivingLoom);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}*/
	}
}
