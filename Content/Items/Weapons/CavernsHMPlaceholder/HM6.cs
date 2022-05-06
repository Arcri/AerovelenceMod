using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.CavernsHMPlaceholder
{
    public class HM6 : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
            DisplayName.SetDefault("Placeholder 6");
            Tooltip.SetDefault("A spell book found deep within the caverns");
        }
        public override void SetDefaults()
        {
            Item.crit = 4;
            Item.damage = 11;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.width = 36;
            Item.height = 34;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.UseSound = SoundID.Item101;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 1, 30, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = Mod.Find<ModProjectile>("CrystalShard").Type;
            Item.shootSpeed = 12f;
        }
        public static Vector2[] randomSpread(float speedX, float speedY, int angle, int num)
        {
            var posArray = new Vector2[num];
            float spread = (float)(angle * 0.075);
            float baseSpeed = (float)System.Math.Sqrt(speedX * speedX + speedY * speedY);
            double baseAngle = System.Math.Atan2(speedX, speedY);
            double randomAngle;
            for (int i = 0; i < 2; ++i)
            {
                randomAngle = baseAngle + (Main.rand.NextFloat() - 0.5f) * spread;
                posArray[i] = new Vector2(baseSpeed * (float)System.Math.Sin(randomAngle), baseSpeed * (float)System.Math.Cos(randomAngle));
            }
            return (Vector2[])posArray;
        }
    }
}