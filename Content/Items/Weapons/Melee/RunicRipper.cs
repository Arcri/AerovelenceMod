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
			item.crit = 4;
			item.damage = 9;
			item.melee = true;
			item.width = 36;
			item.height = 38;
			item.useTime = 6;
			item.useAnimation = 6;
			item.UseSound = SoundID.Item1;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.knockBack = 4;
			item.value = Item.sellPrice(0, 2, 50, 0);
			item.rare = ItemRarityID.Purple;
			item.autoReuse = true;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}
	}
}