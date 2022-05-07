using AerovelenceMod.Content.Projectiles.Weapons.Ranged;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class AstralShot : ModItem
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Shot");
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 28;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 0, 18, 30);

            Item.crit = 4;
            Item.damage = 9;
            Item.knockBack = 1;

            Item.useTime = Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;

            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Arrow;
            Item.shoot = ProjectileID.WoodenArrowFriendly;

            Item.UseSound = SoundID.Item5;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            float numberProjectiles = 2 + Main.rand.Next(1);
            float rotation = MathHelper.ToRadians(20);
            position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
            int type2 = ModContent.ProjectileType<DayNightProj>();
            if (Main.dayTime)
            {
                type = ProjectileID.FireArrow;
            }
            if (!Main.dayTime)
            {
                type = ProjectileID.FrostburnArrow;
            }

            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(15));
                float scale = 1f - (Main.rand.NextFloat() * .3f);
                if (i == 1)
                {
                    Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X * 2, perturbedSpeed.Y * 2, type2, damage, 2f, player.whoAmI);
                    Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, 2f, player.whoAmI);
                }
                else
                {
                    Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X * 2, perturbedSpeed.Y * 2, type, damage, 2f, player.whoAmI);
                }
            }
            return true;
        }
    }
}