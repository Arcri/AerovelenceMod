using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Core.Prim;
using System;

namespace AerovelenceMod.Content.Items.Weapons.Thrown
{
    public class Crackler : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crackler");
        }
        public override void SetDefaults()
        {	
            item.crit = 4;
            item.damage = 30;
            item.ranged = true;
            item.width = 38;
            item.height = 38;
            item.useTime = 30;
            item.useAnimation = 30;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.knockBack = 3;
            item.value = Item.sellPrice(0, 1, 50, 0);
            item.rare = ItemRarityID.Green;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<CracklerProj>();
            item.UseSound = SoundID.Item1;
            item.shootSpeed = 10f;
        }
    }

    public class CracklerProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crackler");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.Shuriken);
            projectile.width = 15;
            projectile.height = 15;
            projectile.thrown = true;
            projectile.penetrate = 1;
        }

        public override void AI()
        {
            Dust dust = Dust.NewDustPerfect(projectile.Center + (10 * ((projectile.rotation - 0.78f).ToRotationVector2())), 6);
            dust.noGravity = true;
            dust.scale = Main.rand.NextFloat(0.7f, 0.9f);
            dust.fadeIn = 1.5f;
        }
        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<CracklerProjTwo>(), projectile.damage, projectile.knockBack, projectile.owner);
        }
    }
    public class CracklerProjTwo : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crackler");
        }

        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 50;
            projectile.thrown = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 30;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.rotation = Main.rand.NextFloat(6.28f);
            color = new Color(Main.rand.Next(60, 255), Main.rand.Next(60, 255), Main.rand.Next(60, 255));
        }

        BasicEffect effect = new BasicEffect(Main.graphics.GraphicsDevice)
        {
            VertexColorEnabled = true,
        };

        Color color = Color.White;
        public override void AI()
        {
            projectile.scale = 1 + (float)Math.Sin((30 - projectile.timeLeft) / 4f);
            if (projectile.scale > 1)
            {
                projectile.scale--;
                projectile.scale /= 2;
                projectile.scale++;
            }
            if (projectile.scale < 0.1f)
                projectile.active = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            StarDraw.DrawStarBasic(effect, projectile.Center, projectile.rotation, projectile.scale * 30, color);
            return false;
        }
    }
}