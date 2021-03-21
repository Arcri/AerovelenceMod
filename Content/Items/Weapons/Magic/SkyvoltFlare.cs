using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class SkyvoltFlare : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
            DisplayName.SetDefault("Skyvolt Flare");
            Tooltip.SetDefault("Casts an elemental meteor that leaves sparks that can hurt enemies");
        }
        public override void SetDefaults()
        {
            item.crit = 11;
            item.damage = 140;
            item.magic = true;
            item.mana = 20;
            item.width = 64;
            item.height = 64;
            item.useTime = 100;
            item.useAnimation = 100;
            item.UseSound = SoundID.Item21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 10, 50, 0);
            item.rare = ItemRarityID.Lime;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SkyvoltMeteor>();
            item.shootSpeed = 3f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(2));
            speedX = perturbedSpeed.X;
            speedY = perturbedSpeed.Y;
            Vector2 muzzleOffset = new Vector2(-7, -8);
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
            return true;
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<EmberFragment>(), 15);
            modRecipe.AddIngredient(ModContent.ItemType<BurnshockBar>(), 10);
            modRecipe.AddIngredient(ItemID.MeteorStaff, 1);
            modRecipe.AddTile(TileID.MythrilAnvil);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}
namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class SkyvoltMeteor : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skyvolt Meteor");
        }
        int t;
        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = true;
            projectile.extraUpdates = 2;
            projectile.alpha = 0;
            projectile.scale = 1f;
        }
        public override void AI()
        {
            t++;
            projectile.velocity *= 1.01f;
            projectile.rotation += projectile.velocity.X * 2f;
            Vector2 position = projectile.Center + Vector2.Normalize(projectile.velocity) * 10f;
            Dust dustPos1 = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 6)];
            dustPos1.position = position;
            dustPos1.velocity = projectile.velocity.RotatedBy(1.5707963705062866) * 0.33f + projectile.velocity / 4f;
            dustPos1.position += projectile.velocity.RotatedBy(1.5707963705062866);
            dustPos1.fadeIn = 0.5f;
            dustPos1.noGravity = true;

            Dust dustPos2 = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 59)];
            dustPos2.position = position;
            dustPos2.velocity = projectile.velocity.RotatedBy(-1.5707963705062866) * 0.33f + projectile.velocity / 4f;
            dustPos2.position += projectile.velocity.RotatedBy(-1.5707963705062866);
            dustPos2.fadeIn = 0.5f;
            dustPos2.noGravity = true;

            for (int i = 0; i < 1; i++)
            {
                int dust = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6);
                Main.dust[dust].velocity *= 0.5f;
                Main.dust[dust].scale *= 1.3f;
                Main.dust[dust].fadeIn = 1f;
                Main.dust[dust].noGravity = true;
            }
            if(t % 25 == 0)
            {
                Vector2 offset = projectile.Center + new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
                Projectile.NewProjectileDirect(offset, new Vector2(Main.rand.NextFloat(-1f, 1f), -5f + Main.rand.NextFloat(-1f, 1f)), ModContent.ProjectileType<ElectricBolt>(), 5, 0.5f, Main.myPlayer);
                t = 0;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int cooldown = 3;
            if (projectile.localAI[1] <= 0f)
            {
                Projectile explosion = Projectile.NewProjectileDirect(target.Center, Vector2.Zero, ModContent.ProjectileType<SkyvoltExplosion>(), damage, 10f, projectile.owner, 0f, 1f);
                Vector2 radius = new Vector2(90f, 90f);
                explosion.Hitbox = new Rectangle(explosion.getRect().X - (int)(radius.X / 2), explosion.getRect().Y - (int)(radius.Y / 2), explosion.getRect().Width + (int)radius.X, explosion.getRect().Height + (int)radius.Y);
                projectile.localAI[1] = cooldown;
            }
            projectile.localNPCImmunity[target.whoAmI] = 6;
            target.immune[projectile.owner] = cooldown;
            Main.PlaySound(SoundID.Item89, projectile.position);
            projectile.position.X += projectile.width / 2;
            projectile.position.Y += projectile.height / 2;
            projectile.width = (int)(128f * projectile.scale);
            projectile.height = (int)(128f * projectile.scale);
            projectile.position.X -= projectile.width / 2;
            projectile.position.Y -= projectile.height / 2;
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 31, 0f, 0f, 100, default, 1.5f);
            }
            for (int t = 0; t < 32; t++)
            {
                int dust = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 100, default, 2.5f);
                Main.dust[dust].noGravity = true;
                Dust dust113 = Main.dust[dust];
                Dust dust2 = dust113;
                dust2.velocity *= 3f;
                dust = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 100, default, 1.5f);
                dust113 = Main.dust[dust];
                dust2 = dust113;
                dust2.velocity *= 2f;
                Main.dust[dust].noGravity = true;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Player target = Main.player[projectile.owner];
            int cooldown = 3;
            if (projectile.localAI[1] <= 0f)
            {
                Projectile explosion = Projectile.NewProjectileDirect(target.Center, Vector2.Zero, ModContent.ProjectileType<SkyvoltExplosion>(), -30, 10f, projectile.owner, 0f, 1f);
                Vector2 radius = new Vector2(90f, 90f);
                explosion.Hitbox = new Rectangle(explosion.getRect().X - (int)(radius.X / 2), explosion.getRect().Y - (int)(radius.Y / 2), explosion.getRect().Width + (int)radius.X, explosion.getRect().Height + (int)radius.Y);
                projectile.localAI[1] = cooldown;
            }
            projectile.localNPCImmunity[target.whoAmI] = 6;
            Main.PlaySound(SoundID.Item89, projectile.position);
            projectile.position.X += projectile.width / 2;
            projectile.position.Y += projectile.height / 2;
            projectile.width = (int)(128f * projectile.scale);
            projectile.height = (int)(128f * projectile.scale);
            projectile.position.X -= projectile.width / 2;
            projectile.position.Y -= projectile.height / 2;
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 31, 0f, 0f, 100, default, 1.5f);
            }
            for (int t = 0; t < 32; t++)
            {
                int dust = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 100, default, 2.5f);
                Main.dust[dust].noGravity = true;
                Dust dust113 = Main.dust[dust];
                Dust dust2 = dust113;
                dust2.velocity *= 3f;
                dust = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 100, default, 1.5f);
                dust113 = Main.dust[dust];
                dust2 = dust113;
                dust2.velocity *= 2f;
                Main.dust[dust].noGravity = true;
            }
            return true;
        }
    }
}


namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    partial class ElectricBolt : ModProjectile
    {
        private void ApplyTrailFx()
        {
            Projectile proj = projectile;
            for (int dusts = 0; dusts < 1; dusts++)
            {
                int castAheadDist = 6;
                var pos = new Vector2(
                    proj.position.X + castAheadDist,
                    proj.position.Y + castAheadDist
                );

                for (int subDusts = 0; subDusts < 3; subDusts++)
                {
                    float dustCastAheadX = proj.velocity.X / 3f * subDusts;
                    float dustCastAheadY = proj.velocity.Y / 3f * subDusts;

                    int dustIdx = Dust.NewDust(
                        Position: pos,
                        Width: proj.width - castAheadDist * 2,
                        Height: proj.height - castAheadDist * 2,
                        Type: 59,
                        SpeedX: 0f,
                        SpeedY: 0f,
                        Alpha: 100,
                        newColor: default,
                        Scale: 1.2f
                    );

                    Main.dust[dustIdx].noGravity = true;
                    Main.dust[dustIdx].velocity *= 0.3f;
                    Main.dust[dustIdx].velocity += proj.velocity * 0.5f;

                    Dust dust = Main.dust[dustIdx];
                    dust.position.X -= dustCastAheadX;
                    dust.position.Y -= dustCastAheadY;
                }

                if (Main.rand.Next(8) == 0)
                {
                    int dustIdx = Dust.NewDust(
                        Position: pos,
                        Width: proj.width - castAheadDist * 2,
                        Height: proj.height - castAheadDist * 2,
                        Type: DustID.Electric,
                        SpeedX: 0f,
                        SpeedY: 0f,
                        Alpha: 100,
                        newColor: default,
                        Scale: 0.75f
                    );
                    Main.dust[dustIdx].velocity *= 0.5f;
                    Main.dust[dustIdx].velocity += proj.velocity * 0.5f;
                }
            }
        }
    }
}
namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    partial class ElectricBolt : ModProjectile
    {
        private static int AquaSceptreAiStyle;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.WaterStream;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electric Bolt");

            var aquaSceptreProj = new Projectile();
            aquaSceptreProj.SetDefaults(ProjectileID.WaterStream);

            AquaSceptreAiStyle = aquaSceptreProj.aiStyle;
        }
        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.damage = 10;
            projectile.height = 12;
            projectile.tileCollide = true;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.melee = true;
            projectile.aiStyle = 2;
            projectile.alpha = 255;
            projectile.timeLeft = 600;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.extraUpdates = 1;
        }
        public override bool PreAI()
        {
            ApplyTrailFx();
            projectile.velocity.Y += 0.2f;
            return false;
        }
    }
}
namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class SkyvoltExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skyvolt Explosion");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 100;
            projectile.height = 100;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.timeLeft = 60;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            projectile.ai[1] += 0.01f;
            projectile.scale = projectile.ai[1];
            if (projectile.ai[0] == 0)
            {
                Main.PlaySound(SoundID.Item14, projectile.Center);
            }

            projectile.ai[0]++;
            if (projectile.ai[0] >= 3 * Main.projFrames[projectile.type])
            {
                projectile.Kill();
                return;
            }

            if (++projectile.frameCounter >= 3)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.hide = true;
                }
            }
            projectile.alpha -= 63;
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }

            projectile.Damage();

            int dusts = 5;
            for (int i = 0; i < dusts; i++)
            {
                if (Main.rand.NextBool(3))
                {
                    float speed = 6f;
                    Vector2 velocity = new Vector2(0f, -speed * Main.rand.NextFloat(0.5f, 1.2f)).RotatedBy(MathHelper.ToRadians(360f / i * dusts + Main.rand.NextFloat(-50f, 50f)));
                    Dust dust1 = Dust.NewDustPerfect(projectile.Center, 59, velocity, 150, default, 1.5f);
                    dust1.noGravity = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int cooldown = 4;
            projectile.localNPCImmunity[target.whoAmI] = 6;
            target.immune[projectile.owner] = cooldown;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Rectangle rectangle = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            Color color = projectile.GetAlpha(lightColor);

            if (!projectile.hide)
            {
                spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, rectangle, color, projectile.rotation, rectangle.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}