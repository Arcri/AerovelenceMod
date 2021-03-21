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
            Item.staff[item.type] = true;
            DisplayName.SetDefault("Beholder's Staff");
            Tooltip.SetDefault("Does Something");
        }
        public override void SetDefaults()
        {
            item.crit = 11;
            item.damage = 82;
            item.magic = true;
            item.mana = 20;
            item.width = 64;
            item.height = 64;
            item.useTime = 65;
            item.useAnimation = 65;
            item.UseSound = SoundID.Item21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 10, 50, 0);
            item.rare = ItemRarityID.Purple;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("BeholderOrb");
            item.shootSpeed = 40f;
        }
        public static Vector2[] randomSpread(float speedX, float speedY, int angle, int num)
        {
            var posArray = new Vector2[num];
            float spread = (float)(angle * 0.075);
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
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<FrostRay>(), 1);
            modRecipe.AddIngredient(ItemID.StaffofEarth, 1);
            modRecipe.AddIngredient(ItemID.SpectreStaff, 1);
            modRecipe.AddIngredient(ItemID.ShadowbeamStaff, 1);
            modRecipe.AddIngredient(ItemID.Ectoplasm, 40);
            modRecipe.AddIngredient(ItemID.ShroomiteBar, 15);
            modRecipe.AddTile(TileID.MythrilAnvil);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}