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

namespace AerovelenceMod.Content.Items.Weapons.Ember
{
    public class PagesOfDawn : ModItem
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
            Item.rare = ItemRarityID.Pink;

            Item.shoot = ModContent.ProjectileType<PagesOfDawnProj>();
            Item.shootSpeed = 10;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.useTime = 7;
            Item.useAnimation = 28;
            Item.reuseDelay = 10;
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
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
    }

    public class PagesOfDawnProj : TrailProjBase
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fire");
        }
        public override void SetDefaults()
        {
            Projectile.damage = 10;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;
        }
        int timer = 0;
        public override void AI()
        {
            if (timer < 30)
                Projectile.velocity *= 1.01f;
            else
                Projectile.velocity.Y += 0.4f;

            //Projectile.velocity = Projectile.velocity.RotatedBy(0.04f);

            timer++;

            trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlamesTexture").Value;
            //trailColor = Color.DodgerBlue;
            trailTime = timer * 0.05f;
            shouldScrollColor = true;
            gradient = true;
            gradientTime = 0.7f;
            gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/FireGradLoop").Value;

            // other things you can adjust
            trailPointLimit = 120;
            trailWidth = 25;
            trailMaxLength = 300;

            //MUST call TrailLogic AFTER assigning trailRot and trailPos
            trailRot = Projectile.velocity.ToRotation();
            trailPos = Projectile.Center;
            TrailLogic();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            TrailDrawing();

            return false;
        }
        public override float WidthFunction(float progress)
        {
            //return trailWidth;

            //float num = 1f;
            //float lerpValue = Utils.GetLerpValue(0f, 0.4f, 1 - progress, clamped: true);
            //num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            //return MathHelper.Lerp(0f, trailWidth, num) * 0.4f; // 0.3f

            if (progress > 0.5)
            {

                float num = 1f;
                float lerpValue = Utils.GetLerpValue(0f, 0.4f, 1 - progress, clamped: true);
                num *= 1f - (1f - lerpValue) * (1f - lerpValue);
                return MathHelper.Lerp(0f, trailWidth, num) * 0.5f; // 0.3f
            }
            else
            {

                float num = 1f;
                float lerpValue = Utils.GetLerpValue(0f, 0.4f, progress, clamped: true);
                num *= 1f - (1f - lerpValue) * (1f - lerpValue);
                return MathHelper.Lerp(0f, trailWidth, num) * 0.3f; // 0.3f
            }
            return 0;
        }

    }

    public class PagesOfDawnFireball : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetDefaults()
        {
            Projectile.damage = 10;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;
        }
        int timer = 0;
        float scale = 1f;
        float alpha = 1f;
        public override void AI()
        {
            Projectile.rotation += 0.05f;

            scale = 0.35f;

            timer++;
        }

        Effect myEffect = null;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Orb = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/bigCircle2");
            Texture2D spin = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/feather_circle");
            Texture2D edge = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/impact_2newbetterfade");

            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Vector2 scale2 = new Vector2(0.95f, 1f) * scale;

            Main.spriteBatch.Draw(Orb, Projectile.Center - Main.screenPosition, null, Color.Black * 0.8f, Projectile.rotation, Orb.Size() / 2, scale * 3f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Orb, Projectile.Center - Main.screenPosition, null, Color.Black * 0.35f, Projectile.rotation -1, Orb.Size() / 2, scale * 2.85f, SpriteEffects.None, 0f);


            if (myEffect == null)
                myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Radial/BoFIrisAlt", AssetRequestMode.ImmediateLoad).Value;

            myEffect.Parameters["causticTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/noise").Value);
            myEffect.Parameters["gradientTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/orangeGrad").Value);
            myEffect.Parameters["distortTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Swirl").Value);

            myEffect.Parameters["flowSpeed"].SetValue(0.3f);
            myEffect.Parameters["vignetteSize"].SetValue(0.4f);
            myEffect.Parameters["vignetteBlend"].SetValue(1f);
            myEffect.Parameters["distortStrength"].SetValue(0.06f);
            myEffect.Parameters["xOffset"].SetValue(0f);
            myEffect.Parameters["uTime"].SetValue(timer * 0.01f);
            myEffect.Parameters["colorIntensity"].SetValue(1f);
            myEffect.Parameters["squashValue"].SetValue(0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();


            Main.spriteBatch.Draw(spin, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, spin.Size() / 2, scale2 * 1.2f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(spin, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation * -1.2f, spin.Size() / 2, scale2 * 1.2f, SpriteEffects.FlipHorizontally, 0f);

            Main.spriteBatch.Draw(spin, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation * -0.8f, spin.Size() / 2, scale2 * 0.6f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(spin, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation * 1.5f, spin.Size() / 2, scale2 * 0.6f, SpriteEffects.FlipHorizontally, 0f);

            //Main.spriteBatch.Draw(edge, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation * -1f, edge.Size() / 2, scale * 1.3f, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(edge, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation * 1f, edge.Size() / 2, scale * 1.3f, SpriteEffects.FlipHorizontally, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        

    }

}
