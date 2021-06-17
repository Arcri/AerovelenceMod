
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
            item.UseSound = SoundID.Item11;
            item.damage = 49;
            item.magic = true;
            item.width = 36;
            item.height = 42;
            item.useTime = 14;
            item.useAnimation = 14;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 25, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.autoReuse = false;
            item.shoot = ModContent.ProjectileType<OriginPulseProj>();
            item.shootSpeed = 3f;
        }
    }

    public class OriginPulseProj : ModProjectile
    { 
        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 36;
            
            projectile.penetrate = -1;
            projectile.magic = projectile.tileCollide = projectile.ignoreWater = projectile.friendly = true;
                        
            projectile.extraUpdates = 14;
            projectile.alpha = 255;
        }
        public override void AI()
        {
            if (++projectile.localAI[1] > 10)
            {
                float amountOfDust = 16f;
                for (int i = 0; i < amountOfDust; ++i)
                {                  
                    Vector2 spinningpoint5 = -Vector2.UnitY.RotatedBy(i * (MathHelper.TwoPi / amountOfDust)) * new Vector2(1f, 4f);
                    spinningpoint5 = spinningpoint5.RotatedBy(projectile.velocity.ToRotation());

                    Dust dust = Dust.NewDustPerfect(projectile.Center + spinningpoint5, 71, spinningpoint5, 0, Color.Blue, 1.3f);
                    dust.noGravity = true;
                    dust.scale *= 1.03f;
                }
                projectile.localAI[1] = 0;
            }

            
        }
    }
}