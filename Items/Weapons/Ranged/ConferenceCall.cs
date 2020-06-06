using System;
using AerovelenceMod.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class ConferenceCall : ModItem
    {
			public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Conference Call");
			Tooltip.SetDefault("Let's just ping everyone all at once");
		}

        public override void SetDefaults()
        {
			item.crit = 11;
            item.damage = 464;
            item.magic = true;
            item.width = 60;
            item.height = 32; 
            item.useTime = 34;
            item.useAnimation = 34;
            item.useStyle = 5;
            item.noMelee = true; //so the item's animation doesn't do damage
            item.knockBack = 13;
            item.value = 10000;
            item.rare = 11;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("ShatteredSoul");
            item.shootSpeed = 32f;
        }
		
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-6, 0);
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