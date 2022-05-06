using AerovelenceMod.Content.NPCs.Bosses.Rimegeist;
using AerovelenceMod.Content.Projectiles.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class CrystalArch : ModItem
    {
        int projectileAmount = 2;
        public override void SetStaticDefaults() => DisplayName.SetDefault("Crystal Arch");

        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item5;
            Item.damage = 20;

            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.autoReuse = true;

            Item.width = 60;
            Item.height = 32;

            Item.useAnimation = Item.useTime = 35;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 2, 50, 0);
            Item.rare = ItemRarityID.Orange;

            Item.shoot = ModContent.ProjectileType<IceArrow>();
            Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 3f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            type = ModContent.ProjectileType<WispProjectileRanged>(); //IceArrow

            for (int i = 0; i < projectileAmount; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.PiOver4 * 0.25f);
                Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X * 2, perturbedSpeed.Y * 2, type, damage, knockBack, player.whoAmI);
                type = Main.rand.Next(new int[] { type, ModContent.ProjectileType<IceBoltFriendly>() });
            }


            float numberProjectiles = 2 + Main.rand.Next(2);
            position += Vector2.Normalize(new Vector2(speedX, speedY)) * 45f;
            for (int i = 0; i < numberProjectiles; i++)
            {

            }
            return false;

        }
    }
    public class IceBoltFriendly : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
        }
        public override void AI()
        {

            Projectile.rotation = Projectile.velocity.ToRotation();
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight, 0f, 0f, 255);
            dust.noGravity = true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            Vector2 drawOrigin = new Vector2(Main.projectileTexture[Projectile.type].Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                spriteBatch.Draw(Main.projectileTexture[Projectile.type], drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
    }
}