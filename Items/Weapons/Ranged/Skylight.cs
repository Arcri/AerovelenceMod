using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class Skylight : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Skylight");
			Tooltip.SetDefault("Pew pew");
		}
		public override Vector2? HoldoutOffset()
        {
            return new Vector2(3, -2);
        }
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item40;
			item.crit = 20;
            item.damage = 93;
            item.ranged = true;
            item.width = 60;
            item.height = 32; 
            item.useTime = 3;
            item.useAnimation = 3;
            item.useStyle = 5;
            item.noMelee = true; //so the item's animation doesn't do damage
            item.knockBack = 6;
            item.value = 10000;
            item.rare = 6;
            item.autoReuse = true;
            item.shoot = AmmoID.Bullet;
			item.useAmmo = AmmoID.Bullet;
            item.shootSpeed = 24f;
		}
    }
}