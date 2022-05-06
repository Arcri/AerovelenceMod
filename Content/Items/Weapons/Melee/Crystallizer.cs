using System;
using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class Crystallizer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystallizer");
            Tooltip.SetDefault("Rains down on enemies when near an enemy");
        }
        public override void SetDefaults()
        {
            Item.channel = true;		
            Item.crit = 2;
            Item.damage = 27;
            Item.DamageType = DamageClass.Melee;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 0, 80, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = false;
            Item.shoot = Mod.Find<ModProjectile>("CrystallizerProj").Type;
            Item.shootSpeed = 2f;
        }
    }

    public class CrystallizerProj : ModProjectile
    {
        private int shootTimer;
        public override void SetDefaults()
        {
            Projectile.extraUpdates = 0;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.damage = 10;
            Projectile.aiStyle = 99;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 6;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 216f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 13f;
        }
        public override void AI()
        {
			
            float distance = 192f;
            bool npcNearby = false;
            for (int k = 0; k < 200; k++)
            {
                if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5 && Main.npc[k].type != NPCID.TargetDummy)
                {
                    Vector2 newMove = Main.npc[k].Center - Projectile.Center;
                    float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                    if (distanceTo < distance)
                    {
                        distance = distanceTo;
                        npcNearby = true;
                    }

                }

            }
            shootTimer++;
            if (shootTimer >= Main.rand.Next(20, 30))
                if (npcNearby)
                {

                    {
                        int type = Mod.Find<ModProjectile>("CrystallizerProjectile").Type;
                        Vector2 offset = Projectile.Center + new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
                        Projectile.NewProjectileDirect(offset, new Vector2(Main.rand.NextFloat(-1f, 1f), -5f + Main.rand.NextFloat(-1f, 1f)), ModContent.ProjectileType<CrystallizerProjectile>(), 5, 0.5f, Main.myPlayer);
                        shootTimer = 0;
                    }
                }

        }
    }

    partial class CrystallizerProjectile : ModProjectile
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
                        Type: 60,
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

    partial class CrystallizerProjectile : ModProjectile
    {
        private static int AquaSceptreAiStyle;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.WaterStream;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Spray");

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