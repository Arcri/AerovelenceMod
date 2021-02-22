using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class EtherealStriker : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ethereal Striker");
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item11;
			item.crit = 8;
            item.damage = 61;
            item.ranged = true;
            item.width = 60;
            item.height = 32;
            item.useTime = 14;
			item.useAnimation = 14;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 25, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("EtherealBolt");
            item.useAmmo = AmmoID.Arrow;
            item.shootSpeed = 18;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float numberProjectiles = 7;
            float rotation = MathHelper.ToRadians(15);
            position += Vector2.Normalize(new Vector2(speedX, speedY)) * 15f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f; // Watch out for dividing by 0 if there is only 1 projectile.
                Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, item.shoot, damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}
	
namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class EtherealBolt : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.friendly = true;
            projectile.penetrate = 5;
            projectile.hostile = false;
            projectile.magic = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 420;
        }
        public override void AI()
        {
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 9)
            {
                int numDust = 6;
                for (int i = 0; i < numDust; i++)
                {
                    Vector2 position = projectile.position;
                    position -= projectile.velocity * ((float)i / numDust);
                    projectile.alpha = 255;
                    int anotherOneBitesThis = Dust.NewDust(position, 1, 1, 132, 0f, 0f, 100, default(Color), 1f);
                    Main.dust[anotherOneBitesThis].position = position;
                    Main.dust[anotherOneBitesThis].velocity *= 0.2f;
                }
            }
        }
    }
}