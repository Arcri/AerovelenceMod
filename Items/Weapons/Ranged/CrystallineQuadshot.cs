using AerovelenceMod.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class CrystallineQuadshot : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystalline Quadshot");
            Tooltip.SetDefault("Fires rock and crystal shards");
        }
        public override void SetDefaults()
        {
            item.crit = 20;
            item.damage = 15;
            item.ranged = true;
            item.width = 32;
            item.height = 32;
            item.useTime = 50;
            item.useAnimation = 50;
            item.UseSound = SoundID.Item68;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 8;
            item.value = Item.sellPrice(0, 1, 15, 0);
            item.rare = ItemRarityID.Green;
            item.autoReuse = false;
            item.shoot = ModContent.ProjectileType<CrystalShard>();
            item.shootSpeed = 24f;
        }

        public static Vector2[] RandomSpread(float speedX, float speedY, int angle, int num)
        {
            var posArray = new Vector2[num];
            float spread = (float)(angle * 0.075);
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
            Vector2[] speeds = RandomSpread(speedX, speedY, 5, 5);
            for (int i = 0; i < 4; ++i)
            {
                type = Main.rand.Next(new int[] { type, ModContent.ProjectileType<RockShard>(), ModContent.ProjectileType<CrystalShard>() });
                Projectile.NewProjectile(position.X, position.Y, speeds[i].X, speeds[i].Y, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}