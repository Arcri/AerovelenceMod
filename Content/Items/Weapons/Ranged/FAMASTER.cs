using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class FAMASTER : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Famaster");
		}
		public override bool ConsumeAmmo(Player player)
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

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ProjectileID.BulletHighVelocity, damage, knockBack, player.whoAmI);
			return false;
		}
    }
}