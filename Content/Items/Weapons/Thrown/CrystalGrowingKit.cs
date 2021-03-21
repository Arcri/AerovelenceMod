using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Thrown
{
    public class CrystalGrowingKit : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Growing Kit");
            Tooltip.SetDefault("'The instructions were missing'");
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
            item.shoot = mod.ProjectileType("CrystalGrowingKitProj");
            item.shootSpeed = 16f;
        }
    }

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
            projectile.tileCollide = false;
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
            int count = 0;

            foreach (Projectile proj in Main.projectile.Where(x => x.active && x.whoAmI != projectile.whoAmI && x.type == projectile.type))
            {
                count++;

                if (count >= 7)
                    proj.Kill();
            }
            i++;
            if (i % 2 == 0)
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 132);
                dust.noGravity = true;
                dust.velocity *= 0.1f;
            }
            projectile.alpha -= 2;
            projectile.velocity *= 0f;
        }
    }
}