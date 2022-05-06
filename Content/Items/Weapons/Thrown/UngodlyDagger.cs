using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Thrown
{
    public class UngodlyDagger : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ungodly Dagger");
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item11;
            Item.crit = 8;
            Item.damage = 56;
            Item.DamageType = DamageClass.Melee;
            Item.width = 22;
            Item.height = 38;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 25, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.shoot = Mod.Find<ModProjectile>("UngodlyDaggerProjectile").Type;
            Item.shootSpeed = 12f;
        }
    }

    public class UngodlyDaggerProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 360;
        }
        public override void AI()
        {
            if (Projectile.timeLeft > 315)
            {
                Projectile.rotation += 1.0471975511965977461542144610932f;
            }
            else
            {
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.5707963267948966192313216916398f;
                Projectile.velocity *= 1.02f;
                if (Projectile.timeLeft % 2 == 0)
                    Projectile.damage++;
            }
        }
    }
}