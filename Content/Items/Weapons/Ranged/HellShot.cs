using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class HellShot : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("HellShot");
		}
        public override void SetDefaults()
        {
			Item.UseSound = SoundID.Item5;
			Item.crit = 9;
            Item.damage = 26;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 30;
            Item.height = 54;
            Item.useTime = 22;
			Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 2, 50, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = AmmoID.Arrow;
			Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 7f;
        }

        public static Vector2[] randomSpread(float speedX, float velocity.Y, int angle, int num)
        {
            var posArray = new Vector2[num];
            float spread = (float)(angle * 0.075);
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
            Vector2[] speeds = randomSpread(speedX, velocity.Y, 5, 5);
            for (int i = 0; i < 5; ++i)
            {
                type = Main.rand.Next(new int[] { type, ProjectileID.Hellwing, ProjectileID.HellfireArrow });
                Projectile.NewProjectile(position.X, position.Y, speeds[i].X, speeds[i].Y, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}