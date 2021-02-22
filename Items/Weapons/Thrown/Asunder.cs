using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Thrown
{
    public class Asunder : ModItem
    {
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Asunder");
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item11;
			item.crit = 8;
            item.damage = 32;
            item.melee = true;
            item.width = 64;
            item.height = 64;
            item.useTime = 17;
			item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 25, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.autoReuse = true;
            item.noUseGraphic = true;
            item.shoot = mod.ProjectileType("AsunderProjectile");
            item.shootSpeed = 12f;
        }
    }
}
	
namespace AerovelenceMod.Items.Weapons.Thrown
{
    public class AsunderProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 72;
            projectile.height = 74;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.hostile = false;
            projectile.melee = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 60;
        }
        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + ((float)Math.PI / 4);
            if (projectile.timeLeft % 30 == 0 && projectile.alpha != 255)
            {
                var proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X, projectile.velocity.Y, projectile.type, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                Main.projectile[proj].alpha = projectile.alpha + (255 / 3);
            }
        }
    }
}