﻿using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.ObjectModel;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Projectiles;
using AerovelenceMod.Content.Dusts.GlowDusts;

namespace AerovelenceMod.Content.Items.Weapons.Ocean
{
    public class OceanMist : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ocean Mist");
            Tooltip.SetDefault("TODO");
        }
        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.DamageType = DamageClass.Magic;
            Item.width = 18;
            Item.height = 18;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<OceanMistHeldProj>();
            Item.shootSpeed = 8f;
            //Item.noUseGraphic = true;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(0, 0);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            
            return true;
        }

    }

    public class OceanMistHeldProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        int timer = 0;
        public int OFFSET = 15; //30
        public ref float Angle => ref Projectile.ai[1];
        public Vector2 direction = Vector2.Zero;
        public float lerpToStuff = 0;
        public bool hasReachedDestination = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Antique Pistol");
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 100;
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.scale = 1;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Player Player = Main.player[Projectile.owner];

            Player.itemTime = 2; 
            Player.itemAnimation = 2; 

            if (Projectile.owner == Main.myPlayer)
            {
                Angle = (Main.MouseWorld - (Player.Center)).ToRotation();
            }

            direction = Angle.ToRotationVector2();
            Player.ChangeDir(direction.X > 0 ? 1 : -1);

            //lerpToStuff = Math.Clamp(MathHelper.Lerp(lerpToStuff, -0.2f, 0.06f), 0, 0.4f);

            direction = Angle.ToRotationVector2().RotatedBy(Player.direction * -1f);
            Projectile.Center = Player.Center + (direction * OFFSET);
            Projectile.velocity = Vector2.Zero;
            Player.itemRotation = direction.ToRotation();

            if (Player.direction != 1)
                Player.itemRotation -= 3.14f;

            Player.itemRotation = MathHelper.WrapAngle(Player.itemRotation);

            Player.heldProj = Projectile.whoAmI;

            Projectile.rotation = direction.ToRotation();

            if (timer == 60)
            {
                Projectile.active = false;
            }

            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];
            Texture2D Weapon = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ocean/OceanMist");
            Texture2D Twirl = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/PixelSwirl");

            SpriteEffects mySE = Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Main.spriteBatch.Draw(Weapon, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, Weapon.Size() / 2, Projectile.scale, mySE, 0f);

            return false;
        }
    }

    public class OceanMistShot : TrailProjBase
    {
        public override string Texture => "Terraria/Images/Projectile_0";
 
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ocean Mist");
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 100;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.09f;
            Projectile.ai[1] += 0.05f;

            trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/GlowTrail").Value;
            trailColor = Color.DarkBlue;
            trailTime = Projectile.ai[1];

            trailPointLimit = 120;
            trailWidth = 20;
            trailMaxLength = 300;


            trailRot = Projectile.velocity.ToRotation();
            trailPos = Projectile.Center;
            TrailLogic();
        }

        public override void Kill(int timeLeft)
        {
            
            ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            int i = 0;
            foreach (Vector2 pos in trailPositions)
            {
                i++;
                if (i % 4 == 0)
                {
                    int a = GlowDustHelper.DrawGlowDust(pos, 0, 0, ModContent.DustType<GlowCircleFlare>(), Color.DodgerBlue, 0.4f, 0.4f, 0f, dustShader2);
                    Main.dust[a].fadeIn = 1;
                    Main.dust[a].velocity *= (i * 0.03f);
                }
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {

            TrailDrawing();

            return false;
        }

        //Controls the width of the function based on progress
        //Progress ranges from 0-1 based on how far along the trail we are
        //Dick around with it to see what i mean
        //Dont override at all if you want to use the default width fuction in super
        public override float WidthFunction(float progress)
        {
            if (progress < 0.5f)
            {
                float num = 1f;
                float lerpValue = Utils.GetLerpValue(0f, 0.4f, progress, clamped: true);
                num *= 1f - (1f - lerpValue) * (1f - lerpValue);
                return MathHelper.Lerp(0f, 30f, num) * 0.4f;
            }
            else if (progress >= 0.5)
            {
                float num = 1f;
                float lerpValue = Utils.GetLerpValue(0f, 0.6f, 1 - progress, clamped: true);
                num *= 1f - (1f - lerpValue) * (1f - lerpValue);
                return MathHelper.Lerp(0f, 30f, num) * 0.4f;
            }
            return 0;
        }
    }

}
