using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Projectiles;

namespace AerovelenceMod.Items.Weapons.Thrown
{
    public class PrismPiercer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prism Piercer");
        }

        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item1;
            item.crit = 8;
            item.damage = 12;
            item.ranged = true;
            item.width = 60;
            item.height = 32;
            item.useTime = 17;
            item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.autoReuse = true;
            item.noUseGraphic = true;
            item.shoot = mod.ProjectileType("PrismPiercerProjectile");
            item.shootSpeed = 16f;
        }
    }
}

namespace AerovelenceMod.Items.Weapons.Thrown
{
    public class PrismPiercerProjectile : ModProjectile
    {
        public bool e;
        public float rot = 0.5f;
        public int i;
        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 38;
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.hostile = false;
            projectile.melee = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 120;
        }
        public override void AI()
        {
            i++;
            if (i % 50 == 0)
            {
                Projectile.NewProjectile(projectile.Center.X + projectile.velocity.X, projectile.Center.Y + projectile.velocity.Y, projectile.velocity.X - 2f, projectile.velocity.Y - 2, ModContent.ProjectileType<CrystalHomingShard>(), projectile.damage, projectile.knockBack * 0.85f, projectile.owner, 0f, 0f);
            }
            projectile.alpha += 2;
            projectile.velocity *= 0.99f;
            if (!e)
            {
                projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
                e = true;
            }
            projectile.rotation += rot;
            rot *= 0.99f;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Vector2 offset = new Vector2(0, 0);

            Projectile.NewProjectile(projectile.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<CrystalHomingShard>(), 6, 1f, Main.myPlayer);
            Main.PlaySound(SoundID.Item68);
            return true;
        }
    }
}