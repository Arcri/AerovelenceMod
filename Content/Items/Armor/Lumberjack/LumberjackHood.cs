#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

#endregion

namespace AerovelenceMod.Content.Items.Armor.Lumberjack
{
	[AutoloadEquip(EquipType.Head)]
	public sealed class LumberjackHood : ModItem
	{
		public override void SetDefaults()
		{
			item.width = item.height = 22;
			item.rare = ItemRarityID.Blue;
			item.value = Item.sellPrice(copper: 20);

			item.defense = 1;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
			=> body.type == ModContent.ItemType<LumberjackShirt>() && legs.type == ModContent.ItemType<LumberjackBoots>();

		public override void UpdateArmorSet(Player player)
		{
			var ap = player.GetModPlayer<AeroPlayer>();

			player.meleeSpeed += 0.05f;
			ap.lumberjackSetBonus = true;
			player.minionDamage += 0.15f;

			int axeProjectileType = ModContent.ProjectileType<Projectiles.Other.ArmorSetBonus.LumberjackAxe>();
			if (player.ownedProjectileCounts[axeProjectileType] < 1)
			{
				Projectile.NewProjectile(player.Center, default, axeProjectileType, 10, 0.5f, player.whoAmI);
			}

			player.setBonus = "Increases melee speed by 5%\nIncreases summon damage by 15%\nA sharp axe accompanies you...";
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddRecipeGroup("Wood", 15);
			recipe.AddIngredient(ItemID.Silk, 3);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
