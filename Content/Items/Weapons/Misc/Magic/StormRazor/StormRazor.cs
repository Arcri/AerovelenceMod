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
using Terraria.GameContent.ItemDropRules;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Magic.StormRazor
{
    public class StormRazor : ModItem
    {
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Magic;
            Item.damage = 50;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.Pink;

            Item.shoot = ModContent.ProjectileType<StormRazorProj>();
            Item.shootSpeed = 10;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.reuseDelay = 0;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.mana = 20;
            Item.noUseGraphic = false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity * 0.8f, type, damage, knockback, player.whoAmI);
            return false;
        }
    }

    public class StormRazorProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
        }
        public override void SetDefaults()
        {
            Projectile.damage = 0;
            Projectile.friendly = false;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.tileCollide = false;

            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.timeLeft = 800;
            Projectile.scale = 1f;

        }


        int timer = 0;
        float scale = 0;
        float alpha = 1;
        public override void AI()
        {
            if (timer < 30)
            {
                float easingProgress = Easings.easeInOutSine((float)timer / 60f);


                scale = MathHelper.Lerp(scale, 0.65f, easingProgress);

            }
            else if (timer >= 30)
            {
                scale = scale + 0.02f;
                alpha -= 0.05f;
            }


            /*
            float ts = 0.4f;
            if (timer < 40)
                scale = Math.Clamp(MathHelper.Lerp(scale, 2f * ts, (float)(timer * 0.02f)), 0, 1.5f * ts);
            else
                scale = Math.Clamp(MathHelper.Lerp(scale, -0.5f, 0.05f), 0, 1.5f * ts);
            */
            Projectile.rotation += 0.12f;


            Projectile.timeLeft = 2;

            if (timer > 180)
                alpha -= 0.02f;

            if (alpha <= 0)
                Projectile.active = false;

            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Flare = Mod.Assets.Request<Texture2D>("Assets/Orbs/impact_2fade2").Value;
            //flare 1


            Texture2D Flare2 = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/ElectricRadialEffect").Value;

            Texture2D Ball = Mod.Assets.Request<Texture2D>("Assets/Orbs/feather_circle").Value;

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Radial/BoFIrisAlt", AssetRequestMode.ImmediateLoad).Value;

            myEffect.Parameters["causticTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Noise_1").Value);
            myEffect.Parameters["gradientTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/SofterBlueGrad").Value);
            myEffect.Parameters["distortTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Swirl").Value);

            myEffect.Parameters["flowSpeed"].SetValue(0.3f);
            myEffect.Parameters["vignetteSize"].SetValue(1f);
            myEffect.Parameters["vignetteBlend"].SetValue(0.8f);
            myEffect.Parameters["distortStrength"].SetValue(0.02f);
            myEffect.Parameters["xOffset"].SetValue(0.0f);
            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
            myEffect.Parameters["colorIntensity"].SetValue(alpha * 2);


            //Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.Black with { A = 0 }, Projectile.rotation, Flare.Size() / 2, scale * 1f, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.Black with { A = 0 }, Projectile.rotation * -1, Flare.Size() / 2, scale * 1.25f, SpriteEffects.None, 0f);            //Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.Black with { A = 0 }, Projectile.rotation * -1, Flare.Size() / 2, scale * 1.25f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, Color.Black * 0.15f, Projectile.rotation, Ball.Size() / 2, scale, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);

            myEffect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation, Ball.Size() / 2, scale * 0.25f, SpriteEffects.None, 0f);


            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation, Flare.Size() / 2, scale * 0.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation, Flare.Size() / 2, scale * 0.5f, SpriteEffects.None, 0f);

            //Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation * -1, Flare.Size() / 2, scale * 0.7f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Flare2, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation + 1, Flare2.Size() / 2, scale * 1.25f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare2, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation * -1 - 1, Flare2.Size() / 2, scale * 1.5f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }


}
