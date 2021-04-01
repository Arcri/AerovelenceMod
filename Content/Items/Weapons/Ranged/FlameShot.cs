using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class FlameShot : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Flameshot");
            Tooltip.SetDefault("Shoots a flame-emissive fire arrow");
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item5;
			item.crit = 9;
            item.damage = 26;
            item.ranged = true;
            item.width = 30;
            item.height = 54;
            item.useTime = 22;
			item.useAnimation = 22;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 2, 50, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
            item.shoot = ProjectileID.FireArrow;
			item.useAmmo = AmmoID.Arrow;
            item.shootSpeed = 7f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), ProjectileID.FireArrow, damage, knockBack, player.whoAmI);
            return false;
        }
    }
    public class FireArrow : GlobalProjectile
    {
        public override void PostAI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            if (projectile.type == ProjectileID.FireArrow)
            {
                if (projectile.active && projectile.type == ProjectileID.FireArrow && player.HeldItem.modItem is FlameShot fs)
                {
                    int projectiles = 3;
                    Vector2 position = projectile.Center;
                    float numberProjectiles = 4f;
                    float rotation = MathHelper.ToRadians(180f);
                    int i = 0;
                    if (Main.rand.NextFloat() <= 0.01f)
                    {
                        while (i < numberProjectiles)
                        {
                            Vector2 perturbedSpeed = Utils.RotatedBy(new Vector2(projectile.velocity.X, projectile.velocity.Y), MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1f)), default) * 0.2f;
                            var m = Projectile.NewProjectileDirect(position, new Vector2(perturbedSpeed.X, perturbedSpeed.Y), ProjectileID.MolotovFire, (int)(player.HeldItem.damage * 1.5f), 0, player.whoAmI, 0f, 0f);
                            m.friendly = true;
                            m.hostile = false;
                            i++;
                        }
                    }
                }
            }
        }
    }
}