using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class CrystalArch : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Arch");
        }
        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item5;
            item.crit = 20;
            item.damage = 27;
            item.ranged = true;
            item.width = 60;
            item.height = 32;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = 10000;
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
            item.shoot = AmmoID.Arrow;
            item.useAmmo = AmmoID.Arrow;
            item.shootSpeed = 8f;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ProjectileID.FrostArrow, damage, knockBack, player.whoAmI);
            return false;
        }
    }
}