using AerovelenceMod.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Magic
{
    public class OriginPulse : ModItem
    {
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Origin Pulse");
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item11;
			item.crit = 8;
            item.damage = 49;
            item.ranged = true;
            item.width = 36;
            item.height = 42;
            item.useTime = 14;
			item.useAnimation = 14;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 25, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("OriginPulseProj");
            item.shootSpeed = 14f;
        }
    }
}

namespace AerovelenceMod.Items.Weapons.Magic
{
    public class OriginPulseProj : ModProjectile
    {

        int t;
        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 36;
            projectile.friendly = true;
            projectile.penetrate = 5;
            projectile.hostile = false;
            projectile.magic = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 420;
        }
        public override void AI()
        {
            t++;
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) - 3.141592653f;
            projectile.scale *= (1 + 0.00238095238095238095238095238095f);
            if (projectile.timeLeft % 30 == 0)
            {
                projectile.damage += 4;
            }
            if(t % 50 == 0)
            {
                Vector2 offset = new Vector2(0, 0);
                Projectile.NewProjectile(projectile.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<VoidBolt>(), 6, 1f, Main.myPlayer);
            }
        }
    }
}