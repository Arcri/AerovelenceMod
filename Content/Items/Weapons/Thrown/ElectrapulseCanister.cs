using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Core.Prim;

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
            item.UseSound = SoundID.Item1;
            item.damage = 12;
            item.melee = true;
            item.width = 60;
            item.height = 32;
            item.useTime = 37;
            item.useAnimation = 37;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 0, 50, 0);
            item.rare = ItemRarityID.Blue;
            item.autoReuse = true;
            item.noUseGraphic = true;
            item.shoot = ModContent.ProjectileType<ElectrapulseCanisterProj>();
            item.shootSpeed = 16f;
        }
    }

    public class ElectrapulseCanisterProj : ModProjectile
    {
        public bool e;
        public float rot = 0.5f;
        public int i;
        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 28;
            projectile.friendly = true;
            projectile.aiStyle = 2;
            projectile.penetrate = 2;
            projectile.hostile = false;
            projectile.melee = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 300;
        }
        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item70, projectile.Center);
             Main.PlaySound(SoundID.Shatter, projectile.Center);
             Gore.NewGore(projectile.position, Vector2.Zero, mod.GetGoreSlot("Gores/CanisterGore1"), 1f);
             Gore.NewGore(projectile.position, Vector2.Zero, mod.GetGoreSlot("Gores/CanisterGore2"), 1f);
             Gore.NewGore(projectile.position, Vector2.Zero, mod.GetGoreSlot("Gores/CanisterGore3"), 1f);
            for (double i = 0; i < 6.28; i += Main.rand.NextFloat(1f, 2f))
            {
                int lightningproj = Projectile.NewProjectile(projectile.Center, new Vector2((float)Math.Sin(i), (float)Math.Cos(i)) * 2.5f, ModContent.ProjectileType<ElectrapulseCanisterProj2>(), projectile.damage, projectile.knockBack, projectile.owner);
                if (Main.netMode != NetmodeID.Server)
                {
                    AerovelenceMod.primitives.CreateTrail(new CanisterPrimTrail(Main.projectile[lightningproj]));
                }
            }
            for (double i = 0; i < 6.28; i+= 0.1)
            {
                Dust dust = Dust.NewDustPerfect(projectile.Center, 226, new Vector2((float)Math.Sin(i) * 1.3f, (float)Math.Cos(i)) * 2.4f);
                dust.noGravity = true;
            }
        }
        public override void AI()
        {
            i++;
            if (i % 4 == 0)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width / 2, projectile.height / 2, 132);
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockBack, bool crit) => projectile.Kill();
    }
    public class ElectrapulseCanisterProj2 : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Canister Lightning");
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.aiStyle = -1;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.damage = 0;
            projectile.timeLeft = 120;
            projectile.alpha = 255;
            projectile.extraUpdates = 5;
        }

        Vector2 initialVelocity = Vector2.Zero;

        private float lerp;
        public Vector2 DrawPos;
        public int boost;
        public override void AI()
        {
            if (initialVelocity == Vector2.Zero)
            {
                initialVelocity = projectile.velocity;
            }
            if (projectile.timeLeft % 10 == 0)
            {
                projectile.velocity = initialVelocity.RotatedBy(Main.rand.NextFloat(-1, 1));
            }
            /* if (projectile.timeLeft % 2 == 0)
             {
                 Dust dust = Dust.NewDustPerfect(projectile.Center, 226);
                 dust.noGravity = true;
                 dust.scale = (float)Math.Sqrt(projectile.timeLeft) / 4;
                 dust.velocity = Vector2.Zero;
             }*/
            DrawPos = projectile.position;
        }
    }
}