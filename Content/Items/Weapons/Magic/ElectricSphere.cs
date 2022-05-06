using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Core.Prim;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class ElectricSphere: ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electric Sphere");
            Tooltip.SetDefault("Hitting a tile or enemy releases a large electric explosion that releases lightning");
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item109;
            Item.damage = 12;
            Item.DamageType = DamageClass.Melee;
            Item.width = 60;
            Item.height = 32;
            Item.useTime = 37;
            Item.useAnimation = 37;
            Item.mana = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 2, 50, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ElectricSphereProj>();
            Item.shootSpeed = 6f;
        }
    }

    public class ElectricSphereProj : ModProjectile
    {
        public bool e;
        public float rot = 0.5f;
        public int i;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }
        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.aiStyle = 0;
            Projectile.penetrate = 2;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 80;
        }
        public override void Kill(int timeLeft)
        {
            //Main.PlaySound(SoundID.Item70, projectile.Center);
             SoundEngine.PlaySound(SoundID.Item113, Projectile.Center);
            for (double i = 0; i < 6.28; i += Main.rand.NextFloat(1f, 2f))
            {
                int lightningproj = Projectile.NewProjectile(Projectile.Center, new Vector2((float)Math.Sin(i), (float)Math.Cos(i)) * 1.1f, ModContent.ProjectileType<ElectricSphereProj2>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                if (Main.netMode != NetmodeID.Server)
                {
                    AerovelenceMod.primitives.CreateTrail(new CanisterPrimTrail(Main.projectile[lightningproj]));
                }
            }
            for (double i = 0; i < 6.28; i+= 0.1)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, 226, new Vector2((float)Math.Sin(i) * 1.3f, (float)Math.Cos(i)) * 2.4f);
                dust.noGravity = true;
            }
        }
        public override void AI()
        {
            i++;
            if (i % 4 == 0)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height / 2, 132);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 3 == 0) //does the exact same thing but more elegant
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 5)
                    Projectile.frame = 0;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockBack, bool crit) => Projectile.Kill();
    }
    public class ElectricSphereProj2 : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electric Sphere Lightning");
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.damage = 0;
            Projectile.timeLeft = 120;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 5;
        }

        Vector2 initialVelocity = Vector2.Zero;

        private float lerp;
        public Vector2 DrawPos;
        public int boost;
        public override void AI()
        {
            if (initialVelocity == Vector2.Zero)
            {
                initialVelocity = Projectile.velocity;
            }
            if (Projectile.timeLeft % 10 == 0)
            {
                Projectile.velocity = initialVelocity.RotatedBy(Main.rand.NextFloat(-1, 1));
            }
            /* if (projectile.timeLeft % 2 == 0)
             {
                 Dust dust = Dust.NewDustPerfect(projectile.Center, 226);
                 dust.noGravity = true;
                 dust.scale = (float)Math.Sqrt(projectile.timeLeft) / 4;
                 dust.velocity = Vector2.Zero;
             }*/
            DrawPos = Projectile.position;
        }
    }
}