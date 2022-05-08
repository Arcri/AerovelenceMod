using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class PagesOfDawn : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
            DisplayName.SetDefault("Pages of Dawn");
            Tooltip.SetDefault("'You feel hot when holding this'");
        }
        public override void SetDefaults()
        {
            Item.crit = 11;
            Item.damage = 25;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 6;
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
            Item.shoot = ProjectileID.BallofFire;
            Item.shootSpeed = 6f;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.HellstoneBar, 12)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(10));
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 3; ++i)
            {
                type = Main.rand.Next(new int[] { type, ProjectileID.BallofFire, ProjectileID.BallofFire });
                Projectile.NewProjectile(Item.GetSource_FromThis(), position.X, position.Y, velocity.X, velocity.Y, type, damage, 1f, player.whoAmI);
            }
            return false;
        }
    }
}