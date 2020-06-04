using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class FlameShot : ModItem
    {
				public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Flame Shot");
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item5;
			item.crit = 9;
            item.damage = 26;
            item.ranged = true;
            item.width = 54;
            item.height = 22;
            item.useTime = 22;
			item.useAnimation = 22;
            item.useStyle = 5;
            item.noMelee = true; //so the item's animation doesn't do damage
            item.knockBack = 4;
            item.value = 10000;
            item.rare = 3;
            item.autoReuse = true;
            item.shoot = AmmoID.Arrow;
			item.useAmmo = AmmoID.Arrow;
            item.shootSpeed = 7f;
        }
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Projectile.NewProjectile(position.X, position.Y, speedX, speedY, 2, damage, knockBack, player.whoAmI);
			return false;
		}
    }
}