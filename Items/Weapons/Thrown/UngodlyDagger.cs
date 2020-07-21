using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Thrown
{
    public class UngodlyDagger : ModItem
    {
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ungodly Dagger");
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item11;
			item.crit = 8;
            item.damage = 56;
            item.melee = true;
            item.width = 60;
            item.height = 32;
            item.useTime = 17;
			item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 25, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.autoReuse = true;
            item.noUseGraphic = true;
            item.shoot = mod.ProjectileType("UngodlyDaggerProjectile");
            item.shootSpeed = 12f;
        }
    }
}

namespace AerovelenceMod.Items.Weapons.Thrown
{
    public class UngodlyDaggerProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 38;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.hostile = false;
            projectile.melee = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 360;
        }
        public override void AI()
        {
            if (projectile.timeLeft > 315)
            {
                projectile.rotation += 1.0471975511965977461542144610932f;
            }
            else
            {
                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 1.5707963267948966192313216916398f;
                projectile.velocity *= 1.02f;
                if (projectile.timeLeft % 2 == 0)
                    projectile.damage++;
            }
        }
    }
}