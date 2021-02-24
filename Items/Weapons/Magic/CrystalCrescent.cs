using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace AerovelenceMod.Items.Weapons.Magic
{
    public class CrystalCrescent : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
            DisplayName.SetDefault("Crystal Crescent");
            Tooltip.SetDefault("Left click to fire a slow but damaging crystal\nLeft click to fire a beam of luminous light");
        }
        public override void SetDefaults()
        {
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useAnimation = 30;
            item.useTime = 30;
            item.shootSpeed = 24f;
            item.knockBack = 5f;
            item.width = 16;
            item.height = 16;
            item.UseSound = SoundID.DD2_SkyDragonsFurySwing;
            item.shoot = ModContent.ProjectileType<CrystalCrescentProj>();
            item.rare = ItemRarityID.Yellow;
            item.value = Item.sellPrice(0, 5);
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.autoReuse = true;
            item.melee = true;
            item.damage = 70;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.useStyle = ItemUseStyleID.HoldingOut;
                item.useAnimation = 30;
                item.useTime = 30;
                item.shootSpeed = 24f;
                item.knockBack = 5f;
                item.width = 16;
                item.height = 16;
                item.UseSound = SoundID.DD2_SkyDragonsFurySwing;
                item.shoot = ModContent.ProjectileType<CrystalCrescentProjAlt>();
                item.rare = ItemRarityID.Yellow;
                item.value = Item.sellPrice(0, 5);
                item.noMelee = true;
                item.noUseGraphic = true;
                item.channel = true;
                item.autoReuse = true;
                item.melee = true;
                item.damage = 70;
            }
            else
            {
                item.useStyle = ItemUseStyleID.HoldingOut;
                item.useAnimation = 30;
                item.useTime = 30;
                item.shootSpeed = 24f;
                item.knockBack = 5f;
                item.width = 16;
                item.height = 16;
                item.UseSound = SoundID.DD2_SkyDragonsFurySwing;
                item.shoot = ModContent.ProjectileType<CrystalCrescentProj>();
                item.rare = ItemRarityID.Yellow;
                item.value = Item.sellPrice(0, 5);
                item.noMelee = true;
                item.noUseGraphic = true;
                item.channel = true;
                item.autoReuse = true;
                item.melee = true;
                item.damage = 70;
            }
            return player.ownedProjectileCounts[item.shoot] < 1;

        }
    }
}

namespace AerovelenceMod.Items.Weapons.Magic
{
    public class CrystalCrescentProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Crescent");
          //  Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {

