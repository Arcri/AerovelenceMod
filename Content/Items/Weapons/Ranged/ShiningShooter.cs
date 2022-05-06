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
            Item.UseSound = SoundID.Item5;
            Item.crit = 4;
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 60;
            Item.height = 32;
            Item.useTime = 65;
            Item.useAnimation = 65;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.shoot = Mod.Find<ModProjectile>("ShiningShooterProj").Type;
            Item.shootSpeed = 6f;
        }
    }


    public class ShiningShooterProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.aiStyle = 1;
        }
    }
}