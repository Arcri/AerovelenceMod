using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
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
            Item.crit = 11;
            Item.damage = 55;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 84;
            Item.height = 22;
            Item.useTime = 34;
            Item.useAnimation = 34;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 13;
            Item.value = Item.sellPrice(0, 15, 50, 20);
            Item.rare = ItemRarityID.Lime;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item36;
            Item.shoot = Mod.Find<ModProjectile>("ShatteredSoul").Type;
            Item.shootSpeed = 10f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-8, 0);
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
            return posArray;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2[] speeds = randomSpread(speedX, speedY, 10, 10);
            for (int i = 0; i < 10; ++i)
            {
                Projectile.NewProjectile(position.X, position.Y, speeds[i].X * Main.rand.NextFloat(0.9f, 1.1f), speeds[i].Y * Main.rand.NextFloat(0.9f, 1.1f), type, damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}