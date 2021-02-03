using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Thrown
{
    public class CrystalGrowingKit : ModItem
    {
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("'The instructions were missing'");
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
            item.rare = ItemRarityID.Pink;
            item.autoReuse = true;
            item.noUseGraphic = true;
            item.shoot = mod.ProjectileType("CrystalGrowingKitProj");
            item.shootSpeed = 16f;
		}
    }
}
	
namespace AerovelenceMod.Items.Weapons.Thrown
{
    public class CrystalGrowingKitProj : ModProjectile
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
            int index = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<CrystalGrowingKitField>(), projectile.damage, projectile.knockBack * 0.85f, projectile.owner, 0f, 0f);
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

namespace AerovelenceMod.Items.Weapons.Thrown
{
    public class CrystalGrowingKitField : ModProjectile
    {
        public int i;
        public override void SetDefaults()
        {
            projectile.width = 66;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.hostile = false;
            projectile.melee = true;
            projectile.alpha = 255;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 500;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.velocity *= 0f;
            return true;
        }
        public override void AI()
        {
            i++;
            if (i % 2 == 0)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width / 2, projectile.height / 2, 132);
            }
            projectile.alpha -= 2;
            projectile.velocity *= 0.99f;
        }
    }
}