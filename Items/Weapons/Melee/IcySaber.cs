using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Melee
{
    public class IcySaber : ModItem
    {
				public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Icy Saber");
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item1;
			item.crit = 20;
            item.damage = 24;
            item.ranged = true;
            item.width = 60;
            item.height = 32;
            item.useTime = 24;
			item.useAnimation = 24;
            item.useStyle = 1;
            item.knockBack = 6;
            item.value = 10000;
            item.rare = 11;
            item.autoReuse = true;
            item.shoot = 119;
            item.shootSpeed = 8f;
        }
    }
}