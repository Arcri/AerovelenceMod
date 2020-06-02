using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class Stinger : ModItem
    {
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-4, 0);
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stinger");
		}
        public override void SetDefaults()
        {
			item.shootSpeed = 24f;
			item.crit = 8;
            item.damage = 12;
            item.ranged = true;
            item.width = 32;
            item.height = 32;
            item.useTime = 20;
            item.useAnimation = 20;
			item.UseSound = SoundID.Item5;
            item.useStyle = 5;
            item.knockBack = 4;
            item.value = 10000;
            item.rare = 2;
			item.shoot = 249;
            item.autoReuse = true;
        }
    }
}