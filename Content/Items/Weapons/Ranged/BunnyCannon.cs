using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class BunnyCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bunny Cannon");
            Tooltip.SetDefault("Killing bunnies is cruel, period.");
            Tooltip.SetDefault("'How did you get this?'");
        }
        public override void SetDefaults()
        {
            item.crit = 20;
            item.damage = 8;
            item.ranged = true;
            item.width = 58;
            item.height = 40;
            item.useTime = 50;
            item.useAnimation = 50;
            item.UseSound = SoundID.Item11;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 8;
            item.value = 10000;
            item.rare = ItemRarityID.Orange;
            item.autoReuse = false;
            item.shoot = ProjectileID.ExplosiveBunny;
            item.shootSpeed = 24f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-8, 0);
        }
        public static Vector2[] randomSpread(float speedX, float speedY, int angle, int num)
        {
            var posArray = new Vector2[num];
            float spread = (float)(angle * 0.005);
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
            Vector2[] speeds = randomSpread(speedX, speedY, 24, 24);
            for (int i = 0; i < 24; ++i)
            {
                Projectile.NewProjectile(position.X, position.Y, speeds[i].X, speeds[i].Y, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}