using AerovelenceMod.Content.Items.Others.Crafting;
using AerovelenceMod.Content.Items.Placeables.Blocks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class BoomBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Boom Bow");
            Tooltip.SetDefault("Replaces arrows with an explosive arrow");
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item5;
            Item.crit = 4;
            Item.damage = 25;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 20;
            Item.height = 40;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BoomArrow>();
            Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 4.5f;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<BloodChunk>(), 20)
                .AddRecipeGroup("Wood", 20)
                .AddTile(TileID.Anvils)
                .Register();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<BoomArrow>();
            return true;
        }
    }
    public class BoomArrow : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 2;
        }
        public override void AI()
        {
            Projectile.velocity.Y += 0.03f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            int dust = Dust.NewDust(Projectile.Center - new Vector2(5), 0, 0, DustID.Blood);
            Main.dust[dust].velocity *= 1f;
        }
        public override void Kill(int timeLeft)
        {
            Explode();
            SoundEngine.PlaySound(SoundID.Item10);
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(5), 0, 0, DustID.Blood, 0, 0, Projectile.alpha);
                dust.velocity *= 0.55f;
                dust.velocity += Projectile.velocity * 0.5f;
                dust.scale *= 1.75f;
                dust.noGravity = true;
            }
        }
        private void Explode()
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            Projectile.NewProjectile(Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BoomArrowExplosion>(), Projectile.damage, 4f);

            Projectile.active = false;

            for (int i = 0; i < 10; i++)
            {
                float rotation = i / (float)10f * MathHelper.TwoPi;

                Vector2 velocity = rotation.ToRotationVector2() * 2f;

                Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.Blood, velocity.X, velocity.Y);
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.scale = Main.rand.NextFloat(0.6f, 1f);
            }

            int smokeAmount = Main.rand.Next(3, 6);

            for (int i = 0; i < smokeAmount; i++)
            {
                var velocity = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));

                Gore.NewGore(Projectile.Center, velocity, Main.rand.Next(61, 64), Main.rand.NextFloat(0.6f, 1f));

                Projectile.netUpdate = true;
            }
            Projectile.netUpdate = true;
        }
    }
    public class BoomArrowExplosion : ModProjectile
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