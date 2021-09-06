using AerovelenceMod.Content.Items.Others.Crafting;
using AerovelenceMod.Content.Items.Placeables.Blocks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            item.UseSound = SoundID.Item5;
            item.crit = 4;
            item.damage = 25;
            item.ranged = true;
            item.width = 20;
            item.height = 40;
            item.useTime = 26;
            item.useAnimation = 26;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2;
            item.value = Item.sellPrice(0, 0, 20, 0);
            item.rare = ItemRarityID.Blue;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<BoomArrow>();
            item.useAmmo = AmmoID.Arrow;
            item.shootSpeed = 4.5f;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BloodChunk>(), 20);
            recipe.AddRecipeGroup("Wood", 20);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            type = ModContent.ProjectileType<BoomArrow>();
            return true;
        }
    }
    public class BoomArrow : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 22;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.ranged = true;
            projectile.extraUpdates = 2;
        }
        public override void AI()
        {
            projectile.velocity.Y += 0.03f;
            projectile.rotation = projectile.velocity.ToRotation();
            int dust = Dust.NewDust(projectile.Center - new Vector2(5), 0, 0, DustID.Blood);
            Main.dust[dust].velocity *= 1f;
        }
        public override void Kill(int timeLeft)
        {
            Explode();
            Main.PlaySound(SoundID.Item10);
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(projectile.Center - new Vector2(5), 0, 0, DustID.Blood, 0, 0, projectile.alpha);
                dust.velocity *= 0.55f;
                dust.velocity += projectile.velocity * 0.5f;
                dust.scale *= 1.75f;
                dust.noGravity = true;
            }
        }
        private void Explode()
        {
            Main.PlaySound(SoundID.Item14, projectile.position);

            Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<BoomArrowExplosion>(), projectile.damage, 4f);

            projectile.active = false;

            for (int i = 0; i < 10; i++)
            {
                float rotation = i / (float)10f * MathHelper.TwoPi;

                Vector2 velocity = rotation.ToRotationVector2() * 2f;

                Dust dust = Dust.NewDustDirect(projectile.Center, 0, 0, DustID.Blood, velocity.X, velocity.Y);
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
    public class BoomArrowExplosion : ModProjectile
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