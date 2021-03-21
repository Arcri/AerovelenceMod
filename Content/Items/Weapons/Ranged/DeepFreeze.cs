using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class DeepFreeze : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deep Freeze");
        }
        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item40;
            item.crit = 4;
            item.damage = 20;
            item.ranged = true;
            item.width = 70;
            item.height = 44;
            item.useTime = 17;
            item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 1;
            item.value = Item.sellPrice(0, 2, 50, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
            item.shoot = AmmoID.Bullet;
            item.useAmmo = AmmoID.Bullet;
            item.shootSpeed = 8f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-6, 0);
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(5));
            speedX = perturbedSpeed.X;
            speedY = perturbedSpeed.Y;
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ProjectileID.NanoBullet, damage, knockBack, player.whoAmI);
            return false;
        }
    }
}