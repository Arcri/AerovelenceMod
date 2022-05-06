using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using AerovelenceMod.Content.Projectiles.Weapons.Magic;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class ScepterOfTheSkies : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
            DisplayName.SetDefault("Scepter of the skies");
            Tooltip.SetDefault("Unleashes the power from the sky");
        }

        public override void SetDefaults()
        {
            Item.crit = 5;
            Item.damage = 58;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.width = 64;
            Item.height = 64;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.UseSound = SoundID.Item21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 10, 50, 0);
            Item.rare = ItemRarityID.Purple;
            Item.autoReuse = true;
            Item.shoot = Mod.Find<ModProjectile>("HomingWisp").Type;
            Item.shootSpeed = 12f;
        }

        public static Vector2[] randomSpread(float speedX, float speedY, int angle, int num)
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
            Vector2[] speeds = randomSpread(speedX, speedY, 5, 2);
            for (int i = 0; i < 2; ++i)
            {
                type = Main.rand.Next(new int[] { type, ModContent.ProjectileType<HomingWisp>(), ModContent.ProjectileType<HomingWispPurple>() });
                Projectile.NewProjectile(position.X, position.Y, speeds[i].X, speeds[i].Y, type, damage, knockBack, player.whoAmI, 1);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<Delvara>(), 1)
                .AddIngredient(ItemID.CrystalVileShard, 1)
                .AddIngredient(ItemID.HallowedBar, 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}