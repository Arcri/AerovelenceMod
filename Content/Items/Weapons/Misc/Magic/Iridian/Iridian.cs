using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;
using Terraria.DataStructures;
using AerovelenceMod.Content.Projectiles.Weapons.Magic;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.Graphics.Shaders;
using AerovelenceMod.Content.Projectiles;
using AerovelenceMod.Content.Items.Weapons.Starglass;
using Terraria.GameContent.Drawing;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Magic.Iridian
{
    public class Iridian : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("");
        }
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Magic;
            Item.damage = 50;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.Purple;

            Item.shoot = ModContent.ProjectileType<IridianShot1>();
            Item.shootSpeed = 25;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.useTime = 5;
            Item.useAnimation = 25;
            Item.reuseDelay = 35;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.mana = 20;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(10, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.12f) * 1.5f, type, damage, knockback, player.whoAmI);

            SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/laser_line") with { Pitch = .55f, Volume = 0.75f }; SoundEngine.PlaySound(style, player.Center);
            SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_176") with { Pitch = 1f, }; SoundEngine.PlaySound(style2, player.Center);
            return false;
        }
    }

    public class IridianShot1 : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shot");
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;

            Projectile.timeLeft = 200;
            Projectile.penetrate = 5;
        }

        BaseTrailInfo trail1 = new BaseTrailInfo();
        BaseTrailInfo trail2 = new BaseTrailInfo();
        int timer = 0;
        public override void AI()
        {
            //EnergyTex White + Extra196 color is really good but not for here
            
            
            //Trail1 Info Dump
            trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlamesTextureButBlack").Value;
            trail1.trailColor = Color.Wheat;
            trail1.trailPointLimit = 400;
            trail1.trailWidth = 15;
            trail1.trailMaxLength = 400;
            trail1.timesToDraw = 2;

            trail1.trailTime = timer * 0.02f;
            trail1.trailRot = Projectile.velocity.ToRotation();
            trail1.trailPos = Projectile.Center + Projectile.velocity;
            trail1.TrailLogic();

            //Trail2 Info Dump
            trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/LintyTrail").Value;
            trail2.trailColor = Color.White;
            trail2.trailPointLimit = 400;
            trail2.trailWidth = 15;
            trail2.trailMaxLength = 400;
            trail2.timesToDraw = 2;

            trail2.gradient = true;
            trail2.gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/RainbowGrad1").Value;
            trail2.shouldScrollColor = true;
            trail2.gradientTime = timer * 0.04f;

            trail2.trailTime = timer * 0.08f;
            trail2.trailRot = Projectile.velocity.ToRotation();
            trail2.trailPos = Projectile.Center + Projectile.velocity;
            trail2.TrailLogic();

            

            timer++;

            Projectile.velocity.Y += 0.5f;
            Projectile.rotation = Projectile.velocity.ToRotation();

        }

        public override bool PreDraw(ref Color lightColor)
        {
            trail1.TrailDrawing(Main.spriteBatch);
            trail2.TrailDrawing(Main.spriteBatch);

            Texture2D Star = Mod.Assets.Request<Texture2D>("Assets/TrailImages/Starlight").Value;

            Vector2 vec2Scale = new Vector2(2f, 1f);

            Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * 1f, Projectile.rotation, Star.Size() / 2, vec2Scale * 1.5f, SpriteEffects.None, 0f);


            //Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(15, 15), null, Color.Red with { A = 0 } * 0.25f, Projectile.rotation, Star.Size() / 2, vec2Scale * 1.5f, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(15, 15), null, Color.Orange with { A = 0 } * 0.25f, Projectile.rotation, Star.Size() / 2, vec2Scale * 1.5f, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(15, 15), null, Color.Yellow with { A = 0 } * 0.25f, Projectile.rotation, Star.Size() / 2, vec2Scale * 1.5f, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(15, 15), null, Color.Green with { A = 0 } * 0.25f, Projectile.rotation, Star.Size() / 2, vec2Scale * 1.5f, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(15, 15), null, Color.Blue with { A = 0 } * 0.25f, Projectile.rotation, Star.Size() / 2, vec2Scale * 1.5f, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(15, 15), null, Color.Purple with { A = 0 } * 0.25f, Projectile.rotation, Star.Size() / 2, vec2Scale * 1.5f, SpriteEffects.None, 0f);

            //Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, Projectile.rotation, Star.Size() / 2, vec2Scale * 1.5f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, Projectile.rotation + MathHelper.PiOver2, Star.Size() / 2, new Vector2(1.5f, 1.5f), SpriteEffects.None, 0f);


            return false;
        }

    }

}
