using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class GlassPulseBow : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Glass Pulse Bow");
		}
        public override void SetDefaults()
        {
            item.shootSpeed = 24f;
            item.crit = 8;
            item.damage = 30;
            item.ranged = true;
            item.width = 32;
            item.height = 32;
            item.useTime = 20;
            item.useAnimation = 20;
            item.UseSound = SoundID.Item5;
            item.shootSpeed = 55;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 0, 35, 20);
            item.rare = ItemRarityID.Green;
            item.shoot = mod.ProjectileType("PrismaticBolt");
            item.autoReuse = true;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            {
                Main.PlaySound(SoundID.Item72);
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(0f));
                Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, mod.ProjectileType("PrismaticBolt"), damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}