using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class Zodiac : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Zodiac");
            Tooltip.SetDefault("Fires two bullets at once");
        }
        public override void SetDefaults()
        {
            item.value = Item.sellPrice(1);
            item.damage = 25;
            item.ranged = true;
            item.width = 68;
            item.height = 28;
            item.useTime = 5;
            item.useAnimation = 5;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 5f;
            item.rare = ItemRarityID.Orange;
            item.noMelee = true;
            item.UseSound = SoundID.Item41;
            item.autoReuse = true;
            item.shootSpeed = 45f;
            item.shoot = AmmoID.Bullet;
            item.useAmmo = AmmoID.Bullet;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float numberProjectiles = 2;
            float rotation = MathHelper.ToRadians(6);
            position += Vector2.Normalize(new Vector2(speedX, speedY)) * 10f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f;
                Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}