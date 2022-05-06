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
            Item.UseSound = SoundID.Item1;
            Item.crit = 20;
            Item.damage = 93;
            Item.DamageType = DamageClass.Melee;
            Item.width = 42;
            Item.height = 56;
            Item.useTime = 27;
            Item.useAnimation = 27;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
            Item.value = 10000;
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 8f;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ProjectileID.BallofFrost, damage, knockBack, player.whoAmI);
            return false;
        }
    }
}