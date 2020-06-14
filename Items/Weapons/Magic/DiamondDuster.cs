using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Projectiles;
using AerovelenceMod;
using AerovelenceMod.Items.Ores.PreHM.Frost;

namespace AerovelenceMod.Items.Weapons.Magic
{
    public class DiamondDuster : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
            DisplayName.SetDefault("Diamond Duster");
            Tooltip.SetDefault("A spell book found deep within the caverns");
        }
        public override void SetDefaults()
        {
            item.crit = 6;
            item.damage = 11;
            item.magic = true;
            item.mana = 15;
            item.width = 28;
            item.height = 30;
            item.useTime = 15;
            item.useAnimation = 15;
            item.UseSound = SoundID.Item101;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 1, 30, 0);
            item.rare = ItemRarityID.Green;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("CrystalShard");
            item.shootSpeed = 40f;
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