using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class CursedFlameStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
            DisplayName.SetDefault("Cursed Flame Staff");
            Tooltip.SetDefault("Fires 3 cursed flames");
        }
        public override void SetDefaults()
        {
            item.crit = 6;
            item.damage = 38;
            item.magic = true;
            item.mana = 7;
            item.width = 28;
            item.height = 44;
            item.useTime = 65;
            item.useAnimation = 65;
            item.UseSound = SoundID.Item34;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 2, 50, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
            item.shoot = ProjectileID.CursedFlameFriendly;
            item.shootSpeed = 9f;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SoulofNight, 15);
            recipe.AddIngredient(ItemID.CursedFlames, 15);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
        public static Vector2[] RandomSpread(float speedX, float speedY, int angle, int num)
        {
            var posArray = new Vector2[num];
            float spread = (float)(angle * 0.075);
            float baseSpeed = (float)Math.Sqrt(speedX * speedX + speedY * speedY);
            double baseAngle = Math.Atan2(speedX, speedY);
            double randomAngle;
            for (int i = 0; i < num; ++i)
            {
                randomAngle = baseAngle + (Main.rand.NextFloat() - 0.1f) * spread;
                posArray[i] = new Vector2(baseSpeed * (float)Math.Sin(randomAngle), baseSpeed * (float)Math.Cos(randomAngle));
            }
            return posArray;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2[] speeds = RandomSpread(speedX, speedY, 5, 5);
            for (int i = 0; i < 3; ++i)
            {
                type = Main.rand.Next(new int[] { type, ProjectileID.CursedFlameFriendly, ProjectileID.CursedFlameFriendly });
                Projectile.NewProjectile(position.X, position.Y, speeds[i].X, speeds[i].Y, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}