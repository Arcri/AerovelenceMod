using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class CursedFlameStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
            DisplayName.SetDefault("Cursed Flame Staff");
            Tooltip.SetDefault("Fires 3 cursed flames");
        }
        public override void SetDefaults()
        {
            Item.crit = 6;
            Item.damage = 38;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 7;
            Item.width = 28;
            Item.height = 44;
            Item.useTime = 65;
            Item.useAnimation = 65;
            Item.UseSound = SoundID.Item34;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 2, 50, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.CursedFlameFriendly;
            Item.shootSpeed = 9f;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.SoulofNight, 15)
                .AddIngredient(ItemID.CursedFlames, 15)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 3; ++i)
            {
                type = Main.rand.Next(new int[] { type, ProjectileID.CursedFlameFriendly, ProjectileID.CursedFlameFriendly });
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, 1f, player.whoAmI);
            }
            return false;
        }
    }
}