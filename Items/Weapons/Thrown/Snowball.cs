using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Thrown
{
    public class Snowball : ModItem
    {
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Snowball");
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item1;
			item.crit = 5;
            item.damage = 44;
            item.ranged = true;
            item.width = 60;
            item.height = 32;
            item.useTime = 17;
			item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 6, 10, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
            item.noUseGraphic = true;
            item.shoot = mod.ProjectileType("SnowballProjectile");
            item.shootSpeed = 5f;
		}
    }
}
namespace AerovelenceMod.Items.Weapons.Thrown
{
    public class SnowballProjectile : ModProjectile
    {
		public override void SetStaticDefaults()
		{
		DisplayName.SetDefault("Snowball blast");
		}
        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 20;
            projectile.aiStyle = 16;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 1;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 65, projectile.oldVelocity.X * 0.2f, projectile.oldVelocity.Y * 0.2f);
			}
            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 10);
        }
    }
}