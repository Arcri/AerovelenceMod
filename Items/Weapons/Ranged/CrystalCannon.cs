using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
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
            item.UseSound = SoundID.Item67;
            item.crit = 4;
            item.damage = 40;
            item.ranged = true;
            item.width = 60;
            item.height = 32;
            item.useTime = 65;
            item.useAnimation = 65;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 3, 50, 0);
            item.rare = ItemRarityID.Pink;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("CrystalCannonProj");
            item.shootSpeed = 11f;
        }
    }
}



namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class CrystalCannonProj : ModProjectile
    {
        private readonly int oneHelixRevolutionInUpdateTicks = 30;
        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = 7;
            projectile.hostile = false;
            projectile.ranged = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 300;
            projectile.aiStyle = 1;
        }
        public override bool PreAI()
        {
            projectile.rotation = projectile.velocity.ToRotation();
            projectile.velocity *= 1.01f;
            float piFraction = MathHelper.Pi / oneHelixRevolutionInUpdateTicks;

            Vector2 newDustPosition = new Vector2(0, (float)Math.Sin(projectile.localAI[0] % oneHelixRevolutionInUpdateTicks * piFraction)) * projectile.height;
            Dust newDust = Dust.NewDustPerfect(projectile.Center + newDustPosition.RotatedBy(projectile.velocity.ToRotation()), 67);
            newDust.noGravity = true;
            newDustPosition.Y *= -1;

            newDust = Dust.NewDustPerfect(projectile.Center + newDustPosition.RotatedBy(projectile.velocity.ToRotation()), 67);
            newDust.noGravity = true;
            newDust.velocity *= 0f;
            return false;
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.scale *= 0.90f;
            Dust.NewDustDirect(projectile.position + projectile.velocity, projectile.width, projectile.height, 240, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            int type = mod.ProjectileType("CrystalCannonShard");
            Vector2 velocity = new Vector2(projectile.velocity.X * -0.6f, projectile.velocity.Y * -0.6f).RotatedByRandom(MathHelper.ToRadians(40));
            Projectile.NewProjectile(projectile.Center, velocity, type, projectile.damage, 5f, projectile.owner);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Dust.NewDustDirect(projectile.position + projectile.velocity, projectile.width, projectile.height, 240, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            int type = mod.ProjectileType("CrystalCannonShard");
            Vector2 velocity = new Vector2(projectile.velocity.X * -0.6f, projectile.velocity.Y * -0.6f).RotatedByRandom(MathHelper.ToRadians(40));
            Projectile.NewProjectile(projectile.Center, velocity, type, projectile.damage, 5f, projectile.owner);
            return true;
        }
    }
}

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class CrystalCannonShard : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Shard");
            Main.projFrames[projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 10;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.ranged = true;
            projectile.penetrate = 5;
            projectile.timeLeft = 600;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            projectile.velocity.Y += 0.2f;
        }
        public override void Kill(int timeLeft)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Item10, projectile.position);
        }
    }
}