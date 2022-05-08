using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Core.Prim;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.Thrown
{
    public class ElectrapulseCanister : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electrapulse Canister");
            Tooltip.SetDefault("Hitting a tile or enemy releases a large electric explosion that releases lightning");
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item1;
            Item.damage = 12;
            Item.DamageType = DamageClass.Melee;
            Item.width = 60;
            Item.height = 32;
            Item.useTime = 37;
            Item.useAnimation = 37;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<ElectrapulseCanisterProj>();
            Item.shootSpeed = 16f;
        }
    }

    public class ElectrapulseCanisterProj : ModProjectile
    {
        public bool e;
        public float rot = 0.5f;
        public int i;
        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.aiStyle = 2;
            Projectile.penetrate = 2;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
        }
        public override void Kill(int timeLeft)
        {
            var s = Projectile.GetSource_Death();
            SoundEngine.PlaySound(SoundID.Item70, Projectile.Center);
             SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);
             Gore.NewGore(s, Projectile.position, Vector2.Zero, Mod.Find<ModGore>("Gores/CanisterGore1").Type, 1f);
             Gore.NewGore(s, Projectile.position, Vector2.Zero, Mod.Find<ModGore>("Gores/CanisterGore2").Type, 1f);
             Gore.NewGore(s, Projectile.position, Vector2.Zero, Mod.Find<ModGore>("Gores/CanisterGore3").Type, 1f);
            for (double i = 0; i < 6.28; i += Main.rand.NextFloat(1f, 2f))
            {
                int lightningproj = Projectile.NewProjectile(s, Projectile.Center, new Vector2((float)Math.Sin(i), (float)Math.Cos(i)) * 2.5f, ModContent.ProjectileType<ElectrapulseCanisterProj2>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
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
        }
        public override void OnHitNPC(NPC target, int damage, float knockBack, bool crit) => Projectile.Kill();
    }
    public class ElectrapulseCanisterProj2 : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Canister Lightning");
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