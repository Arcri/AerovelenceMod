using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod;

namespace AerovelenceMod.Items.Others
{
	public class KnowledgeFruit : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fruit of Knowledge");
			Tooltip.SetDefault("Permanently increases maximum life by 50\n" +
                "You must have 500 max life or more to eat this item.");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.useAnimation = 30;
			item.useTime = 30;
			item.useStyle = ItemUseStyleID.HoldingUp;
			item.UseSound = SoundID.Item4;
			item.consumable = true;
			item.rare = ItemRarityID.Cyan;
		}

		public override bool CanUseItem(Player player)
		{
			AeroPlayer modPlayer = player.GetModPlayer<AeroPlayer>();
			return !modPlayer.KnowledgeFruit && player.statLifeMax >= 500;
		}

		public override bool UseItem(Player player)
		{
			if (player.itemAnimation > 0 && player.itemTime == 0)
			{
				player.itemTime = item.useTime;
				if (Main.myPlayer == player.whoAmI)
				{
					player.HealEffect(50, true);
				}
                AeroPlayer modPlayer = player.GetModPlayer<AeroPlayer>();
				modPlayer.KnowledgeFruit = true;
			}
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe modRecipe = new ModRecipe(mod);
			modRecipe.AddIngredient(ItemID.LifeFruit, 1);
            modRecipe.AddIngredient(ItemID.FragmentVortex, 3);
            modRecipe.AddIngredient(ItemID.FragmentNebula, 3);
            modRecipe.AddIngredient(ItemID.FragmentSolar, 3);
            modRecipe.AddIngredient(ItemID.FragmentStardust, 3);
            modRecipe.AddIngredient(ItemID.SoulofSight, 45);
            modRecipe.AddTile(TileID.MythrilAnvil);
            modRecipe.SetResult(this, 1);
			modRecipe.AddRecipe();
		}
	}
}
