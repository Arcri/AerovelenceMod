using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Melee
{
    public class SlimyKnife : ModItem
    {
				public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Slimy Knife");
			Tooltip.SetDefault("Relic");
		}
        public override void SetDefaults()
        {
			item.crit = 20;
            item.damage = 16;
            item.melee = true;
            item.width = 32;
            item.height = 32;
            item.useTime = 6;
            item.useAnimation = 6;
			item.UseSound = SoundID.Item1;
            item.useStyle = 1;
            item.knockBack = 4;
            item.value = 10000;
            item.rare = 11;
            item.autoReuse = true;
        }
    }
}