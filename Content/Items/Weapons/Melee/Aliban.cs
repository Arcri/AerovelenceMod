using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class Aliban : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aliban");
        }
        public override void SetDefaults()
        {
            Item.useTurn = true;
            Item.crit = 7;
            Item.damage = 150;
            Item.DamageType = DamageClass.Melee;
            Item.width = 44;
            Item.height = 48;
            Item.shootSpeed = 3f;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4;
            Item.shoot = Mod.Find<ModProjectile>("AlibanProj").Type;
            Item.value = Item.sellPrice(0, 25, 65, 20);
            Item.rare = ItemRarityID.Red;
            Item.autoReuse = true;
        }
    }

    public class AlibanProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aliban Scythe");
        }

        public bool e;
        public float rot = 0.5f;
        public int i;
        public override void SetDefaults()
        {
            Projectile.width = 72;
            Projectile.height = 72;
            Projectile.friendly = true;
            Projectile.penetrate = 15;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
        }
        public override void AI()
        {
            i++;
            if (i % 2 == 0)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.IceRod);
            }
            Projectile.velocity *= 1.03f;
            if (!e)
            {
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;
                e = true;
            }
            Projectile.rotation += rot;
            rot *= 1.005f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 67, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
            SoundEngine.PlaySound(SoundID.Item10);
            return true;
        }
        public override void PostDraw(Color lightColor)
        {
        Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Items/Weapons/Melee/AlibanProj_Glow");
            Main.EntitySpriteDraw(
                texture,
                new Vector2
                (
                    Projectile.Center.Y - Main.screenPosition.X,
                    Projectile.Center.X - Main.screenPosition.Y
                ),
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                Projectile.rotation,
                texture.Size(),
                Projectile.scale,
                SpriteEffects.None,
                0
            );
        }
    }
}