            projectile.width = 68;
            projectile.height = 68;
            projectile.aiStyle = 140;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
           // projectile.alpha = 255;
            projectile.hide = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 6;
            projectile.ownerHitCheck = true;
        }
        public override void AI()
        {
            
            float speed = 50f;
            float rotSpeed = 2f;
            float scaleFactor = 20f;
            Player player = Main.player[projectile.owner];
            float num3 = -(float)Math.PI / 4f;
            Vector2 value = player.RotatedRelativePoint(player.MountedCenter);
            Vector2 vector = Vector2.Zero;
            int hitDirection = base.projectile.direction;
            

            if (player.dead)
            {
                projectile.Kill();
                return;
            }
            {
                Lighting.AddLight(player.Center, 0.75f, 0.9f, 1.15f);
                int num10 = Math.Sign(projectile.velocity.X);
                projectile.velocity = new Vector2(num10, 0f);
                if (projectile.ai[0] == 0f)
                {
                    projectile.rotation = new Vector2(num10, 0f - player.gravDir).ToRotation() + num3 + (float)Math.PI;
                    if (projectile.velocity.X < 0f)
                    {
                        projectile.rotation -= (float)Math.PI / 2f;
                    }
                }
                projectile.alpha -= 128;
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                }
                _ = projectile.ai[0] / speed;
                float num11 = 1f;
                projectile.ai[0] += num11;
                projectile.rotation += (float)Math.PI * 2f * rotSpeed / speed * num10;
                bool flag2 = projectile.ai[0] == (int)(speed / 2f);
                if (projectile.ai[0] >= speed || (flag2 && !player.controlUseItem))
                {
                    projectile.Kill();
                    player.reuseDelay = 2;
                }
                else if (flag2)
                {
                    Vector2 mouseWorld2 = Main.MouseWorld;
                    int num12 = (player.DirectionTo(mouseWorld2).X > 0f) ? 1 : (-1);
                    if (num12 != projectile.velocity.X)
                    {
                        player.ChangeDir(num12);
                        projectile.velocity = new Vector2(num12, 0f);
                        projectile.netUpdate = true;
                        projectile.rotation -= (float)Math.PI;
                    }
                }
                if ((projectile.ai[0] == num11 || (projectile.ai[0] == (int)(speed / 2f) && projectile.active)) && projectile.owner == Main.myPlayer)
                {
                    Vector2 mouseWorld3 = Main.MouseWorld;
                    _ = player.DirectionTo(mouseWorld3) * 0f;
                }
                float num13 = projectile.rotation - (float)Math.PI / 4f * num10;
                vector = (num13 + ((num10 == -1) ? ((float)Math.PI) : 0f)).ToRotationVector2() * (projectile.ai[0] / speed) * scaleFactor;
                Vector2 value2 = projectile.Center + (num13 + ((num10 == -1) ? ((float)Math.PI) : 0f)).ToRotationVector2() * 30f;
                Vector2 vector3 = num13.ToRotationVector2();
                Vector2 value3 = vector3.RotatedBy((float)Math.PI / 2f * projectile.spriteDirection);
                if (Main.rand.Next(2) == 0)
                {
                    Dust dust4 = Dust.NewDustDirect(value2 - new Vector2(5f), 10, 10, 31, player.velocity.X, player.velocity.Y, 150);
                    dust4.velocity = projectile.DirectionTo(dust4.position) * 0.1f + dust4.velocity * 0.1f;
                }
                for (int j = 0; j < 4; j++)
                {
                    float scaleFactor2 = 1f;
                    float scaleFactor3 = 1f;
                    switch (j)
                    {
                        case 1:
                            scaleFactor3 = -1f;
                            break;
                        case 2:
                            scaleFactor3 = 1.25f;
                            scaleFactor2 = 0.5f;
                            break;
                        case 3:
                            scaleFactor3 = -1.25f;
                            scaleFactor2 = 0.5f;
                            break;
                    }
                    {
                    }
                    if (Main.rand.Next(6) != 0)
                    {
                        Dust dust5 = Dust.NewDustDirect(projectile.position, 0, 0, 226, 0f, 0f, 100);
                        dust5.position = projectile.Center + vector3 * (60f + Main.rand.NextFloat() * 20f) * scaleFactor3;
                        dust5.velocity = value3 * (4f + 4f * Main.rand.NextFloat()) * scaleFactor3 * scaleFactor2;
                        dust5.noGravity = true;
                        dust5.noLight = true;
                        dust5.scale = 0.5f;
                        dust5.customData = this;
                        if (Main.rand.Next(4) == 0)
                        {
                            dust5.noGravity = false;
                        }
                    }
                }
            }
            projectile.position = value - projectile.Size / 2f;
            projectile.position += vector;
            projectile.spriteDirection = projectile.direction;
            projectile.timeLeft = 2;
            player.ChangeDir(projectile.direction);
            player.heldProj = player.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = MathHelper.WrapAngle(projectile.rotation);
        }
    }
}
namespace AerovelenceMod.Items.Weapons.Magic
{
    public class CrystalCrescentProjAlt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Crescent");
            //Main.projFrames[projectile.type] = 4;
        }
        
        public override void SetDefaults()
        {
            projectile.width = 68;
            projectile.height = 68;
            projectile.aiStyle = 142;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
           // projectile.alpha = 255;
            projectile.hide = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 6;
            projectile.ownerHitCheck = true;
        }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter);
            projectile.direction = player.direction;
            player.heldProj = projectile.whoAmI;
            projectile.Center = vector;
            int hitDirection = base.projectile.direction;
            {

                Lighting.AddLight(player.Center, 0.75f, 0.9f, 1.15f);
                projectile.spriteDirection = (projectile.direction = player.direction);
                projectile.alpha -= 127;
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                }
                float num7 = player.itemAnimation / (float)player.itemAnimationMax;
                float num8 = 1f - num7;
                float num9 = projectile.velocity.ToRotation();
                float num10 = projectile.velocity.Length();
                float num11 = 22f;
                Vector2 spinningpoint2 = new Vector2(1f, 0f).RotatedBy((float)Math.PI + num8 * ((float)Math.PI * 2f)) * new Vector2(num10, projectile.ai[0]);
                projectile.position += spinningpoint2.RotatedBy(num9) + new Vector2(num10 + num11, 0f).RotatedBy(num9);
                Vector2 destination2 = vector + spinningpoint2.RotatedBy(num9) + new Vector2(num10 + num11 + 40f, 0f).RotatedBy(num9);
                projectile.rotation = player.AngleTo(destination2) + (float)Math.PI / 4f * player.direction;
                if (projectile.spriteDirection == -1)
                {
                    projectile.rotation += (float)Math.PI;
                }
                float f = projectile.rotation - (float)Math.PI / 4f * (float)Math.Sign(projectile.velocity.X);
                player.DirectionTo(projectile.Center);
                player.DirectionTo(destination2);
                Vector2 vector3 = projectile.velocity.SafeNormalize(Vector2.UnitY);
                if ((player.itemAnimation == 2 || player.itemAnimation == 6 || player.itemAnimation == 10) && projectile.owner == Main.myPlayer)
                {
                    Vector2 velocity = vector3 + Main.rand.NextVector2Square(-0.2f, 0.2f);
                    velocity *= 12f;
                    switch (player.itemAnimation)
                    {
                        case 2:
                            velocity = vector3.RotatedBy(0.38397246599197388);
                            break;
                        case 6:
                            velocity = vector3.RotatedBy(-0.38397246599197388);
                            break;
                        case 10:
                            velocity = vector3.RotatedBy(0.0);
                            break;
                    }
                    velocity *= 10f + (float)Main.rand.Next(4);
                    // Projectile.NewProjectile(base.projectile.Center, velocity, ModContent.ProjectileType<CrystalCrescentProjAlt>(), projectile.damage, 0f, projectile.owner);
                }
                for (int k = 0; k < 3; k += 2)
                {
                    float scaleFactor = 1f;
                    float num12 = 1f;
                    switch (k)
                    {
                        case 1:
                            num12 = -1f;
                            break;
                        case 2:
                            num12 = 1.25f;
                            scaleFactor = 0.5f;
                            break;
                        case 3:
                            num12 = -1.25f;
                            scaleFactor = 0.5f;
                            break;
                    }
                    if (Main.rand.Next(6) != 0)
                    {
                        num12 *= 1.2f;
                        Dust dust3 = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 226, 0f, 0f, 100);
                        dust3.velocity = vector3 * (4f + 4f * Main.rand.NextFloat()) * num12 * scaleFactor;
                        dust3.noGravity = true;
                        dust3.noLight = true;
                        dust3.scale = 0.75f;
                        dust3.fadeIn = 0.8f;
                        dust3.customData = this;
                        if (Main.rand.Next(3) == 0)
                        {
                            dust3.noGravity = false;
                            dust3.fadeIn = 0f;
                        }
                    }
                }
            }
        }
    }
}