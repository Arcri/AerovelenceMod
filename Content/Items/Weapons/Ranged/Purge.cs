using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
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
			Item.value = Item.sellPrice(0, 25, 0, 0);
			Item.UseSound = SoundID.Item40;
			Item.crit = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 60;
            Item.height = 32;
            Item.useTime = 13;
			Item.useAnimation = 13;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6;
            Item.rare = ItemRarityID.Cyan;
            Item.autoReuse = true;
            Item.shoot = AmmoID.Bullet;
			Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 8f;
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