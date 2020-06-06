using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class FAMASTER : ModItem
    {
				public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("FAMASTER");
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-14, 0);
		}
		public override bool ConsumeAmmo(Player player)
		{
			return Main.rand.NextFloat() >= .33f;
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item31;
			item.crit = 20;
            item.damage = 72;
            item.ranged = true;
            item.width = 60;
            item.height = 32;
            item.useTime = 5;
			item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true; //so the item's animation doesn't do damage
            item.knockBack = 6;
            item.value = 10000;
            item.rare = ItemRarityID.Purple;
            item.autoReuse = true;
            item.shoot = AmmoID.Bullet;
			item.useAmmo = AmmoID.Bullet;
            item.shootSpeed = 8f;
        }
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ProjectileID.BulletHighVelocity, damage, knockBack, player.whoAmI);
			return false;
		}
    }
}