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
            Tooltip.SetDefault("Killing bunnies is cruel, period. Yet you STILL did it.");
            Tooltip.SetDefault("'How did you get this?'");
        }
        public override void SetDefaults()
        {
            Item.crit = 20;
            Item.damage = 8;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 58;
            Item.height = 40;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.UseSound = SoundID.Item11;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 8;
            Item.value = 10000;
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = false;
            Item.shoot = ProjectileID.ExplosiveBunny;
            Item.shootSpeed = 24f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-8, 0);
        }
        public static Vector2[] randomSpread(float speedX, float velocity.Y, int angle, int num)
        {
            var posArray = new Vector2[num];
            float spread = (float)(angle * 0.005);
            float baseSpeed = (float)System.Math.Sqrt(speedX * speedX + velocity.Y * velocity.Y);
            double baseAngle = System.Math.Atan2(speedX, velocity.Y);
            double randomAngle;
            for (int i = 0; i < num; ++i)
            {
                randomAngle = baseAngle + (Main.rand.NextFloat() - 0.5f) * spread;
                posArray[i] = new Vector2(baseSpeed * (float)System.Math.Sin(randomAngle), baseSpeed * (float)System.Math.Cos(randomAngle));
            }
            return (Vector2[])posArray;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2[] speeds = randomSpread(speedX, velocity.Y, 24, 24);
            for (int i = 0; i < 24; ++i)
            {
                Projectile.NewProjectile(position.X, position.Y, speeds[i].X, speeds[i].Y, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}