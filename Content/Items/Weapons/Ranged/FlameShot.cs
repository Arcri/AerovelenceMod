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
            item.shoot = AmmoID.Arrow;
			item.useAmmo = AmmoID.Arrow;
            item.shootSpeed = 7f;
        }
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ProjectileID.FireArrow, damage, knockBack, player.whoAmI);
			return false;
		}
    }
}