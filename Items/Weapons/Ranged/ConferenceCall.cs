using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class ConferenceCall : ModItem
    {
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-6, 0);
		}
				public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Conference Call");
			Tooltip.SetDefault("Let's just ping everyone all at once");
		}
        public override void SetDefaults()
        {
			item.crit = 8;
            item.damage = 35;
            item.ranged = true;
            item.width = 68;
            item.height = 22;
            item.useTime = 32;
            item.useAnimation = 32;
			item.UseSound = SoundID.Item36;
            item.useStyle = 5;
            item.noMelee = true; //so the item's animation doesn't do damage
            item.knockBack = 8;
            item.value = 10000;
            item.rare = 8;
            item.autoReuse = true;
            item.shoot = AmmoID.Bullet;
			item.useAmmo = AmmoID.Bullet;
            item.shootSpeed = 24f;
        }
		public static Vector2[] randomSpread(float speedX, float speedY, int angle, int num)
        {
            var posArray = new Vector2[num];
            float spread = (float)(angle * 0.0125);
            float baseSpeed = (float)System.Math.Sqrt(speedX * speedX + speedY * speedY);
            double baseAngle = System.Math.Atan2(speedX, speedY);
            double randomAngle;
            for (int i = 0; i < num; ++i)
            {
                randomAngle = baseAngle + (Main.rand.NextFloat() - 0.5f) * spread;
                posArray[i] = new Vector2(baseSpeed * (float)System.Math.Sin(randomAngle), baseSpeed * (float)System.Math.Cos(randomAngle));
            }
            return (Vector2[])posArray;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2[] speeds = randomSpread(speedX, speedY, 10, 10);
            for (int i = 0; i < 10; ++i)
            {
                Projectile.NewProjectile(position.X, position.Y, speeds[i].X, speeds[i].Y, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}