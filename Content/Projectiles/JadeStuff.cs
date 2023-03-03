using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.Graphics;
using ReLogic.Content;

namespace AerovelenceMod.Content.Projectiles
{
    public class JadeFirePulse : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Jade Pulse");
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.hostile = false;
            Projectile.penetrate = -1;

            Projectile.scale = 0.1f;

            Projectile.timeLeft = 1025; //30
            Projectile.tileCollide = false; //false;
            Projectile.width = 20;
            Projectile.height = 20;
        }

        int timer = 0;
        public override void AI()
        {
            if (timer == 0)
            {
                Projectile.rotation = Main.rand.NextFloat(6.28f);
            }
            //Projectile.scale = 1f;
            Projectile.scale = MathHelper.Lerp(Projectile.scale, 0.7f, 0.15f); //1.51
            Projectile.rotation += 0.03f;
            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];
            Texture2D texture = Mod.Assets.Request<Texture2D>("Assets/EnergyBalls/energyball_10").Value;

            Vector2 tTex = new Vector2(Projectile.scale, Projectile.scale);
            
            //tstar caus, heartOriginal grad, eball4 tex (3draws)
            //tstar, energyball 10 tex, eball 4 grad


            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/FireBallShader", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["caustics"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/GaussianStar").Value);
            myEffect.Parameters["distort"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/noise").Value);
            myEffect.Parameters["gradient"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/EnergyBalls/energyball_10red").Value);
            myEffect.Parameters["uTime"].SetValue(timer * 0.03f); //0.04

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            int height1 = texture.Height;
            Vector2 origin1 = new Vector2((float)texture.Width / 2f, (float)height1 / 2f);

            if (Projectile.timeLeft > 20)
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin1, tTex, SpriteEffects.None, 0.0f);
            if (Projectile.timeLeft > 10)
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin1, tTex, SpriteEffects.None, 0.0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin1, tTex, SpriteEffects.None, 0.0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin1, tTex, SpriteEffects.None, 0.0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }

    public class JadeMagmaPulse : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Jade Magma Pulse");
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.hostile = false;
            Projectile.penetrate = -1;

            Projectile.scale = 0.1f;

            Projectile.timeLeft = 300; //30
            Projectile.tileCollide = false; //false;
            Projectile.width = 20;
            Projectile.height = 20;
        }

        int timer = 0;
        public override void AI()
        {
            Projectile.scale = MathHelper.Lerp(Projectile.scale, 0.7f, 0.15f); //1.51
            Projectile.velocity = Vector2.Zero;
            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/circle_02").Value;

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/FireBallShader", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["caustics"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/FireGrad").Value);
            myEffect.Parameters["distort"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/noise").Value);
            myEffect.Parameters["gradient"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/FireGrad").Value);
            myEffect.Parameters["uTime"].SetValue(timer * 0.06f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, 0, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0.0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }

}