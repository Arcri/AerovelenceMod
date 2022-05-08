using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class EtherealStriker : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ethereal Striker");
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item11;
            Item.crit = 8;
            Item.damage = 61;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 60;
            Item.height = 32;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 25, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.autoReuse = true;
            Item.shoot = Mod.Find<ModProjectile>("EtherealBolt").Type;
            Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 18;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
        float numberProjectiles = 7;
            float rotation = MathHelper.ToRadians(15);
            position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 15f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f; // Watch out for dividing by 0 if there is only 1 projectile.
                Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, Item.shoot, damage, knockback, player.whoAmI);
            }
            return false;
        }
    }

    public class EtherealBolt : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 420;
        }
        public override void AI()
        {
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 9)
            {
                int numDust = 6;
                for (int i = 0; i < numDust; i++)
                {
                    Vector2 position = Projectile.position;
                    position -= Projectile.velocity * ((float)i / numDust);
                    Projectile.alpha = 255;
                    int anotherOneBitesThis = Dust.NewDust(position, 1, 1, DustID.Firework_Blue, 0f, 0f, 100, default, 1f);
                    Main.dust[anotherOneBitesThis].position = position;
                    Main.dust[anotherOneBitesThis].velocity *= 0.2f;
                }
            }
        }
    }
}