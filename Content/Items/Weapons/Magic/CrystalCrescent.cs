using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class CrystalCrescent : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
            DisplayName.SetDefault("Crystal Crescent");
            Tooltip.SetDefault("Left click to fire a slow but damaging crystal\nLeft click to fire a beam of luminous light\nVERY UNFINISHED");
        }
        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.shootSpeed = 24f;
            Item.knockBack = 5f;
            Item.width = 16;
            Item.height = 16;
            Item.UseSound = SoundID.DD2_SkyDragonsFurySwing;
            Item.shoot = ModContent.ProjectileType<CrystalCrescentProj>();
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 1);
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 13;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.useAnimation = 30;
                Item.useTime = 30;
                Item.shootSpeed = 24f;
                Item.knockBack = 5f;
                Item.width = 16;
                Item.height = 16;
                Item.UseSound = SoundID.DD2_SkyDragonsFurySwing;
                Item.shoot = ModContent.ProjectileType<CrystalCrescentProjAlt>();
                Item.rare = ItemRarityID.Yellow;
                Item.value = Item.sellPrice(0, 5);
                Item.noMelee = true;
                Item.noUseGraphic = true;
                Item.channel = true;
                Item.autoReuse = true;
                Item.DamageType = DamageClass.Melee;
                Item.damage = 70;
            }
            else
            {
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.useAnimation = 30;
                Item.useTime = 30;
                Item.shootSpeed = 24f;
                Item.knockBack = 5f;
                Item.width = 16;
                Item.height = 16;
                Item.UseSound = SoundID.DD2_SkyDragonsFurySwing;
                Item.shoot = ModContent.ProjectileType<CrystalCrescentProj>();
                Item.rare = ItemRarityID.Yellow;
                Item.value = Item.sellPrice(0, 5);
                Item.noMelee = true;
                Item.noUseGraphic = true;
                Item.channel = true;
                Item.autoReuse = true;
                Item.DamageType = DamageClass.Melee;
                Item.damage = 70;
            }
            return player.ownedProjectileCounts[Item.shoot] < 1;

        }
    }

    public class CrystalCrescentProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Crescent");
            //  Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {

            Projectile.width = 68;
            Projectile.height = 68;
            Projectile.aiStyle = 140;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            // projectile.alpha = 255;
            Projectile.hide = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
            Projectile.ownerHitCheck = true;
        }
        public override void AI()
        {
            
            float speed = 50f;
            float rotSpeed = 2f;
            float scaleFactor = 20f;
            Player player = Main.player[Projectile.owner];
            float num3 = -(float)Math.PI / 4f;
            Vector2 value = player.RotatedRelativePoint(player.MountedCenter);
            Vector2 vector = Vector2.Zero;
            int hitDirection = base.Projectile.direction;
            

            if (player.dead)
            {
                Projectile.Kill();
                return;
            }
            {
                Lighting.AddLight(player.Center, 0.75f, 0.9f, 1.15f);
                int num10 = Math.Sign(Projectile.velocity.X);
                Projectile.velocity = new Vector2(num10, 0f);
                if (Projectile.ai[0] == 0f)
                {
                    Projectile.rotation = new Vector2(num10, 0f - player.gravDir).ToRotation() + num3 + (float)Math.PI;
                    if (Projectile.velocity.X < 0f)
                    {
                        Projectile.rotation -= (float)Math.PI / 2f;
                    }
                }
                Projectile.alpha -= 128;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
                _ = Projectile.ai[0] / speed;
                float num11 = 1f;
                Projectile.ai[0] += num11;
                Projectile.rotation += (float)Math.PI * 2f * rotSpeed / speed * num10;
                bool flag2 = Projectile.ai[0] == (int)(speed / 2f);
                if (Projectile.ai[0] >= speed || (flag2 && !player.controlUseItem))
                {
                    Projectile.Kill();
                    player.reuseDelay = 2;
                }
                else if (flag2)
                {
                    Vector2 mouseWorld2 = Main.MouseWorld;
                    int num12 = (player.DirectionTo(mouseWorld2).X > 0f) ? 1 : (-1);
                    if (num12 != Projectile.velocity.X)
                    {
                        player.ChangeDir(num12);
                        Projectile.velocity = new Vector2(num12, 0f);
                        Projectile.netUpdate = true;
                        Projectile.rotation -= (float)Math.PI;
                    }
                }
                if ((Projectile.ai[0] == num11 || (Projectile.ai[0] == (int)(speed / 2f) && Projectile.active)) && Projectile.owner == Main.myPlayer)
                {
                    Vector2 mouseWorld3 = Main.MouseWorld;
                    _ = player.DirectionTo(mouseWorld3) * 0f;
                }
                float num13 = Projectile.rotation - (float)Math.PI / 4f * num10;
                vector = (num13 + ((num10 == -1) ? ((float)Math.PI) : 0f)).ToRotationVector2() * (Projectile.ai[0] / speed) * scaleFactor;
                Vector2 value2 = Projectile.Center + (num13 + ((num10 == -1) ? ((float)Math.PI) : 0f)).ToRotationVector2() * 30f;
                Vector2 vector3 = num13.ToRotationVector2();
                Vector2 value3 = vector3.RotatedBy((float)Math.PI / 2f * Projectile.spriteDirection);
                if (Main.rand.Next(2) == 0)
                {
                    Dust dust4 = Dust.NewDustDirect(value2 - new Vector2(5f), 10, 10, 31, player.velocity.X, player.velocity.Y, 150);
                    dust4.velocity = Projectile.DirectionTo(dust4.position) * 0.1f + dust4.velocity * 0.1f;
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
                        Dust dust5 = Dust.NewDustDirect(Projectile.position, 0, 0, 226, 0f, 0f, 100);
                        dust5.position = Projectile.Center + vector3 * (60f + Main.rand.NextFloat() * 20f) * scaleFactor3;
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
            Projectile.position = value - Projectile.Size / 2f;
            Projectile.position += vector;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = player.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = MathHelper.WrapAngle(Projectile.rotation);
        }
    }

    public class CrystalCrescentProjAlt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Crescent");
            //Main.projFrames[projectile.type] = 4;
        }
        
        public override void SetDefaults()
        {
            Projectile.width = 68;
            Projectile.height = 68;
            Projectile.aiStyle = 142;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            // projectile.alpha = 255;
            Projectile.hide = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
            Projectile.ownerHitCheck = true;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter);
            Projectile.direction = player.direction;
            player.heldProj = Projectile.whoAmI;
            Projectile.Center = vector;
            int hitDirection = base.Projectile.direction;
            {

                Lighting.AddLight(player.Center, 0.75f, 0.9f, 1.15f);
                Projectile.spriteDirection = (Projectile.direction = player.direction);
                Projectile.alpha -= 127;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
                float num7 = player.itemAnimation / (float)player.itemAnimationMax;
                float num8 = 1f - num7;
                float num9 = Projectile.velocity.ToRotation();
                float num10 = Projectile.velocity.Length();
                float num11 = 22f;
                Vector2 spinningpoint2 = new Vector2(1f, 0f).RotatedBy((float)Math.PI + num8 * ((float)Math.PI * 2f)) * new Vector2(num10, Projectile.ai[0]);
                Projectile.position += spinningpoint2.RotatedBy(num9) + new Vector2(num10 + num11, 0f).RotatedBy(num9);
                Vector2 destination2 = vector + spinningpoint2.RotatedBy(num9) + new Vector2(num10 + num11 + 40f, 0f).RotatedBy(num9);
                Projectile.rotation = player.AngleTo(destination2) + (float)Math.PI / 4f * player.direction;
                if (Projectile.spriteDirection == -1)
                {
                    Projectile.rotation += (float)Math.PI;
                }
                float f = Projectile.rotation - (float)Math.PI / 4f * (float)Math.Sign(Projectile.velocity.X);
                player.DirectionTo(Projectile.Center);
                player.DirectionTo(destination2);
                Vector2 vector3 = Projectile.velocity.SafeNormalize(Vector2.UnitY);
                if ((player.itemAnimation == 2 || player.itemAnimation == 6 || player.itemAnimation == 10) && Projectile.owner == Main.myPlayer)
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
                        Dust dust3 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 226, 0f, 0f, 100);
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