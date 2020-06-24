using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Accessories
{
	[AutoloadEquip(EquipType.Wings)]
	public class BruhWings : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Boosts max life by 100\nHealth regen increased by 5%\nMovement speed increased by 50%\nGets faster when you use it");
		        DisplayName.SetDefault("Bruh Wings");
                }
                
                public override void SetDefaults()
		{
			item.width = 22;
			item.height = 20;
			item.value = 10000;
			item.rare = 10;
			item.accessory = true;
		}
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.wingTimeMax = 300;
			player.statLifeMax2 += 100;
			player.lifeRegen =+ 5;
			player.maxRunSpeed += 50.0f;
		}

		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
			ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
		{
			ascentWhenFalling = 1.75f;
			ascentWhenRising = 0.25f;
			maxCanAscendMultiplier = 2f;
			maxAscentMultiplier = 3f;
			constantAscend = 0.350f;
		}

		public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
		{
			speed = 12.5f;
			acceleration *= 1f;
	        }

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID. SoulofFlight, 500);
            recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
  	        }
       }
} 