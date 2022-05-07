using System;
using AerovelenceMod.Content.Projectiles.Weapons.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class BeholdersStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
            DisplayName.SetDefault("Beholder's Staff");
            Tooltip.SetDefault("Does Something");
        }
        public override void SetDefaults()
        {
            Item.crit = 11;
            Item.damage = 82;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.width = 64;
            Item.height = 64;
            Item.useTime = 65;
            Item.useAnimation = 65;
            Item.UseSound = SoundID.Item21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 10, 50, 0);
            Item.rare = ItemRarityID.Purple;
            Item.autoReuse = true;
            Item.shoot = Mod.Find<ModProjectile>("BeholderOrb").Type;
            Item.shootSpeed = 40f;
        }
        public static Vector2[] randomSpread(float speedX, float velocity.Y, int angle, int num)
        {
            var posArray = new Vector2[num];
            float spread = (float)(angle * 0.075);
            float baseSpeed = (float)Math.Sqrt(speedX * speedX + velocity.Y * velocity.Y);
            double baseAngle = Math.Atan2(speedX, velocity.Y);
            double randomAngle;
            for (int i = 0; i < num; ++i)
            {
                randomAngle = baseAngle + (Main.rand.NextFloat() - 0.5f) * spread;
                posArray[i] = new Vector2(baseSpeed * (float)Math.Sin(randomAngle), baseSpeed * (float)Math.Cos(randomAngle));
            }
            return posArray;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            float speed = 5f;
            Vector2 velocity = new Vector2(speed, speed).RotatedByRandom(MathHelper.ToRadians(360));
            Vector2 velocityEye = new Vector2(speed, speed) * 0;

            Projectile.NewProjectile(Main.MouseWorld, velocityEye, ModContent.ProjectileType<EyeOfTheBeholder>(), 50, 5f);
            for (int i = 0; i < 4; i++)
            {
                Projectile.NewProjectile(player.Center, velocity, type, 30, 5f, player.whoAmI);
                Projectile.NewProjectile(player.Center, velocity, type, 30, 5f, player.whoAmI);
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<FrostRay>(), 1)
                .AddIngredient(ItemID.StaffofEarth, 1)
                .AddIngredient(ItemID.SpectreStaff, 1)
                .AddIngredient(ItemID.ShadowbeamStaff, 1)
                .AddIngredient(ItemID.Ectoplasm, 40)
                .AddIngredient(ItemID.ShroomiteBar, 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}