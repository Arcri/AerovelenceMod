using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Thrown
{
    public class ElectrapulseCanister : ModItem
    {
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Electrapulse Canister");
            Tooltip.SetDefault("Hitting a tile or enemy releases a large electric explosion that releases lightning");
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item1;
			item.crit = 8;
            item.damage = 12;
            item.melee = true;
            item.width = 60;
            item.height = 32;
            item.useTime = 17;
			item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 0, 50, 0);
            item.rare = ItemRarityID.Blue;
            item.autoReuse = true;
            item.noUseGraphic = true;
            item.shoot = mod.ProjectileType("ElectrapulseCanisterProj");
            item.shootSpeed = 16f;
		}
    }
}
	
namespace AerovelenceMod.Items.Weapons.Thrown
{
    public class ElectrapulseCanisterProj : ModProjectile
    {
        public bool e;
        public float rot = 0.5f;
        public int i;
        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 28;
            projectile.friendly = true;
            projectile.aiStyle = 2;
            projectile.penetrate = 3;
            projectile.hostile = false;
            projectile.melee = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 120;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            int spawnX = (int)(projectile.Center.X / 64) * 64;
            int spawnY = (int)((projectile.position.Y - projectile.height) / 16) * 16;
            int index = Projectile.NewProjectile(spawnX, spawnY + 70, projectile.velocity.X, projectile.velocity.Y, ModContent.ProjectileType<CrystalGrowingKitField>(), projectile.damage, projectile.knockBack * 0.85f, projectile.owner, 0f, 0f);
        Main.PlaySound(SoundID.Shatter, projectile.position);
            projectile.Kill();
            return true;
        }
        public override void AI()
        {
			i++;
			if (i % 2 == 0)
			{
				int dust = Dust.NewDust(projectile.position, projectile.width / 2, projectile.height / 2, 132);
			}
			

        }
    }
}