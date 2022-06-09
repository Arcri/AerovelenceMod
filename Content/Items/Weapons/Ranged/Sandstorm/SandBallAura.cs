using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Shaders;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace AerovelenceMod.Content.Items.Weapons.Ranged.Sandstorm
{
	public class SandBallAura : ModProjectile
	{

		Vector2 Centerfold = Vector2.Zero;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sandy Aura");
			//ProjectileID.Sets.TrailingMode[projectile.type] = 2;
			//ProjectileID.Sets.TrailCacheLength[projectile.type] = 18;

		}

		public override void SetDefaults()
		{
			projectile.width = 100;
			projectile.height = 100;
			projectile.friendly = true;
			projectile.ranged = true;
			projectile.penetrate = 1;
			projectile.tileCollide = false;
			projectile.timeLeft = 320;
			drawOffsetX = -80;
			drawOriginOffsetY = -80;
			projectile.scale = 0.02f;
			projectile.alpha = 255;
		}

		public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
		{
			drawCacheProjsBehindProjectiles.Add(index);
			drawCacheProjsBehindNPCsAndTiles.Add(index); //Doesn't work lmao but i don't want to delete

		}
		public override void AI()
		   {
			int du1 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.AmberBolt, 0f, 0f, 75, default(Color), .9f);
			Main.dust[du1].noGravity = true;

			//Applies OnFire to NPCs in Aura
			for (int npcWho = 0; npcWho < 200; npcWho++)
			{
				if (Main.npc[npcWho].CanBeChasedBy(projectile, false))
				{
					float lengthToNPC = Math.Abs((Main.npc[npcWho].Center - projectile.Center).Length());
					if (lengthToNPC < 80)
						Main.npc[npcWho].AddBuff(BuffID.OnFire, 5);
				}
			}

			foreach (Projectile projectileWho in Main.projectile) 
            {
				if (projectileWho.type == ModContent.ProjectileType<SandstormArrow>() || (projectileWho.type == ModContent.ProjectileType<SandBallAura>()))// && projectile != projectileWho)) //I'm pretty sure this is more efficient, but it also might not matter
                {
					//Makes it so that you cannot spawn another 
					if (projectileWho.type == ModContent.ProjectileType<SandBallAura>() && projectile != projectileWho) //There are some wacky cases that I can't figure out where it allows you to replace the current aura, but idk why
                    {
						projectileWho.Kill();
                    }

					if (projectileWho.alpha == 1 && projectileWho.active == false && projectileWho.type == ModContent.ProjectileType<SandstormArrow>())
                    {
						projectileWho.alpha = 2;
						for (int i = 0; i < Main.maxNPCs; i++)
						{
							if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && Vector2.Distance(projectileWho.Center, Main.npc[i].Center) < 35f)
							{
								int Direction = 0;
								if (projectileWho.position.X - Main.npc[i].position.X < 0)
									Direction = 1;
								else
									Direction = -1;
								Main.npc[i].StrikeNPC(6, projectile.knockBack, Direction);
							}
						}
						for (int b = 0; b < 15; b++)
						{
							int mydusty = Dust.NewDust(projectileWho.Center, 4, 4, DustID.Rainbow, new Vector2(Main.rand.NextFloat(3, 6), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(0, 360))).X, new Vector2(Main.rand.NextFloat(3, 8), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(0, 360))).Y, 0, Color.BlanchedAlmond, 0.8f);
							Main.dust[mydusty].noGravity = true;
						}
						Main.PlaySound(SoundID.Item100.WithVolume(.7f).WithPitchVariance(-1f), projectileWho.Center);
						 //So we don't have infinite explosions
					}


					//Makes some dust if the proj has passed through aura
					if (projectileWho.alpha == 1 && projectileWho.active == true)
                    {
						int du2 = Dust.NewDust(new Vector2(projectileWho.position.X, projectileWho.position.Y), projectileWho.width, projectileWho.height, DustID.AmberBolt, 0f, 0f, 75, default(Color), .9f);
						Main.dust[du2].noGravity = true;
					}

					float lengthToProj = Math.Abs((projectileWho.Center - projectile.Center).Length());
					if (lengthToProj < 80)
                    {
						if (projectileWho.alpha != 2)
							projectileWho.alpha = 1;

					}

				}
            }

			//Keeps track of the position on the first tick so that it doesn't scale to the wrong place
			if (projectile.scale == 0.02f)
				Centerfold = projectile.Center;
			
			projectile.width = 100;
			projectile.height = 100;
			projectile.scale = MathHelper.Clamp(projectile.scale + 0.02f, 0.1f, 0.5f);
			projectile.Center = Centerfold;

		}



        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

			// Retrieve reference to shader
			/*!*/var deathShader = GameShaders.Misc["AerovelenceMod:SandAura"].UseImage("Images/Misc/Perlin").UseColor(Color.SandyBrown.ToVector3()); 
			deathShader.Apply(null);
			return true;
		}

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }


        public override void Kill(int timeLeft)
        {
			for (int l = 0; l < 40; l++)
			{
				int mydust = Dust.NewDust(projectile.Center, 4, 4, DustID.Rainbow, new Vector2(Main.rand.NextFloat(8, 13), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(0, 360))).X, new Vector2(Main.rand.NextFloat(8, 13), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(0, 360))).Y, 0, Color.BlanchedAlmond, 1f);
				Main.dust[mydust].noGravity = true;
			}
			Main.PlaySound(SoundID.DD2_BookStaffCast, projectile.Center);
		}

        public override Color? GetAlpha(Color lightColor)
        {
			return Color.White;
        }
    }
}


