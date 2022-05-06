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
			Item.UseSound = SoundID.Item5;
			Item.crit = 9;
            Item.damage = 26;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 30;
            Item.height = 54;
            Item.useTime = 22;
			Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 2, 50, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.FireArrow;
			Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 7f;
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