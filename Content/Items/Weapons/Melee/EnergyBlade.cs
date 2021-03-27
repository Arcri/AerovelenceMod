using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class EnergyBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Energy Blade");
        }
        public override void SetDefaults()
        {
            item.useTurn = true;
            item.crit = 7;
            item.damage = 150;
            item.melee = true;
            item.width = 44;
            item.height = 48;
            item.shootSpeed = 3f;
            item.useTime = 17;
            item.useAnimation = 17;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 4;
            item.shoot = mod.ProjectileType("EnergyBladeProj");
            item.value = Item.sellPrice(0, 25, 65, 20);
            item.rare = ItemRarityID.Red;
            item.autoReuse = true;
        }
    }

    public class EnergyBladeProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Energy Blade");
        }

        public bool e;
        public int i;
        public override void SetDefaults()
        {
            projectile.width = 72;
            projectile.height = 72;
            projectile.friendly = true;
            projectile.penetrate = 15;
            projectile.hostile = false;
            projectile.melee = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 600;
        }
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();
            i++;
            if (i % 2 == 0)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 67);
            }
            projectile.velocity *= 1.03f;
            if (!e)
            {
                projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
                e = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 67, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
            Main.PlaySound(SoundID.Item10);
            return true;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture(Texture + "_Glow");
            spriteBatch.Draw(
                texture,
                new Vector2
                (
                    projectile.Center.Y - Main.screenPosition.X,
                    projectile.Center.X - Main.screenPosition.Y
                ),
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                projectile.rotation,
                texture.Size(),
                projectile.scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}