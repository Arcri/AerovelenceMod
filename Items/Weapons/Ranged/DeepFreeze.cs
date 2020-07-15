using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
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
            item.crit = 20;
            item.damage = 19;
            item.ranged = true;
            item.width = 60;
            item.height = 32;
            item.useTime = 13;
            item.useAnimation = 13;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 8, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
            item.shoot = AmmoID.Bullet;
            item.useAmmo = AmmoID.Bullet;
            item.shootSpeed = 8f;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ProjectileID.NanoBullet, damage, knockBack, player.whoAmI);
            return false;
        }
    }
}