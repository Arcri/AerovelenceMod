using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Thrown
{
    public class PrismPiercer : ModItem
    {
        public float Speed;
        public float HeldTime;

        public float MaxSpeed = 12f;
        public float ChargeCooldown = 20f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prism Piercer");
            Tooltip.SetDefault("Fires knives that electrify enemies");
        }


        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item1;
            item.damage = 24;

            item.width = 22;
            item.height = 40;
            item.useTime = item.useAnimation = 27;

            item.useStyle = ItemUseStyleID.SwingThrow;
            item.channel = item.noMelee = item.autoReuse = item.noUseGraphic = item.melee = true;

            item.knockBack = 4;
            item.value = Item.sellPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Green;
            item.reuseDelay = 120;

            item.shoot = ModContent.ProjectileType<PrismPiercerProjectile>();
            item.shootSpeed = 25f;
        }

        public override void UseStyle(Player player)
        {
            if (player.channel && Speed < MaxSpeed)
            {
                player.itemAnimation = 2;
                player.itemTime = 2;
            }
            else Speed = 1f;

            HeldTime++;
            if(HeldTime % ChargeCooldown == 0)
            {
                Speed++;
                if(Speed > 16f)
                    Speed = 20f;
                if(Speed == 5)
                {
                    for(double i = 0; i < 6.45; i += 0.1)
                    {
                        Dust dust = Dust.NewDustPerfect(player.Center, 61, new Vector2((float)Math.Sin(i) * 2.3f, (float)Math.Cos(i)) * 4.4f);
                        dust.noGravity = true;
                        dust.scale = 1.03f;
                    }
                }
                if(Speed == 10)
                {
                    for (double i = 0; i < 6.45; i += 0.1)
                    {
                        Dust dust = Dust.NewDustPerfect(player.Center, 64, new Vector2((float)Math.Sin(i) * 2.3f, (float)Math.Cos(i)) * 4.4f);
                        dust.noGravity = true;
                        dust.scale = 1.03f;
                    }
                }
                if (Speed == 14)
                {
                    for (double i = 0; i < 6.45; i += 0.1)
                    {
                        Dust dust = Dust.NewDustPerfect(player.Center, 60, new Vector2((float)Math.Sin(i) * 2.3f, (float)Math.Cos(i)) * 4.4f);
                        dust.noGravity = true;
                        dust.scale = 1.03f;
                    }
                }

                item.shootSpeed = Speed;
            }

        }
    }
    public class PrismPiercerProjectile : ModProjectile
    {
       
        public override void SetDefaults()
        {
            projectile.width = projectile.height = 38;

            projectile.friendly = projectile.melee = true;

            projectile.hostile = false;

            projectile.timeLeft = 120;
            projectile.penetrate = -1;
            projectile.light = 0.5f;
            projectile.aiStyle = 1;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 132, 0.5f);
            }
            Main.PlaySound(SoundID.Item10);
            return true;
        }

        private bool rotChanged = false;
        public override void AI()
        {
            projectile.velocity.X *= 0.987f;
            projectile.velocity.Y += 0.03f;

            if (!rotChanged)
            {
                projectile.rotation = projectile.DirectionTo(Main.MouseWorld).ToRotation() - MathHelper.PiOver2;
                rotChanged = true;
            }
        }

        
    }
}