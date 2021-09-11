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
			item.height = 41;
			item.width = 39;
			item.rare = ItemRarityID.Green;
			item.value = Item.sellPrice(0, 0, 60, 0);

			item.mana = 10;
			item.damage = 20;
			item.knockBack = 5;
			
			item.useTime = item.useAnimation = 15;
			item.useStyle = ItemUseStyleID.HoldingOut;
			
			item.magic = true;
			item.noMelee = true;
			item.autoReuse = true;

			item.shootSpeed = 7.5f;
			item.shoot = ModContent.ProjectileType<Projectiles.Weapons.Magic.RedSeed>();
			
			item.UseSound = SoundID.Item20;
		}

        public override bool CanUseItem(Player player)
			=> player.ownedProjectileCounts[item.shoot] < 8;

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
