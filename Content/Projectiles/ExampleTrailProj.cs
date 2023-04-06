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

namespace AerovelenceMod.Content.Projectiles
{
	public class ExampleTrailProj : TrailProjBase
	{
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Example Trail");
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
 
        public override void AI()
        {
            Projectile.velocity.Y += 0.09f;
            Projectile.ai[1] += 0.05f;

            trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trail7").Value;
            trailColor = Color.DodgerBlue;
            trailTime = Projectile.ai[1];

            // other things you can adjust
            trailPointLimit = 120;
            trailWidth = 20;
            trailMaxLength = 300;
            

            //MUST call TrailLogic AFTER assigning trailRot and trailPos
            trailRot = Projectile.velocity.ToRotation();
            trailPos = Projectile.Center;
            TrailLogic();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //trailTime = Projectile.ai[1];
            //Draw whatever you want under trail//
            TrailDrawing();
            //Draw whatever you want over trail//

            return false;
        }

        //Controls the width of the function based on progress
        //Progress ranges from 0-1 based on how far along the trail we are
        //Dick around with it to see what i mean
        //Dont override at all if you want to use the default width fuction in super
        public override float WidthFunction(float progress)
        {
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.4f, progress, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(0f, 30f, num) * 0.5f; // 0.3f
        }

    }

    public class TrailGradientTest : TrailProjBase
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Test Trail");
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

        public override void AI()
        {
            Projectile.velocity.Y += 0.09f;
            Projectile.ai[1] += 0.04f;

            trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/LintyTrail").Value;
            //trailColor = Color.DodgerBlue;
            trailTime = Projectile.ai[1];
            shouldScrollColor = true;
            gradient = true;
            gradientTime = Projectile.ai[1] * 0.2f;
            gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/CyverGrad").Value;

            // other things you can adjust
            trailPointLimit = 120;
            trailWidth = 40;
            trailMaxLength = 300;


            //MUST call TrailLogic AFTER assigning trailRot and trailPos
            trailRot = Projectile.velocity.ToRotation();
            trailPos = Projectile.Center;
            TrailLogic();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //trailTime = Projectile.ai[1];
            //Draw whatever you want under trail//
            TrailDrawing();
            //Draw whatever you want over trail//

            return false;
        }

        //Controls the width of the function based on progress
        //Progress ranges from 0-1 based on how far along the trail we are
        //Dick around with it to see what i mean
        //Dont override at all if you want to use the default width fuction in super
        public override float WidthFunction(float progress)
        {
            //return trailWidth;

            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.4f, 1 - progress, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(0f, trailWidth, num) * 0.5f; // 0.3f
        }

    }

}