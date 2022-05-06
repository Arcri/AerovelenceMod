using System;
using AerovelenceMod.Content.Items.Placeables.Blocks;
using AerovelenceMod.Content.Items.Placeables.CrystalCaverns;
using AerovelenceMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class Cydia : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cydia");
            Tooltip.SetDefault("Launches a homing crystal scythe that leaves behind a deadly aura");
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item71;
            Item.crit = 20;
            Item.damage = 52;
            Item.DamageType = DamageClass.Melee;
            Item.width = 52;
            Item.height = 52;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CydiaProj>();
            Item.shootSpeed = 15f;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.OrichalcumBar, 15)
                .AddIngredient(ModContent.ItemType<CavernCrystal>(), 15)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    internal sealed class CydiaProj : ModProjectile
    {
        int i;
        private readonly int oneHelixRevolutionInUpdateTicks = 30;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.penetrate = 5;
            Projectile.alpha = 3;
            Projectile.timeLeft = 200;
            Projectile.friendly = true;
        }
        public override bool PreAI()
        {
            i++;
            RunHomingAI();
            ++Projectile.localAI[0];
            float piFraction = MathHelper.Pi / oneHelixRevolutionInUpdateTicks;
            float piFractionVelocity = MathHelper.Pi / oneHelixRevolutionInUpdateTicks;
            float ReversepiFraction = MathHelper.Pi + oneHelixRevolutionInUpdateTicks;
            Vector2 newDustPosition = new Vector2(0, (float)Math.Sin((Projectile.localAI[0] % oneHelixRevolutionInUpdateTicks) * piFraction)) * Projectile.height;
            Dust newDust = Dust.NewDustPerfect(Projectile.Center + newDustPosition.RotatedBy(Projectile.velocity.ToRotation()), 67);
            newDust.noGravity = true;
            newDustPosition.Y *= -1;
            newDust = Dust.NewDustPerfect(Projectile.Center + newDustPosition.RotatedBy(Projectile.velocity.ToRotation()), 67);
            newDust.noGravity = true;
            newDust.velocity *= 0f;
            if(i % 50 == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity / 2, ModContent.ProjectileType<CydiaProj2>(), Projectile.damage, Projectile.knockBack);
            }
            Vector2 newDustPosition2 = new Vector2(0, (float)Math.Sin((Projectile.localAI[0] % oneHelixRevolutionInUpdateTicks) * ReversepiFraction)) * Projectile.height;
            Dust newDust2 = Dust.NewDustPerfect(Projectile.Center + newDustPosition2.RotatedBy(Projectile.velocity.ToRotation()), 68);
            newDust2.noGravity = true;
            newDustPosition2.Y *= -1;
            newDust2 = Dust.NewDustPerfect(Projectile.Center + newDustPosition2.RotatedBy(Projectile.velocity.ToRotation()), 160);
            newDust2.noGravity = true;
            newDust2.velocity *= 0f;
            Projectile.rotation += Projectile.velocity.Length() * 0.1f * Projectile.direction;
            Vector2 Velocity2 = new Vector2(0, (float)Math.Sin(Projectile.localAI[0] % oneHelixRevolutionInUpdateTicks * piFraction)) * Projectile.height;
            return (false);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Vector2 offset = new Vector2(0, 0);
            SoundEngine.PlaySound(SoundID.Item10);
            for (float i = 0; i < 360; i += 0.5f)
            {
                float ang = (float)(i * Math.PI) / 180;
                float x = (float)(Math.Cos(ang) * 15) + Projectile.Center.X;
                float y = (float)(Math.Sin(ang) * 15) + Projectile.Center.Y;
                Vector2 vel = Vector2.Normalize(new Vector2(x - Projectile.Center.X, y - Projectile.Center.Y)) * 7;
                int dustIndex = Dust.NewDust(new Vector2(x - 3, y - 3), 6, 6, 160, vel.X, vel.Y);
                Main.dust[dustIndex].noGravity = true;
            }
            return true;
        }
        public override void Kill(int timeLeft)
        {
            timeLeft = 20;
            for (float i = 0; i < 360; i += 0.5f)
            {
                float ang = (float)(i * Math.PI) / 180;

                float x = (float)(Math.Cos(ang) * 15) + Projectile.Center.X;
                float y = (float)(Math.Sin(ang) * 15) + Projectile.Center.Y;

                Vector2 vel = Vector2.Normalize(new Vector2(x - Projectile.Center.X, y - Projectile.Center.Y)) * 7;

                int dustIndex = Dust.NewDust(new Vector2(x - 3, y - 3), 6, 6, 160, vel.X, vel.Y);
                Main.dust[dustIndex].noGravity = true;
            }
        }
        private void RunHomingAI()
        {
            Projectile proj = Projectile;
            float projPosMidX = proj.position.X + proj.width / 2;
            float projPosMidY = proj.position.Y + proj.height / 2;
            float closestNpcPosX = proj.Center.X;
            float closestNpcPosY = proj.Center.Y;
            float closestNpcDistBothAxis = 400f;
            bool targetNpcFound = false;

            for (int npcWho = 0; npcWho < 200; npcWho++)
            {
                NPC npc = Main.npc[npcWho];
                if (!npc.CanBeChasedBy(proj, false))
                {
                    continue;
                }
                if (proj.Distance(npc.Center) >= closestNpcDistBothAxis)
                {
                    continue;
                }
                if (!Collision.CanHit(proj.Center, 1, 1, npc.Center, 1, 1))
                {
                    continue;
                }
                float npcPosMidX = npc.position.X + npc.width / 2;
                float npcPosMidY = npc.position.Y + npc.height / 2;

                float bothAxisDist = Math.Abs(projPosMidX - npcPosMidX) + Math.Abs(projPosMidY - npcPosMidY);
                if (bothAxisDist < closestNpcDistBothAxis)
                {
                    closestNpcDistBothAxis = bothAxisDist;
                    closestNpcPosX = npcPosMidX;
                    closestNpcPosY = npcPosMidY;
                    targetNpcFound = true;
                }
            }
            if (!targetNpcFound)
            {
                return;
            }
            Vector2 projPosMid = new Vector2(projPosMidX, projPosMidY);
            float closestNpcDistX = closestNpcPosX - projPosMid.X;
            float closestNpcDistY = closestNpcPosY - projPosMid.Y;
            float closestNpcDist = (float)Math.Sqrt((closestNpcDistX * closestNpcDistX) + (closestNpcDistY * closestNpcDistY));
            closestNpcDist = 6f / closestNpcDist;
            closestNpcDistX *= closestNpcDist;
            closestNpcDistY *= closestNpcDist;
            proj.velocity.X = ((proj.velocity.X * 20f) + closestNpcDistX) / 21f;
            proj.velocity.Y = ((proj.velocity.Y * 20f) + closestNpcDistY) / 21f;
            Projectile.velocity *= 1.005f;
        }
		public override bool PreDraw(ref Color lightColor) 
            => this.DrawAroundOrigin(Main.spriteBatch, lightColor * Projectile.Opacity);
    }

    internal sealed class CydiaProj2 : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.penetrate = 5;
            Projectile.alpha = 3;
            Projectile.timeLeft = 100;
            Projectile.damage = 60;
            Projectile.scale = 1f;

            Projectile.friendly = true;
        }
        public override bool PreAI()
        {
            Projectile.scale *= 0.99f;
            Vector2 from = Projectile.position;
            for (int i = 0; i < 360; i += 20)
            {
                Vector2 circular = new Vector2(24 * Projectile.scale, 0).RotatedBy(MathHelper.ToRadians(i));
                circular.X *= 0.7f;
                circular = circular.RotatedBy(Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X));
                Vector2 dustVelo = new Vector2(0, 0).RotatedBy(Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X));
                Dust dust = Dust.NewDustDirect(from - new Vector2(5) + circular, 0, 0, 67, 0, 0, Projectile.alpha);
                dust.velocity *= 0.15f;
                dust.velocity += dustVelo;
                dust.noGravity = true;
            }
            return true;
        }
    }
}