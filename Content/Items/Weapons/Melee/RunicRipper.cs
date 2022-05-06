using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class RunicRipper : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Runic Ripper");
		}
		public override void SetDefaults()
		{
			Item.crit = 4;
			Item.damage = 9;
			Item.DamageType = DamageClass.Melee;
			Item.width = 36;
			Item.height = 38;
			Item.useTime = 6;
			Item.useAnimation = 6;
			Item.UseSound = SoundID.Item1;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 4;
			Item.value = Item.sellPrice(0, 2, 50, 0);
			Item.rare = ItemRarityID.Purple;
			Item.autoReuse = true;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}
	}
}