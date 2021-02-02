using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class ShiningShooter : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shining Shooter");
        }
        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item5;
            item.crit = 20;
            item.damage = 27;
            item.ranged = true;
            item.width = 60;
            item.height = 32;
            item.useTime = 65;
            item.useAnimation = 65;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = 10000;
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("ShiningShooterProj");
            item.shootSpeed = 6f;
        }
    }
}



namespace AerovelenceMod.Items.Weapons.Ranged
{
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