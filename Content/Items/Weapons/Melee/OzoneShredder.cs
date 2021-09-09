using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class OzoneShredder : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ozone Shredder");
        }
        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item1;
            item.crit = 20;
            item.damage = 93;
            item.melee = true;
            item.width = 42;
            item.height = 56;
            item.useTime = 27;
            item.useAnimation = 27;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 5;
            item.value = 10000;
            item.rare = ItemRarityID.Yellow;
            item.autoReuse = true;
            item.shoot = ProjectileID.WoodenArrowFriendly;
            item.shootSpeed = 8f;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ProjectileID.BallofFrost, damage, knockBack, player.whoAmI);
            return false;
        }
    }
}