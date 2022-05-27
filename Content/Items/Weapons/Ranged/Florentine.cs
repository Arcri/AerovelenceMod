using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class Florentine : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Florentine");
			Tooltip.SetDefault("Has a chance to shoot electric volleys alongside bullets\n33% chance to not consume ammo");
		}
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
			return Main.rand.NextFloat() >= .33f;
		}
        public override void SetDefaults()
        {
			Item.UseSound = SoundID.Item31;
			Item.crit = 20;
            Item.damage = 72;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 48;
            Item.height = 32;
            Item.useTime = 5;
			Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6;
			Item.value = Item.sellPrice(0, 15, 50, 0);
			Item.rare = ItemRarityID.Lime;
            Item.autoReuse = true;
            Item.shoot = AmmoID.Bullet;
			Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 8f;
        }

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-10, 0);
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
        Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ProjectileID.BulletHighVelocity, damage, knockback, player.whoAmI);
			return false;
		}
    }
}
