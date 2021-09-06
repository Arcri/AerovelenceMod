using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class LightOfTheAncients : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Light of the Ancients");
			Tooltip.SetDefault("'I feel like I'm going to break this thing...'");
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item41;
			item.crit = 8;
            item.damage = 28;
            item.ranged = true;
            item.width = 46;
            item.height = 28; 
            item.useTime = 18;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 3, 50, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = false;
            item.shoot = ModContent.ProjectileType<LightOfTheAncientsProj>();
            item.shootSpeed = 24f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(3));
            speedX = perturbedSpeed.X;
            speedY = perturbedSpeed.Y;
            return true;
        }
    }
    public class LightOfTheAncientsProj : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.ranged = true;
            projectile.extraUpdates = 2;

            
        }
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();
            int dust = Dust.NewDust(projectile.Center - new Vector2(5), 0, 0, 127);
            Main.dust[dust].velocity *= 1f;
            int numDust = 5;
            for (int i = 0; i < numDust; i++)
            {
                int dustType;
                dustType = 127;
                Vector2 position = projectile.position;
                position -= projectile.velocity * ((float)i / numDust);
                projectile.alpha = 255;
                int anotherOneBitesThis = Dust.NewDust(position, 1, 1, dustType, 0f, 0f, 127, default, 1f);
                Main.dust[anotherOneBitesThis].position = position;
                Main.dust[anotherOneBitesThis].velocity *= 0.2f;
                Main.dust[anotherOneBitesThis].noGravity = true;
            }
        }
        public override void Kill(int timeLeft)
        {
            Explode();
            Main.PlaySound(SoundID.Item10);
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(projectile.Center - new Vector2(5), 0, 0, 127, 0, 0, projectile.alpha);
                dust.velocity *= 0.55f;
                dust.velocity += projectile.velocity * 0.5f;
                dust.scale *= 1.75f;
                dust.noGravity = true;
            }
        }
        private void Explode()
        {
            Main.PlaySound(SoundID.Item14, projectile.position);

            Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<LightOfTheAncientsExplosion>(), projectile.damage, 4f);

            projectile.active = false;

            for (int i = 0; i < 10; i++)
            {
                float rotation = i / (float)10f * MathHelper.TwoPi;

                Vector2 velocity = rotation.ToRotationVector2() * 2f;

                Dust dust = Dust.NewDustDirect(projectile.Center, 0, 0, 127, velocity.X, velocity.Y);
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.scale = Main.rand.NextFloat(0.6f, 1f);
            }

            int smokeAmount = Main.rand.Next(3, 6);

            for (int i = 0; i < smokeAmount; i++)
            {
                var velocity = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));

                Gore.NewGore(projectile.Center, velocity, Main.rand.Next(61, 64), Main.rand.NextFloat(0.6f, 1f));

                projectile.netUpdate = true;
            }
            projectile.netUpdate = true;
        }
    }
    public class LightOfTheAncientsExplosion : ModProjectile
    {
        public override string Texture => "Terraria/Item_" + ItemID.HermesBoots;

        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;

            projectile.width = projectile.height = 80;

            projectile.alpha = 255;
            projectile.timeLeft = 5;

            projectile.penetrate = -1;
        }
    }
}