using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class ShiningShooter : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shining Shooter");
            Tooltip.SetDefault("Fires a large, slow travelling crystal");
        }
        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item5;
            item.crit = 4;
            item.damage = 12;
            item.ranged = true;
            item.width = 60;
            item.height = 32;
            item.useTime = 65;
            item.useAnimation = 65;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 0, 50, 0);
            item.rare = ItemRarityID.Blue;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("ShiningShooterProj");
            item.shootSpeed = 6f;
        }
    }


    public class ShiningShooterProj : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 40;
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.hostile = false;
            projectile.ranged = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 300;
            projectile.aiStyle = 1;
        }
    }
}