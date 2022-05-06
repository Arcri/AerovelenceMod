using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class CrystalCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Cannon");
            Tooltip.SetDefault("Launches a gigantic crystal that speeds up over time\nThe crystal will also break into smaller shards when hit");
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item67;
            Item.crit = 4;
            Item.damage = 40;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 60;
            Item.height = 32;
            Item.useTime = 65;
            Item.useAnimation = 65;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 3, 50, 0);
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.shoot = Mod.Find<ModProjectile>("CrystalCannonProj").Type;
            Item.shootSpeed = 11f;
        }
    }


    public class CrystalCannonProj : ModProjectile
    {
        private readonly int oneHelixRevolutionInUpdateTicks = 30;
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = 7;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.aiStyle = 1;
        }
        public override bool PreAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity *= 1.01f;
            float piFraction = MathHelper.Pi / oneHelixRevolutionInUpdateTicks;

            Vector2 newDustPosition = new Vector2(0, (float)Math.Sin(Projectile.localAI[0] % oneHelixRevolutionInUpdateTicks * piFraction)) * Projectile.height;
            Dust newDust = Dust.NewDustPerfect(Projectile.Center + newDustPosition.RotatedBy(Projectile.velocity.ToRotation()), 67);
            newDust.noGravity = true;
            newDustPosition.Y *= -1;

            newDust = Dust.NewDustPerfect(Projectile.Center + newDustPosition.RotatedBy(Projectile.velocity.ToRotation()), 67);
            newDust.noGravity = true;
            newDust.velocity *= 0f;
            return false;
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.scale *= 0.90f;
            Dust.NewDustDirect(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 240, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            int type = Mod.Find<ModProjectile>("CrystalCannonShard").Type;
            Vector2 velocity = new Vector2(Projectile.velocity.X * -0.6f, Projectile.velocity.Y * -0.6f).RotatedByRandom(MathHelper.ToRadians(40));
            Projectile.NewProjectile(Projectile.Center, velocity, type, Projectile.damage, 5f, Projectile.owner);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Dust.NewDustDirect(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 240, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            int type = Mod.Find<ModProjectile>("CrystalCannonShard").Type;
            Vector2 velocity = new Vector2(Projectile.velocity.X * -0.6f, Projectile.velocity.Y * -0.6f).RotatedByRandom(MathHelper.ToRadians(40));
            Projectile.NewProjectile(Projectile.Center, velocity, type, Projectile.damage, 5f, Projectile.owner);
            return true;
        }
    }

    public class CrystalCannonShard : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Shard");
            Main.projFrames[Projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 10;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.2f;
        }
        public override void Kill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }
    }
}