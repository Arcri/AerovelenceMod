using AerovelenceMod.Content.Projectiles.Weapons.Ranged;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
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
            item.width = 12;
            item.height = 28;
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(0, 0, 18, 30);

            item.crit = 4;
            item.damage = 9;
            item.knockBack = 1;

            item.useTime = item.useAnimation = 22;
            item.useStyle = ItemUseStyleID.HoldingOut;

            item.ranged = true;
            item.noMelee = true;

            item.shootSpeed = 16f;
            item.useAmmo = AmmoID.Arrow;
            item.shoot = ProjectileID.WoodenArrowFriendly;

            item.UseSound = SoundID.Item5;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float numberProjectiles = 2 + Main.rand.Next(1);
            float rotation = MathHelper.ToRadians(20);
            position += Vector2.Normalize(new Vector2(speedX, speedY)) * 45f;
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
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(15));
                float scale = 1f - (Main.rand.NextFloat() * .3f);
                if (i == 1)
                {
                    Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X * 2, perturbedSpeed.Y * 2, type2, damage, knockBack, player.whoAmI);
                    Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
                }
                else
                {
                    Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X * 2, perturbedSpeed.Y * 2, type, damage, knockBack, player.whoAmI);
                }
            }
            return true;
        }
    }
}
