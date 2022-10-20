﻿using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry
{
	public class CyverWhip : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DamageType = DamageClass.SummonMeleeSpeed;
			Item.damage = 57;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.Green;

			Item.shoot = ModContent.ProjectileType<CyverWhipProj>();
			Item.shootSpeed = 2; //2

			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 50; //30
			Item.useAnimation = 50; //60
			Item.UseSound = SoundID.Item152;
			Item.noMelee = true;
			Item.noUseGraphic = true;

			Item.rare = ItemRarityID.LightRed;
		}

		public override void AddRecipes()
		{
			//CreateRecipe()
				//.AddIngredient<Items.Others.Crafting.BurnshockBar>(10)
				//.AddTile(TileID.WorkBenches)
				//.Register();
		}

		public override bool MeleePrefix()
		{
			return true;
		}
	}
}