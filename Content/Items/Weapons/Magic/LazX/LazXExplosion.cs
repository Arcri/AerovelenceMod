using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace AerovelenceMod.Content.Items.Weapons.Magic.LazX
{
	public class LazXExplosion : ModProjectile
	{

		bool hasHit = false;

		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Magical Blast");
			Main.projFrames[projectile.type] = 24;

		}

		public override void SetDefaults()
		{
			projectile.width = 50;
			projectile.height = 50;
			projectile.tileCollide = false;
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.aiStyle = -1;
			projectile.penetrate = -1;
			projectile.hostile = false;
			projectile.alpha = 0;
			projectile.hide = true;
			projectile.timeLeft = 60;
			projectile.scale = 1.5f;
			drawOffsetX = 0;
			drawOriginOffsetY = 0;
			projectile.extraUpdates = 1;
		}

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
			drawCacheProjsBehindProjectiles.Add(index);
        }

        public override void AI()
        {
			if (hasHit)
				projectile.damage = 0;

			projectile.frame++;
			if (projectile.frame == 24)
				projectile.active = false;
				
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return lightColor * projectile.Opacity; 
		}


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
		
		    spriteBatch.End();
		    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			return true; //Shut up I don't think this messes anything up
			spriteBatch.End(); 
		    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			hasHit = true;
            base.OnHitNPC(target, damage, knockback, crit);
        }

    }
}


