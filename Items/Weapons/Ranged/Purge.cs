using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class Purge : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Purge");
            Tooltip.SetDefault("Damage increases the slower the player is moving");
        }
        public override void SetDefaults()
        {
			item.value = Item.sellPrice(0, 25, 0, 0);
			item.UseSound = SoundID.Item40;
			item.crit = 20;
            item.ranged = true;
            item.width = 60;
            item.height = 32;
            item.useTime = 13;
			item.useAnimation = 13;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.rare = ItemRarityID.Cyan;
            item.autoReuse = true;
            item.shoot = AmmoID.Bullet;
			item.useAmmo = AmmoID.Bullet;
            item.shootSpeed = 8f;
        }
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int deg = 4;
            float numberProjectiles = 5;
            float rotation = MathHelper.ToRadians(deg);
            position += Vector2.Normalize(new Vector2(speedX, speedY)) * deg;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f; // Watch out for dividing by 0 if there is only 1 projectile.
                int value = player.velocity.Length() != 0 ? 2500 / (int)Math.Ceiling((double)player.velocity.Length()) : 150;
                perturbedSpeed *= 7;
                Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, ProjectileID.NailFriendly, (int)MathHelper.Clamp(value, 25, 150), knockBack, player.whoAmI);
            }
			return false;
		}
    }
}