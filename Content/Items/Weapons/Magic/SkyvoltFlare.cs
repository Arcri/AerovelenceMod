using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.DataStructures;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class SkyvoltFlare : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
            DisplayName.SetDefault("Skyvolt Flare");
            Tooltip.SetDefault("Casts an elemental meteor that leaves sparks that can hurt enemies");
        }
        public override void SetDefaults()
        {
            Item.crit = 11;
            Item.damage = 140;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.width = 64;
            Item.height = 64;
            Item.useTime = 100;
            Item.useAnimation = 100;
            Item.UseSound = SoundID.Item21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 10, 50, 0);
            Item.rare = ItemRarityID.Lime;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SkyvoltMeteor>();
            Item.shootSpeed = 3f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = new Vector2(-7, -8);
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<EmberFragment>(), 15)
                .AddIngredient(ModContent.ItemType<BurnshockBar>(), 10)
                .AddIngredient(ItemID.MeteorStaff, 1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
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
            Projectile.width = 34;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 2;
            Projectile.alpha = 0;
            Projectile.scale = 1f;
        }
        public override void AI()
        {
            t++;
            Projectile.velocity *= 1.01f;
            Projectile.rotation += Projectile.velocity.X * 2f;
            Vector2 position = Projectile.Center + Vector2.Normalize(Projectile.velocity) * 10f;
            Dust dustPos1 = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6)];
            dustPos1.position = position;
            dustPos1.velocity = Projectile.velocity.RotatedBy(1.5707963705062866) * 0.33f + Projectile.velocity / 4f;
            dustPos1.position += Projectile.velocity.RotatedBy(1.5707963705062866);
            dustPos1.fadeIn = 0.5f;
            dustPos1.noGravity = true;

            Dust dustPos2 = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 59)];
            dustPos2.position = position;
            dustPos2.velocity = Projectile.velocity.RotatedBy(-1.5707963705062866) * 0.33f + Projectile.velocity / 4f;
            dustPos2.position += Projectile.velocity.RotatedBy(-1.5707963705062866);
            dustPos2.fadeIn = 0.5f;
            dustPos2.noGravity = true;

            for (int i = 0; i < 1; i++)
            {
                int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 6);
                Main.dust[dust].velocity *= 0.5f;
                Main.dust[dust].scale *= 1.3f;
                Main.dust[dust].fadeIn = 1f;
                Main.dust[dust].noGravity = true;
            }
            if (t % 25 == 0)
            {
                Vector2 offset = Projectile.Center + new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
                Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), offset, new Vector2(Main.rand.NextFloat(-1f, 1f), -5f + Main.rand.NextFloat(-1f, 1f)), ModContent.ProjectileType<ElectricBolt>(), 5, 0.5f, Main.myPlayer);
                t = 0;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int cooldown = 3;
            if (Projectile.localAI[1] <= 0f)
            {
                Projectile explosion = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), target.Center, Vector2.Zero, ModContent.ProjectileType<SkyvoltExplosion>(), damage, 10f, Projectile.owner, 0f, 1f);
                Vector2 radius = new Vector2(90f, 90f);
                explosion.Hitbox = new Rectangle(explosion.getRect().X - (int)(radius.X / 2), explosion.getRect().Y - (int)(radius.Y / 2), explosion.getRect().Width + (int)radius.X, explosion.getRect().Height + (int)radius.Y);
                Projectile.localAI[1] = cooldown;
            }
            Projectile.localNPCImmunity[target.whoAmI] = 6;
            target.immune[Projectile.owner] = cooldown;
            SoundEngine.PlaySound(SoundID.Item89, Projectile.position);
            Projectile.position.X += Projectile.width / 2;
            Projectile.position.Y += Projectile.height / 2;
            Projectile.width = (int)(128f * Projectile.scale);
            Projectile.height = (int)(128f * Projectile.scale);
            Projectile.position.X -= Projectile.width / 2;
            Projectile.position.Y -= Projectile.height / 2;
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 31, 0f, 0f, 100, default, 1.5f);
            }
            for (int t = 0; t < 32; t++)
            {
                int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 2.5f);
                Main.dust[dust].noGravity = true;
                Dust dust113 = Main.dust[dust];
                Dust dust2 = dust113;
                dust2.velocity *= 3f;
                dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 1.5f);
                dust113 = Main.dust[dust];
                dust2 = dust113;
                dust2.velocity *= 2f;
                Main.dust[dust].noGravity = true;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Player target = Main.player[Projectile.owner];
            int cooldown = 3;
            if (Projectile.localAI[1] <= 0f)
            {
                Projectile explosion = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), target.Center, Vector2.Zero, ModContent.ProjectileType<SkyvoltExplosion>(), -30, 10f, Projectile.owner, 0f, 1f);
                Vector2 radius = new Vector2(90f, 90f);
                explosion.Hitbox = new Rectangle(explosion.getRect().X - (int)(radius.X / 2), explosion.getRect().Y - (int)(radius.Y / 2), explosion.getRect().Width + (int)radius.X, explosion.getRect().Height + (int)radius.Y);
                Projectile.localAI[1] = cooldown;
            }
            Projectile.localNPCImmunity[target.whoAmI] = 6;
            SoundEngine.PlaySound(SoundID.Item89, Projectile.position);
            Projectile.position.X += Projectile.width / 2;
            Projectile.position.Y += Projectile.height / 2;
            Projectile.width = (int)(128f * Projectile.scale);
            Projectile.height = (int)(128f * Projectile.scale);
            Projectile.position.X -= Projectile.width / 2;
            Projectile.position.Y -= Projectile.height / 2;
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 31, 0f, 0f, 100, default, 1.5f);
            }
            for (int t = 0; t < 32; t++)
            {
                int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 2.5f);
                Main.dust[dust].noGravity = true;
                Dust dust113 = Main.dust[dust];
                Dust dust2 = dust113;
                dust2.velocity *= 3f;
                dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 1.5f);
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
            Projectile proj = Projectile;
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
            Projectile.width = 12;
            Projectile.damage = 10;
            Projectile.height = 12;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.aiStyle = 2;
            Projectile.alpha = 255;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
        }
        public override bool PreAI()
        {
            ApplyTrailFx();
            Projectile.velocity.Y += 0.2f;
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
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.ai[1] += 0.01f;
            Projectile.scale = Projectile.ai[1];
            if (Projectile.ai[0] == 0)
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            }

            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 3 * Main.projFrames[Projectile.type])
            {
                Projectile.Kill();
                return;
            }

            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.hide = true;
                }
            }
            Projectile.alpha -= 63;
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }

            Projectile.Damage();

            int dusts = 5;
            for (int i = 0; i < dusts; i++)
            {
                if (Main.rand.NextBool(3))
                {
                    float speed = 6f;
                    Vector2 velocity = new Vector2(0f, -speed * Main.rand.NextFloat(0.5f, 1.2f)).RotatedBy(MathHelper.ToRadians(360f / i * dusts + Main.rand.NextFloat(-50f, 50f)));
                    Dust dust1 = Dust.NewDustPerfect(Projectile.Center, 59, velocity, 150, default, 1.5f);
                    dust1.noGravity = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int cooldown = 4;
            Projectile.localNPCImmunity[target.whoAmI] = 6;
            target.immune[Projectile.owner] = cooldown;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            {
                Texture2D texture = (Texture2D)TextureAssets.Projectile[Projectile.type];
                Rectangle rectangle = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
                Color color = Projectile.GetAlpha(lightColor);

                if (!Projectile.hide)
                {
                    Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, rectangle, color, Projectile.rotation, rectangle.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
                }
                return false;
            }
        }
    }
}