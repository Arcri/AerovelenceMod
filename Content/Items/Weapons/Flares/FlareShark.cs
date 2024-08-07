﻿/*
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Flares
{
    public class FlareShark : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Flareshark");
            // Tooltip.SetDefault("33% chance to not consume ammo\nShoots flares alongside bullets");
        }
        public override void SetDefaults()
        {
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item110;
            Item.crit = 4;
            Item.damage = 11;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 46;
            Item.height = 28;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0;
            Item.value = Item.sellPrice(0, 9, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FrostFlare>();
            //Item.useAmmo = AmmoID.Flare;
            Item.shootSpeed = 13f;
        }
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextFloat() >= .33f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            {
                {
                    type = Main.rand.Next(new int[] { type, ModContent.ProjectileType<FrostFlare>(), ModContent.ProjectileType<FrostFlare>() });
                }
            }
            Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(5));
            velocity.X = perturbedSpeed.X;
            velocity.Y = perturbedSpeed.Y;
            if (type != ProjectileID.Bullet) velocity *= 1.5f;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.Minishark, 1)
                .AddIngredient(ItemID.FlareGun, 1)
                .AddIngredient(ItemID.IllegalGunParts, 1)
                .AddIngredient(ItemID.HellstoneBar, 15)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
*/