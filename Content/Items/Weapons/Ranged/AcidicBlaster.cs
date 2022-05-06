using System;
using AerovelenceMod.Content.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class AcidicBlaster : ModItem
    {
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-4, 0);
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Acidic Blaster");
		}
        public override void SetDefaults()
        {
			Item.shootSpeed = 24f;
			Item.crit = 8;
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 72;
            Item.height = 32;
            Item.useTime = 20;
            Item.useAnimation = 20;
			Item.UseSound = SoundID.Item5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 0, 35, 20);
            Item.rare = ItemRarityID.Green;
			Item.shoot = ModContent.ProjectileType<DiseasedBlob>();
            Item.autoReuse = true;
        }
        public static Vector2[] randomSpread(float speedX, float speedY, int angle, int num)
        {
            var posArray = new Vector2[num];
            float spread = (float)(angle * 0.033);
            float baseSpeed = (float)Math.Sqrt(speedX * speedX + speedY * speedY);
            double baseAngle = Math.Atan2(speedX, speedY);
            double randomAngle;
            for (int i = 0; i < num; ++i)
            {
                randomAngle = baseAngle + (Main.rand.NextFloat() - 0.5f) * spread;
                posArray[i] = new Vector2(baseSpeed * (float)Math.Sin(randomAngle), baseSpeed * (float)Math.Cos(randomAngle));
            }
            return posArray;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2[] speeds = randomSpread(speedX, speedY, 3, 3);
            for (int i = 0; i < 3; ++i)
            {
                Projectile.NewProjectile(position.X, position.Y, speeds[i].X, speeds[i].Y, Mod.Find<ModProjectile>("DiseasedBlob").Type, damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}