
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class OriginPulse : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Origin Pulse");
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item11;
            Item.damage = 49;
            Item.DamageType = DamageClass.Magic;
            Item.width = 36;
            Item.height = 42;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 25, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<OriginPulseProj>();
            Item.shootSpeed = 3f;
        }
    }

    public class OriginPulseProj : ModProjectile
    { 
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 36;
            
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
                        
            Projectile.extraUpdates = 14;
            Projectile.alpha = 255;
        }
        public override void AI()
        {
            if (++Projectile.localAI[1] > 10)
            {
                float amountOfDust = 16f;
                for (int i = 0; i < amountOfDust; ++i)
                {                  
                    Vector2 spinningpoint5 = -Vector2.UnitY.RotatedBy(i * (MathHelper.TwoPi / amountOfDust)) * new Vector2(1f, 4f);
                    spinningpoint5 = spinningpoint5.RotatedBy(Projectile.velocity.ToRotation());

                    Dust dust = Dust.NewDustPerfect(Projectile.Center + spinningpoint5, 71, spinningpoint5, 0, Color.Blue, 1.3f);
                    dust.noGravity = true;
                    dust.scale *= 1.03f;
                }
                Projectile.localAI[1] = 0;
            }

            
        }
    }
}