using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

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
			Item.UseSound = SoundID.Item41;
			Item.crit = 8;
            Item.damage = 28;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 46;
            Item.height = 28; 
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 3, 50, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<LightOfTheAncientsProj>();
            Item.shootSpeed = 24f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(3));
            velocity.X = perturbedSpeed.X;
            velocity.Y = perturbedSpeed.Y;
        }
    }
    public class LightOfTheAncientsProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 2;

            
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            int dust = Dust.NewDust(Projectile.Center - new Vector2(5), 0, 0, 127);
            Main.dust[dust].velocity *= 1f;
            int numDust = 5;
            for (int i = 0; i < numDust; i++)
            {
                int DustType;
                DustType = 127;
                Vector2 position = Projectile.position;
                position -= Projectile.velocity * ((float)i / numDust);
                Projectile.alpha = 255;
                int anotherOneBitesThis = Dust.NewDust(position, 1, 1, DustType, 0f, 0f, 127, default, 1f);
                Main.dust[anotherOneBitesThis].position = position;
                Main.dust[anotherOneBitesThis].velocity *= 0.2f;
                Main.dust[anotherOneBitesThis].noGravity = true;
            }
        }
        public override void Kill(int timeLeft)
        {
            Explode();
            SoundEngine.PlaySound(SoundID.Item10);
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(5), 0, 0, 127, 0, 0, Projectile.alpha);
                dust.velocity *= 0.55f;
                dust.velocity += Projectile.velocity * 0.5f;
                dust.scale *= 1.75f;
                dust.noGravity = true;
            }
        }
        private void Explode()
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<LightOfTheAncientsExplosion>(), Projectile.damage, 4f);

            Projectile.active = false;

            for (int i = 0; i < 10; i++)
            {
                float rotation = i / (float)10f * MathHelper.TwoPi;

                Vector2 velocity = rotation.ToRotationVector2() * 2f;

                Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0, 127, velocity.X, velocity.Y);
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.scale = Main.rand.NextFloat(0.6f, 1f);
            }

            int smokeAmount = Main.rand.Next(3, 6);

            for (int i = 0; i < smokeAmount; i++)
            {
                var velocity = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));

                Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, velocity, Main.rand.Next(61, 64), Main.rand.NextFloat(0.6f, 1f));

                Projectile.netUpdate = true;
            }
            Projectile.netUpdate = true;
        }
    }
    public class LightOfTheAncientsExplosion : ModProjectile
    {
        public override string Texture => "Terraria/Item_" + ItemID.HermesBoots;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.width = Projectile.height = 80;

            Projectile.alpha = 255;
            Projectile.timeLeft = 5;

            Projectile.penetrate = -1;
        }
    }
}